using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;

public class SpellCaster 
{
    public int mana;
    public int max_mana;
    public int mana_reg;
    public int spell_power = 5;
    public Hittable.Team team;
    public int activeSpell = 0;
    public List<Spell> spells;

    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            mana += mana_reg;
            mana = Mathf.Min(mana, max_mana);
            yield return new WaitForSeconds(1);
        }
    }

    public SpellCaster(int mana, int mana_reg, Hittable.Team team)
    {
        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.team = team;
        spells = new List<Spell>();
        spells.Add(new SpellBuilder().Build(this));
    }

    public void SwitchSpell()
    {
        if (activeSpell+1 < spells.Count) //if there are more spells, switch to the next one
        {
            activeSpell++;
        } else
        {
            activeSpell = 0; //return to first spell
        }
    }

    public IEnumerator Cast(Vector3 where, Vector3 target)
    {        
        if (mana >= spells[activeSpell].GetManaCost() && spells[activeSpell].IsReady())
        {
            mana -= spells[activeSpell].GetManaCost();
            GameManager.Instance.waveSpellsCasted++;
            yield return spells[activeSpell].Cast(where, target, team);
        }
        yield break;
    }

}
