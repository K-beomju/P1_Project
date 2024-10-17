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
            frostRainArrows.Add(arrow);

            FrostRainArrow rainArrow = arrow.GetComponent<FrostRainArrow>();
            rainArrow.SetInfo(EffectData.DataId, Owner, SkillData, startPos);
            rainArrow.ApplyEffect();

            yield return spawnDelay;
        }
    }

    public override void ClearEffect()
    {
        for (int i = 0; i < frostRainArrows.Count; i++)
        {
            if (frostRainArrows[i] != null)
                Managers.Resource.Destroy(frostRainArrows[i]);
        }
        base.ClearEffect();
    }
}
