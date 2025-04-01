using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    SpriteRenderer _spriteRenderer;

    Sequence _seq;
    float _speed = 30.0f;
    int _currentIndex;

    public int ComboCount
    {
        get
        {
            if (Managers.UI.SceneUI is UI_GameScene)
            {
                (Managers.UI.SceneUI as UI_GameScene)?.Combo(_currentIndex + 1);
            }

            return _currentIndex + 1;
        }
    }

    List<MonsterController> _targets;
    MonsterController _target;
    Vector3 _dest;
    Vector2 _attackDir;
    Vector3 _playerPos = new Vector3(0f, -2.2f, -2.2f);

    public Define.ECreatureState State { get; set; } = Define.ECreatureState.Idle;
    public bool IsBusy { get; set; } = true;

    public int Damage
    {
        get
        {
            int weaponID = Managers.Game.CurrentWeaponID();
            int level = Managers.Game.WeaponLevel[weaponID - 1];
            return Managers.Data.Weapons[weaponID].WeaponLevelData[level - 1].Damage;
        }
    }

    public Define.EKnockbackDirection Knockback
    {
        get
        {
            int weaponID = Managers.Game.CurrentWeaponID();
            return Managers.Data.Weapons[weaponID].KnockbackDirection;
        }
    }

    int _hp = 0;
    public int Hp
    {
        get { return _hp; }
        set
        {
            _hp = value;
            if (_statusBar != null && MaxHp > 0)
                _statusBar.SetHp(_hp, MaxHp);
        }
    }

    public int MaxHp { get; private set; }

    StatusBar _statusBar;

    [SerializeField]
    SpriteRenderer _weapon;

    [SerializeField]
    ParticleSystem _damagedEffect;

    [SerializeField]
    ParticleSystem _landingEffect;

    protected Coroutine _coWait;

    protected void WaitFor(float seconds)
    {
        if (_coWait != null)
            StopCoroutine(_coWait);

        _coWait = StartCoroutine(CoWait(seconds));
    }

    IEnumerator CoWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coWait = null;
    }

    protected override void Awake()
    {
        base.Awake();

        _statusBar = Utils.FindChild<StatusBar>(gameObject, recursive: true);
        _spriteRenderer = GetComponent<SpriteRenderer>();

        Hp = MaxHp;

        PlayerAppearance();

        ChangeWeaponImage();

        IsBusy = false;
    }

    public void SetInfo(Vector2 pos)
    {
        MaxHp = 100;
        Hp = 100;

        transform.position = pos;
    }

    void Update()
    {
        if (_coWait != null)
            return;

        switch (State)
        {
            case Define.ECreatureState.Idle:
                UpdateIdle();
                break;
            case Define.ECreatureState.Moving:
                UpdateMoving();
                break;
            case Define.ECreatureState.Attack:
                UpdateAttack();
                break;
            case Define.ECreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    void UpdateIdle()
    {
        if (_targets != null && _currentIndex < _targets.Count)
        {
            _target = _targets[_currentIndex];
            _dest = _target.transform.position;
            _attackDir = (_dest - transform.position).normalized;
            State = Define.ECreatureState.Moving;
            Attack(_target);
        }
        else if (IsBusy)
        {
            _dest = _playerPos;
            State = Define.ECreatureState.Moving;
            Return();
        }
    }

    void UpdateMoving()
    {
        float moveDist = _speed * Time.deltaTime;
        Vector3 dir = (_dest - transform.position);

        if (dir.magnitude < 1.0f)
        {
            if (_target == null)
            {
                State = Define.ECreatureState.Idle;

                IsBusy = false;
            }

            WaitFor(0.1f);
        }
    }

    void UpdateAttack()
    {
        _currentIndex++;
        _target = null;
        State = Define.ECreatureState.Idle;
    }

    void UpdateDead()
    {

    }

    void Attack(MonsterController target)
    {
        int templateID = Managers.Game.CurrentWeaponID();

        switch (Managers.Data.Weapons[templateID].AttackType)
        {
            case Define.EWeaponAttackType.Melee:
                MeleeAttack(target);
                break;

            case Define.EWeaponAttackType.Range:
                RangeAttack(target);
                break;

            case Define.EWeaponAttackType.Trap:

                break;
        }
    }

    void MeleeAttack(MonsterController target)
    {
        float flipValue = FlipValue(target);

        _weapon.transform.localPosition = new Vector3(Mathf.Abs(_weapon.transform.localPosition.x) * flipValue, _weapon.transform.localPosition.y, _weapon.transform.localPosition.z);
        _weapon.transform.localScale = new Vector3(Mathf.Abs(_weapon.transform.localScale.x) * flipValue, _weapon.transform.localScale.y, _weapon.transform.localScale.z);

        float posY = target.transform.position.y > 0 ? 0f : 1f;
        Vector3 targetPos = target.transform.position + new Vector3(-flipValue, posY, -1f);

        _seq = DOTween.Sequence();

        _seq.Append(transform.DOJump(targetPos, 2.5f, 1, 0.2f).SetEase(Ease.InCubic)).SetSpeedBased(false);

        _seq.Join(_weapon.transform.DOLocalRotate(Vector3.forward * 80f * flipValue, 0.02f));
        _seq.Append(_weapon.transform.DOLocalRotate(Vector3.forward * -80f * flipValue, 0.08f).OnComplete(() => target.OnDamaged(this, _attackDir)));
        _seq.Append(_weapon.transform.DOLocalRotate(_playerPos, 0.02f));

        _seq.AppendInterval(Mathf.Max(0.25f / (ComboCount * ComboCount), 0f)).OnComplete(() => UpdateAttack());
        Managers.Sound.Play(Define.ESound.Effect, "Sound_Combo1");
    }

    void RangeAttack(MonsterController target)
    {
        float flipValue = FlipValue(target);

        _weapon.transform.localPosition = new Vector3(Mathf.Abs(_weapon.transform.localPosition.x) * flipValue, _weapon.transform.localPosition.y, _weapon.transform.localPosition.z);
        _weapon.transform.localScale = new Vector3(Mathf.Abs(_weapon.transform.localScale.x) * flipValue, _weapon.transform.localScale.y, _weapon.transform.localScale.z);

        _seq = DOTween.Sequence();

        _seq.Append(_weapon.transform.DOLocalRotate(Vector3.forward * 80f * flipValue, 0.02f));
        _seq.Append(_weapon.transform.DOLocalRotate(Vector3.forward * -80f * flipValue, 0.08f).OnComplete(() =>
        {
            int weaponID = Managers.Game.CurrentWeaponID();
            string objectID = Managers.Data.Weapons[weaponID].ObjectID;

            var go = Managers.Resource.Instantiate(objectID);
            go.gameObject.SetActive(true);
            go.transform.position = this.transform.position;
            Shoot(go, target);
        }));

        _seq.Append(_weapon.transform.DOLocalRotate(_playerPos, 0.02f));
    }

    void Shoot(GameObject projectile, MonsterController target)
    {
        _seq = DOTween.Sequence();

        _seq.Append(projectile.transform.DOMove(target.transform.position, 0.2f).OnComplete(() =>
        {
            target.OnDamaged(this, _attackDir);
            projectile.SetActive(false);
            UpdateAttack();
        }));

        Managers.Sound.Play(Define.ESound.Effect, "Sound_Combo1");
    }

    void Return()
    {
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOMove(_playerPos, 0.3f).SetSpeedBased());
    }

    public void OnDamaged(MonsterController mc)
    {
        if (State == Define.ECreatureState.Dead)
            return;
        _damagedEffect.Play();
        ShakePlayer();
        Managers.Object.Camera.CameraAnimation("CamActionHit");
        Hp = Math.Max(0, Hp - mc.Damage);
        if (Hp <= 0)
            OnDead();
    }

    void ShakePlayer()
    {
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOShakePosition(0.1f, 0.5f, 2, 3f, false, true));
    }

    void OnDead()
    {
        State = Define.ECreatureState.Dead;
        //게임 오버
        (Managers.Scene.CurrentScene as GameScene).GameOver();
    }

    void PlayerAppearance()
    {
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOMove(_playerPos, 0.2f).SetEase(Ease.InSine).OnComplete(() => { PlayLandingEffect(); }));
    }

    void PlayLandingEffect()
    {
        _landingEffect.gameObject.SetActive(false);
        _landingEffect.gameObject.SetActive(true);

        _landingEffect.Play();
    }

    public void StartAttack(List<MonsterController> targets)
    {
        _currentIndex = 0;
        _targets = targets;
        State = Define.ECreatureState.Idle;
        IsBusy = true;
    }

    public void RevivePlayer()
    {
        Hp = MaxHp;
        _currentIndex = 0;
        _targets.Clear();
    }

    public void ChangeWeaponImage()
    {
        _weapon.sprite = Managers.Resource.Load<Sprite>(Managers.Data.Weapons[Managers.Game.CurrentWeaponID()].Sprite);        
    }

    float FlipValue(MonsterController target)
    {
        float flipValue;

        if (target.transform.position.x > 0)
        {
            _spriteRenderer.flipX = false;
            flipValue = 1f;
        }
        else
        {
            _spriteRenderer.flipX = true;
            flipValue = -1f;
        }

        return flipValue;
    }
}
