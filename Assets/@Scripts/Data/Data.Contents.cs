using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Data
{
    //¿¢¼¿ ÆÄ½Ì¿¡ Á¦¿ÜÇÒ ¾îÆ®¸®ºäÆ® Á¤ÀÇ
    [AttributeUsage(AttributeTargets.Field)]
    public class ExcludeFieldAttribute : Attribute
    {
    }

    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int TemplateId;
        public string NameTextID;
        public float ColliderOffsetX;
        public float ColliderOffsetY;
        public float ColliderRadius;
        public float MaxHp;
        public float UpMaxHpBonus;
        public float Atk;
        public float MissChance;
        public float AtkBonus;
        public float MoveSpeed;
        public float CriRate;
        public float CriDamage;
        public string IconImage;
        public string SkeletonDataID;
        public int DefaultSkillId;
        public int EnvSkillId;
        public int SkillAId;
        public int SkillBId;
       
    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> creatures = new List<CreatureData>();
        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (CreatureData creature in creatures)
                dict.Add(creature.TemplateId, creature);
            return dict;
        }

        public bool Validate()
        {
            return true;
        }
    }
    #endregion

    #region BossData
    [Serializable]
    public class BossData
    {
        public int TemplateID;
        public string NameID;
        public string Prefab;
        public string SpriteID;
        public int Hp;
        public int Damage;
        public int MoveTurn;
        public int MoveSpeed;
        public int AttackRange;
        public int DropCoin;
        public float UseSkillAnimationDuration;
        public float PositionY;
        public string EffectSound;
        [ExcludeField]
        public List<BossSkillData> bossSkillData = new List<BossSkillData>();
    }

    [Serializable]
    public class BossDataLoader : ILoader<int, BossData>
    {
        public List<BossData> bosses = new List<BossData>();

        public Dictionary<int, BossData> MakeDict()
        {
            Dictionary<int, BossData> dict = new Dictionary<int, BossData>();            
            foreach (BossData boss in bosses)
                dict.Add(boss.TemplateID, boss);

            return dict;
        }

        public bool Validate()
        {
            //Managers.Data.BossSkilles
            return true;
        }
    }

    #endregion

    #region BossSkillData
    [Serializable]
    public class BossSkillData
    {
        public int BossID;
        public Define.ESkillType SkillID;
        public int Cooltime;
        public int MonsterCount;
        public int Value;
        public int Turn;
    }

    [Serializable]
    public class BossSkillDataLoader : ILoader<int, BossSkillData>
    {
        public List<BossSkillData> bossSkills = new List<BossSkillData>();
        public Dictionary<int, BossSkillData> MakeDict()
        {
            Dictionary<int, BossSkillData> dict = new Dictionary<int, BossSkillData>();
            int i = 0;
            foreach (BossSkillData bossSkill in bossSkills)
            {
                dict.Add(i, bossSkill);
                i++;
            }
                
            return dict;
        }

        public bool Validate()
        {
            return true;
        }
    }
    #endregion

    #region ChapterResourceData
    [Serializable]
    public class ChapterResourceData
    {
        public int TemplateID;
        public string MapTop;
        public string MapBottom;
        public string MapCenter;
        public string Object1;
        public string Object2;
        public string Object3;
        public string Dot;
        public string StageBlock;
        public string StageTileSpine;
        public string Shadow;
    }

    [Serializable]
    public class ChapterResourceDataLoader : ILoader<int, ChapterResourceData>
    {
        public List<ChapterResourceData> chapters = new List<ChapterResourceData>();
        public Dictionary<int, ChapterResourceData> MakeDict()
        {
            Dictionary<int, ChapterResourceData> dic = new Dictionary<int, ChapterResourceData>();

            foreach (ChapterResourceData stage in chapters)
                dic.Add(stage.TemplateID, stage);

            return dic;
        }

        public bool Validate()
        {
            return true;
        }
    }
    #endregion

    #region MonsterData
    [Serializable]
    public class MonsterData
    {
        public int TemplateID;
        public string NameID;
        public string Prefab;
        public string SpriteID;
        public int Hp;
        public int Damage;
        public int MoveTurn;
        public int MoveSpeed;
        public int AttackRange;
        public int DropCoin;
        public int SpecialAbility;
    }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dic = new Dictionary<int, MonsterData>();

            foreach (MonsterData monster in monsters)
                dic.Add(monster.TemplateID, monster);

            return dic;
        }

        public bool Validate()
        {
            return true;
        }
    }
    #endregion

    #region RespawnData
    [Serializable]
    public class RespawnData
    {
        public int MonsterID;
        public int SummonPoint;
        public int MinPoint;
        public int MaxPoint;
        public int ClearCount;
    }

    [Serializable]
    public class RespawnInfoData
    {
        public int RespawnID;
        public int MonsterID;
        public int SummonPoint;
        public int MinPoint;
        public int MaxPoint;
        public int ClearCount;
    }

    [Serializable]
    public class RespawnInfoDataLoader : ILoader<int, RespawnInfoData>
    {
        public List<RespawnInfoData> respawns = new List<RespawnInfoData>();
        public Dictionary<int, RespawnInfoData> MakeDict()
        {
            Dictionary<int, RespawnInfoData> dic = new Dictionary<int, RespawnInfoData>();

            int i = 0;
            foreach (RespawnInfoData respawn in respawns)
            {
                dic.Add(i, respawn);
                i++;
            }

            return dic;
        }

        public bool Validate()
        {
            //foreach (var respawn in respawns)
            //{
            //    Managers.Data.RespawnInfoDatas.Add(respawn);
            //}

            foreach (var stage in Managers.Data.Stages.Values)
            {
                stage.respawnData = new List<RespawnData>();

                var respawnInfo = respawns
                    .Where(respawn => respawn.RespawnID == stage.StageID)
                    .ToList();

                foreach (var info in respawnInfo)
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
            
            return true;
        }
    }
    #endregion

    #region StageData
    [Serializable]
    public class StageData
    {
        public int TemplateID;
        public int ChapterID;
        public int StageID;
        public int ClearGoldReward;
        public int BossTemplateID;
        public int Turn;
        public string MapName;
        [ExcludeField]
        public List<RespawnData> respawnData;
    }
    [Serializable]
    public class StageDataLoader : ILoader<int, StageData>
    {
        public List<StageData> stages = new List<StageData>();
        public Dictionary<int, StageData> MakeDict()
        {
            Dictionary<int, StageData> dic = new Dictionary<int, StageData>();

            foreach (StageData stage in stages)
                dic.Add(stage.TemplateID, stage);

            return dic;
        }

        public bool Validate()
        {
            return true;
        }
    }
    #endregion

    #region TextData
    [Serializable]
    public class TextData
    {
        public string TextID;
        public string Kor;
        public string Eng;
    }

    [Serializable]
    public class TextDataLoader : ILoader<string, TextData>
    {
        public List<TextData> texts = new List<TextData>();

        public Dictionary<string, TextData> MakeDict()
        {
            Dictionary<string, TextData> dic = new Dictionary<string, TextData>();

            foreach (TextData text in texts)
                dic.Add(text.TextID, text);

            return dic;
        }

        public bool Validate()
        {
            return true;
        }
    }
    #endregion

    #region WeaponData
    [Serializable]
    public class WeaponData
    {
        public int TemplateID;
        public Define.EWeaponRangeType RangeType;
        public string NameID;
        public string Sprite;
        public string RadarID;
        public int TableID;
        public Define.EKnockbackDirection KnockbackDirection;
        public float GachaRate;
        public Define.EWeaponAttackType AttackType;
        public string ObjectID;
        [ExcludeField]
        public List<WeaponLevelData> WeaponLevelData;
    }

    [Serializable]
    public class WeaponDataLoader : ILoader<int, WeaponData>
    {
        public List<WeaponData> weapons = new List<WeaponData>();

        public Dictionary<int, WeaponData> MakeDict()
        {
            Dictionary<int, WeaponData> dic = new Dictionary<int, WeaponData>();

            foreach (WeaponData weapon in weapons)
                dic.Add(weapon.TemplateID, weapon);

            return dic;
        }

        public bool Validate()
        {
            return true;
        }
    }
    #endregion

    #region WeaponLevelData
    [Serializable]
    public class WeaponLevelData
    {
        public int Level;
        public int Damage;
        public int Exp;
        public int Cost;
    }

    [Serializable]
    public class WeaponLevelInfoData
    {
        public int TableID;
        public int Level;
        public int Damage;
        public int Exp;
        public int Cost;
    }

    public class WeaponLevelInfo
    {
        public int TableID;
        public List<WeaponLevelData> weaponLevelDatas = new List<WeaponLevelData>();
    }

    [Serializable]
    public class WeaponLevelInfoDataLoader : ILoader<int, WeaponLevelInfoData>
    {
        public List<WeaponLevelInfoData> weaponLevelInfos = new List<WeaponLevelInfoData>();

        public Dictionary<int, WeaponLevelInfoData> MakeDict()
        {
            Dictionary<int, WeaponLevelInfoData> dic = new Dictionary<int, WeaponLevelInfoData>();

            int i = 0;
            foreach (WeaponLevelInfoData info in weaponLevelInfos)
            {
                dic.Add(i, info);
                i++;
            }

            return dic;
        }

        public bool Validate()
        {
            return true;
        }
    }
    #endregion
}