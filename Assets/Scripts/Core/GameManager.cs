using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER
    }
    public GameState state;

    public int countdown;
    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }

    public GameObject player;
    
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }
    public float waveTime = 0.0f;
    private float totalWaveTime = 0.0f;
    private int totalEnemiesRemoved = 0;
    private int waveEnemiesRemoved = 0;
    public int totalSpellsCasted = 0;
    public int waveSpellsCasted = 0;

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        totalEnemiesRemoved++;
        waveEnemiesRemoved++;
    }

    public void AddWaveTime(float addedTime)
    {
        //totalwa
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    private GameManager()
    {
        enemies = new List<GameObject>();
    }

    public void UpdateText(TextMeshProUGUI UItext, string newStats)
    {
        UItext.text = newStats;
    }

    public void WaveWon(int currentWave, TextMeshProUGUI waveStatsText)
    {
        UpdateText(waveStatsText, 
        "Wave " + currentWave + " Stats\n" +
        "=+=+=+=+=+=+=\n" + 
        "Seconds Elapsed: " + Mathf.Round(waveTime) + "\n" +
        "Damage Dealt: " + "A lot" + "\n" +
        "Spells Used: " + waveSpellsCasted + "\n" +
        "Enemies Killed: " + waveEnemiesRemoved
        );
        totalSpellsCasted += waveSpellsCasted;
        waveSpellsCasted = 0;
        totalWaveTime += waveTime;
        waveEnemiesRemoved = 0;
    }

    public void LevelWon(TextMeshProUGUI waveStatsText)
    {
        //update text to say player wins
        GameManager.Instance.UpdateText(waveStatsText, 
        "You Freaking Beat the Level!!!\n" +
        "=+=+=+=+=+=+=\n" + 
        "Total Seconds Elapsed: " + Mathf.Round(totalWaveTime) + "\n" +
        "Total Damage Dealt: " + "A lot" + "\n" +
        "Total Spells Used: " + totalSpellsCasted + "\n" +
        "Total Enemies Killed: " + totalEnemiesRemoved
        );
        state = GameState.GAMEOVER; 
    }

    public void ResetGame()
    {
        totalSpellsCasted = 0;
        totalEnemiesRemoved = 0;
        totalWaveTime = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        EventBus.Instance.DestroyInstance();
        theInstance = null;
    }
}
