using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    #region GamePlay

    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
        DungeonScene
    }

    public enum EGameSceneState
    {
        None,
        Play,
        Pause,
        Boss,
        RankUp,
        Over,
        Clear,
    }

    public enum EBackendState
    {
        Failure,
        Maintainance,
        Retry,
        Success
    }

    public enum EStageType
    {
        NormalStage,
        BossStage
    }

    #endregion

    #region Entity

    public enum EObjectType
    {
        None,
        Hero,
        Monster,
        BossMonster,
        RankUpMonster
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

    #endregion

    #region Event

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

        /// <summary>장비 뽑기 관련 UI가 갱신될 때 호출</summary>
        DrawEquipmentUIUpdated,

        /// <summary>스킬 뽑기 관련 UI가 갱신될 때 호출</summary>
        DrawSkillUIUpdated,

        /// <summary>뽑기 레벨업 시 UI 갱신을 위해 호출</summary>
        DrawLevelUpUIUpdated,

        /// <summary>장비 아이템 클릭할 때 호출</summary>
        EquipmentItemClick,

        /// <summary>스킬 아이템 클릭할 때 호출</summary>
        SkillItemClick,

        /// <summary>게임씬 스킬 슬롯 쿨타임 체크</summary>
        CompleteSkillCool,

        /// <summary>특성 아이템 클릭할 때 호출</summary>
        AttributeItemClick,

        /// <summary>영웅 특성 업그레이드 정보 갱신</summary>
        HeroAttributeUpdated,

        /// <summary>영웅 유물 정보 갱신</summary>
        HeroRelicUpdated,

        /// <summary>영웅 전투력 갱신</summary>
        HeroTotalPowerUpdated,

        /// <summary>영웅 승급전 랭크 갱신</summary>
        HeroRankUpdated,

        /// <summary>영웅 승급전 진행중</summary>
        HeroRankChallenging
    }

    public enum EItemDisplayType
    {
        Basic,
        ImageOnly,
        SlotItem,
        Draw,
        Enhance
    }

    #endregion

    #region Effect

    public enum EEffectSpawnType
    {
        Instant,
        HasDuration
    }

    public enum EEffectType
    {
        Buff,
        Attack
    }

    #endregion

    #region Draw (Skill & Equipment)

    public enum EOwningState
    {
        Unowned,
        Owned,
    }

    public enum ESound
    {
        Bgm,
        Effect,
        Max,
    }

    // 뽑기에서 뽑을 아이템 
    public enum EDrawType
    {
        Weapon,
        Armor,
        Ring,
        Skill
    }

    public enum ESkillSlotType
    {
        Lock,         // 슬롯이 잠겨 있는 상태
        None,         // 슬롯이 열려 있지만 아무것도 장착되지 않은 상태
        Equipped      // 슬롯에 스킬이 장착된 상태
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
        Weapon,
        Armor,
        Ring
    }

    #endregion

    #region Upgrade

    // 획득 가능한 재화 아이템 
    public enum EItemType
    {
        Gold = 1,
        Dia,
        ExpPoint,
        Relic,
        GoldDungeonKey,
        DiaDungeonKey,
        AbilityPoint
    }

    public enum EHeroUpgradeType
    {
        Growth_Atk,
        Growth_Hp,
        Growth_Recovery,
        Growth_CriRate,
        Growth_CriDmg
    }

    public enum EHeroAttrType
    {
        Attribute_Atk,
        Attribute_MaxHp,
        Attribute_CriRate,
        Attribute_CriDmg,
        Attribute_SkillTime,
        Attribute_SkillDmg
    }

    public enum EHeroRelicType
    {
        Relic_Atk,
        Relic_MaxHp,
        Relic_Recovery,
        Relic_MonsterDmg,
        Relic_BossMonsterDmg,
        Relic_ExpRate,
        Relic_GoldRate
    }

    public enum EHeroRankUpStatType
    {
        None,
        RankUp_Atk,
        RankUp_MaxHp,
        RankUp_Recovery,
        RankUp_CriDmg,
        RankUp_GoldRate,
        RankUp_ExpRate
    }

    public enum EStatModType
    {
        Add,
        PercentAdd,
        PercentMult
    }

    #endregion

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
    public const float CREATRE_DRAWITEM_DELAY = 0.03f;

    public static class AnimName
    {
        public readonly static int HashCombo = Animator.StringToHash("IsCombo");
        public readonly static int HashAttack = Animator.StringToHash("IsAttack");
        public readonly static int HashMove = Animator.StringToHash("IsMove");
        public readonly static int HashDead = Animator.StringToHash("Dead");

    }

    public static class SortingLayers
    {
        public const int AURA = 299;
        public const int UI_HPBAR = 299;
        public const int CREATURE = 300;
        public const int HERO = 310;
        public const int PROJECTILE = 310;
        public const int SKILL_EFFECT = 310;
        public const int DAMAGE_FONT = 410;
        public const int UI_POPUP = 450;
        public const int UI_SCENE = 500;
        public const int UI_RESULTPOPUP = 600;
        public const int UI_TOTALPOWER = 610;
        public const int UI_NOTIFICATION = 610;
        public const int UI_FADEPOPUP = 620;
        public const int UI_SLEEPMODEPOPUP = 1000;
    }

    public enum EQuestPeriodType
    {
        Once, // 단발성
        Daily, // 일일
        Weekly, // 주간 
        Infinite // 무한으로
    }

    public enum EQuestCondition
    {
        None,

    }

    public enum EQuestObjectiveType
    {
        KillMonster,
        UpgradeStat,
        UpgradeAttribute,
        StageClear,
        SpawnWeapon,
        SpawnArmor,
        SpawnRing,
        SpawnSkill,
        SpawnRelic,
        EquipWeapon,
        EquipArmor,
        EquipRing,
        EquipSkill
    }


    #region Dungeon

    public enum EDungeonType
    {
        Unknown,
        Gold,
        Dia,
        Promotion,
        WorldBoss
    }

    #endregion

    public enum EFadeType
    {
        FadeIn,
        FadeOut,
        FadeInOut,
    }

    #region  AdBuff

    public enum EAdBuffType
    {
        Atk,
        IncreaseGold,
        IncreaseExp
    }

    #endregion

    public enum ERankType
    {
        Unknown,
        Iron,
        Bronze,
        Gold,
        Dia,
        Master,
        GrandMaster,
    }

    public enum ERankState
    {
        Locked,           // 잠겨있는 상태
        Completed,        // 이미 깬 상태
        Current,          // 현재 상태
        Pending           // 진행해야 할 상태
    }

    public enum ERankAbilityState
    {
        Locked,        // 잠긴 상태, 능력을 획득할 수 없음
        Unlocked,      // 해제된 상태, 능력을 획득할 수 있음
        Restricted,    // 임의로 잠긴 상태, 능력을 변경할 수 없음,
        Acquired       // 능력이 존재하며 활성화된 상태

    }

}

