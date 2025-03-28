using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    class MissionData
    {
        public int currentCount;
        public int clearCount;
    }

    Dictionary<int, MissionData> _stageMissionData;
    int _remainingTurn;

    Define.EBattleState _state { get; set; } = Define.EBattleState.Ready;

    public Define.EBattleState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (value)
            {
                case Define.EBattleState.Ready:
                    _gameSceneUI.ShowWeaponButtons(false);
                    OnChangeWeapon(Define.EWeaponRangeType.None);
                    break;
                case Define.EBattleState.PlayerInput:
                    _gameSceneUI.ShowWeaponButtons(true);
                    OnChangeWeapon(Managers.Game.WeaponType);
                    break;
                case Define.EBattleState.PlayerAttack:
                    _gameSceneUI.ShowWeaponButtons(false);
                    OnChangeWeapon(Define.EWeaponRangeType.None);
                    break;
                case Define.EBattleState.BossAttack:
                    _gameSceneUI.ShowWeaponButtons(false);
                    OnChangeWeapon(Define.EWeaponRangeType.None);
                    break;
                case Define.EBattleState.MonsterAttack:
                    _gameSceneUI.ShowWeaponButtons(false);
                    OnChangeWeapon(Define.EWeaponRangeType.None);
                    break;
                case Define.EBattleState.GameOver:
                    _gameSceneUI.ShowWeaponButtons(false);
                    OnChangeWeapon(Define.EWeaponRangeType.None);
                    break;
            }
        }
    }

    UI_GameScene _gameSceneUI;

    [SerializeField]
    SpriteRenderer _bg;

    [SerializeField]
    RadarController _shortRangeRadar;
    [SerializeField]
    RadarController _middleRangeRadar;
    [SerializeField]
    RadarController _longRangeRadar;

    IEnumerator _currentSequenceCoroutine;
    bool _gameStart = false;

    StageData _stageData;

    

    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        gameObject.AddComponent<CaptureScreenShot>();
