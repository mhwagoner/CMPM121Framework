using TMPro;
using UnityEngine;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardScreen;
    private TextMeshProUGUI rewardScreenText;
    private GameObject nextButton;
    private GameObject retryButton;
    public GameObject[] dropButtons;
    private GameObject takeButton;
    private GameObject rewardSpellUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextButton = rewardScreen.transform.Find("NextButton").gameObject;
        retryButton = rewardScreen.transform.Find("RetryButton").gameObject;
        takeButton = rewardScreen.transform.Find("TakeButton").gameObject;
        rewardSpellUI = rewardScreen.transform.Find("RewardSpell").gameObject;
        rewardScreenText = rewardScreen.transform.Find("RewardScreenText").gameObject.GetComponent<TextMeshProUGUI>();
        GameManager.Instance.rewardScreenText = rewardScreenText;
        GameManager.Instance.rewardSpellUI = rewardSpellUI;
        rewardScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state != GameManager.GameState.REWARDS) //if not on rewards screen, hide drop buttons and reward spell UI
        {
            rewardSpellUI.SetActive(false);

            foreach (GameObject button in dropButtons)
            {
                button.SetActive(false);
            }
        }
        if (GameManager.Instance.state == GameManager.GameState.REWARDS) //rewards screen
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next Wave";
            nextButton.SetActive(true);
            retryButton.SetActive(false);
            rewardSpellUI.SetActive(true);
            //if player has less than 4 spells:
            if (GameManager.Instance.player.GetComponent<PlayerController>().spellcaster.spells.Count < 4)
            {
                takeButton.SetActive(true);
                foreach (GameObject button in dropButtons)//hide drop buttons
                {
                    button.SetActive(false);
                }
            } 
            else if (GameManager.Instance.player.GetComponent<PlayerController>().spellcaster.spells.Count >= 4)
            {
                takeButton.SetActive(false);
                foreach (GameObject button in dropButtons)//show drop buttons
                {
                    button.SetActive(true);
                }
            }
            rewardScreen.SetActive(true);
        }
        else if (GameManager.Instance.state == GameManager.GameState.WAVEEND) //stats screen
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
            nextButton.SetActive(true);
            retryButton.SetActive(false);
            takeButton.SetActive(false);
            rewardScreen.SetActive(true);
        }
        else if (GameManager.Instance.state == GameManager.GameState.GAMEOVER) //game end screen
        {
            nextButton.SetActive(false);
            retryButton.SetActive(true);
            takeButton.SetActive(false);
            rewardScreen.SetActive(true);
        }
        else
        {
            rewardScreen.SetActive(false);
        }
    }
}
