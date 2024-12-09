using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExplosiveGrenadeEffect : EffectBase
{
    private List<GameObject> grenades = new List<GameObject>();

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        StartCoroutine(SpawnGrenadeCo());
    }

    private IEnumerator SpawnGrenadeCo()
    {
        for (int i = 0; i < SkillData.SkillCount; i++)
        {
            GameObject grenade = Managers.Object.SpawnGameObject(Owner.CenterPosition, EffectData.ProjectileKey);
            if (grenade == null)
                yield break;

            grenades.Add(grenade);
            Vector3 targetPosition = Owner.CenterPosition + new Vector3(Random.Range(-2f, 2f), Random.Range(-2, 2));
            float height = 1f;
            grenade.transform.DOJump(targetPosition, height, 1, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                grenade.transform.DOJump(grenade.transform.position, height / 2, 1, 0.5f).SetEase(Ease.OutQuad);
            });

            ExplosiveGrenade explosiveGrenade = grenade.GetComponent<ExplosiveGrenade>();
            explosiveGrenade.SetInfo(EffectData.DataId, Owner, SkillData);
            yield return new WaitForSeconds(0.1f); // 각 슈루탄 생성 간 딜레이
        }

        yield return new WaitForSeconds(1);
    }
}
