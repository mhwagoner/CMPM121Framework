using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;    
    private Dictionary<string, Enemy> enemy_types = new Dictionary<string, Enemy>();
    public int currentwave = 1;
    public int currentcount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject selector = Instantiate(button, level_selector.transform);
        selector.transform.localPosition = new Vector3(0, 130);
        selector.GetComponent<MenuSelectorController>().spawner = this;
        selector.GetComponent<MenuSelectorController>().SetLevel("Start");
        LoadEnemies();
        LoadLevels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname)
    {
        level_selector.gameObject.SetActive(false);
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()      // should keep track of what wave we are on
    {
        currentwave += 1;
        // if (currentwave > selectedLevel.waves)  // waiting for buttons to be done
        // {
        //     //stop running the game
        // }
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
        for (int i = 0; i < 10; ++i)
        {
            yield return SpawnEnemy();
        }

        // foreach (var enemy_type in selectedLevel.spawns)
        // {
        //     currentcount = 0;
        //     while (currentcount <= enemy_type.count) // need to RPNcalculator the count first
        //     {
        //         yield return new WaitForSeconds(enemy_type.delay);
        //         for (int i = 0; i < enemy_type.sequence; i++)  // figure out better way to do the sequence spawning
        //         {                                              // and to make sure it spawns not over the count limit
        //             yield return SpawnEnemy();
        //         }
        //     }
        // }

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
        }
    }
}