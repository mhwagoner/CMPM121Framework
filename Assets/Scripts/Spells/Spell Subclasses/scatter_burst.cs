using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class scatter_burst : ModifierSpell
{
    private string delay;
    private float max_angle;
    private string N;

    public scatter_burst(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JToken attributes)
    {
        base.SetAttributes(attributes);

        delay = (string) attributes["delay"];
        max_angle = (float)attributes["max_angle"];
        N = (string)attributes["N"];
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers, System.Action<Hittable, Vector3> OnHit)
    {
        for(int i = 0; i < RPNEvaluator.RPNEvaluator.Evaluate(N, attributeDictionary); i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(-max_angle, max_angle));
            CoroutineManager.Instance.Run(base.Cast(where, rotation * (target - where) + where, team, modifiers.ToList(), OnHit));
            yield return new WaitForSeconds(RPNEvaluator.RPNEvaluator.Evaluatef(delay, attributeDictionary));
        }
    }
}
