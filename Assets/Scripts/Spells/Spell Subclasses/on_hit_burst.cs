using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class on_hit_burst : ModifierSpell
{
    private string N;
    private string secondary_damage;

    public on_hit_burst(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JToken attributes)
    {
        base.SetAttributes(attributes);

        N = (string) attributes["N"];
        secondary_damage = (string) attributes["secondary_damage"];
    }

    public override void OnHit(Hittable other, Vector3 impact)
    {
        int projectileCount = RPNEvaluator.RPNEvaluator.Evaluate(N, attributeDictionary);

        float randomAngleOffset = Random.Range(0, Mathf.PI * 2);

        for (int i = 0; i < projectileCount; i++)
        {
            // Set the angle of each spawned projectile to be evenly spaced
            float angleFloat = ((float)i / (float)projectileCount) * Mathf.PI * 2 + randomAngleOffset;
            Vector3 angle = new Vector3(Mathf.Cos(angleFloat), Mathf.Sin(angleFloat), 0);
            GameManager.Instance.projectileManager.CreateProjectile(projectile.sprite, ValueModifier.ApplyValueModifiers(projectile.trajectory, "trajectory", valueModifiers), impact, angle, RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(projectile.speed, "speed", valueModifiers), attributeDictionary), OnHitSecondary, RPNEvaluator.RPNEvaluator.Evaluatef(projectile.lifetime, attributeDictionary));
        }

        base.OnHit(other, impact);
    }

    public void OnHitSecondary(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage((int)RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(secondary_damage, "damage", valueModifiers), attributeDictionary), GetDamageType()));
        }
    }
}
