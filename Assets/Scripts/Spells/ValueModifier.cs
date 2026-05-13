using System.Collections.Generic;
using UnityEngine;

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
        if (modifiers == null)
        {
            return "";
        }

        foreach (ValueModifier modifier in modifiers)
        {
            if (type == modifier.type)
            {
                switch (modifier.behavior)
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