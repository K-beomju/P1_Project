using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseObject : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public Collider2D Collider { get; protected set; }
    public SpriteRenderer Sprite { get; protected set; }

    public Vector3 CenterPosition => Collider.bounds.center;

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
        DataTemplateID = dataTemplateID;
    }

}
