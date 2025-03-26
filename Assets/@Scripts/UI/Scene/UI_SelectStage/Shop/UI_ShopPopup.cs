
using System.Diagnostics;
using System.Drawing;
using UnityEngine.EventSystems;

public class UI_ShopPopup : UI_Popup
{
    enum GameObjects
    {
        CoinProductScroll,
        DiaProductScroll,
        WeaponGacha,
        SelectCoinTab,
        SelectDiaTab,
        SelectWeaponTab,
        Gacha1_Normal,
        Gacha1_Pressed,
        Gacha10_Normal,
        Gacha10_Pressed,
        Block,
    }

    enum Buttons
    {
        CoinTabButton,
        DiaTabButton,
        WeaponTabButton,
        CloseButton,
        Gacha1_Button,
        Gacha10_Button,
    }

    protected override void Awake()
    {
        base.Awake();

        Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonMain");

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        GetObject((int)GameObjects.Block).SetActive(false);

        GetButton((int)Buttons.CoinTabButton).gameObject.BindEvent(OnClickCoinTabButton);
        GetButton((int)Buttons.DiaTabButton).gameObject.BindEvent(OnClickDiaTabButton);
        GetButton((int)Buttons.WeaponTabButton).gameObject.BindEvent(OnClickWeaponTabButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        GetButton((int)Buttons.Gacha1_Button).gameObject.BindEvent(OnClickGacha1_Button, Define.ETouchEvent.Click);
        GetButton((int)Buttons.Gacha1_Button).gameObject.BindEvent(OnPressGacha1_Button, Define.ETouchEvent.PointerDown);
        GetButton((int)Buttons.Gacha1_Button).gameObject.BindEvent(OnReleaseGacha1_Button, Define.ETouchEvent.PointerUp);

        GetButton((int)Buttons.Gacha10_Button).gameObject.BindEvent(OnClickGacha10_Button, Define.ETouchEvent.Click);
        GetButton((int)Buttons.Gacha10_Button).gameObject.BindEvent(OnPressGacha10_Button, Define.ETouchEvent.PointerDown);
        GetButton((int)Buttons.Gacha10_Button).gameObject.BindEvent(OnReleaseGacha10_Button, Define.ETouchEvent.PointerUp);

        RefreshUI();
    }

    public void SetInfo()
    {

    }

    private void RefreshUI()
    {
        OnClickCoinTabButton();
    }

    #region EventHandler

    void OnClickCoinTabButton()
    {
        GetObject((int)GameObjects.SelectCoinTab).SetActive(true);
        GetObject((int)GameObjects.CoinProductScroll).SetActive(true);
        GetObject((int)GameObjects.SelectDiaTab).SetActive(false);
        GetObject((int)GameObjects.DiaProductScroll).SetActive(false);
        GetObject((int)GameObjects.SelectWeaponTab).SetActive(false);
        GetObject((int)GameObjects.WeaponGacha).SetActive(false);
    }
    void OnClickCoinTabButton(PointerEventData evt)
    {
        OnClickCoinTabButton();
    }

    void OnClickDiaTabButton(PointerEventData evt)
    {
        GetObject((int)GameObjects.SelectCoinTab).SetActive(false);
        GetObject((int)GameObjects.CoinProductScroll).SetActive(false);
        GetObject((int)GameObjects.SelectDiaTab).SetActive(true);
        GetObject((int)GameObjects.DiaProductScroll).SetActive(true);
        GetObject((int)GameObjects.SelectWeaponTab).SetActive(false);
        GetObject((int)GameObjects.WeaponGacha).SetActive(false);
    }

    void OnClickWeaponTabButton(PointerEventData evt)
    {
        GetObject((int)GameObjects.SelectCoinTab).SetActive(false);
        GetObject((int)GameObjects.CoinProductScroll).SetActive(false);
        GetObject((int)GameObjects.SelectDiaTab).SetActive(false);
        GetObject((int)GameObjects.DiaProductScroll).SetActive(false);
        GetObject((int)GameObjects.SelectWeaponTab).SetActive(true);
        GetObject((int)GameObjects.WeaponGacha).SetActive(true);

        GetObject((int)GameObjects.Gacha1_Pressed).SetActive(false);
        GetObject((int)GameObjects.Gacha1_Normal).SetActive(true);

        GetObject((int)GameObjects.Gacha10_Pressed).SetActive(false);
        GetObject((int)GameObjects.Gacha10_Normal).SetActive(true);
    }

    void OnClickCloseButton(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Sound.Play(Define.ESound.Effect, "Sound_HomeButton");
    }

    void OnClickGacha1_Button(PointerEventData evt)
    {
        UnityEngine.Debug.Log("OnClickGacha1_Button");

        GetObject((int)GameObjects.Block).SetActive(true);
        GetObject((int)GameObjects.Gacha1_Pressed).SetActive(false);
        GetObject((int)GameObjects.Gacha1_Normal).SetActive(true);

        //var popup = Managers.UI.ShowPopupUI<UI_WeaponGachaPopup>();
        //popup.SetInfo(10, GetObject((int)GameObjects.Block).SetActive(false));
    }

    void OnPressGacha1_Button(PointerEventData evt)
    {
        UnityEngine.Debug.Log("OnPressGacha1_Button");

        GetObject((int)GameObjects.Gacha1_Pressed).SetActive(true);
        GetObject((int)GameObjects.Gacha1_Normal).SetActive(false);
    }

    void OnReleaseGacha1_Button(PointerEventData evt)
    {
        UnityEngine.Debug.Log("OnReleaseGacha1_Button");

        if (GetObject((int)GameObjects.Gacha1_Pressed).activeInHierarchy == true)
        {
            GetObject((int)GameObjects.Gacha1_Pressed).SetActive(false);
            GetObject((int)GameObjects.Gacha1_Normal).SetActive(true);
        }
    }

    void OnClickGacha10_Button(PointerEventData evt)
    {
        UnityEngine.Debug.Log("OnClickGacha10_Button");

        GetObject((int)GameObjects.Block).SetActive(true);
        GetObject((int)GameObjects.Gacha10_Pressed).SetActive(false);
        GetObject((int)GameObjects.Gacha10_Normal).SetActive(true);

        //var popup = Managers.UI.ShowPopupUI<UI_WeaponGachaPopup>();
        //popup.SetInfo(10, GetObject((int)GameObjects.Block).SetActive(false));
    }

    void OnPressGacha10_Button(PointerEventData evt)
    {
        UnityEngine.Debug.Log("OnPressGacha10_Button");

        GetObject((int)GameObjects.Gacha10_Pressed).SetActive(true);
        GetObject((int)GameObjects.Gacha10_Normal).SetActive(false);
    }

    void OnReleaseGacha10_Button(PointerEventData evt)
    {
        if (GetObject((int)GameObjects.Gacha10_Pressed).activeInHierarchy == true)
        {
            GetObject((int)GameObjects.Gacha10_Pressed).SetActive(false);
            GetObject((int)GameObjects.Gacha10_Normal).SetActive(true);
        }
    }

    #endregion
}
