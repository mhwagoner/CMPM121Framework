using UnityEngine;
using System.Collections.Generic;

public class Relic
{
    private string name;
    private int sprite;
    private RelicTrigger trigger;
    private RelicEffect effect;

    public string GetLabel()
    {
        return name;
    }
}

public class RelicTrigger
{
    private string description;
    private string type;
    private string amount;
}

public class RelicEffect
{
    private string description;
    private string type;
    private string amount;
    private string until;
}
