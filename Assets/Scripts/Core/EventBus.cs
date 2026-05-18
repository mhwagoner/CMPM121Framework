using UnityEngine;
using System;
using Unity.VisualScripting;

public class EventBus 
{
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    public event Action<Vector3, Damage, Hittable> OnDamage;
    
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
    }

    public void OnRelicPickup(Relic relic)
    {
        
    }

    public void DestroyInstance()
    {
        theInstance = null;
    }

}
