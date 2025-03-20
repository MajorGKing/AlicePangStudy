using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StageBlock : UI_SubItem
{
    enum GameObjects
    {
        StageTileSpine,
        ClearFlag,
    }

    enum Texts
    {
        StageText,
    }

    enum Buttons
    {
        SelectStageButton,
    }
    enum Images
    {
        Shadow
    }
    int _stage;
    UI_SelectStageScene _selectStageSceneUI;

    protected override void Awake()
    {
        base.Awake();

        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindObjects(typeof(GameObjects));
        BindImages(typeof(Images));

        GetButton((int)Buttons.SelectStageButton).gameObject.BindEvent(OnClickStageButton);
    }

    public void SetInfo(int stage, UI_SelectStageScene scene)
    {
        _stage = stage;
        _selectStageSceneUI = scene;

        RefreshUI();
    }

    private void RefreshUI()
    {

    }

    private void OnClickStageButton(PointerEventData evt)
    {

    }
}
