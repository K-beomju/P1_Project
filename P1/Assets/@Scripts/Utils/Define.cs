using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
    }

    public enum EGameSceneState
    {
        None,
        Play,
        Pause,
        Boss,
        Over,
        Clear,
    }

    public enum EStageType
    {
        NormalStage,
        BossStage
    }

    public enum EObjectType
    {
        None,
        Hero,
        Monster,
        BossMonster
    }

    public enum EUIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum EEventType
    {
        None,

        /// <summary>몬스터 마리 수 변화 시 호출</summary>
        MonsterCountChanged,

        /// <summary>플레이어 재화 정보가 갱신될 때 호출</summary>
        CurrencyUpdated,

        /// <summary>플레이어 경험치 정보가 갱신될 때 호출</summary>
        ExperienceUpdated,

        /// <summary>영웅 성장 및 업그레이드 정보 갱신</summary>
        HeroUpgradeUpdated,

        /// <summary>플레이어 레벨업 시 호출</summary>
        PlayerLevelUp,

        /// <summary>뽑기 진행 중 레벨 정보가 업데이트될 때 호출</summary>
        DrawDataUpdated,

        /// <summary>뽑기 관련 UI가 갱신될 때 호출</summary>
        DrawUIUpdated,

        /// <summary>뽑기 레벨업 시 UI 갱신을 위해 호출</summary>
        DrawLevelUpUIUpdated
    }

    public enum ECreatureState
    {
        None,
        Idle,
        Move,
        Attack,
        Dead
    }

    public enum EHeroMoveState
    {
        None,
        TargetMonster
    }

    public enum ESound
    {
        Bgm,
        Effect,
        Max,
    }

    public enum EStatModType
    {
        Add,
        PercentAdd,
        PercentMult,
    }

    public enum EHeroUpgradeType
    {
        Growth_Atk,
        Growth_Hp
    }

    public enum EGoodType
    {
        None,
        Gold,
        Dia
    }

    public enum ERareType
    {
        None,
        /// <summary> 노말 </summary>
        Normal,
        /// <summary> 고급 </summary>
        Advanced,
        /// <summary> 희귀 </summary>
        Rare,
        /// <summary> 영웅 </summary>
        Legendary,
        /// <summary> 전설 </summary>
        Mythical,
        /// <summary> 천상 </summary>
        Celestial
    }

    public enum EEquipmentType
    {
        None,
        Sword,
        Armor,
        Ring
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
        Env = 8,
        Obstacle = 9,
        Projectile = 10,
    }

    public const float DASH_DISTANCE_THRESHOLD = 9f;
    public const float LERP_SPEED = 0.1f;
    public const float CREATRE_EQUIPMENT_DELAY = 0.05f;

    public static class HeroAnimation
    {
        public readonly static int HashCombo = Animator.StringToHash("IsCombo");
        public readonly static int HashAttack = Animator.StringToHash("IsAttack");
        public readonly static int HashMove = Animator.StringToHash("IsMove");
        public readonly static int HashAttackSpeed = Animator.StringToHash("AttackSpeed");
    }

    public static class SortingLayers
    {
        public const int SPELL_INDICATOR = 200;
        public const int UI_HPBAR = 299;
        public const int CREATURE = 300;
        public const int ENV = 300;
        public const int PROJECTILE = 310;
        public const int SKILL_EFFECT = 310;
        public const int DAMAGE_FONT = 410;
        public const int UI_POPUP = 450;
        public const int UI_GAMESCENE = 500;
        public const int UI_RESULTPOPUP = 600;

    }

}

