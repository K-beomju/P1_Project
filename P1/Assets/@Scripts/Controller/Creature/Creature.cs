using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Creature : BaseObject
{
    #region Stat
    public int Level { get; protected set; }
    public CreatureStat Atk { get; protected set; }
    public CreatureStat Def { get; protected set; }
    public CreatureStat AttackRange { get; protected set; }
    public CreatureStat AttackDelay { get; protected set; }
    public CreatureStat AttackSpeedRate { get; protected set; }

    public CreatureStat MoveSpeed { get; protected set; }

	public float Hp { get; set; }
    public CreatureStat MaxHp { get; protected set; }
    #endregion

    #region Config
    public Animator Anim { get; protected set; }
    public Rigidbody2D Rigid { get; protected set; }
    public BaseObject Target { get; protected set; }
    #endregion


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

        // Rendering
        Anim = GetComponent<Animator>();
        Rigid = GetComponent<Rigidbody2D>();    
        _sprite.sortingOrder = SortingLayers.CREATURE;
        return true;
    }

    public override void SetInfo(int dataTemplateID)
    {
        base.SetInfo(dataTemplateID);

         // InfoSetting
        SetCreatureInfo(dataTemplateID);

        // State
        CreatureState = ECreatureState.Idle;

          // AI
        StartCoroutine(CoUpdateAI());
    }

    public virtual void SetCreatureInfo(int dataTemplateID)
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
