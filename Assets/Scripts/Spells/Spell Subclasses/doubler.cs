using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class doubler : ModifierSpell
{
    private float delay;

    public doubler(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JToken attributes)
    {
        base.SetAttributes(attributes);

        delay = (float) attributes["delay"];
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers, System.Action<Hittable, Vector3> OnHit)
    {
        // Initial cast
        CoroutineManager.Instance.Run(base.Cast(where, target, team, modifiers.ToList(), OnHit));

        // Delay for second cast
        yield return new WaitForSeconds(delay);
        yield return base.Cast(where, target, team, modifiers, OnHit);
    }
}
