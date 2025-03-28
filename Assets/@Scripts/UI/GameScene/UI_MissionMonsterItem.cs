using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MissionMonsterItem : UI_Base
{
    enum GameObjects
    {
        ClearObject,
    }

    enum Texts
    {
        KillCountText,
    }

    enum Images
    {
        MonsterIcon,
    }

    int _monsterID;
    int _remainKillCount;

    protected override void Awake()
    {
        base.Awake();

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindImages(typeof(Images));
    }

    public void SetInfo(int monsterID, int remainKillCount)
    {
        _monsterID = monsterID;
        _remainKillCount = remainKillCount;
        RefreshUI();
    }

    void RefreshUI()
    {
        if (_remainKillCount > 0)
        {
            GetText((int)Texts.KillCountText).text = ($"{Mathf.Max(_remainKillCount, 0)}");
            GetText((int)Texts.KillCountText).gameObject.SetActive(true);
            GetObject((int)GameObjects.ClearObject).SetActive(false);
        }
        else
        {
            GetText((int)Texts.KillCountText).gameObject.SetActive(false);
            GetObject((int)GameObjects.ClearObject).SetActive(true);
        }

        if (Managers.Data.Monsters.TryGetValue(_monsterID, out MonsterData monsterData) == false)
        {
            Debug.Log($"UI_MissionMonsterItem RefreshUI Failed {_monsterID}");
            return;
        }

        if (GetImage((int)Images.MonsterIcon).sprite.name != monsterData.SpriteID)
        {
            GetImage((int)Images.MonsterIcon).sprite = Managers.Resource.Load<Sprite>(monsterData.SpriteID);
        }
    }

    public void ShakeImage()
    {
        GetImage((int)Images.MonsterIcon).transform.DORotate(Vector3.forward * 2, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        GetImage((int)Images.MonsterIcon).transform.DOScale(Vector3.one * 1.04f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void SetRemainMonsterCount(int remainKillCount)
    {
        _remainKillCount = remainKillCount;
        RefreshUI();
    }
}
