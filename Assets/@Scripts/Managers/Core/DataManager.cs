using System.Collections.Generic;
using System.Linq;
using Data;
using Newtonsoft.Json;
using UnityEngine;

public interface IValidate
{
    bool Validate();
}

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
    bool Validate();
}

public class DataManager
{
    private HashSet<IValidate> _loaders = new HashSet<IValidate>();
    public Dictionary<int, BossData> Bosses { get; private set; }
    public Dictionary<int, BossSkillData> BossSkilles { get; private set; }
    public Dictionary<int, ChapterResourceData> ChapterResources { get; private set; }
    public Dictionary<int, MonsterData> Monsters { get; private set; }
    public Dictionary<int, RespawnInfoData> RespawnInfosDic { get; private set; }
    public List<RespawnInfoData> RespawnInfoDatas { get; private set; } = new List<RespawnInfoData>();
    public Dictionary<int, StageData> Stages { get; private set; }
    public Dictionary<string, TextData> Texts { get; private set; }
    public Dictionary<int, WeaponData> Weapons { get; private set; }
    public Dictionary<int, WeaponLevelInfoData> WeaponLevelInfoDatas { get; private set; }
    public Dictionary<int, List<WeaponLevelData>> WeaponLevelInfosDic { get; private set; }

    public void Init()
    {
        Bosses = LoadJson<BossDataLoader, int, BossData>("BossData").MakeDict();
        BossSkilles = LoadJson<BossSkillDataLoader, int, BossSkillData>("BossSkillData").MakeDict();
        ChapterResources = LoadJson<ChapterResourceDataLoader, int, ChapterResourceData>("ChapterResourceData").MakeDict();
        Monsters = LoadJson<MonsterDataLoader, int, MonsterData>("MonsterData").MakeDict();
        RespawnInfosDic = LoadJson<RespawnInfoDataLoader, int, RespawnInfoData>("RespawnData").MakeDict();
        Stages = LoadJson<StageDataLoader, int, StageData>("StageData").MakeDict();
        Texts = LoadJson<TextDataLoader, string, TextData>("TextData").MakeDict();
        Weapons = LoadJson<WeaponDataLoader, int, WeaponData>("WeaponData").MakeDict();
        WeaponLevelInfoDatas = LoadJson<WeaponLevelInfoDataLoader, int, WeaponLevelInfoData>("WeaponLevelData").MakeDict();



        // Add boss skilles
        foreach (var boss in Bosses.Values)
        {
            boss.bossSkillData = BossSkilles.Values
                .Where(skill => skill.BossID == boss.TemplateID)
                .ToList();
        }

        // Add respawnData in StageData
        foreach(var stage in Stages.Values)
        {
            stage.respawnData = new List<RespawnData>();

            var respawnInfo = RespawnInfoDatas
                .Where(respawn => respawn.RespawnID == stage.TemplateID)
                .ToList();

            foreach(var info in respawnInfo)
            {
                stage.respawnData.Add(new RespawnData
                {
                    MonsterID = info.MonsterID,
                    SummonPoint = info.SummonPoint,
                    MinPoint = info.MinPoint,
                    MaxPoint = info.MaxPoint,
                    ClearCount = info.ClearCount
                });
            }
        }

        // Add WeaponLevelInfo in Weapons
        foreach(var info in WeaponLevelInfoDatas.Values)
        {
            if (!WeaponLevelInfosDic.TryGetValue(info.TableID, out var weaponLevelList))
            {
                // 없으면 새 리스트 생성 후 딕셔너리에 추가
                weaponLevelList = new List<WeaponLevelData>();
                WeaponLevelInfosDic[info.TableID] = weaponLevelList;
            }

            // WeaponLevelData 추가
            weaponLevelList.Add(new WeaponLevelData
            {
                Level = info.Level,
                Damage = info.Damage,
                Exp = info.Exp,
                Cost = info.Cost
            });
        }

        foreach (var weapon in Weapons.Values)
        {
            if (WeaponLevelInfosDic.TryGetValue(weapon.TableID, out var weaponLevelList))
            {
                weapon.WeaponLevelData = weaponLevelList;
            }
            else
            {
                weapon.WeaponLevelData = new List<WeaponLevelData>(); // 값이 없을 경우 빈 리스트 할당
            }
        }


        Validate();

        Debug.Log("Data Init!!");
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
	}

    private bool Validate()
    {
        bool success = true;

        foreach (var loader in _loaders)
        {
            if (loader.Validate() == false)
                success = false;
        }

        _loaders.Clear();

        return success;
    }

}
