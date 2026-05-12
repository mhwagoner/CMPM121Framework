using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
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

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers)
    {
        // Get the rotation to apply then cast
        Quaternion rotation = Quaternion.Euler(0, 0, angle / 2);
        CoroutineManager.Instance.Run(base.Cast(where, rotation * target, team, modifiers));

        // Invert the rotation for the second cast
        rotation = Quaternion.Inverse(rotation);
        return base.Cast(where, rotation * target, team, modifiers);
    }
}
