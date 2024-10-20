using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DarkWarriorEffect : EffectBase
{
    private List<GameObject> monsters = new List<GameObject>();
    private Sprite[] sprites = new Sprite[5];
    private float moveSpeed = 50; // 이동 속도 설정

    public override void SetInfo(int templateID, Hero owner, SkillData skillData)
    {
        base.SetInfo(templateID, owner, skillData);

        List<GameObject> closestMonsters = FindClosestTargets(Managers.Object.Monsters, 5);
        if (closestMonsters == null || closestMonsters.Count == 0)
        {
            base.ClearEffect();
            return;
        }

        monsters.AddRange(closestMonsters);

        int currnetIndex = 0;
        while (monsters.Count < 5)
        {
            monsters.Add(monsters[currnetIndex % monsters.Count]);
            currnetIndex++;
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = Managers.Resource.Load<Sprite>($"Sprites/Skill/DarkWarrior/sword_0{i}");
        }
    }

    public override void ApplyEffect()
    {
        StartCoroutine(ShowDarkWarrior());
    }

    private IEnumerator ShowDarkWarrior()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            // 스프라이트 변경
            Sprite.sprite = sprites[i % sprites.Length];

            if (monsters[i] == null)
            {
                continue;
            }

            Vector3 targetPosition = monsters[i].transform.position;

            // 이동 시작
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                // 이동 중 몬스터가 죽었는지 체크
                if (monsters[i] == null)
                {
                    break;
                }
                yield return null;
            }

            // 몬스터가 존재하면 효과 적용
            if (monsters[i] != null)
            {
                DarkWarriorEx darkWarrior = Managers.Object.
                    SpawnGameObject(monsters[i].transform.position, EffectData.ExplosionKey).GetComponent<DarkWarriorEx>();
                darkWarrior.SetInfo(EffectData.DataId, Owner, SkillData, monsters[i].GetComponent<Creature>());
            }
            yield return new WaitForSeconds(0.1f);
        }

        base.ClearEffect();
    }

    private List<GameObject> FindClosestTargets(IEnumerable<BaseObject> objs, int count)
    {
        // 오브젝트들을 거리 순으로 정렬
        List<BaseObject> sortedObjs = objs.OrderBy(obj =>
            (obj.transform.position - Owner.CenterPosition).sqrMagnitude).ToList();

        List<GameObject> closestMonsters = new List<GameObject>();

        // 가장 가까운 몬스터들을 리스트에 추가
        for (int i = 0; i < sortedObjs.Count && i < count; i++)
        {
            closestMonsters.Add(sortedObjs[i].gameObject);
        }

        return closestMonsters;
    }
}
