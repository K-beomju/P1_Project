using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroGhost : InitBase
{
    public float ghostDelay;
    private float ghostDelayTime;
    public bool makeGhost;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        this.ghostDelayTime = this.ghostDelay;

        return true;
    }

    void FixedUpdate()
    {
        if (this.makeGhost)
        {
            if (this.ghostDelayTime > 0)
            {
                this.ghostDelayTime -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Managers.Resource.Instantiate("Object/HeroGhost", pooling: true);
                Sprite currentSprite = this.GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.position = this.transform.position;
                currentGhost.transform.localScale = this.transform.localScale;

                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                this.ghostDelayTime = this.ghostDelay;

                // 일정 시간 후 풀로 반환
                StartCoroutine(ReturnToPoolWithDelay(currentGhost, 1f)); // 0.5초 후 반환
            }
        }
    }

    private IEnumerator ReturnToPoolWithDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.Pool.Push(obj);
    }
}
