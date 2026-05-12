using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUI spell1ui;

    public string hpmax = "95 wave 5 * +";
    public string manamax = "90 wave 10 * +";
    public string manaRegeneration = "wave 10 +";
    public string spellPower = "wave 10 *";
    public int speed;

    public Unit unit;
    public TextMeshProUGUI waveStatsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;
    }

    public void StartLevel()
    {
        spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
        StartCoroutine(spellcaster.ManaRegeneration());
        
        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
        spell1ui.SetSpell(spellcaster.spells[0]);
    }

    public void UpdatePlayerStats()
    {
        Dictionary<string, int> attributeDictionary = new Dictionary<string, int>();
        attributeDictionary["wave"] = GameManager.Instance.currentWave;

        hp.SetMaxHP(RPNEvaluator.RPNEvaluator.Evaluate(hpmax, attributeDictionary));
        spellcaster.max_mana = RPNEvaluator.RPNEvaluator.Evaluate(manamax, attributeDictionary);
        spellcaster.mana_reg = RPNEvaluator.RPNEvaluator.Evaluate(manaRegeneration, attributeDictionary);
        spellcaster.spell_power = RPNEvaluator.RPNEvaluator.Evaluate(spellPower, attributeDictionary);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        unit.movement = value.Get<Vector2>()*speed;
    }

    void Die()
    {
        Debug.Log("You Lost");
        //update text to say player loses
        GameManager.Instance.LevelLost();
    }

}