#endif

        Debug.Log("@>> GameScene Init()");
        SceneType = Define.EScene.GameScene;

        Managers.Object.ResetStageObjects();

        _shortRangeRadar.SetInfo(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID].RadarID);
        _middleRangeRadar.SetInfo(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID].RadarID);
        _longRangeRadar.SetInfo(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID].RadarID);

        OnChangeWeapon(Define.EWeaponRangeType.None);
        Managers.Game.DisableWeaponRangeType = Define.EWeaponRangeType.None;

        StartCoroutine(CoWaitLoad());
    }

    IEnumerator CoWaitLoad()
    {
        // TEMP
        Managers.Object.SpawnPlayer("Player", Vector2.up * 12f);
        int templateID = (Managers.Game.SelectedChapter - 1) * 20 + Managers.Game.SelectedStage;

        if (Managers.Data.Stages.TryGetValue(templateID, out StageData stageData) == false)
            yield break;

        _stageData = stageData;
        _stageMissionData = new Dictionary<int, MissionData>();
        _remainingTurn = _stageData.Turn;

        _bg.sprite = Managers.Resource.Load<Sprite>(_stageData.MapName);

        var gameSceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();

        _gameSceneUI = gameSceneUI;

        yield return null;

        for (int i = 0; i < _stageData.respawnData.Count; i++)
        {
            if (_stageData.respawnData[i].ClearCount <= 0)
                continue;

            _stageMissionData.Add(_stageData.respawnData[i].MonsterID,
                    new MissionData
                    {
                        currentCount = 0,
                        clearCount = _stageData.respawnData[i].ClearCount,
                    });
        }
        _gameSceneUI.SetInfo(_stageData.respawnData, _remainingTurn);

        Managers.Object.LoadStageData(_stageData);

        Managers.Sound.Play(Define.ESound.Bgm, "Sound_Battle1");

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(RespawnMonsters(Define.LAYER_COUNT - 1));
        _gameStart = true;
    }

    IEnumerator RespawnMonsters(int layer)
    {
        Managers.Object.ClearFreeIndex(layer);

        yield return StartCoroutine(Managers.Object.SpawnMonster(layer));
    }

    void Update()
    {
        if (!_gameStart)
            return;

        switch (State)
        {
            case Define.EBattleState.Ready:
                UpdateReady();
                break;
            case Define.EBattleState.PlayerInput:
                UpdatePlayerInput();
                break;
            case Define.EBattleState.PlayerAttack:
                UpdatePlayerAttack();
                break;
            case Define.EBattleState.BossAttack:
                UpdateBossAttack();
                break;
                //case BattleState.MonsterAttack:
                //	UpdateMonsterAttack();
                //	break;
        }
    }

    void UpdatePlayerInput()
    {

    }



    public void BossAttack()
    {
        if (Managers.Object.Boss == null)
        {
            State = Define.EBattleState.MonsterAttack;
            StartMonsterAttack();
            return;
        }

        Managers.Object.Boss.StartBossTurn();
    }

    void UpdateBossAttack()
    {
        if (Managers.Object.Boss.IsBusy)
            return;

        State = Define.EBattleState.MonsterAttack;
        StartMonsterAttack();
    }

    public void UpdatePlayerAttack()
    {
        if (Managers.Object.Player.IsBusy)
            return;

        if (CheckClearStage())
        {
            int killReward = Managers.Game.CurrentStageGetCoin;
            var popup = Managers.UI.ShowPopupUI<UI_GameClearPopup>();
            State = Define.EBattleState.GameOver;

            Managers.Game.GetStageCoin(_stageData.ClearGoldReward);

            Managers.Game.CheckBreakStageRecord();

            _gameSceneUI.Combo(0);
            return;
        }

        if (_currentSequenceCoroutine != null)
            return;

        _gameSceneUI.Combo(0);

        _remainingTurn--;

        _gameSceneUI.SetTurn(_remainingTurn);
        if (_remainingTurn <= 0)
        {
            GameOver();
            return;
        }
        State = Define.EBattleState.BossAttack;

        BossAttack();
    }

    void StartMonsterAttack()
    {
        Managers.Sound.Play(Define.ESound.Effect, "Sound_MonsterTurn");
        _currentSequenceCoroutine = MonsterAttackCo();

        StartCoroutine(_currentSequenceCoroutine);
    }

    IEnumerator MonsterAttackCo()
    {
        yield return new WaitForSeconds(0.1f);
        // 가까운 몬스터 순서대로 AI 시작하고 상태 전환
        var monsters = Managers.Object.GetMonsters();

        foreach (var monster in monsters)
            monster.StartMonsterTurn();

        foreach (MonsterController monster in monsters)
            yield return StartCoroutine(monster.StartMonsterAttack());

        while (true)
        {
            if (monsters.Count == monsters.FindAll(monster => !monster.IsBusy).Count)
            {
                break;
            }
            yield return null;
        }

        yield return StartCoroutine(RespawnMonsters(LAYER_COUNT - 1));
        State = Define.EBattleState.Ready;

        _currentSequenceCoroutine = null;
    }

    void UpdateReady()
    {
        var monsters = Managers.Object.GetMonsters();
        foreach (var monster in monsters)
        {
            if (monster.IsBusy)
                return;
        }
        State = Define.EBattleState.PlayerInput;
        Managers.Game.PlayerInput();
    }

    public void GameOver()
    {
        State = Define.EBattleState.GameOver;
        if (_currentSequenceCoroutine != null)
        {
            StopCoroutine(_currentSequenceCoroutine);
            _currentSequenceCoroutine = null;
        }

        _gameSceneUI.Combo(0);

        var monsters = Managers.Object.GetMonsters();
        foreach (MonsterController monster in monsters)
            monster.EndTurn();

        Managers.UI.ShowPopupUI<UI_GameOverPopup>();
    }

    public void RestartGame()
    {
        if (_currentSequenceCoroutine != null)
        {
            StopCoroutine(_currentSequenceCoroutine);
            _currentSequenceCoroutine = null;
        }

        Managers.Object.ResetStageObjects();

        OnChangeWeapon(Define.EWeaponRangeType.Short);

        Managers.Object.Player.RevivePlayer();

        StartCoroutine(RespawnMonsters(LAYER_COUNT - 1));
        State = Define.EBattleState.PlayerInput;
        Managers.Game.PlayerInput();
    }

    public void RevivePlayer()
    {
        if (_currentSequenceCoroutine != null)
        {
            StopCoroutine(_currentSequenceCoroutine);
            _currentSequenceCoroutine = null;
        }

        _remainingTurn += 5;

        Managers.Object.Player.RevivePlayer();
        State = Define.EBattleState.PlayerInput;
        Managers.Game.PlayerInput();
    }

    public void PlayerAttackStart()
    {
        List<MonsterController> monsters = Managers.Object.GetLowestHpSelectedMonsters();
        Managers.Object.Player.StartAttack(monsters);

        State = Define.EBattleState.PlayerAttack;
    }

    public override void Clear()
    {
    }



    public bool CheckClearStage()
    {
        for (int i = 0; i < _stageData.respawnData.Count; i++)
        {
            int monsterID = _stageData.respawnData[i].MonsterID;

            if (!Check_ClearMonster(monsterID))
                return false;

        }

        Debug.Log("Game Clear");
        return true;
    }

    bool Check_ClearMonster(int monsterTemplateID)
    {
        if (!_stageMissionData.TryGetValue(monsterTemplateID, out MissionData missionData))
            return true;

        if (missionData.currentCount >= missionData.clearCount)
            return true;
        else
            return false;
    }

    public int CountRemainMonster(MonsterController mc)
    {
        if (!_stageMissionData.TryGetValue(mc.TemplateID, out MissionData missionData))
            return 0;

        missionData.currentCount++;
        return missionData.clearCount - missionData.currentCount;
    }

    #region EventHandler
    void OnChangeWeapon(Define.EWeaponRangeType weaponType)
    {
        switch (weaponType)
        {
            case Define.EWeaponRangeType.None:
                _shortRangeRadar.gameObject.SetActive(false);
                _middleRangeRadar.gameObject.SetActive(false);
                _longRangeRadar.gameObject.SetActive(false);
                break;
            case Define.EWeaponRangeType.Short:
                _shortRangeRadar.gameObject.SetActive(true);
                _shortRangeRadar.Rotate();
                _middleRangeRadar.gameObject.SetActive(false);
                _longRangeRadar.gameObject.SetActive(false);
                break;
            case Define.EWeaponRangeType.Middle:
                _shortRangeRadar.gameObject.SetActive(false);
                _middleRangeRadar.gameObject.SetActive(true);
                _middleRangeRadar.Rotate();
                _longRangeRadar.gameObject.SetActive(false);
                break;
            case Define.EWeaponRangeType.Long:
                _shortRangeRadar.gameObject.SetActive(false);
                _middleRangeRadar.gameObject.SetActive(false);
                _longRangeRadar.gameObject.SetActive(true);
                _longRangeRadar.Rotate();
                break;
        }
        Managers.Object.Player?.ChangeWeaponImage();
    }

    void OnPlayerAttack()
    {
        if (State != Define.EBattleState.PlayerInput)
            return;

        switch (Managers.Game.WeaponType)
        {
            case Define.EWeaponRangeType.None:
                return;
            case Define.EWeaponRangeType.Short:
                _shortRangeRadar.RadarFadeInOut();
                break;
            case Define.EWeaponRangeType.Middle:
                _middleRangeRadar.RadarFadeInOut();
                break;
            case Define.EWeaponRangeType.Long:
                _longRangeRadar.RadarFadeInOut();
                break;
        }
    }

    void OnMonsterDead(MonsterController mc)
    {
        Managers.Object.DespawnMonster(mc);
    }

    void OnEnable()
    {
        if (Managers.Game == null)
            return;

        Managers.Game.OnChangeWeapon -= OnChangeWeapon;
        Managers.Game.OnChangeWeapon += OnChangeWeapon;
        Managers.Game.OnPlayerAttack -= OnPlayerAttack;
        Managers.Game.OnPlayerAttack += OnPlayerAttack;
        Managers.Game.OnMonsterDead -= OnMonsterDead;
        Managers.Game.OnMonsterDead += OnMonsterDead;
    }

    void OnDisable()
    {
        if (Managers.Game == null)
            return;

        Managers.Game.OnChangeWeapon -= OnChangeWeapon;
        Managers.Game.OnPlayerAttack -= OnPlayerAttack;
        Managers.Game.OnMonsterDead -= OnMonsterDead;
        Managers.Object.Player = null;
    }
    #endregion
}