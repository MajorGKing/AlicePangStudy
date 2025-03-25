using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SelectStageSceneBottom : UI_Base
{
    enum Buttons
    {
        InventoryButton,
        PlayButton,
        ShopButton,
    }

    UI_SelectStageScene _selectStageSceneUI;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetInfo(UI_SelectStageScene sceneUI)
    {
        _selectStageSceneUI = sceneUI;
    }
}
