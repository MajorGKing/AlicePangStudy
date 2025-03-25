using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SelectStageSceneTop : UI_Base
{
    enum Texts
    {
        CoinText,
        DiaText,
    }

    enum Buttons
    {
        CoinPlusButton,
        DiaPlusButton,
    }

    UI_SelectStageScene _selectStageSceneUI;


    protected override void Awake()
    {
        base.Awake();


    }

    public void SetInfo(UI_SelectStageScene sceneUI)
    {
        _selectStageSceneUI = sceneUI;
        Refresh();
    }

    public void Refresh()
    {

    }
}
