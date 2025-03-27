using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InventoryPopup : UI_Popup
{
    enum GameObjects
    {
        BG,
        ShortRangeWeaponPanel,
        MiddleRangeWeaponPanel,
        LongRangeWeaponPanel,
        ShortRangeWeaponSlot,
        MiddleRangeWeaponSlot,
        LongRangeWeaponSlot,
    }

    enum Buttons
    {
        CloseButton,
    }

    Dictionary<int, UI_WeaponInventoryItem> _weaponInventoryItemUI = new Dictionary<int, UI_WeaponInventoryItem>();

    UI_WeaponInventoryItem _shortRangeWeaponItem;
    UI_WeaponInventoryItem _middleRangeWeaponItem;
    UI_WeaponInventoryItem _longRangeWeaponItem;

    int _shortRangeWeaponTemplateID;
    int _middleRangeWeaponTemplateID;
    int _longRangeWeaponTemplateID;

    protected override void Awake()
    {
        base.Awake();

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetObject((int)GameObjects.BG).SetActive(false);

        for (int i = 0; i < Define.WEAPON_COUNT; i++)
        {
            WeaponData weaponData = Managers.Data.Weapons[i + 1];
            Transform parent;
            switch (weaponData.RangeType)
            {
                case Define.EWeaponRangeType.Short:
                    parent = GetObject((int)GameObjects.ShortRangeWeaponPanel).transform;
                    break;
                case Define.EWeaponRangeType.Middle:
                    parent = GetObject((int)GameObjects.MiddleRangeWeaponPanel).transform;
                    break;
                default:
                    parent = GetObject((int)GameObjects.LongRangeWeaponPanel).transform;
                    break;
            }

            var item = Managers.UI.MakeSubItem<UI_WeaponInventoryItem>(parent, "UI_WeaponInventoryItem");
            item.SetInfo(weaponData);
            _weaponInventoryItemUI.Add(weaponData.TemplateID, item);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parent);
        }

        {
            var item = Managers.UI.MakeSubItem<UI_WeaponInventoryItem>(GetObject((int)GameObjects.ShortRangeWeaponSlot).transform, "UI_WeaponInventoryItem");
            _shortRangeWeaponItem = item;
            _shortRangeWeaponItem.SetInfo(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID]);
            _weaponInventoryItemUI[Managers.Game.ShortRangeWeaponID].gameObject.SetActive(true);
        }

        {
            var item = Managers.UI.MakeSubItem<UI_WeaponInventoryItem>(GetObject((int)GameObjects.MiddleRangeWeaponSlot).transform, "UI_WeaponInventoryItem");
            _middleRangeWeaponItem = item;
            _middleRangeWeaponItem.SetInfo(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID]);
            _weaponInventoryItemUI[Managers.Game.MiddleRangeWeaponID].gameObject.SetActive(true);
        }

        {
            var item = Managers.UI.MakeSubItem<UI_WeaponInventoryItem>(GetObject((int)GameObjects.LongRangeWeaponSlot).transform, "UI_WeaponInventoryItem");
            _longRangeWeaponItem = item;
            _longRangeWeaponItem.SetInfo(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID]);
            _weaponInventoryItemUI[Managers.Game.LongRangeWeaponID].gameObject.SetActive(true);
        }

        GetObject((int)GameObjects.BG).SetActive(true);
    }

    public void SetInfo()
    {

    }

    void RefreshUI()
    {
        
    }

    #region EventHandler
    void OnClickCloseButton(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Sound.Play(Define.ESound.Effect, "Sound_HomeButton");
    }
    #endregion
}
