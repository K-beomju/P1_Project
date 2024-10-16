using Data;
using System;
using UnityEngine;
using static Define;

public class CrimsonShadowEffect : EffectBase
{
    public Action OnAttackAction = () => { };

    public override void SetInfo(int templateID, Hero owner, SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);

        Sprite.sortingOrder = SortingLayers.AURA;
        float scaleX = Mathf.Abs(Owner.transform.localScale.x);
        transform.localScale = new Vector3(scaleX, 1, 1);
        transform.localPosition += new Vector3(0, 1, 0);
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        Owner.OnAttackAction -= SpawnExplosionEffect;
        Owner.OnAttackAction += SpawnExplosionEffect;
    }

    public void SpawnExplosionEffect()
    {
        int rotateZ = UnityEngine.Random.Range(-180, 180);
        GameObject effectObj = Managers.Object.SpawnGameObject(Owner.Target.CenterPosition, EffectData.ExplosionKey);
        ExplosionEffect effect = effectObj.AddComponent<ExplosionEffect>();
        effect.SetInfo(EffectData.DataId, Owner, SkillData);
        effectObj.transform.Rotate(new Vector3(0, 0, rotateZ));
    }

    public override void ClearEffect()
    {
        Owner.OnAttackAction -= SpawnExplosionEffect;
        base.ClearEffect();
    }
}
