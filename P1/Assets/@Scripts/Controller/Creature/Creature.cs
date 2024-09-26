using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Creature : BaseObject
{
    #region Stat
    public int Level;
    public float Atk { get; protected set; }
    public float Def { get; protected set; }
    public float MaxHp { get; protected set; }
    public float Hp { get; set; }
    public float AttackRange { get; protected set; }
    public float AttackDelay { get; protected set; }
    public float AttackSpeedRate { get; protected set; }

    public float MoveSpeed { get; protected set; }
    #endregion

    #region Config
    public Animator Anim { get; protected set; }
    public Rigidbody2D Rigid { get; protected set; }
    public BaseObject Target { get; protected set; }
    public UI_HpBarWorldSpace HpBar { get; protected set; }
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
        Sprite.sortingOrder = SortingLayers.CREATURE;
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

    public virtual void ReSetStats()
    {
        // TODO: Some Setting
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


    public virtual void OnDamaged(Creature attacker)
    {
        float finalDamage = attacker.Atk; // TODO: 방어력이나 다른 계산이 있을 경우 적용
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

        if(HpBar != null && !HpBar.gameObject.activeSelf)
        HpBar.gameObject.SetActive(true);

        if (Hp <= 0)
        {
            OnDead();
        }

        Color originalColor = Sprite.color;

        Sprite.DOColor(Color.red, 0.05f)
            .OnComplete(() => Sprite.DOColor(originalColor, 0.05f));

        UI_DamageTextWorldSpace damageText = Managers.UI.MakeWorldSpaceUI<UI_DamageTextWorldSpace>();
        damageText.SetInfo(CenterPosition, finalDamage, false);
    }

    public virtual void OnDead()
    {

    }

    protected void LookAt(Vector2 dir)
    {
        Vector3 scale = transform.localScale;
        scale.x = dir.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
