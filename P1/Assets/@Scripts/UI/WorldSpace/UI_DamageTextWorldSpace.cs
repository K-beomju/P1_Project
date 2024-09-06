using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using static Define;

public class UI_DamageTextWorldSpace : UI_Base
{
    private enum TMPTexts
    {
        DamageText,
    }

    private TMP_Text _damageText;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Bind
        BindTMPTexts(typeof(TMPTexts));
        _damageText = GetTMPText((int)TMPTexts.DamageText);
           
        Canvas canvas = GetComponent<Canvas>();
        // 애매함 
        canvas.sortingOrder = SortingLayers.DAMAGE_FONT;
        return true;
    }

    public void SetInfo(Vector3 pos, float damage = 0, bool isCritical = false)
	{
		transform.position = pos + new Vector3(0, 0.25f, 0);

		if (damage < 0)
		{
			_damageText.color = Util.HexToColor("4EEE6F");
		}
		else if (isCritical)
		{
			_damageText.color = Util.HexToColor("EFAD00");
		} 

		_damageText.text = $"{Mathf.Abs(damage)}";
		_damageText.alpha = 1;
		DoAnimation();
	}

	private void DoAnimation()
	{
		Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(transform.position.y + 0.5f, 0.2f))
        .Join(_damageText.DOFade(0, 0.5f).SetEase(Ease.InQuint))
		.OnComplete(() =>
		{
			Managers.Resource.Destroy(gameObject);
		});
	}
}