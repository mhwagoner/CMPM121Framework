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
    private Level selectedLevel;
    private SpawnPoint spawn_point;
    public List<SpawnPoint> SpawnPoints_type;
    private int currentWave = 1;
    private float waveStartTime;
    private float waveEndTime;
    private int enemy_spawns_cleared = 0;

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
        currentWave += 1;
        if (currentWave > selectedLevel.waves)
        {
            //stop running the game
        }
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()     // needs to spawn each type of enemy in the levels spawn list
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        //start timer

        enemy_spawns_cleared = 0; // track how many enemy coroutines have finished

        foreach (var spawn_type in selectedLevel.spawns)
        {
            StartCoroutine(SpawnEnemyType(spawn_type));
        }

        yield return new WaitWhile(() => (GameManager.Instance.enemy_count > 0 || enemy_spawns_cleared < selectedLevel.spawns.Count));
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
        //end timer
        //update variable in gameManager

    }

    IEnumerator SpawnEnemyType(Spawn spawn_type)    // spawns all enemies of a single type
    {
        Dictionary<string, int> spawn_var = new Dictionary<string, int>
        {
            { "base",  enemy_types[spawn_type.enemy].hp},
            { "wave", currentWave }
        };
        int currentCount = 0;
        int count = RPNEvaluator.RPNEvaluator.Evaluate(spawn_type.count, spawn_var);

        while (currentCount < count)     // the main loop that spawns the total count of each enemy
        {
            foreach (int n in spawn_type.sequence)
            {
                for (int i = 0; i < n; i++)     // spawn as many enemies as values of sequence
                {
                    if (currentCount < count)   // stop spawning if already spawned wave total
                    {
                        currentCount += 1;
                        SpawnEnemy(spawn_type, spawn_var);
                    }
                }

                yield return new WaitForSeconds(spawn_type.delay);
            }
        }

        enemy_spawns_cleared++;
    }

    public void SpawnEnemy(Spawn spawn_type, Dictionary<string, int> spawn_var)    // spawns single enemy
    {
        string[] location = spawn_type.location.Split(' ');
        if (location.Length == 1)
        {
           spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];  // random location
        }
        else
        {
            // go through SpawnPoints array and add the right kind/type to its own list
            foreach (SpawnPoint sp in SpawnPoints)
            {
                if (sp.kind.ToString() == location[1].ToUpper())
                {
                    SpawnPoints_type.Add(sp);
                }
            }
            spawn_point = SpawnPoints_type[Random.Range(0, SpawnPoints_type.Count)];
        }
        Vector2 offset = Random.insideUnitCircle * 1.8f;
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        print(spawn_type.enemy);
        Enemy enemy_data = enemy_types[spawn_type.enemy];
        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(enemy_data.sprite);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(RPNEvaluator.RPNEvaluator.Evaluate(spawn_type.hp, spawn_var), Hittable.Team.MONSTERS, new_enemy);
        en.speed = enemy_data.speed;
        en.damage = enemy_data.damage;
        GameManager.Instance.AddEnemy(new_enemy);
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