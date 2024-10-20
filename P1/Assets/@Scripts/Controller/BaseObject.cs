using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseObject : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public Collider2D Collider { get; protected set; }
    public SpriteRenderer Sprite { get; protected set; }

    public Vector3 CenterPosition
    {
        get
        {
            if (Collider == null || Collider.Equals(null))
                return transform.position; // Collider가 없으면 transform.position 반환

            return Collider.bounds.center;
        }
    }
    public int DataTemplateID { get; set; }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Collider = GetComponent<Collider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        return true;
    }

    public virtual void SetInfo(int dataTemplateID)
    {
        if (dataTemplateID == 0)
            return;

        DataTemplateID = dataTemplateID;
    }

    protected void LookAt(Vector2 dir)
    {
        Vector3 scale = transform.localScale;
        scale.x = dir.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
