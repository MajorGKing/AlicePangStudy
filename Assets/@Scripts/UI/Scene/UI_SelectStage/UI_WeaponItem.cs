using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_WeaponItem : UI_SubItem
{
    enum Texts
    {
        DamageText,
    }
    enum Images
    {
        WeaponImage,
    }

    enum Buttons
    {
        WeaponInfoButton,
    }

    WeaponData _weaponData;

    protected override void Awake()
    {
        base.Awake();

        BindTexts(typeof(Texts));
        BindImages(typeof(Images));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.WeaponInfoButton).gameObject.BindEvent(OnClickWeaponInfoButton);
    }

    public void SetInfo(WeaponData weaponData)
    {
        _weaponData = weaponData;

        RefreshUI();
    }

    void RefreshUI()
    {
        GetImage((int)Images.WeaponImage).sprite = Managers.Resource.Load<Sprite>(_weaponData.Sprite);

        int templateID = _weaponData.TemplateID;
        int level = Managers.Game.WeaponLevel[templateID - 1];
        int damage = Managers.Data.Weapons[templateID].WeaponLevelData[level - 1].Damage;
        GetText((int)Texts.DamageText).text = damage.ToString();
    }

    #region EventHandler
    void OnClickWeaponInfoButton(PointerEventData evt)
    {
        var popup = Managers.UI.ShowPopupUI<UI_InventoryPopup>();
        (Managers.UI.SceneUI as UI_SelectStageScene).InventoryPopupUI = popup;
    }
    #endregion
}
