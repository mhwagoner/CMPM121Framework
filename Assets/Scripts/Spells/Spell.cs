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
        attributeDictionary["wave"] = GameManager.Instance.currentWave;
        attributeDictionary["power"] = owner.spell_power;

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

    public virtual string GetFullName()
    {
        return name;
    }

    public string GetDescription()
    {
        return description;
    }

    public virtual string GetFullDescription()
    {
        return GetDescription();
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

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, List<ValueModifier> modifiers, System.Action<Hittable, Vector3> OnHit)
    {
        attributeDictionary["wave"] = GameManager.Instance.currentWave;
        attributeDictionary["power"] = owner.spell_power;
        valueModifiers = modifiers; // save value modifiers

        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(projectile.sprite, ValueModifier.ApplyValueModifiers(projectile.trajectory, "trajectory", valueModifiers), where, target - where, RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(projectile.speed, "speed", valueModifiers), attributeDictionary), OnHit);
        yield return new WaitForEndOfFrame();
    }

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        yield return Cast(where, target, team, new List<ValueModifier>(), OnHit);
    }

    public virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage((int)RPNEvaluator.RPNEvaluator.Evaluatef(ValueModifier.ApplyValueModifiers(damage, "damage", valueModifiers), attributeDictionary), GetDamageType()));
        }

    }
}