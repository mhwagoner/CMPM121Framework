using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ModifierSpell : Spell
{
    public Spell baseSpell;
    public List<ValueModifier> modifiers;

    public ModifierSpell(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JToken attributes)
    {
        base.SetAttributes(attributes);
        modifiers = new List<ValueModifier>();

        if (attributes["damage_adder"] != null) { modifiers.Add(new ValueModifier((string)attributes["damage_adder"], "damage", "add")); }
        if (attributes["damage_multiplier"] != null) { modifiers.Add(new ValueModifier((string)attributes["damage_multiplier"], "damage", "multiply")); }

        if (attributes["mana_adder"] != null) { modifiers.Add(new ValueModifier((string)attributes["mana_adder"], "mana", "add")); }
        if (attributes["mana_multiplier"] != null) { modifiers.Add(new ValueModifier((string)attributes["mana_multiplier"], "mana", "multiply")); }

        if (attributes["cooldown_adder"] != null) { modifiers.Add(new ValueModifier((string)attributes["cooldown_adder"], "cooldown", "add")); }
        if (attributes["cooldown_multiplier"] != null) { modifiers.Add(new ValueModifier((string)attributes["cooldown_multiplier"], "cooldown", "multiply")); }

        if (attributes["speed_adder"] != null) { modifiers.Add(new ValueModifier((string)attributes["speed_adder"], "speed", "add")); }
        if (attributes["speed_multiplier"] != null) { modifiers.Add(new ValueModifier((string)attributes["speed_multiplier"], "speed", "multiply")); }

        if (attributes["projectile_trajectory"] != null) { modifiers.Add(new ValueModifier((string)attributes["projectile_trajectory"], "trajectory", "replace")); }

        return;
    }

    public override int GetManaCost()
    {
        return (int)RPNEvaluator.RPNEvaluator.Evaluate(baseSpell.GetManaCost().ToString(), attributeDictionary);
    }

    public override int GetDamage()
    {
        return (int)RPNEvaluator.RPNEvaluator.Evaluatef(baseSpell.GetDamage().ToString(), attributeDictionary);
    }

    public override float GetCooldown()
    {
        return RPNEvaluator.RPNEvaluator.Evaluatef(baseSpell.GetCooldown().ToString(), attributeDictionary);
    }

    public override Damage.Type GetDamageType()
    {
        return baseSpell.GetDamageType();
    }

    public override int GetIcon()
    {
        return baseSpell.GetIcon();
    }

    public override string GetFullName()
    {
        return name + " " + baseSpell.GetFullName();
    }

    public override string GetFullDescription()
    {
        return name + ": " + description + "\n" + baseSpell.GetFullDescription();
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers, System.Action<Hittable, Vector3> OnHit)
    {
        attributeDictionary["wave"] = GameManager.Instance.currentWave;
        attributeDictionary["power"] = owner.spell_power;
        valueModifiers = modifiers; // save value modifiers

        // See if modifiers need to be added
        if (this.modifiers != null)
        {
            modifiers.AddRange(this.modifiers);
        }

        return baseSpell.Cast(where, target, team, modifiers, OnHit);
    }

    public override void OnHit(Hittable other, Vector3 impact)
    {
        baseSpell.OnHit(other, impact);
    }
}