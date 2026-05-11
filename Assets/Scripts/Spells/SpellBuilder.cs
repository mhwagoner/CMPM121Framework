using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class SpellBuilder 
{
    private List<JProperty> baseSpells;
    private List<JProperty> modifierSpells;
    private JToken baseSpellTokens;
    private JToken modifierSpellTokens;

    public Spell Build(SpellCaster owner)
    {
        int randomSpell = UnityEngine.Random.Range(0, baseSpells.Count);
        Spell baseSpell = LoadSpell(baseSpells[randomSpell], owner);

        // Add a random number of modifiers
        List<JProperty> modifierSpellsCopy = modifierSpells.ToList();
        int modifierCount = UnityEngine.Random.Range(0, modifierSpells.Count);
        for(int i = 0; i < modifierCount; i++)
        {
            // Add the modifier
            int randomModifier = UnityEngine.Random.Range(0, modifierSpellsCopy.Count);
            ModifierSpell modifierSpell = LoadModifier(modifierSpellsCopy[randomModifier], owner);
            modifierSpell.baseSpell = baseSpell;

            baseSpell = modifierSpell;// Replace baseSpell
            modifierSpellsCopy.RemoveAt(randomModifier); // Remove modifier to avoid duplicates
        }

        Debug.Log(baseSpell.GetFullName());

        return baseSpell;
    }

   
    public SpellBuilder()
    {
        LoadSpells();
    }

    private Spell LoadSpell(JProperty spell, SpellCaster owner)
    {
        Type type = Type.GetType(spell.Name) ?? typeof(Spell); // See if spell is a subclass

        Spell newSpell = (Spell) Activator.CreateInstance(type, owner);
        newSpell.SetAttributes(spell.Value);

        return newSpell;
    }

    private ModifierSpell LoadModifier(JProperty spell, SpellCaster owner)
    {
        Type type = Type.GetType(spell.Name) ?? typeof(ModifierSpell);

        ModifierSpell newSpell = (ModifierSpell)Activator.CreateInstance(type, owner);
        newSpell.SetAttributes(spell.Value);

        return newSpell;
    }

    // Might be best if moved to a json loader script
    private void LoadSpells()
    {
        var spelltext = Resources.Load<TextAsset>("spells");
        JObject spells = JObject.Parse(spelltext.text);

        baseSpells = new List<JProperty>();
        modifierSpells = new List<JProperty>();
        baseSpellTokens = spells["spell"];
        modifierSpellTokens = spells["modifier"];

        foreach(JProperty token in baseSpellTokens)
        {
            baseSpells.Add(token);
        }

        foreach (JProperty token in modifierSpellTokens)
        {
            modifierSpells.Add(token);
        }
    }
}
