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

    public virtual void SetAttributes(JToken attributes)
    {
        attributeDictionary["power"] = 1; // placeholder
        attributeDictionary["wave"] = 1; // placeholder

        name = attributes.SelectToken("name").Value<string>();
        mana_cost = attributes.SelectToken("mana_cost").Value<string>();
        damage = attributes.SelectToken("damage.amount").Value<string>();
        damage_type = Damage.TypeFromString(attributes.SelectToken("damage.type").Value<string>());
        cooldown = attributes.SelectToken("cooldown").Value<string>();
        icon = attributes.SelectToken("icon").Value<int>();
        description = attributes.SelectToken("description").Value<string>();

        projectile = attributes.SelectToken("projectile").ToObject<Projectile>();
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

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        var spelltext = Resources.Load<TextAsset>("spells");
        SetAttributes(JObject.Parse(spelltext.text)["arcane_bolt"]);
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(0, projectile.trajectory, where, target - where, 15f, OnHit);
        yield return new WaitForEndOfFrame();
    }

    protected virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), GetDamageType()));
        }

    }
}

public class ValueModifier
{
    public string value = "";
    public string type = "";

    public static string ApplyValueModifiers(string value, string type, List<ValueModifier> modifiers)
    {
        foreach(ValueModifier modifier in modifiers)
        {
            if (type == modifier.type)
            {
                value += modifier.value;
            }
        }

        return value;
    }
}

public class ModifierSpell : Spell
{
    public Spell baseSpell;
    public List<ValueModifier> modifiers; // To be passed via the cast method

    public ModifierSpell(SpellCaster owner) : base(owner) { }

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

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        return baseSpell.Cast(where, target, team);
    }
}

public class Projectile
{
    public string trajectory = "straight";
    public string speed = "5.0f";
    public int sprite = 0;
    public string lifetime = "-1";
}