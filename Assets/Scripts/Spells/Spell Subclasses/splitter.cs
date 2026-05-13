using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class splitter : ModifierSpell
{
    private float angle;

    public splitter(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JToken attributes)
    {
        base.SetAttributes(attributes);

        angle = (float) attributes["angle"];
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers, System.Action<Hittable, Vector3> OnHit)
    {
        // Get the rotation to apply then cast
        Quaternion rotation = Quaternion.Euler(0, 0, angle / 2);
        CoroutineManager.Instance.Run(base.Cast(where, rotation * (target - where) + where, team, modifiers.ToList(), OnHit));

        // Invert the rotation for the second cast
        Quaternion rotationInverse = Quaternion.Inverse(rotation);
        CoroutineManager.Instance.Run(base.Cast(where, rotationInverse * (target - where) + where, team, modifiers, OnHit));
        yield return new WaitForEndOfFrame();
    }
}
