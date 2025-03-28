using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StartStagePopup : UI_Popup
{
    enum GameObjects
    {
        MissionListPanel,
        WeaponListPanel,
        Popup,
        Block,
    }

    enum Texts
    {
        StageText,
    }

    enum Buttons
    {
        StartButton,
        CloseButton,
    }

    StageData _stageData;

    UI_WeaponItem _shortRangeWeaponItemUI;
    UI_WeaponItem _middleRangeWeaponItemUI;
    UI_WeaponItem _longRangeWeaponItemUI;

    UI_MissionMonsterItem[] _missionMonsterItemUI = new UI_MissionMonsterItem[4];

    protected override void Awake()
    {
        base.Awake();

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        GetObject((int)GameObjects.Block).BindEvent(OnClickCloseButton);

        GameObject parent = GetObject((int)GameObjects.WeaponListPanel);
        {
            var item = Managers.UI.MakeSubItem<UI_WeaponItem>(parent.transform, "UI_WeaponItem");
            _shortRangeWeaponItemUI = item;
            _shortRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID]);
        }
        {
            var item = Managers.UI.MakeSubItem<UI_WeaponItem>(parent.transform, "UI_WeaponItem");
            _middleRangeWeaponItemUI = item;
            _middleRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID]);
        }
        {
            var item = Managers.UI.MakeSubItem<UI_WeaponItem>(parent.transform, "UI_WeaponItem");
            _longRangeWeaponItemUI = item;
            _longRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID]);
        }

    }

    public void SetInfo(StageData stageData)
    {
        _stageData = stageData;

        GameObject parent = GetObject((int)GameObjects.MissionListPanel);
        parent.DestroyChildren();
        List<RespawnData> respawnData = _stageData.respawnData;
        int missionCount = 0;
        for (int i = 0; i < respawnData.Count; i++)
        {
            if (respawnData[i].ClearCount > 0)
            {
                int index = i;
                int missionIndex = missionCount;

                var item = Managers.UI.MakeSubItem<UI_MissionMonsterItem>(parent.transform, "UI_MissionMonsterItem");
                _missionMonsterItemUI[missionIndex] = item;
                _missionMonsterItemUI[missionIndex]?.SetInfo(respawnData[index].MonsterID, respawnData[index].ClearCount);

                missionCount++;
            }
        }
            
        RefreshUI();
    }

    void RefreshUI()
    {
        _shortRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID]);
        _middleRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID]);
        _longRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID]);

        GetText((int)Texts.StageText).text = $"STAGE {_stageData.StageID}";

        //GetObject((int)GameObjects.Popup).transform.DOScale(1f, 0.2f).From(0f);
        GetObject((int)GameObjects.Popup).SetActive(false);
        //StartCoroutine(CoWaitLoad());
        GetObject((int)GameObjects.Popup).SetActive(true);
        GetObject((int)GameObjects.Popup).transform.DOScale(1f, 0.2f).From(0f);
    }

    //IEnumerator CoWaitLoad()
    //{
    //    yield return new WaitForSeconds(0.1f);

    //    GetObject((int)GameObjects.Popup).SetActive(true);
    //    GetObject((int)GameObjects.Popup).transform.DOScale(1f, 0.2f).From(0f);
    //}

    #region EventHandler
    void OnClickStartButton(PointerEventData evt)
    {
        Managers.Game.SelectedChapter = _stageData.ChapterID;
        Managers.Game.SelectedStage = _stageData.StageID;
        Managers.Game.SaveGame();
        Managers.Scene.LoadScene(Define.EScene.GameScene);

        Managers.Sound.Clear();
        Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonSelected");
    }

    void OnClickCloseButton(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Sound.Play(Define.ESound.Effect, "Sound_HomeButton");
    }
    #endregion
}