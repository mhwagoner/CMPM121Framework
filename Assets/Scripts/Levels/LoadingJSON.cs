using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class LoadingJSON    // could this work? hopefully we could still access everything we need to if we did this
{
    private Dictionary<string, Enemy> enemy_types = new Dictionary<string, Enemy>();
    private Dictionary<string, Level> level_types = new Dictionary<string, Level>();

    // https://yawgmoth.github.io/CMPM121/slides/lecture4.html#15 :3c
    public void LoadEnemies()
    {
        var enemytext = Resources.Load<TextAsset>("enemies");
        JToken jo = JToken.Parse(enemytext.text);
        foreach (var enemy in jo)
        {
            Enemy en = enemy.ToObject<Enemy>();
            enemy_types[en.name] = en;
        }
    }

    public void LoadLevels()
    {
        var leveltext = Resources.Load<TextAsset>("levels");
        JToken jo = JToken.Parse(leveltext.text);
        foreach (var level in jo)
        {
            Level lvl = level.ToObject<Level>();
            level_types[lvl.name] = lvl;
        }
    }
}