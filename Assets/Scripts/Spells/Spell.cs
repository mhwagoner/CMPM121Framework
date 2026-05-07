using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Spell 
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;
    protected string name = "Bolt";
    protected string mana_cost = "10";
    protected string damage = "10";
    protected string cooldown = "0.75";
    protected int icon = 0;
    protected Damage.Type damage_type = Damage.Type.ARCANE;
    protected string trajectory = "straight";

    public virtual void SetAttributes(JObject attributes)
    {
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

    public int GetManaCost()
    {
        // apply value modifiers, add to string before RPN eval?
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        return (int)RPNEvaluator.RPNEvaluator.Evaluatef(mana_cost, dictionary);
    }

    public int GetDamage()
    {
        // apply value modifiers
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        return (int) RPNEvaluator.RPNEvaluator.Evaluatef(damage, dictionary);
    }

    public float GetCooldown()
    {
        // apply value modifiers
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        return RPNEvaluator.RPNEvaluator.Evaluatef(cooldown, dictionary);
    }

    public Damage.Type GetDamageType()
    {
        // modifiers
        return damage_type;
    }

    public string GetTrajectory()
    {
        // modifiers
        return trajectory;
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
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(0, GetTrajectory(), where, target - where, 15f, OnHit);
        yield return new WaitForEndOfFrame();
    }

    void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), GetDamageType()));
        }

    }

}
