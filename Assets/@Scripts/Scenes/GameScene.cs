using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameScene : BaseScene
{
    class MissionData
    {
        public int currentCount;
        public int clearCount;
    }

    Dictionary<int, MissionData> _stageMissionData;
    int _remainingTurn;

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

    Define.EBattleState _state { get; set; } = Define.EBattleState.Ready;

    public Define.EBattleState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (value)
            {
                //case Define.EBattleState.Ready:
                //    _gameSceneUI.ShowWeaponButtons(false);
                //    OnChangeWeapon(WeaponRangeType.None);
                //    break;
                //case Define.EBattleState.PlayerInput:
                //    _gameSceneUI.ShowWeaponButtons(true);
                //    OnChangeWeapon(Managers.Game.WeaponType);
                //    break;
                //case Define.EBattleState.PlayerAttack:
                //    _gameSceneUI.ShowWeaponButtons(false);
                //    OnChangeWeapon(WeaponRangeType.None);
                //    break;
                //case Define.EBattleState.BossAttack:
                //    _gameSceneUI.ShowWeaponButtons(false);
                //    OnChangeWeapon(WeaponRangeType.None);
                //    break;
                //case Define.EBattleState.MonsterAttack:
                //    _gameSceneUI.ShowWeaponButtons(false);
                //    OnChangeWeapon(WeaponRangeType.None);
                //    break;
                //case Define.EBattleState.GameOver:
                //    _gameSceneUI.ShowWeaponButtons(false);
                //    OnChangeWeapon(WeaponRangeType.None);
                //    break;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        gameObject.AddComponent<CaptureScreenShot>();
#endif

        Debug.Log("@>> GameScene Init()");
        SceneType = Define.EScene.GameScene;

    }
    
    public override void Clear()
    {
    }

    public void PlayerAttackStart()
    {

    }
}