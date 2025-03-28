using Data;
using DG.Tweening;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
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

    public void SetPos(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, pos.y * Random.Range(0.99f, 1.01f));
    }

    void ChangeAttackSprite()
    {
        _spriteRenderer.sprite = _attackSprite;
    }

    void ChangeNormalSprite()
    {
        _spriteRenderer.sprite = _normalSprite;
    }


    void OnDead()
    {
        State = Define.ECreatureState.Dead;

        //Managers.Object.CountRemainMonster(this);
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

    public void StartMonsterTurn()
    {

    }

    public float GetDistance(Vector2 origin)
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        return (pos - origin).magnitude;
    }

    void SetFlip()
    {
        if (transform.position.x > 0)
            _spriteRenderer.flipX = true;
        else
            _spriteRenderer.flipX = false;
    }
}
