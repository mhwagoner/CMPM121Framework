using TMPro;
using UnityEngine;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardScreen;
    private TextMeshProUGUI rewardScreenText;
    private GameObject nextButton;
    private GameObject retryButton;
    private GameObject dropButton;
    private GameObject takeButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextButton = rewardScreen.transform.Find("NextButton").gameObject;
        retryButton = rewardScreen.transform.Find("RetryButton").gameObject;
        takeButton = rewardScreen.transform.Find("TakeButton").gameObject;
        dropButton = rewardScreen.transform.Find("DropButton").gameObject;
        rewardScreenText = rewardScreen.transform.Find("RewardScreenText").gameObject.GetComponent<TextMeshProUGUI>();
        GameManager.Instance.rewardScreenText = rewardScreenText;
        rewardScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.REWARDS)
        {
            retryButton.SetActive(false);
            nextButton.GetComponentInChildren<TextMeshPro>().text = "Next Wave";
            nextButton.SetActive(true);
            //if player has less than 4 spells:
            //takeButton.SetActive(true);
            //if player has 4 spells:
            //dropButton.SetActive(true);
            rewardScreen.SetActive(true);
        }
        else if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            retryButton.SetActive(false);
            nextButton.GetComponentInChildren<TextMeshPro>().text = "Next";
            nextButton.SetActive(true);
            rewardScreen.SetActive(true);
        }
        else if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            nextButton.SetActive(false);
            retryButton.SetActive(true);
            rewardScreen.SetActive(true);
        }
        else
        {
            rewardScreen.SetActive(false);
        }
    }
}
