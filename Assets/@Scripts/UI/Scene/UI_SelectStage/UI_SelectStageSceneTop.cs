using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.CoinPlusButton).gameObject.BindEvent(OnClickCoinPlusButton);
        GetButton((int)Buttons.DiaPlusButton).gameObject.BindEvent(OnClickDiaPlusButton);

        RefreshUI();

    }

    public void SetInfo(UI_SelectStageScene sceneUI)
    {
        if (_init != true)
            return;

        _selectStageSceneUI = sceneUI;
        RefreshUI();
    }

    public void RefreshUI()
    {
        GetText((int)Texts.CoinText).text = Managers.Game.Coin.ToString();
        GetText((int)Texts.DiaText).text = Managers.Game.Dia.ToString();
    }

    #region EventHandler
    void OnClickCoinPlusButton(PointerEventData evt)
    {
        Managers.Game.Coin += 10;
        RefreshUI();
    }

    void OnClickDiaPlusButton(PointerEventData evt)
    {
        Managers.Game.Dia += 10;
        RefreshUI();
    }
    #endregion
}
