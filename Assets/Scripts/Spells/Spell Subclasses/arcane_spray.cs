using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arcane_spray : Spell
{
    private string N;
    private float spray;

    public arcane_spray(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JToken attributes)
    {
        base.SetAttributes(attributes);

        N = (string)attributes["N"];
        spray = (float)attributes["spray"];
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers, System.Action<Hittable, Vector3> OnHit)
    {
        attributeDictionary["wave"] = GameManager.Instance.currentWave;
        attributeDictionary["power"] = owner.spell_power;
        valueModifiers = modifiers; // save value modifiers

        this.team = team;
        
        for(int i = 0; i < RPNEvaluator.RPNEvaluator.Evaluate(N, attributeDictionary); i++)
        {
            GameManager.Instance.projectileManager.CreateProjectile(projectile.sprite, ValueModifier.ApplyValueModifiers(projectile.trajectory, "trajectory", valueModifiers), where, target - where * (1 + Random.Range(-spray, spray)), RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(projectile.speed, "speed", valueModifiers), attributeDictionary), OnHit, RPNEvaluator.RPNEvaluator.Evaluatef(projectile.lifetime, attributeDictionary));
        }
        yield return new WaitForEndOfFrame();
    }
}
