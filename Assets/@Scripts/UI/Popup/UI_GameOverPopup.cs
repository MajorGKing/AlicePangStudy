using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GameOverPopup : UI_Popup
{
    enum GameObjects
    {
        TicketImage,
        ADImage,
    }

    enum Images
    {

    }

    enum Buttons
    {
        ReviveButton,
        ExitButton,
    }

    enum Texts
    {
        StageText,
    }

    protected override void Awake()
    {
        base.Awake();

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.ReviveButton).gameObject.BindEvent(OnClickReviveButton);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);

        Managers.Sound.Clear();
        Managers.Sound.Play(Define.ESound.Effect, "Sound_GameOverFirst");
        Managers.Sound.Play(Define.ESound.Effect, "Sound_GameOver");
    }

    #region EventHandler
    void OnClickReviveButton(PointerEventData evt)
    {
        Debug.Log("OnReviveButton");
        //???? ????
        Managers.UI.ClosePopupUI(this);
        (Managers.Scene.CurrentScene as GameScene).RevivePlayer();
        Managers.Sound.Play(Define.ESound.Bgm, "Sound_Battle1");
    }

    //void OnClickRestartButton()
    //{
    //    Debug.Log("OnRestartButton");
    //    (Managers.Scene.CurrentScene as GameScene).RestartGame();
    //    Managers.UI.ClosePopupUI(this);
    //}

    void OnClickExitButton(PointerEventData evt)
    {
        Debug.Log("OnExitButton");
        Managers.UI.ClosePopupUI(this);
        Managers.Scene.LoadScene(Define.EScene.SelectStageScene);
    }
    #endregion
}
