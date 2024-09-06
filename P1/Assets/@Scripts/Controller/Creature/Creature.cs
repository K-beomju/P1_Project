using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Creature : BaseObject
{
    public float Hp;
    public float MaxHp;
    public Animator _anim { get; protected set; }

    protected ECreatureState _creatureState = ECreatureState.None;
    public virtual ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState != value)
            {
                _creatureState = value;
                UpdateAnimation();
            }
        }
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Stat
        SetCreatureInfo();

        // Rendering
        _anim = GetComponent<Animator>();
        _sprite.sortingOrder = SortingLayers.CREATURE;

        // State
        CreatureState = ECreatureState.Idle;

        // AI
        StartCoroutine(CoUpdateAI());
        return true;
    }

    protected virtual void SetCreatureInfo()
    {
    }

    protected virtual void UpdateAnimation()
    {

    }

    protected IEnumerator CoUpdateAI()
    {
        while (true)
        {
            switch (CreatureState)
            {
                case ECreatureState.Idle:
                    UpdateIdle();
                    break;
                case ECreatureState.Move:
                    UpdateMove();
                    break;
                case ECreatureState.Attack:
                    UpdateAttack();
                    break;
                case ECreatureState.Dead:
                    UpdateDead();
                    break;
            }
            yield return null;
        }
    }


    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMove() { }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateDead() { }
}
