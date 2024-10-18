using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingArrowEffect : EffectBase
{
    private List<GameObject> homingArrows = new List<GameObject>();


    public override void ApplyEffect()
    {
        base.ApplyEffect();
        SpawnHomingArrow();
    }

    private void SpawnHomingArrow()
    {
        for (int i = 0; i < SkillData.SkillCount; i++)
        {
            GameObject arrow = Managers.Object.SpawnGameObject(Owner.CenterPosition, EffectData.ProjectileKey);
            if (arrow != null)
            {
                homingArrows.Add(arrow);
                HomingArrow homingArrow = arrow.GetComponent<HomingArrow>();
                homingArrow.SetInfo(EffectData.DataId, Owner, SkillData); // 시작 위치 전달
                homingArrow.ApplyEffect(); // 투사체 발사
            }
        }
    }

    public override void ClearEffect()
    {
        foreach (var arrow in homingArrows)
        {
            if (arrow != null)
                Managers.Resource.Destroy(arrow);
        }
        base.ClearEffect();
    }


}
