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

    public enum EUIEvent
    {
        Click,
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum EJoystickState
    {
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum EHeroState
    {
        None,
        Idle,
        Move,
        Attack
    }

    public enum EHeroMoveState
    {
        None,
        TargetMonster,
        ForceMove
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
        public readonly static int HashMoveDir = Animator.StringToHash("MoveDir");
    }

}

