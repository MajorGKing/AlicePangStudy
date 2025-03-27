using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_WeaponInfoPopup : UI_Popup
{
    enum GameObjects
    {
        Block,
        XPFill_Before,
        XPFill_After,
        XPFill_Full_Before,
        XPFill_Full_After,
        XPIcon_Before,
        XPIcon_After,
        AfterWeaponInfo,
        BeforeWeaponInfo,
        DisableAfterSlot,
        DisableUpgradeButton,
        DisableEquipButton,
        Popup,
    }

    enum Images
    {
        WeaponImage_Before,
        WeaponImage_After,
    }

    enum Texts
    {
        LevelText_Before,
        LevelText_After,
        CostText,
        ExpText_Before,
        ExpText_After,
        DamageText_Before,
        DamageText_After,
        WeaponNameText,
        EquipText,
    }

    enum Buttons
    { 
        WeaponEquipButton,
        WeaponUpgradeButton,
        CloseButton, 
        
    }

    enum Sliders
    {
        ExpSlider_Before,
        ExpSlider_After,
    }

    WeaponData _weaponData;
    UI_WeaponInventoryItem _weaponInventoryItemUI;
    int _index;

    bool isActive = false;

    protected override void Awake()
    {
        Debug.Log("UI_WeaponInfoPopup Awake");

        base.Awake();

        BindObjects(typeof(GameObjects));
        BindImages(typeof(Images));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)Buttons.WeaponEquipButton).gameObject.BindEvent(OnClickEquipButton);
        GetButton((int)Buttons.WeaponUpgradeButton).gameObject.BindEvent(OnClickUpgradeButton);

        GetObject((int)GameObjects.Block).gameObject.BindEvent(OnClickCloseButton);
    }

    public void SetInfo(WeaponData weaponData, UI_WeaponInventoryItem weaponInventoryItemUI)
    {
        Debug.Log("UI_WeaponInfoPopup SetInfo");

        _weaponData = weaponData;
        _weaponInventoryItemUI = weaponInventoryItemUI;

        RefreshUI();
    }

    private void RefreshUI()
    {
        int weaponIndex = _weaponData.TemplateID - 1;
        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];

        GetText((int)Texts.LevelText_Before).text = $"LV{Managers.Game.WeaponLevel[weaponIndex]}";
        GetText((int)Texts.WeaponNameText).text = _weaponData.NameID;

        GetImage((int)Images.WeaponImage_Before).sprite = Managers.Resource.Load<Sprite>(_weaponData.Sprite);
        GetImage((int)Images.WeaponImage_After).sprite = Managers.Resource.Load<Sprite>(_weaponData.Sprite);

        if (weaponLevel == 0)
        {
            GetText((int)Texts.ExpText_Before).gameObject.SetActive(false);
            GetText((int)Texts.ExpText_After).gameObject.SetActive(false);
            GetSlider((int)Sliders.ExpSlider_Before).gameObject.SetActive(false);
            GetSlider((int)Sliders.ExpSlider_After).gameObject.SetActive(false);
            GetText((int)Texts.DamageText_Before).gameObject.SetActive(false);
            GetText((int)Texts.DamageText_After).gameObject.SetActive(false);
            GetText((int)Texts.CostText).gameObject.SetActive(false);
        }
        else if (weaponLevel < _weaponData.WeaponLevelData.Count)
        {
            GetText((int)Texts.LevelText_After).text = $"LV{Managers.Game.WeaponLevel[weaponIndex] + 1}";

            GetText((int)Texts.ExpText_Before).text = $"{Managers.Game.WeaponExp[weaponIndex]} / {_weaponData.WeaponLevelData[weaponLevel - 1].Exp}";
            GetSlider((int)Sliders.ExpSlider_Before).value = Managers.Game.WeaponExp[weaponIndex] / (float)_weaponData.WeaponLevelData[weaponLevel - 1].Exp;
            if (weaponLevel < _weaponData.WeaponLevelData.Count - 1)
            {
                GetText((int)Texts.ExpText_After).text = $"{Managers.Game.WeaponExp[weaponIndex] - _weaponData.WeaponLevelData[weaponLevel - 1].Exp} / {_weaponData.WeaponLevelData[weaponLevel].Exp}";
                GetSlider((int)Sliders.ExpSlider_After).value = (Managers.Game.WeaponExp[weaponIndex] - _weaponData.WeaponLevelData[weaponLevel - 1].Exp) / (float)_weaponData.WeaponLevelData[weaponLevel].Exp;
                ExpFullResource(Managers.Game.WeaponExp[weaponIndex] >= _weaponData.WeaponLevelData[weaponLevel - 1].Exp, (Managers.Game.WeaponExp[weaponIndex] - _weaponData.WeaponLevelData[weaponLevel - 1].Exp) >= _weaponData.WeaponLevelData[weaponLevel].Exp);

                if(CanUpgrade())
                {
                    Debug.Log("Can Upgrade");
                    GetObject((int)GameObjects.AfterWeaponInfo).GetComponent<DOTweenAnimation>().DOPlay();
                }
                else
                {
                    Debug.Log("Can not Upgrade");
                    GetObject((int)GameObjects.AfterWeaponInfo).GetComponent<DOTweenAnimation>().DOPause();
                }
            }
            else
            {
                GetObject((int)GameObjects.AfterWeaponInfo).SetActive(false);
                ExpFullResource(Managers.Game.WeaponExp[weaponIndex] >= _weaponData.WeaponLevelData[weaponLevel - 1].Exp);
            }

            int currentLevelDamage = _weaponData.WeaponLevelData[weaponLevel - 1].Damage;
            int nextLevelDamage = _weaponData.WeaponLevelData[weaponLevel].Damage;
            GetText((int)Texts.DamageText_Before).text = $"{currentLevelDamage}";
            GetText((int)Texts.DamageText_After).text = $"{nextLevelDamage}";

            GetText((int)Texts.CostText).text = $"{_weaponData.WeaponLevelData[weaponLevel - 1].Cost}";
            GetButton((int)Buttons.WeaponUpgradeButton).interactable = CanUpgrade();
        }
        else
        {
            GetObject((int)GameObjects.AfterWeaponInfo).SetActive(false);
            GetText((int)Texts.LevelText_After).text = $"MAX";
            GetText((int)Texts.ExpText_Before).text = $"MAX";
            GetSlider((int)Sliders.ExpSlider_Before).value = 1f;
            GetText((int)Texts.DamageText_Before).text = $"{_weaponData.WeaponLevelData[weaponLevel - 1].Damage}";
            GetText((int)Texts.CostText).text = $"MAX";
            GetButton((int)Buttons.WeaponUpgradeButton).interactable = false;
        }

        if (_weaponData.TemplateID == Managers.Game.ShortRangeWeaponID || _weaponData.TemplateID == Managers.Game.MiddleRangeWeaponID || _weaponData.TemplateID == Managers.Game.LongRangeWeaponID)
        {
            GetText((int)Texts.EquipText).text = "장착중";
            GetObject((int)GameObjects.DisableEquipButton).SetActive(true);
        }
        else
        {
            GetText((int)Texts.EquipText).text = "장착";
            GetObject((int)GameObjects.DisableEquipButton).SetActive(false);
        }

        if (isActive == false)
        {
            GetObject((int)GameObjects.Popup).transform.DOScale(1f, 0.2f).From(0f);
            isActive = true;
        }
        else
        {
            GetText((int)Texts.DamageText_Before).transform.DOScale(1f, 0.2f).From(0f);
        }
    }

    private void ExpFullResource(bool isFull_Before, bool isFull_After = false)
    {
        GetObject((int)GameObjects.XPFill_Before).SetActive(!isFull_Before);
        GetObject((int)GameObjects.XPFill_Full_Before).SetActive(isFull_Before);
        GetObject((int)GameObjects.XPIcon_Before).SetActive(!isFull_Before);
        if (isFull_Before)
        {
            GetObject((int)GameObjects.XPFill_After).SetActive(!isFull_After);
            GetObject((int)GameObjects.XPFill_Full_After).SetActive(isFull_After);
            GetObject((int)GameObjects.XPIcon_After).SetActive(!isFull_After);
            GetObject((int)GameObjects.DisableAfterSlot).SetActive(false);
        }
        else
        {
            GetSlider((int)Sliders.ExpSlider_After).gameObject.SetActive(false);
            GetObject((int)GameObjects.DisableAfterSlot).SetActive(true);
        }
    }

    private bool CanUpgrade()
    {
        int weaponIndex = _weaponData.TemplateID - 1;
        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];

        int needExp = _weaponData.WeaponLevelData[weaponLevel - 1].Exp;
        int haveExp = Managers.Game.WeaponExp[weaponIndex];

        if (needExp > haveExp)
        {
            GetObject((int)GameObjects.DisableUpgradeButton).SetActive(true);
            return false;
        }

        if (Managers.Game.CheckCoin(_weaponData.WeaponLevelData[weaponLevel - 1].Cost) == false)
        {
            GetObject((int)GameObjects.DisableUpgradeButton).SetActive(true);
            return false;
        }

        GetObject((int)GameObjects.DisableUpgradeButton).SetActive(false);
        return true;
    }

    #region EventHandler
    private void OnClickCloseButton(PointerEventData evt)
    {
        Managers.Sound.Play(Define.ESound.Effect, "Sound_HomeButton");
        Managers.UI.ClosePopupUI(this);
        Managers.Game.SaveGame();
    }

    private void OnClickEquipButton(PointerEventData evt)
    {
        if (_weaponData.TemplateID == Managers.Game.ShortRangeWeaponID || _weaponData.TemplateID == Managers.Game.MiddleRangeWeaponID || _weaponData.TemplateID == Managers.Game.LongRangeWeaponID)
            return;

        int weaponIndex = _weaponData.TemplateID - 1;
        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];
        int weaponTemplateId = _weaponData.TemplateID;

        if (weaponLevel < 1)
            return;

        switch (_weaponData.RangeType)
        {
            case Define.EWeaponRangeType.Short:
                Managers.Game.ShortRangeWeaponID = weaponTemplateId;
                break;
            case Define.EWeaponRangeType.Middle:
                Managers.Game.MiddleRangeWeaponID = weaponTemplateId;
                break;
            default:
                Managers.Game.LongRangeWeaponID = weaponTemplateId;
                break;
        }

        (Managers.UI.SceneUI as UI_SelectStageScene).InventoryPopupUI.SetInfo();
        Managers.UI.ClosePopupUI(this);
        Managers.Sound.Play(Define.ESound.Effect, "Sound_EquipButton");
        Managers.Game.SaveGame();
    }

    private void OnClickUpgradeButton(PointerEventData evt)
    {
        int weaponIndex = _weaponData.TemplateID - 1;
        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];

        if (CanUpgrade() == false)
            return;

        // Coin사용
        Managers.Game.SpendCoin(_weaponData.WeaponLevelData[weaponLevel - 1].Cost);

        int needExp = _weaponData.WeaponLevelData[weaponLevel - 1].Exp;

        Managers.Game.WeaponExp[weaponIndex] -= needExp;
        Managers.Game.WeaponLevel[weaponIndex]++;

        Managers.Game.SaveGame();
        Managers.Sound.Play(Define.ESound.Effect, "Sound_UpgradeWeapon");
        
        RefreshUI();
        _weaponInventoryItemUI.RefreshUI();
    }
    #endregion
}
