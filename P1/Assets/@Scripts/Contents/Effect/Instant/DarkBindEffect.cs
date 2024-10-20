using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBindEffect : EffectBase
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        List<BaseObject> visibleEnemies = FindVisibleEnemies();
        foreach (var enemy in visibleEnemies)
        {
            GameObject effectObj = Managers.Object.SpawnGameObject(enemy.CenterPosition, EffectData.ExplosionKey);
            effectObj.transform.SetParent(enemy.transform, false);
            effectObj.transform.localPosition = new Vector3(0, 0.3f,0);
            DarkBindExplosionEx effectBase = effectObj.GetComponent<DarkBindExplosionEx>();
            effectBase.SetInfo(EffectData.DataId, Owner, SkillData,enemy as Creature);
        }
    }


    private List<BaseObject> FindVisibleEnemies()
    {
        List<BaseObject> visibleEnemies = new List<BaseObject>();

        Camera cam = Camera.main;
        foreach (BaseObject enemy in Managers.Object.Monsters)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(enemy.transform.position);
            if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                visibleEnemies.Add(enemy); // 화면에 보이면 리스트에 추가
            }
        }

        return visibleEnemies;
    }
}
