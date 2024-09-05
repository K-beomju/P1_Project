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



    public enum EUIEvent
    {
        Click,
        PointerDown,
        PointerUp,
        Drag,
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

    public static class HeroAnimation
    {
        public readonly static int HashCombo = Animator.StringToHash("IsCombo");
        public readonly static int HashAttack = Animator.StringToHash("IsAttack");
        public readonly static int HashMove = Animator.StringToHash("IsMove");
        public readonly static int HashAttackSpeed = Animator.StringToHash("AttackSpeed");
    }

}

