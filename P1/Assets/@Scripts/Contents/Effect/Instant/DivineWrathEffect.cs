using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;

public class DivineWrathEffect : EffectBase
{
    private GameObject swordObject;
    private Camera mainCam;
    private bool isArrive;
    private int areaCount;

    public override void SetInfo(int templateID, Hero owner, SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);

        isArrive = false;
        areaCount = 0;
        mainCam = Camera.main;
        swordObject = transform.GetChild(0).gameObject;

        Vector2 centerPos = mainCam.ScreenToWorldPoint(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        transform.position = new Vector3(centerPos.x, centerPos.y, transform.position.z);
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        StartCoroutine(ActionDivineWrathCo());
    }

    protected override void ProcessDot()
    {
        if (!isArrive)
            return;
        if (areaCount >= 3)
        {
            base.ClearEffect();
            return;
        }

        areaCount += 1;
        GameObject areaEf = Managers.Object.SpawnGameObject(Vector2.zero, "Object/Effect/Explosion/DivineArea");
        areaEf.transform.SetParent(transform, false);
        areaEf.transform.localPosition = Vector2.zero;
        areaEf.GetComponent<EffectBase>().SetInfo(EffectData.DataId, Owner, SkillData);
    }

    private IEnumerator ActionDivineWrathCo()
    {
        // 카메라 위쪽에 위치
        float yPos = mainCam.ScreenToWorldPoint(new Vector3(0, Screen.height * 1.1f, 0)).y;
        swordObject.transform.position = new Vector3(swordObject.transform.position.x, yPos, swordObject.transform.position.z);

        Sprite.DOFade(1, 0.2f);
        // 검이 땅에 박히는 연출
        swordObject.transform.DOLocalMoveY(1, 0.1f).SetEase(Ease.Linear).SetDelay(0.1f).OnComplete(() =>
        {
            // 검이 땅에 박힌 후, 흔들림 효과 연출
            GameObject dustEf = Managers.Object.SpawnGameObject(Vector2.zero, "Object/Effect/Explosion/DevineDust");
            dustEf.transform.SetParent(transform, false);
            dustEf.transform.localPosition = Vector2.zero;
            swordObject.transform.DOShakePosition(0.5f, strength: new Vector3(0.1f, 0.1f, 0), vibrato: 10, randomness: 90, snapping: false, fadeOut: true);

            isArrive = true;
            ProcessDot();
        });
        yield return null;
    }



}
