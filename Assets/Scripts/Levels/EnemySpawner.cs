using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Diagnostics;
using RPNEvaluator;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;    
    private Dictionary<string, Enemy> enemy_types = new Dictionary<string, Enemy>();
    private Dictionary<string, Level> level_types = new Dictionary<string, Level>();
    private Dictionary<string, int> spawn_var = new Dictionary<string, int>
    { 
        { "base", 0 }, 
        { "wave", 0 } 
    };
    private Level selectedLevel;
    private int currentwave = 1;
    private int currentcount;
    private int count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //parsing enemy and level types from JSON and putting them in dictionaries
        LoadEnemies();
        LoadLevels();

        //instantiate unique button for each level type
        foreach (var lvl in level_types)
        {
            UnityEngine.Debug.Log(lvl.Key);
            GameObject level_button = Instantiate(button, level_selector.transform);
            level_button.transform.localPosition = new Vector3(0, 130);
            level_button.GetComponent<MenuSelectorController>().spawner = this;
            level_button.GetComponent<MenuSelectorController>().SetLevel(lvl.Key);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname) //level name needs to be used somewhere to determine enemies and waves
    {
        selectedLevel = level_types[levelname];

        level_selector.gameObject.SetActive(false);
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()      // should keep track of what wave we are on
    {
        currentwave += 1;
        spawn_var["wave"] = currentwave;
        if (currentwave > selectedLevel.waves)
        {
            //stop running the game
        }
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()     // needs to spawn each type of enemy in the levels spawn list
    {                           // and should spawn the sequence with delay found in spawn class
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        // for (int i = 0; i < 10; ++i)
        // {
        //     yield return SpawnEnemy();
        // }

        foreach (var enemy_type in selectedLevel.spawns)    // not sure if this means all the enemy types spawn at the same time or one at a time
        {                                                   // I think it's one at a time, which is not what we want
            currentcount = 0;
            spawn_var["base"] = enemy_types[enemy_type.enemy].hp;
            count = RPNEvaluator.Evaluate(enemy_type.count, spawn_var); // not sure why it can't find it
            while (true)                                    // the loop that spawns the total count of each enemy
            {
                foreach (int n in enemy_type.sequence)
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (currentcount >= count)
                        {
                            goto count_finished;            // hope this don't cause problems cause I heard it's bad to use this
                        }
                        currentcount += 1;
                        yield return SpawnEnemy();
                    }
                    yield return new WaitForSeconds(enemy_type.delay);
                }
            }
            count_finished:;
        }

        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }

    IEnumerator SpawnEnemy()
    {
        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;
                
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(0);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(50, Hittable.Team.MONSTERS, new_enemy);
        en.speed = 10;
        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }

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