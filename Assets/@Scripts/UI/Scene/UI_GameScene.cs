using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;


public class UI_GameScene : UI_Scene
{
    #region Enum
    enum GameObjects
    {
        MissionPanel,
        CoinObject,
        TouchArea,
    }

    enum Buttons
    {
        ShortRangeButton,
        MiddleRangeButton,
        LongRangeButton,
        SelectedShortRangeButton,
        SelectedMiddleRangeButton,
        SelectedLongRangeButton,
        PauseButton,
    }

    enum Images
    {
        ShortRangeWeaponImage,
        MiddleRangeWeaponImage,
        LongRangeWeaponImage,
        SelectedShortRangeWeaponImage,
        SelectedMiddleRangeWeaponImage,
        SelectedLongRangeWeaponImage,
    }

    enum Texts
    {
        ComboText,
        CoinText,
        TurnText,
        ShortRangeWeaponDamageText,
        MiddleRangeWeaponDamageText,
        LongRangeWeaponDamageText,
        SelectedShortRangeWeaponDamageText,
        SelectedMiddleRangeWeaponDamageText,
        SelectedLongRangeWeaponDamageText,
    }
    #endregion

    public Dictionary<int, UI_MissionMonsterItem> MissionMonsterItems;
    List<RespawnData> _respawnData;

    private float elapsedTime;
    private float updateInterval = 0.3f;

    int _showCoin = 0;
    int _remainingTurn;

    public int ShowCoin
    {
        get { return _showCoin; }

        set
        {
            _showCoin = value;
            SetCoinText(_showCoin);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));
        BindImages(typeof(Images));

        GetButton((int)Buttons.ShortRangeButton).gameObject.BindEvent(OnClickShortRangeButton);
        GetButton((int)Buttons.MiddleRangeButton).gameObject.BindEvent(OnClickMiddleRangeButton);
        GetButton((int)Buttons.LongRangeButton).gameObject.BindEvent(OnClickLongRangeButton);

        GetButton((int)Buttons.SelectedShortRangeButton).gameObject.BindEvent(OnClickShortRangeButton);
        GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.BindEvent(OnClickMiddleRangeButton);
        GetButton((int)Buttons.SelectedLongRangeButton).gameObject.BindEvent(OnClickLongRangeButton);

        GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(false);
        GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(false);
        GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(false);

        GetButton((int)Buttons.PauseButton).gameObject.BindEvent(OnClickShowPausePopup);

        GetObject((int)GameObjects.TouchArea).BindEvent(OnClickTouchArea);

        ShowWeaponButtons(false);

        SetCoinText(0);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= updateInterval)
        {
            float fps = 1.0f / Time.deltaTime;
            float ms = Time.deltaTime * 1000.0f;
            string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);
            // GetText((int)Texts.FpsText).text = text;

            elapsedTime = 0;
        }
    }

    public void SetInfo(List<RespawnData> respawnData, int turn)
    {
        _respawnData = respawnData;
        _remainingTurn = turn;
        RefreshUI();
    }

    void RefreshUI()
    {

    }

    public void ChangeSelectedButton(Define.EWeaponRangeType weaponRangeType)
    {

    }
    public void DisableWeaponButton(Define.EWeaponRangeType disableWeaponRangeType)
    {

    }

    public void Combo(int comboCount)
    {

    }

    public void SetCoinText(int coin)
    {
        GetText((int)Texts.CoinText).text = coin.ToString();
    }

    public void MonsterDead(int monsterTemplateID, int remainMonster)
    {

    }

    public void ShowWeaponButtons(bool show)
    {
        GetButton((int)Buttons.ShortRangeButton).gameObject.SetActive(show);
        GetButton((int)Buttons.MiddleRangeButton).gameObject.SetActive(show);
        GetButton((int)Buttons.LongRangeButton).gameObject.SetActive(show);
        
    }
    bool isShakeMission = false;
    public void SetTurn(int turn)
    {
        // TODO ILHAK
    }

    #region EventHandler
    void OnClickShortRangeButton(PointerEventData evt)
    {

    }

    void OnClickMiddleRangeButton(PointerEventData evt)
    {

    }

    void OnClickLongRangeButton(PointerEventData evt)
    {

    }

    void OnClickShowPausePopup(PointerEventData evt)
    {

    }

    void OnClickTouchArea(PointerEventData evt)
    {

    }
    #endregion
}