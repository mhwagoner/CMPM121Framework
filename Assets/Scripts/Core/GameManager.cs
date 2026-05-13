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
        REWARDS,
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
    public TextMeshProUGUI rewardScreenText;
    public GameObject rewardSpellUI;
    public EnemySpawner enemySpawner;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }
    public float waveTime = 0.0f;
    private float totalWaveTime = 0.0f;
    private int totalEnemiesRemoved = 0;
    private int waveEnemiesRemoved = 0;
    public int totalSpellsCasted = 0;
    public int waveSpellsCasted = 0;
    public int currentWave = 0; // needs to be accessable by Spells
    public Spell rewardSpell;

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
        waveTime = addedTime;
        totalWaveTime += addedTime;
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

    public void UpdateText(string newText)
    {
        rewardScreenText.text = newText;
    }

    public void UpdateText(TextMeshProUGUI UItext, string newText)
    {
        UItext.text = newText;
    }

    public void WaveWon(int currentWave)
    {
        state = GameManager.GameState.WAVEEND;
        UpdateText( 
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
        this.currentWave = currentWave;

        // Level up player stats
        player.GetComponent<PlayerController>().UpdatePlayerStats();
    }

    public void Rewards()
    {
        state = GameState.REWARDS;

        //generate new spell
        rewardSpell = new SpellBuilder().Build(player.GetComponent<PlayerController>().spellcaster);
        rewardSpellUI.GetComponent<SpellUI>().SetSpell(rewardSpell);

        UpdateText( 
        "New Spell: " + rewardSpell.GetFullName() + "\n" +
        "=+=+=+=+=+=+=\n"
        );
    }

    public void LevelWon()
    {
        //update text to say player wins
        UpdateText(
        "You Freaking Beat the Level!!!\n" +
        "=+=+=+=+=+=+=\n" + 
        "Total Seconds Elapsed: " + Mathf.Round(totalWaveTime) + "\n" +
        "Total Damage Dealt: " + "A lot" + "\n" +
        "Total Spells Used: " + totalSpellsCasted + "\n" +
        "Total Enemies Killed: " + totalEnemiesRemoved
        );
        state = GameState.GAMEOVER;
    }

    public void LevelLost()
    {
        //update text to say player loses
        UpdateText(
        "You Freaking Lost Bro!!!\n" +
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
