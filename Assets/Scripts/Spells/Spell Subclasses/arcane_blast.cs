using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class arcane_blast : Spell
{
    private string N;
    private string secondary_damage;
    private Projectile secondary_projectile;

    public arcane_blast(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JToken attributes)
    {
        base.SetAttributes(attributes);

        N = (string)attributes["N"];
        secondary_damage = (string)attributes["secondary_damage"];
        secondary_projectile = attributes["secondary_projectile"].ToObject<Projectile>();
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers, System.Action<Hittable, Vector3> OnHit)
    {
        attributeDictionary["wave"] = GameManager.Instance.currentWave;
        attributeDictionary["power"] = owner.spell_power;
        valueModifiers = modifiers; // save value modifiers

        this.team = team;
        Debug.Log("Where: " + where);
        GameManager.Instance.projectileManager.CreateProjectile(projectile.sprite, ValueModifier.ApplyValueModifiers(projectile.trajectory, "trajectory", valueModifiers), where, target - where, RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(projectile.speed, "speed", valueModifiers), attributeDictionary), OnHit);
        yield return new WaitForEndOfFrame();
    }

    public override void OnHit(Hittable other, Vector3 impact)
    {
        int projectileCount = RPNEvaluator.RPNEvaluator.Evaluate(N, attributeDictionary);

        for (int i = 0; i < projectileCount; i++)
        {
            // Set the angle of each spawned projectile to be evenly spaced
            float angleFloat = ((float) i / (float) projectileCount) * Mathf.PI * 2;
            Vector3 angle = new Vector3(Mathf.Cos(angleFloat), Mathf.Sin(angleFloat), 0);
            GameManager.Instance.projectileManager.CreateProjectile(projectile.sprite, ValueModifier.ApplyValueModifiers(secondary_projectile.trajectory, "trajectory", valueModifiers), impact, angle, RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(secondary_projectile.speed, "speed", valueModifiers), attributeDictionary), OnHitSecondary, RPNEvaluator.RPNEvaluator.Evaluatef(secondary_projectile.lifetime, attributeDictionary));
        }

        if (other.team != team)
        {
            other.Damage(new Damage((int)RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(damage, "damage", valueModifiers), attributeDictionary), GetDamageType()));
        }
    }

    private void OnHitSecondary(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage((int)RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(secondary_damage, "damage", valueModifiers), attributeDictionary), GetDamageType()));
        }
    }
}
