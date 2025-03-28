using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GameClearPopup : UI_Popup
{
    enum GameObjects
    {

    }

    enum Images
    {
        TicketImage,
        ADImage,
    }

    enum Texts
    {
        StageText,
        KillMonsterRewardText,
        ClearStageRewardText,
    }

    enum Buttons
    {
        DoubleRewardButton,
        ExitButton,
    }

    int _killReward;
    int _stageReward;
    int _stage;

    protected override void Awake()
    {
        base.Awake();

        BindObjects(typeof(GameObjects));
        BindImages(typeof(Images));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
        GetButton((int)Buttons.DoubleRewardButton).gameObject.BindEvent(OnClickDoubleRewardButton);

        RefreshUI();
    }

    public void SetInfo(int killReward, int stageReward, int stage)
    {
        _killReward = killReward;
        _stageReward = stageReward;
        _stage = stage;

        RefreshUI();
    }

    void RefreshUI()
    {
        if (_init == false)
            return;

        GetText((int)Texts.ClearStageRewardText).text = _stageReward.ToString();
        GetText((int)Texts.KillMonsterRewardText).text = _killReward.ToString();
        GetText((int)Texts.StageText).text = _stage.ToString();
        Managers.Sound.Clear();
        Managers.Sound.Play(Define.ESound.Effect, "Sound_StageClear");
    }

    #region EventHandler

    void OnClickDoubleRewardButton(PointerEventData evt)
    {
        // TODO ILHAK AD
        Debug.Log("On click show ads button");
    }

    void OnClickExitButton(PointerEventData evt)
    {
        Managers.Scene.LoadScene(Define.EScene.SelectStageScene);
    }

    #endregion
}
