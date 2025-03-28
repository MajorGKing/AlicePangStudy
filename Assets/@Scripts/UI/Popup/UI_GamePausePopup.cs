using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GamePausePopup : UI_Popup
{
    enum GameObjects
    {
        BG,
    }

    enum Buttons
    {
        ExitButton,
        ContinueButton,
    }

    protected override void Awake()
    {
        base.Awake();

        BindButtons(typeof(Buttons));
        BindObjects(typeof(GameObjects));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
        GetButton((int)Buttons.ContinueButton).gameObject.BindEvent(OnClickContinueButton);

        GetObject((int)GameObjects.BG).gameObject.BindEvent(OnClickContinueButton);
        Managers.Sound.Play(Define.ESound.Effect, "Sound_HomeButton");
    }

    #region EventHandler
    void OnClickExitButton(PointerEventData ect)
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Scene.LoadScene(Define.EScene.SelectStageScene);
    }

    void OnClickContinueButton(PointerEventData ect)
    {
        Managers.UI.ClosePopupUI(this);
    }
    #endregion
}
