using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class air_strike : Spell
{
    private string N;
    private float spray;

    public air_strike(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JToken attributes)
    {
        base.SetAttributes(attributes);

        N = (string)attributes["N"];
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers, System.Action<Hittable, Vector3> OnHit)
    {
        attributeDictionary["wave"] = GameManager.Instance.currentWave;
        attributeDictionary["power"] = owner.spell_power;
        valueModifiers = modifiers; // save value modifiers

        this.team = team;

        int projectileCount = RPNEvaluator.RPNEvaluator.Evaluate(N, attributeDictionary);

        for (int i = 0; i < projectileCount; i++)
        {
            // Set the angle of each spawned projectile to be evenly spaced
            float angleFloat = ((float)i / (float)projectileCount) * Mathf.PI * 2;
            Vector3 angle = new Vector3(Mathf.Cos(angleFloat), Mathf.Sin(angleFloat), 0);
            GameManager.Instance.projectileManager.CreateProjectile(projectile.sprite, ValueModifier.ApplyValueModifiers(projectile.trajectory, "trajectory", valueModifiers), target, angle, RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(projectile.speed, "speed", valueModifiers), attributeDictionary), OnHit, RPNEvaluator.RPNEvaluator.Evaluatef(projectile.lifetime, attributeDictionary));
        }
        yield return new WaitForEndOfFrame();
    }
}
