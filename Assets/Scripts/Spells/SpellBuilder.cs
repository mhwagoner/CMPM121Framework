using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;


public class SpellBuilder 
{
    private List<Spell> baseSpells;
    private List<ModifierSpell> modifierSpells;

    public Spell Build(SpellCaster owner)
    {
        ModifierSpell mod = modifierSpells[4];
        ModifierSpell mod1 = modifierSpells[0];
        Spell baseSpell = baseSpells[1];
        mod1.baseSpell = mod;
        mod.baseSpell = baseSpell;
        baseSpell.owner = owner;

        return mod1;
    }

   
    public SpellBuilder()
    {
        LoadSpells();
    }

    // Might be best if moved to a json loader script
    private void LoadSpells()
    {
        var spelltext = Resources.Load<TextAsset>("spells");
        JObject spells = JObject.Parse(spelltext.text);

        baseSpells = new List<Spell>();
        modifierSpells = new List<ModifierSpell>();

        foreach(JProperty token in spells.Properties())
        {
            switch(token.Name)
            {
                default:
                    if (token.Value["icon"] != null)
                    {
                        Spell newSpell = new Spell(null);
                        newSpell.SetAttributes(token.Value);
                        baseSpells.Add(newSpell);
                    }
                    else
                    {
                        ModifierSpell newModifierSpell = new ModifierSpell(null);
                        newModifierSpell.SetAttributes(token.Value);
                        modifierSpells.Add(newModifierSpell);
                    }
                    break;
            }
        }
    }
}
