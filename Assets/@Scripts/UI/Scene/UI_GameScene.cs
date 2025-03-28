using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;


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

    public void SetInfo(List<RespawnData> respawnData, int turn)
    {
        _respawnData = respawnData;
        _remainingTurn = turn;
        RefreshUI();
    }

    void RefreshUI()
    {
        MissionMonsterItems = new Dictionary<int, UI_MissionMonsterItem>();
        GameObject parent = GetObject((int)GameObjects.MissionPanel);

        for (int i = 0; i < _respawnData.Count; i++)
        {
            if (_respawnData[i].ClearCount <= 0)
            {
                continue;
            }

            {
                var item = Managers.UI.MakeSubItem<UI_MissionMonsterItem>(parent.transform);
                item.SetInfo(_respawnData[i].MonsterID, _respawnData[i].ClearCount);
                MissionMonsterItems.Add(_respawnData[i].MonsterID, item);
            }
        }

        GetImage((int)Images.ShortRangeWeaponImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID].Sprite);
        GetImage((int)Images.SelectedShortRangeWeaponImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID].Sprite);

        GetImage((int)Images.MiddleRangeWeaponImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID].Sprite);
        GetImage((int)Images.SelectedMiddleRangeWeaponImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID].Sprite);

        GetImage((int)Images.LongRangeWeaponImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID].Sprite);
        GetImage((int)Images.SelectedLongRangeWeaponImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID].Sprite);

        int templateID = Managers.Game.ShortRangeWeaponID;
        int level = Managers.Game.WeaponLevel[templateID - 1];
        int damage = Managers.Data.Weapons[templateID].WeaponLevelData[level - 1].Damage;
        GetText((int)Texts.ShortRangeWeaponDamageText).text = damage.ToString();
        GetText((int)Texts.SelectedShortRangeWeaponDamageText).text = damage.ToString();

        templateID = Managers.Game.MiddleRangeWeaponID;
        level = Managers.Game.WeaponLevel[templateID - 1];
        damage = Managers.Data.Weapons[templateID].WeaponLevelData[level - 1].Damage;
        GetText((int)Texts.MiddleRangeWeaponDamageText).text = damage.ToString();
        GetText((int)Texts.SelectedMiddleRangeWeaponDamageText).text = damage.ToString();

        templateID = Managers.Game.LongRangeWeaponID;
        level = Managers.Game.WeaponLevel[templateID - 1];
        damage = Managers.Data.Weapons[templateID].WeaponLevelData[level - 1].Damage;
        GetText((int)Texts.LongRangeWeaponDamageText).text = damage.ToString();
        GetText((int)Texts.SelectedLongRangeWeaponDamageText).text = damage.ToString();

        GetText((int)Texts.TurnText).text = _remainingTurn.ToString();
    }

    public void ShowWeaponButtons(bool show)
    {
        GetButton((int)Buttons.ShortRangeButton).gameObject.SetActive(show);
        GetButton((int)Buttons.MiddleRangeButton).gameObject.SetActive(show);
        GetButton((int)Buttons.LongRangeButton).gameObject.SetActive(show);
        switch (Managers.Game.WeaponType)
        {
            case Define.EWeaponRangeType.Short:
                GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(show);
                break;

            case Define.EWeaponRangeType.Middle:
                GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(show);
                break;

            case Define.EWeaponRangeType.Long:
                GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(show);
                break;
        }
    }

    bool isShakeMission = false;
    public void SetTurn(int turn)
    {
        _remainingTurn = turn;
        if (_remainingTurn < 6 && !isShakeMission)
        {
            for (int i = 0; i < _respawnData.Count; i++)
            {
                if (MissionMonsterItems.TryGetValue(_respawnData[i].MonsterID, out UI_MissionMonsterItem item))
                    item.ShakeImage();
            }
            isShakeMission = true;
        }
        GetText((int)Texts.TurnText).text = _remainingTurn.ToString();
    }

    public void MonsterDead(int monsterTemplateID, int remainMonster)
    {
        if (MissionMonsterItems.TryGetValue(monsterTemplateID, out UI_MissionMonsterItem item))
            item.SetRemainMonsterCount(remainMonster);
    }

    public Vector3 CoinPosition()
    {
        Vector3 screenPosition = Camera.main.ScreenToWorldPoint(GetObject((int)GameObjects.CoinObject).transform.position);
        return screenPosition;
    }

    public void ChangeSelectedButton(Define.EWeaponRangeType weaponRangeType)
    {
        switch (weaponRangeType)
        {
            case Define.EWeaponRangeType.Short:
                GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(true);
                GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(false);
                GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(false);
                break;

            case Define.EWeaponRangeType.Middle:
                GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(false);
                GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(true);
                GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(false);
                break;

            case Define.EWeaponRangeType.Long:
                GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(false);
                GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(false);
                GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(true);
                break;
        }
    }

    public void Combo(int comboCount)
    {
        if (comboCount < 2)
            GetText((int)Texts.ComboText).text = string.Empty;
        else if (comboCount >= 100)
        {
            int digit100 = comboCount / 100;
            int digit10 = (comboCount - digit100) / 10;
            int digit1 = comboCount % 10;
            GetText((int)Texts.ComboText).text = $"<sprite=11><sprite=12><sprite=13><sprite=14><sprite=12><sprite=10><sprite={digit100}><sprite={digit10}><sprite={digit1}>";
        }
        else if (comboCount >= 10)
        {
            int digit10 = comboCount / 10;
            int digit1 = comboCount % 10;
            GetText((int)Texts.ComboText).text = $"<sprite=11><sprite=12><sprite=13><sprite=14><sprite=12><sprite=10><sprite={digit10}><sprite={digit1}>";
        }
        else
        {
            GetText((int)Texts.ComboText).text = $"<sprite=11><sprite=12><sprite=13><sprite=14><sprite=12><sprite=10><sprite={comboCount}>";
        }
    }

    public void SetCoinText(int coin)
    {
        GetText((int)Texts.CoinText).text = coin.ToString();
    }

    public void DisableWeaponButton(Define.EWeaponRangeType disableWeaponRangeType)
    {
        switch (disableWeaponRangeType)
        {
            case Define.EWeaponRangeType.None:
                Debug.Log("None");
                GetButton((int)Buttons.ShortRangeButton).interactable = true;
                GetButton((int)Buttons.MiddleRangeButton).interactable = true;
                GetButton((int)Buttons.LongRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedShortRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedMiddleRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedLongRangeButton).interactable = true;
                break;

            case Define.EWeaponRangeType.Short:
                Debug.Log("Short");

                GetButton((int)Buttons.ShortRangeButton).interactable = false;
                GetButton((int)Buttons.MiddleRangeButton).interactable = true;
                GetButton((int)Buttons.LongRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedShortRangeButton).interactable = false;
                GetButton((int)Buttons.SelectedMiddleRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedLongRangeButton).interactable = true;
                break;

            case Define.EWeaponRangeType.Middle:
                Debug.Log("Middle");

                GetButton((int)Buttons.ShortRangeButton).interactable = true;
                GetButton((int)Buttons.MiddleRangeButton).interactable = false;
                GetButton((int)Buttons.LongRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedShortRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedMiddleRangeButton).interactable = false;
                GetButton((int)Buttons.SelectedLongRangeButton).interactable = true;
                break;

            case Define.EWeaponRangeType.Long:
                Debug.Log("Long");

                GetButton((int)Buttons.ShortRangeButton).interactable = true;
                GetButton((int)Buttons.MiddleRangeButton).interactable = true;
                GetButton((int)Buttons.LongRangeButton).interactable = false;
                GetButton((int)Buttons.SelectedShortRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedMiddleRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedLongRangeButton).interactable = false;
                break;
        }
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



    #region EventHandler
    void OnClickShortRangeButton(PointerEventData evt)
    {
        if (Managers.Game.DisableWeaponRangeType == Define.EWeaponRangeType.Short)
            return;

        Debug.Log("OnClickShortRangeButton");

        // 두번째 클릭이라면.
        if (Managers.Game.WeaponType == Define.EWeaponRangeType.Short)
        {
            Managers.Sound.Stop(Define.ESound.SubBgm);
            Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonSelected");
            ShowWeaponButtons(false);
            Managers.Game.PlayerAttack();
        }
        else
        {
            Managers.Game.WeaponType = Define.EWeaponRangeType.Short;
            Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonMain");
        }
    }

    void OnClickMiddleRangeButton(PointerEventData evt)
    {
        if (Managers.Game.DisableWeaponRangeType == Define.EWeaponRangeType.Middle)
            return;

        Debug.Log("OnClickMiddleRangeButton");

        // 두번째 클릭이라면.
        if (Managers.Game.WeaponType == Define.EWeaponRangeType.Middle)
        {
            Managers.Sound.Stop(Define.ESound.SubBgm);
            Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonSelected");
            ShowWeaponButtons(false);
            Managers.Game.PlayerAttack();
        }
        else
        {
            Managers.Game.WeaponType = Define.EWeaponRangeType.Middle;
            Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonMain");
        }
    }

    void OnClickLongRangeButton(PointerEventData evt)
    {
        if (Managers.Game.DisableWeaponRangeType == Define.EWeaponRangeType.Long)
            return;

        Debug.Log("OnClickLongRangeButton");

        // 두번째 클릭이라면.
        if (Managers.Game.WeaponType == Define.EWeaponRangeType.Long)
        {
            Managers.Sound.Stop(Define.ESound.SubBgm);
            Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonSelected");
            ShowWeaponButtons(false);
            Managers.Game.PlayerAttack();
        }
        else
        {
            Managers.Game.WeaponType = Define.EWeaponRangeType.Long;
            Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonMain");
        }
    }

    void OnClickShowPausePopup(PointerEventData evt)
    {
        Managers.UI.ShowPopupUI<UI_GamePausePopup>();
    }

    void OnClickTouchArea(PointerEventData evt)
    {
        if (Managers.Game.WeaponType == Define.EWeaponRangeType.None)
            return;

        ShowWeaponButtons(false);
        Managers.Game.PlayerAttack();
    }
    #endregion
}