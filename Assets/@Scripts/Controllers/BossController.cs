using Data;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BossController : BaseController
{
    public int TemplateID { get; set; }
    BossData _bossData;

    public bool IsBusy { get; set; }

    StatusBar _statusBar;

    Animator _animator;
    SkeletonAnimation _spineAni;

    public int MaxHp { get; set; }
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

    float _angleSpeed = Define.RADAR_SPEED;

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
                    //_spriteRenderer.DOFade(0f, 0.5f).SetEase(Ease.InSine).OnComplete(() => Managers.Game.MonsterDead(this));
                    break;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _statusBar = Utils.FindChild<StatusBar>(gameObject, recursive: true);

        if (TemplateID < 6)
            _animator = Utils.GetOrAddComponent<Animator>(gameObject);
        else
            _spineAni = Utils.GetOrAddComponent<SkeletonAnimation>(gameObject);

        ResetMoveTurn();

        Hp = MaxHp;
    }

    public void SetInfo(BossData bossData)
    {
        _bossData = bossData;
        transform.position = Vector3.up * bossData.PositionY;

        TemplateID = _bossData.TemplateID;

        if (TemplateID == 1)
        {
            Managers.Game.OnPlayerAttack -= StopRadarEffectSound;
            Managers.Game.OnPlayerAttack += StopRadarEffectSound;

            Managers.Game.OnPlayerInput -= PlayRadarEffectSound;
            Managers.Game.OnPlayerInput += PlayRadarEffectSound;
        }

        MaxHp = bossData.Hp;
        Hp = MaxHp;
    }

    void PlayRadarEffectSound()
    {
        if (_angleSpeed > Define.RADAR_SPEED)
            Managers.Sound.Play(Define.ESound.SubBgm, "Sound_RadarFast");
        else if (_angleSpeed < Define.RADAR_SPEED)
            Managers.Sound.Play(Define.ESound.SubBgm, "Sound_RadarSlow");
        else
            Managers.Sound.Stop(Define.ESound.SubBgm);
    }

    void StopRadarEffectSound()
    {
        Managers.Sound.Stop(Define.ESound.SubBgm);
    }

    void ResetMoveTurn()
    {
        if (_init == false)
            return;
    }

    public void StartBossTurn()
    {
        Managers.Sound.Play(Define.ESound.Effect, "Sound_ClockBoss");
        StartCoroutine(StartBossAttack());
    }

    public IEnumerator StartBossAttack()
    {
        IsBusy = true;

        _animator.SetTrigger("UseSkill");

        for (int i = 0; i < _bossData.bossSkillData.Count; i++)
        {
            Define.ESkillType skillID = _bossData.bossSkillData[i].SkillID;

            switch (skillID)
            {
                case Define.ESkillType.ControllRadar:
                    ChangeRadarSpeed();
                    break;

                case Define.ESkillType.SummonMonster:
                    SummonMonster(_bossData.bossSkillData[i]);
                    break;

                case Define.ESkillType.ReduceTurn:
                    ReduceMonsterTurn(_bossData.bossSkillData[i]);
                    break;

                case Define.ESkillType.LockWeapon:
                    LockTheWeapon();
                    break;
            }
        }

        yield return new WaitForSeconds(_bossData.UseSkillAnimationDuration);

        _animator.SetTrigger("Idle");

        IsBusy = false;

    }

    public void OnDamaged()
    {
        _animator.SetTrigger("Damaged");

        if (Hp <= 0)
            OnDead();
    }

    void OnDead()
    {

    }

    void ChangeRadarSpeed()
    {
        int random = Random.Range(0, 3);
        _angleSpeed = RADAR_SPEED;

        switch (random)
        {
            case 0:
                _angleSpeed *= 0.4f;
                break;
            case 1:

                break;

            case 2:
                _angleSpeed *= 3f;
                break;
        }

        Managers.Game.RadarAngleSpeed = _angleSpeed;
    }

    void SummonMonster(BossSkillData bossSkillData)
    {
        StartCoroutine(Managers.Object.BossSummonMonster(transform.position, 78, bossSkillData.MonsterCount));
    }

    void ReduceMonsterTurn(BossSkillData bossSkillData)
    {
        int count = Mathf.Min(Managers.Object.Monsters.Length, bossSkillData.Value);
        int value = bossSkillData.Value;
        List<MonsterController> monsters = Managers.Object.GetHighestTurnMonsters();
        for (int i = 0; i < count; i++)
        {
            monsters[i].MoveTurnRemaining = Mathf.Max(monsters[i].MoveTurnRemaining - value, 0);
        }
    }

    void LockTheWeapon()
    {
        int random = Random.Range(1, 4);

        if (random == (int)Managers.Game.WeaponType)
        {
            Managers.Game.WeaponType = (Define.EWeaponRangeType)((random + 1) % 3 + 1);
        }

        Managers.Game.DisableWeaponRangeType = (Define.EWeaponRangeType)random;
    }
}
