using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_WeaponInventoryItem : UI_SubItem
{
    enum GameObjects
    {
        XPFill,
        XPFill_Full,
        XPIcon,
        XPIcon_Full,
        ExpFullIcon,
        Block,
    }

    enum Buttons
    {
        WeaponInfoButton,
    }

    enum Images
    {
        WeaponImage,
    }

    enum Texts
    {
        LevelText,
        DamageText,
        ExpText,
    }

    enum Sliders
    {
        ExpSlider,
    }

    public WeaponData WeaponData { get; private set; } = new WeaponData();

    protected override void Awake()
    {
        Debug.Log("UI_WeaponInventoryItem Awake");

        base.Awake();

        BindButtons(typeof(Buttons));
        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindImages(typeof(Images));
        BindSliders(typeof(Sliders));

        GetButton((int)Buttons.WeaponInfoButton).gameObject.BindEvent(ShowPopup);

        RefreshUI();
    }

    public void SetInfo(WeaponData weaponData)
    {
        Debug.Log("UI_WeaponInventoryItem Set Info");

        WeaponData = weaponData;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        if (WeaponData == null)
            return;

        int weaponIndex = WeaponData.TemplateID - 1;
        if (weaponIndex < 0 || weaponIndex >= Managers.Game.WeaponLevel.Length)
        {
            Debug.Log($"Invalid Index {weaponIndex}");
            return;
        }

        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];

        GetText((int)Texts.LevelText).text = $"LV{weaponLevel}";
        if (weaponLevel == 0)
        {
            //GetImage((int)Images.WeaponImage).color = Color.gray;
            GetText((int)Texts.ExpText).gameObject.SetActive(false);
            GetSlider((int)Sliders.ExpSlider).gameObject.SetActive(false);
            GetObject((int)GameObjects.Block).SetActive(true);
            ExpFullResource(false);
        }
        else if (weaponLevel < WeaponData.WeaponLevelData.Count)
        {
            //GetImage((int)Images.WeaponImage).color = Color.white;
            GetText((int)Texts.ExpText).text = $"{Managers.Game.WeaponExp[weaponIndex]} / {WeaponData.WeaponLevelData[weaponLevel - 1].Exp}";
            GetSlider((int)Sliders.ExpSlider).value = Managers.Game.WeaponExp[weaponIndex] / (float)WeaponData.WeaponLevelData[weaponLevel - 1].Exp;
            if (Managers.Game.WeaponExp[weaponIndex] >= WeaponData.WeaponLevelData[weaponLevel - 1].Exp)
                ExpFullResource(true);
            else
                ExpFullResource(false);

            GetText((int)Texts.DamageText).text = WeaponData.WeaponLevelData[weaponLevel - 1].Damage.ToString();
            GetText((int)Texts.ExpText).gameObject.SetActive(true);
            GetSlider((int)Sliders.ExpSlider).gameObject.SetActive(true);
            GetObject((int)GameObjects.Block).SetActive(false);
        }
        else
        {
            //GetImage((int)Images.WeaponImage).color = Color.white;
            GetText((int)Texts.ExpText).text = $"MAX";
            GetSlider((int)Sliders.ExpSlider).value = 1f;
            GetText((int)Texts.DamageText).text = WeaponData.WeaponLevelData[weaponLevel - 1].Damage.ToString();
            GetText((int)Texts.ExpText).gameObject.SetActive(true);
            GetSlider((int)Sliders.ExpSlider).gameObject.SetActive(true);
            GetObject((int)GameObjects.Block).SetActive(false);
        }

        GetImage((int)Images.WeaponImage).sprite = Managers.Resource.Load<Sprite>(WeaponData.Sprite);
    }

    void ExpFullResource(bool isFull)
    {
        GetObject((int)GameObjects.ExpFullIcon).SetActive(isFull);
        GetObject((int)GameObjects.XPFill).SetActive(!isFull);
        GetObject((int)GameObjects.XPFill_Full).SetActive(isFull);
        GetObject((int)GameObjects.XPIcon).SetActive(!isFull);
        GetObject((int)GameObjects.XPIcon_Full).SetActive(isFull);
    }

    #region EventHandler
    private void ShowPopup(PointerEventData evt)
    {
        var popup = Managers.UI.ShowPopupUI<UI_WeaponInfoPopup>();
        popup.SetInfo(WeaponData, this);
    }
    #endregion
}
