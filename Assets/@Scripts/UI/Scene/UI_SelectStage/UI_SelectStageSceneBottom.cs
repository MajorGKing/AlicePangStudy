using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.InventoryButton).gameObject.BindEvent(OnClickInventoryButton);
        GetButton((int)Buttons.PlayButton).gameObject.BindEvent(OnClickPlayButton);
        GetButton((int)Buttons.ShopButton).gameObject.BindEvent(OnClickShopButton);
    }

    public void SetInfo(UI_SelectStageScene sceneUI)
    {
        _selectStageSceneUI = sceneUI;
    }

    void RefreshUI()
    {

    }

    #region EventHandler
    void OnClickInventoryButton(PointerEventData evt)
    {
        var popup = Managers.UI.ShowPopupUI<UI_InventoryPopup>();
        _selectStageSceneUI.InventoryPopupUI = popup;
    }

    void OnClickPlayButton(PointerEventData evt)
    {
        _selectStageSceneUI.ShowStartStagePopup();
    }

    void OnClickShopButton(PointerEventData evt)
    {
        Managers.UI.ShowPopupUI<UI_ShopPopup>();
    }
    #endregion
}
