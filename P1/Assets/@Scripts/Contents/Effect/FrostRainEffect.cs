using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostRainEffect : EffectBase
{
    private List<GameObject> frostRainArrows = new List<GameObject>();
    private WaitForSeconds spawnDelay = new WaitForSeconds(0.1f);

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        StartCoroutine(SpawnRainArrowCo());
    }

    private IEnumerator SpawnRainArrowCo()
    {
        for (int i = 0; i < SkillData.SkillCount; i++)
        {
            //0.5에서 1.5정도 
            float xPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * Random.Range(0.2f, 2.2f), 0, 0)).x;
            float yPos = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height * 1.1f, 0)).y;

            Vector3 startPos = new Vector3(xPos, yPos, 0);

            GameObject arrow = Managers.Object.SpawnGameObject(startPos, "Object/Effect/Projectile/FrostRainArrow");
            if (arrow != null)
                frostRainArrows.Add(arrow);

            FrostRainArrow rainArrow = arrow.GetComponent<FrostRainArrow>();
            rainArrow.startPos = startPos;
            rainArrow.SetInfo(EffectData.DataId, Owner, SkillData);
            rainArrow.ApplyEffect();

            yield return spawnDelay;
        }
    }

    public override void ClearEffect()
    {
        // 공통적으로 사용되는 정리 로직
        foreach (var arrow in frostRainArrows)
        {
            if (arrow != null)
                Managers.Resource.Destroy(arrow);
        }
        base.ClearEffect();
    }
}
