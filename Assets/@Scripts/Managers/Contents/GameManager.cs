using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using UnityEngine;
using static Define;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class GameData
{
    public int Chapter = 1;
    public int Stage = 1;

    public int Coin;
    public int Dia;

    public int[] WeaponLevel = new int[Define.WEAPON_COUNT];
    public int[] WeaponExp = new int[Define.WEAPON_COUNT];

    public int ShortRangeWeaponID;
    public int MiddleRangeWeaponID;
    public int LongRangeWeaponID;

    public int SelectedChapter;
    public int SelectedStage;

    public bool BGMOn = true;
    public bool EffectSoundOn = true;

    public int[] DailyQuestID = new int[Define.DAILY_QUEST_COUNT];
    public int LastStoryID = -1;
}

public class GameManager
{
    public GameData SaveData { get; private set; } = new GameData();
    
    string _path;
    public bool IsLoaded = false;

    public float ZRotation { get; set; }
    public int CurrentStageGetCoin { get; set; }
    public float RadarAngleSpeed { get; set; } = Define.RADAR_SPEED;
    public event Action<Define.EWeaponRangeType> OnChangeWeapon;
    Define.EWeaponRangeType _weaponType = Define.EWeaponRangeType.Short;
    public Define.EWeaponRangeType WeaponType
    {
        get { return _weaponType; }
        set
        {
            _weaponType = value;
            OnChangeWeapon?.Invoke(value);
            (Managers.UI.SceneUI as UI_GameScene).ChangeSelectedButton(_weaponType);
        }
    }

    Define.EWeaponRangeType _disableWeaponRangeType = Define.EWeaponRangeType.None;
    public Define.EWeaponRangeType DisableWeaponRangeType
    {
        get { return _disableWeaponRangeType; }
        set
        {
            _disableWeaponRangeType = value;

            (Managers.UI.SceneUI as UI_GameScene)?.DisableWeaponButton(_disableWeaponRangeType);
        }
    }

    public event Action OnPlayerInput;
    public void PlayerInput()
    {
        OnPlayerInput?.Invoke();
    }

    public event Action OnPlayerAttack;

    public void PlayerAttack()
    {
        OnPlayerAttack?.Invoke();
    }


    #region Stage
    public int HighestChapter
    {
        get { return SaveData.Chapter; }
        set { SaveData.Chapter = value; }
    }

    public int HighestStage
    {
        get { return SaveData.Stage; }
        set { SaveData.Stage = value; }
    }

    public int SelectedChapter
    {
        get { return SaveData.SelectedChapter; }
        set { SaveData.SelectedChapter = value; }
    }

    public int SelectedStage
    {
        get { return SaveData.SelectedStage; }
        set { SaveData.SelectedStage = value; }
    }
    #endregion

    #region Player
    public int ShortRangeWeaponID
    {
        get { return SaveData.ShortRangeWeaponID; }
        set { SaveData.ShortRangeWeaponID = value; }
    }

    public int MiddleRangeWeaponID
    {
        get { return SaveData.MiddleRangeWeaponID; }
        set { SaveData.MiddleRangeWeaponID = value; }
    }

    public int LongRangeWeaponID
    {
        get { return SaveData.LongRangeWeaponID; }
        set { SaveData.LongRangeWeaponID = value; }
    }

    public int Coin
    {
        get { return SaveData.Coin; }
        set { SaveData.Coin = value; }
    }

    public int Dia
    {
        get { return SaveData.Dia; }
        set { SaveData.Dia = value; }
    }

    public int LastStoryID
    {
        get { return SaveData.LastStoryID; }
        set { SaveData.LastStoryID = value; }
    }
    #endregion

    #region Weapon
    public int[] WeaponLevel { get { return SaveData.WeaponLevel; } }
    public int[] WeaponExp { get { return SaveData.WeaponExp; } }
    #endregion

    #region Util
    public int[] DailyQuestID { get { return SaveData.DailyQuestID; } }
    #endregion

    #region Option
    public bool BGMOn
    {
        get { return SaveData.BGMOn; }
        set { SaveData.BGMOn = value; }
    }

    public bool EffectSoundOn
    {
        get { return SaveData.EffectSoundOn; }
        set { SaveData.EffectSoundOn = value; }
    }
    #endregion

    

    public void Init()
    {
        _path = Application.persistentDataPath + "/SaveData.json";
        if (LoadGame())
            return;

        ShortRangeWeaponID = 1;
        MiddleRangeWeaponID = 9;
        LongRangeWeaponID = 11;

        WeaponLevel[0] = 1;
        WeaponLevel[8] = 1;
        WeaponLevel[10] = 1;

        IsLoaded = true;

        SaveGame();
    }

    public event Action<MonsterController> OnMonsterDead;

    public void MonsterDead(MonsterController mc)
    {
        OnMonsterDead?.Invoke(mc);
    }

    public void GetStageCoin(int stageReward)
    {
        Coin += CurrentStageGetCoin + stageReward;
        CurrentStageGetCoin = 0;
        SaveGame();
    }

    public bool CheckCoin(int coin)
	{
		if (Coin >= coin)
			return true;
		else
			return false;
	}

	public bool SpendCoin(int coin)
    {
		if (CheckCoin(coin))
		{
			Coin -= coin;

            if (Managers.UI.SceneUI is UI_SelectStageScene)
            {
                (Managers.UI.SceneUI as UI_SelectStageScene).TopUI.RefreshUI();
            }
            return true;
		}

        return false;
    }

    public void CheckBreakStageRecord()
    {
        if (SelectedChapter != HighestChapter || SelectedStage != HighestStage)
            return;

        if (SelectedStage < 20)
        {
            SelectedStage++;
            HighestStage = SelectedStage;
        }
        else if (SelectedChapter < 6)
        {
            ShowStoryPopup(SelectedChapter);
            SelectedChapter++;
            HighestChapter = SelectedChapter;
            SelectedStage = 1;
            HighestStage = SelectedStage;
        }
        else
        {
            ShowStoryPopup(SelectedChapter);
        }
        SaveGame();
    }

    void ShowStoryPopup(int chapter)
    {
        if (LastStoryID < chapter)
        {
            Managers.UI.ShowPopupUI<UI_StoryPopup>($"UI_StoryPopup{chapter}");
            LastStoryID = chapter;
            SaveGame();
        }
    }

    public int CurrentWeaponID()
    {
        int weaponID = 1;

        switch (WeaponType)
        {
            case Define.EWeaponRangeType.Short:
                weaponID = ShortRangeWeaponID;
                break;

            case Define.EWeaponRangeType.Middle:
                weaponID = MiddleRangeWeaponID;
                break;

            case Define.EWeaponRangeType.Long:
                weaponID = LongRangeWeaponID;
                break;
        }

        return weaponID;
    }

    #region Save&Load
    public void SaveGame()
    {
        string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
        File.WriteAllText(_path, jsonStr);
    }

    public bool LoadGame()
    {
        if (File.Exists(_path) == false)
            return false;

        string fileStr = File.ReadAllText(_path);
        GameData data = JsonUtility.FromJson<GameData>(fileStr);
        if (data != null)
            Managers.Game.SaveData = data;

        IsLoaded = true;
        return true;
    }
    #endregion
}