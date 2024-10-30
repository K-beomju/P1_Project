using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Define;

public class Creature : BaseObject
{
    #region Stat
    public int Level;
    public float Atk { get; protected set; }
    public float MaxHp { get; protected set; }
    public float Hp { get; set; }
    public float Recovery { get; protected set; }
    public float CriRate { get; protected set; }
    public float CriDmg { get; protected set; }
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
    private Color originalColor = Color.white;
    #endregion

    #region Buff
    public CreatureStat ReduceDmgBuff;

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


    public virtual void OnDamaged(Creature attacker, EffectBase effect = null)
    {
        if (CreatureState == ECreatureState.Dead)
            return;

        float finalDamage = 0;
        bool isCriticalHit = false;

        if (effect == null)
        {
            finalDamage = attacker.Atk;

            // 치명타 체크 (0~1000 범위에서 비교)
            float randomValue = Random.Range(0.0f, 100.0f);
            isCriticalHit = randomValue < attacker.CriRate; // 예: CriRate가 0.1일 경우 0.1% 확률로 치명타 발생

            if (isCriticalHit)
            {
                float criticalMultiplier = attacker.CriDmg / 100.0f;
                finalDamage += finalDamage * criticalMultiplier;
            }

        }
        else
        {
            float baseStat = attacker.Atk;
            float ownedValue = effect.SkillData.DamageMultiplier;
            finalDamage = baseStat * (ownedValue / 100f);  // 공격력의 ownedValue%만큼 데미지 계산

            // 스킬 데미지 특성 적용
            if (attacker is Hero)
            {
                HeroInfo heroInfo = Managers.Hero.PlayerHeroInfo;
                float skillDmgBonus = 1.0f + heroInfo.SkillDmgAttr / 100f;
                finalDamage *= skillDmgBonus;

                //Debug.Log($"스킬 데미지 특성 적용: {heroInfo.SkillDmgAttr}% 증가, 최종 데미지: {finalDamage}");
            }
        }

        // 일반 몬스터 공격력, 보스 몬스터 공격력 계산 
        if (attacker is Hero)
        {
            HeroInfo heroInfo = Managers.Hero.PlayerHeroInfo;
            float monsterDmgBonusPercentage =
                (this is BossMonster ? heroInfo.BossMonsterDmgRelic : heroInfo.MonsterDmgRelic);

            // 정확한 퍼센트 값으로 변환 후 적용
            float monsterDmgBonus = 1.0f + monsterDmgBonusPercentage / 100f;
            finalDamage *= monsterDmgBonus;

            //Debug.Log($"몬스터/보스 추가 데미지 적용: {monsterDmgBonusPercentage}% 증가, 최종 데미지: {finalDamage}");
        }

        // 피해 감소 버프가 있을 경우 최종 피해량에 반영
        if (ReduceDmgBuff.Value > 1)
        {
            //Debug.LogWarning("받는 피해 감소가 있습니다." + ReduceDmgBuff.Value);
            float damageReductionMultiplier = 1 - (ReduceDmgBuff.Value - 1);
            finalDamage *= damageReductionMultiplier;
        }

        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);
        UI_DamageTextWorldSpace damageText = Managers.UI.MakeWorldSpaceUI<UI_DamageTextWorldSpace>();
        damageText.SetInfo(CenterPosition, finalDamage, isCriticalHit, effect);

        if (HpBar != null && !HpBar.gameObject.activeSelf)
            HpBar.gameObject.SetActive(true);

        if (Hp <= 0)
        {
            OnDead();
            CreatureState = ECreatureState.Dead;
            return;
        }

        Sprite.color = originalColor;
        Sprite.DOKill(); // 기존 색상 변경 애니메이션 중지
        Sprite.DOColor(Color.red, 0.05f)
            .OnComplete(() => Sprite.DOColor(originalColor, 0.05f));
    }

    public virtual void OnDead()
    {

    }


}
