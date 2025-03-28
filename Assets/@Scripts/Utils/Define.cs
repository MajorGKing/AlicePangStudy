using System;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class Define
{
    public const char MAP_TOOL_WALL = '0';
    public const char MAP_TOOL_NONE = '1';

    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
        SelectStageScene,
    }

    public enum ESound
    {
        Bgm,
        SubBgm,
        Effect,
        Max,
    }

    public enum ETouchEvent
    {
        PointerUp,
        PointerDown,
        Click,
        Pressed,
        BeginDrag,
        Drag,
        EndDrag,
    }

    public enum ELanguage
	{
        Korean,
        English,
        French,
        SimplifiedChinese,
        TraditionalChinese,
        Japanese,
	}

	public enum ELayer
	{
		Default = 0,
		TransparentFX = 1,
		IgnoreRaycast = 2,
		Dummy1 = 3,
		Water = 4,
		UI = 5,
		Hero = 6,
		Monster = 7,
		Boss = 8,
		//
		Env = 11,
		Obstacle = 12,
		//
		Projectile = 20,
	}

    public enum ESkillType
    {
        ControllRadar,
        SummonMonster,
        ReduceTurn,
        LockWeapon,
    }

    public enum EWeaponRangeType
    {
        None,
        Short,
        Middle,
        Long
    }

    public enum EWeaponAttackType
    {
        Melee,
        Range,
        Trap,
    }

    public enum EKnockbackDirection
    {
        Front,
        Back,
        Clockwise,
        AntiClockwise,
    }

    public enum EBattleState
    {
        Ready,
        PlayerInput,
        PlayerAttack,
        BossAttack,
        MonsterAttack,
        GameOver,
    }

    public enum ECreatureState
    {
        Idle,
        Moving,
        Attack,
        Dead
    }

    public const float START_DEGREE = DELTA_DEGREE / 2;
    public const float DELTA_DEGREE = (2 * Mathf.PI / 24);
    public const int SLICE_COUNT = 24;
    public const int LAYER_COUNT = 6;
    public const float RADAR_SPEED = 200f;

    public const int WEAPON_COUNT = 26;

    public const int DAILY_QUEST_COUNT = 5;

    public const string TEST_ID = "TEST";
}