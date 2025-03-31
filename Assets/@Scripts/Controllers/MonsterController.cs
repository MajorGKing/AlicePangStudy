using Data;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class MonsterController : BaseController
{
    SpriteRenderer _spriteRenderer;
    public Color _damagedColor = new Vector4(1f, 0.874f, 0.874f, 1f);

    Sequence _seq;
    float _speed = 30.0f;
    int _currentIndex;

    //[SerializeField]
    Sprite _normalSprite;
    //[SerializeField]
    Sprite _damagedSprite;
    //[SerializeField]
    Sprite _attackSprite;

    [SerializeField]
    GameObject _shadow;

    [SerializeField]
    GameObject _redShadow;

    [SerializeField]
    GameObject _selectedMark;

    bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            if (_selected)
            {
                _shadow.SetActive(false);
                _redShadow.SetActive(true);
                _selectedMark.SetActive(true);
            }
            else
            {
                _shadow.SetActive(true);
                _redShadow.SetActive(false);
                _selectedMark.SetActive(false);
            }
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
            {
                _statusBar.SetHp(_hp, MaxHp);
            }
        }
    }

    public int MaxHp { get; set; }
    public int TemplateID { get; set; }

    public int CellIndex { get; set; } = -1;
    public bool IsBusy { get; set; } = true;

    Vector3 _dest;
    MonsterData _monsterData;
    public int Damage { get { return _monsterData.Damage; } }

    StatusBar _statusBar;

    [SerializeField]
    ParticleSystem _hitEffect;

    Sequence transformSeq;
    Sequence colorSeq;

    public event Action OnMonsterAttack;
    public event Action OnMonsterDead;

    int _turnRemaining;
    public int MoveTurnRemaining
    {
        get { return _turnRemaining; }
        set
        {
            _turnRemaining = value;
            if (_statusBar != null)
                _statusBar.SetTurnText(_turnRemaining);
        }
    }

    Vector3 _dieMoveDir;
    float _dieMoveSpeed = 37.0f;
    Define.ECreatureState _state = Define.ECreatureState.Idle;
    public Define.ECreatureState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (value)
            {
                case Define.ECreatureState.Idle:
                    break;
                case Define.ECreatureState.Moving:
                    break;
                case Define.ECreatureState.Attack:
                    break;
                case Define.ECreatureState.Dead:
                    OnMonsterDead?.Invoke();
                    int remainMonster = (Managers.Scene.CurrentScene as GameScene).CountRemainMonster(this);
                    (Managers.UI.SceneUI as UI_GameScene).MonsterDead(TemplateID, remainMonster);
                    _spriteRenderer.DOFade(0f, 0.5f).SetEase(Ease.InSine).OnComplete(() => Managers.Game.MonsterDead(this));
                    break;
            }
        }
    }

    private void Awake()
    {
        Selected = false;

        _statusBar = Utils.FindChild<StatusBar>(gameObject, recursive: true);

        _spriteRenderer = Utils.GetOrAddComponent<SpriteRenderer>(Utils.FindChild(gameObject, "MonsterSprite"));
        _spriteRenderer.sprite = _normalSprite;

        ResetMoveTurn();
        Hp = MaxHp;

        SetFlip();
    }

    void Appearance(float interval)
    {
        transform.localScale = Vector3.zero;
        transformSeq = DOTween.Sequence();
        Invoke("AppearanceSound", interval);
        transformSeq.AppendInterval(interval);
        transformSeq.Append(transform.DOScale(1f, 0.3f).From(0f).OnComplete(() => IdleAnimation()));
    }

    void AppearanceSound()
    {
        Managers.Sound.Play(Define.ESound.Effect, "Sound_MonsterPop");
    }

    void IdleAnimation()
    {
        IsBusy = false;

        _spriteRenderer.transform.DOShakeScale(0.6f, new Vector3(0f, 0.1f, 0f), 1, 0).SetLoops(-1);
    }

    public void SetInfo(MonsterData monsterData, float interval)
    {
        _monsterData = monsterData;

        Hp = monsterData.Hp;
        MaxHp = monsterData.Hp;
        TemplateID = monsterData.TemplateID;

        switch (monsterData.SpecialAbility)
        {
            case 0:
                OnMonsterAttack += UpdateAttack;
                break;

            case 1:
                OnMonsterAttack += UpdateAttack;
                OnMonsterAttack += OnDead;
                break;

            case 2:
                OnMonsterAttack += HealBoss;
                OnMonsterAttack += OnDead;
                OnMonsterDead += AttackBoss;
                break;
        }

        _normalSprite = Managers.Resource.Load<Sprite>(monsterData.SpriteID);
        if (_spriteRenderer != null)
            _spriteRenderer.sprite = _normalSprite;

        _damagedSprite = Managers.Resource.Load<Sprite>(monsterData.SpriteID + "_3");
        _attackSprite = Managers.Resource.Load<Sprite>(monsterData.SpriteID + "_4");

        Appearance(interval);

        ResetMoveTurn();
    }

    bool CanMove()
    {
        return MoveTurnRemaining <= 0;
    }

    bool CanAttack()
    {
        return CellIndex < Define.SLICE_COUNT * _monsterData.AttackRange;
    }

    public void ResetMoveTurn()
    {
        MoveTurnRemaining = _monsterData.MoveTurn;
    }

    public void StartMonsterTurn()
    {
        if (State == Define.ECreatureState.Dead)
            return;

        MoveTurnRemaining = Math.Max(0, MoveTurnRemaining - 1);

        if (CanMove())
        {

            if (CanAttack())
            {
                State = Define.ECreatureState.Attack;
                return;
            }
            else
            {
                int moveIndex = Managers.Object.TryMoveForward(this, _monsterData.MoveSpeed);
                if (moveIndex != -1)
                {
                    State = Define.ECreatureState.Moving;
                    IsBusy = true;
                    SetFlip();
                    SetDestination(Managers.Object.Pos[moveIndex]);
                }
                else
                {
                    IsBusy = false;
                    ResetMoveTurn();
                }
            }
        }
        else
        {
            IsBusy = false;
        }
    }

    public IEnumerator StartMonsterAttack()
    {
        if (State == Define.ECreatureState.Attack && !IsBusy)
        {
            IsBusy = true;

            OnMonsterAttack?.Invoke();

            yield return new WaitUntil(() => !IsBusy);

            State = Define.ECreatureState.Idle;
        }
        else if (State != Define.ECreatureState.Moving)
        {
            IsBusy = false;
        }
    }

    private void Update()
    {
        if (CellIndex < 0 || CellIndex > 143)
            Debug.LogError(gameObject);
        switch (State)
        {
            case Define.ECreatureState.Idle:
                UpdateIdle();
                break;
            case Define.ECreatureState.Moving:
                UpdateMoving();
                break;
            case Define.ECreatureState.Attack:
                break;
            case Define.ECreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    void UpdateIdle()
    {
        if (IsBusy == false)
            return;
    }

    void UpdateMoving()
    {
        float moveDist = _speed * Time.deltaTime;
        Vector3 dir = (_dest - transform.position);

        if (dir.magnitude < 0.1f)
        {
            transform.position = _dest;
            State = Define.ECreatureState.Idle;
            if (IsBusy)
                ResetMoveTurn();

            IsBusy = false;
        }
    }

    void UpdateAttack()
    {
        ChangeAttackSprite();
        Invoke("ChangeNormalSprite", 0.5f);

        transformSeq = DOTween.Sequence();

        Vector3 currentPos = transform.position;
        Vector3 targetPos = Vector3.Normalize(transform.position);//new Vector3(0f, 0f, transform.position.z);

        transformSeq.Append(transform.DOMove(targetPos, 0.25f).SetEase(Ease.OutExpo).OnComplete(() => Managers.Object.Player.OnDamaged(this)));
        transformSeq.AppendInterval(0.05f).OnComplete(() => ChangeNormalSprite());
        transformSeq.Append(transform.DOMove(currentPos, 0.15f).SetEase(Ease.Linear));
        transformSeq.AppendInterval(0.05f).OnComplete(() => EndAttack());
        Managers.Sound.Play(Define.ESound.Effect, "Sound_MonsterAttack");
    }

    void HealBoss()
    {
        Managers.Object.Boss.Hp = Mathf.Min(Managers.Object.Boss.Hp + Hp, Managers.Object.Boss.MaxHp);

        IsBusy = false;
    }

    void AttackBoss()
    {
        Managers.Object.Boss.Hp -= Damage;
    }

    void EndAttack()
    {
        IsBusy = false;
        ResetMoveTurn();
    }

    public void EndTurn()
    {
        IsBusy = false;
        if (MoveTurnRemaining <= 0)
            ResetMoveTurn();
    }

    void UpdateDead()
    {
        transform.position += _dieMoveDir * _dieMoveSpeed * Time.deltaTime;
    }

    public void SetDestination(Vector3 dest)
    {
        _dest = new Vector3(dest.x, dest.y, dest.y * Random.Range(0.99f, 1.01f));

        transformSeq = DOTween.Sequence();


        transformSeq.AppendInterval(Random.Range(0f, 0.3f));
        transformSeq.Append(transform.DOJump(_dest, 0.8f, 1, 0.2f));
    }

    public float GetDistance(Vector2 origin)
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        return (pos - origin).magnitude;
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, pos.y * Random.Range(0.99f, 1.01f));
    }

    public void OnDamaged(PlayerController pc, Vector3 attackDir)
    {
        if (State == Define.ECreatureState.Dead)
            return;

        _hitEffect.Play();
        ChangeDamagedSprite();

        CancelInvoke();

        Invoke("ChangeNormalSprite", 0.5f);
        int damage = pc.Damage * pc.ComboCount;
        Managers.Object.ShowDamageText(transform.position, damage);
        Managers.Object.Camera.CameraAnimation("CamAction");

        Hp = Math.Max(0, Hp - damage);
        if (Hp <= 0)
            OnDead(attackDir);
        else
            Knockback();
    }

    public void OnDamaged(MonsterController mc, Vector2 attackDir)
    {
        if (State == Define.ECreatureState.Dead)
            return;
        _hitEffect.Play();
        ChangeDamagedSprite();

        Invoke("ChangeNormalSprite", 0.3f);

        Managers.Object.ShowDamageText(transform.position, mc.Damage);

        Hp = Math.Max(0, Hp - mc.Damage);
        if (Hp <= 0)
            OnDead(attackDir);
    }

    void OnDead(Vector2 attackDir)
    {
        _dieMoveDir = attackDir;
        State = Define.ECreatureState.Dead;

        Managers.Game.CurrentStageGetCoin = Managers.Game.CurrentStageGetCoin + _monsterData.DropCoin;
        Managers.Object.DropCoin(transform.position, _monsterData.DropCoin);

        transformSeq = DOTween.Sequence();
        transformSeq.Append(transform.DORotate(Vector3.forward * 720f, 0.5f, RotateMode.FastBeyond360));
    }
    
    void OnDead()
    {
        State = Define.ECreatureState.Dead;
    }

    void Knockback()
    {
        int knockbackIndex;
        switch (Managers.Object.Player.Knockback)
        {
            case Define.EKnockbackDirection.Front:
                knockbackIndex = Managers.Object.TryMoveForward(this);
                break;

            case Define.EKnockbackDirection.Back:
                knockbackIndex = Managers.Object.TryMoveBackward(this);
                break;

            case Define.EKnockbackDirection.Clockwise:
                knockbackIndex = Managers.Object.TryMoveClockwise(this, true);
                break;

            case Define.EKnockbackDirection.AntiClockwise:
                knockbackIndex = Managers.Object.TryMoveClockwise(this, false);
                break;

            default:
                knockbackIndex = -1;
                break;
        }

        if (knockbackIndex == -1)
        {
            transformSeq = DOTween.Sequence();

            _dest = transform.position;
            Vector3 attackDir = transform.position.normalized;

            transformSeq.Append(transform.DOMove(_dest + attackDir * 2, 0.1f));
            transformSeq.Append(transform.DOMove(_dest, 0.1f));
            State = Define.ECreatureState.Moving;
        }
        else if (Managers.Object.Monsters[knockbackIndex] == this)
        {
            transformSeq = DOTween.Sequence();

            _dest = Managers.Object.Pos[knockbackIndex] + Random.Range(-0.01f, 0.01f) * Vector3.forward;
            Vector3 attackDir = (_dest - transform.position).normalized;

            transformSeq.Append(transform.DOMove(_dest + attackDir * 2, 0.1f));
            transformSeq.Append(transform.DOMove(_dest, 0.1f));
            State = Define.ECreatureState.Moving;
        }
        else
        {
            transformSeq = DOTween.Sequence();
            transformSeq.Append(transform.DOMove(Managers.Object.Monsters[knockbackIndex].transform.position, 0.2f).OnComplete(() => HitKnockbackDamage(knockbackIndex)));
            transformSeq.Append(transform.DOMove(Managers.Object.Monsters[CellIndex].transform.position, 0.2f));
        }
    }

    void HitKnockbackDamage(int collisionMonsterIndex)
    {
        Vector2 attackDir = (Managers.Object.Monsters[collisionMonsterIndex].transform.position - transform.position).normalized;

        Managers.Object.Monsters[collisionMonsterIndex].OnDamaged(this, attackDir);
    }

    void ChangeDamagedSprite()
    {
        _spriteRenderer.sprite = _damagedSprite;

        colorSeq = DOTween.Sequence();

        colorSeq.Append(_spriteRenderer.DOColor(_damagedColor, 0.3f).SetLoops(1, LoopType.Restart));
        colorSeq.Append(_spriteRenderer.DOColor(Color.white, 0.2f));
    }

    void ChangeAttackSprite()
    {
        _spriteRenderer.sprite = _attackSprite;
    }

    void ChangeNormalSprite()
    {
        _spriteRenderer.sprite = _normalSprite;
    }

    private void OnDisable()
    {
        //DOTween.Kill(seq);
        transformSeq.Kill(true);
        colorSeq.Kill(true);
        _spriteRenderer.transform.DOKill();
        //Managers.Game.MonsterDead(this);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Selected = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Selected = false;
    }

    void SetFlip()
    {
        if (transform.position.x > 0)
            _spriteRenderer.flipX = true;
        else
            _spriteRenderer.flipX = false;
    }
}
