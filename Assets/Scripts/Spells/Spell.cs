using Newtonsoft.Json.Linq;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class Spell
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;
    protected string name = "";
    protected string mana_cost = "1";
    protected string damage = "1";
    protected string cooldown = "1";
    protected int icon = 0;
    protected string description = "";
    protected Damage.Type damage_type = Damage.Type.ARCANE;
    protected Projectile projectile = new Projectile();
    protected Dictionary<string, int> attributeDictionary = new Dictionary<string, int>();
    protected List<ValueModifier> valueModifiers;

    public virtual void SetAttributes(JToken attributes)
    {
        attributeDictionary["power"] = 1; // placeholder
        attributeDictionary["wave"] = 1; // placeholder

        name = (string)(attributes["name"] ?? "");
        mana_cost = (string)(attributes["mana_cost"] ?? "1");
        if (attributes["damage"] != null)
        {
            damage = (string)(attributes["damage"]["amount"] ?? "1");
            damage_type = Damage.TypeFromString((string)(attributes["damage"]["type"] ?? "arcane"));
        }
        cooldown = (string)(attributes["cooldown"] ?? "3");
        icon = (int) (attributes["icon"] ?? 0);
        description = (string)(attributes["description"] ?? "");
        if (attributes["projectile"] != null)
        {
            projectile = attributes["projectile"].ToObject<Projectile>();
        }
        return;
    }

    public Spell(SpellCaster owner)
    {
        this.owner = owner;
    }

    public string GetName()
    {
        return name;
    }

    public virtual int GetManaCost()
    {
        return (int)RPNEvaluator.RPNEvaluator.Evaluatef(mana_cost, attributeDictionary);
    }

    public virtual int GetDamage()
    {
        return (int) RPNEvaluator.RPNEvaluator.Evaluatef(damage, attributeDictionary);
    }

    public virtual float GetCooldown()
    {
        return RPNEvaluator.RPNEvaluator.Evaluatef(cooldown, attributeDictionary);
    }

    public virtual Damage.Type GetDamageType()
    {
        return damage_type;
    }

    public virtual int GetIcon()
    {
        return icon;
    }

    public bool IsReady()
    {
        return (last_cast + GetCooldown() < Time.time);
    }

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers)
    {
        valueModifiers = modifiers;

        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(projectile.sprite, ValueModifier.ApplyValueModifiers(projectile.trajectory, "trajectory", valueModifiers), where, target - where, RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(projectile.speed, "speed", valueModifiers), attributeDictionary), OnHit);
        yield return new WaitForEndOfFrame();
    }

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        yield return Cast(where, target, team, new List<ValueModifier>());
    }

    protected virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage((int)RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(damage, "damage", valueModifiers), attributeDictionary), GetDamageType()));
        }

    }
}

public class ValueModifier
{
    public string value = "";
    public string type = "";
    public string behavior = "add";

    public ValueModifier(string value, string type, string behavior)
    {
        this.value = value;
        this.type = type;
        this.behavior = behavior;
    }

    public static string ApplyValueModifiers(string value, string type, List<ValueModifier> modifiers)
    {
        // Modifier list is empty
        if(modifiers == null)
        {
            return "";
        }

        foreach(ValueModifier modifier in modifiers)
        {
            if (type == modifier.type)
            {
                switch(modifier.behavior)
                {
                    case "add":
                        value += (" " + modifier.value + " +");
                        break;

                    case "multiply":
                        value += (" " + modifier.value + " *");
                        break;

                    case "replace":
                        value = modifier.value;
                        break;
                }
            }
        }

        return value;
    }
}

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

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers)
    {
        // See if modifiers need to be added
        if (this.modifiers != null)
        {
            modifiers.AddRange(this.modifiers);
        }

        return baseSpell.Cast(where, target, team, modifiers);
    }
}

public class Projectile
{
    public string trajectory = "straight";
    public string speed = "5.0f";
    public int sprite = 0;
    public string lifetime = "-1";
}