using Synty.Interface.FantasyWarriorHUD.Samples;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager2 : MonoBehaviour
{
    public static UIManager2 Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject MainPanel;

    public GameObject SinglePlayerPanel;
    public GameObject MultiPlayerPanel;
    public GameObject SettingsPanel;

    public GameObject CompletePanel;
    public GameObject StartGamePanel;

    public GameObject WaitingPanel;
    public GameObject CountDownPanel;

    public GameObject YouWinPanel;
    public GameObject YouLosePanel;



    [Header("To be deleted")]// 현재 책임이 너무 많은 상태로 Refactoring 예정
    public GameObject title;
    public GameObject CompletePlaceInStartGame;
    public GameObject WaitInStartGame;
    public GameObject CountdownText;
    

    private void Awake()
    {
        // 싱글턴 설정
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지하고 싶으면 사용
    }

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()// Main
    {
        title.SetActive(true);

        MainPanel.SetActive(true);
        SinglePlayerPanel.SetActive(false);
        MultiPlayerPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        CompletePanel.SetActive(false);
        StartGamePanel.SetActive(false);
        WaitingPanel.SetActive(false);
        CountDownPanel.SetActive(false);
       
    }

    public void PressSinglePlayer()// Main -> SinglePlayer
    {
        Debug.Log("Press SinglePlayer");
        MainPanel.SetActive(false);
        SinglePlayerPanel.SetActive(true);

    }
    
    public void PressMultiPlayer()// Main -> MultiPlayer
    {
        Debug.Log("Press MultiPlayer");
        MainPanel.SetActive(false);
        MultiPlayerPanel.SetActive(true);        
    }

    public void PressSettings()//Main -> Setting
    {
        Debug.Log("Press Setting");
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        
    }

    public void PressQuit()//Main -> Quit
    {
        Debug.Log("Press Quit");
        
    }

    public void PressTableinMultiPlayer()//MultiPlayer->Table
    {
        Debug.Log("Press Desk");

    }

    public void PressBedinMultiPlayer()//MultiPlayer->Bed
    {
        Debug.Log("Press Bed");
    }

    public void PressCompleteinMultiPlayer()//MultiPlayer->Complete
    {
        Debug.Log("Press Complete");

        MultiPlayerPanel.SetActive(false);
        CompletePanel.SetActive(true);

        // Place Battle field

    }

    public void ShowWait()//Complete waiting
    {
        Debug.Log("Waiting");
        WaitingPanel.SetActive(true);
        //disable after waiting
        StartCoroutine(WaitingForPlayer());
        WaitingPanel.SetActive(false);

    }

    public void PressStartGameinComplete()//Complete->Start Game
    {
        Debug.Log("Press Battle Start");

        //세션 접속
        title.SetActive(false);
        CompletePanel.SetActive(false);
        StartGamePanel.SetActive(true);

        //서로 Complete를 누른 이후
        
        

        StartCountdown();
    }


    public void PressCompletePlaceInStartGame()//Start Game->Complete Place
    {
        Debug.Log("CompletePlace");
        CompletePlaceInStartGame.SetActive(false);
        WaitInStartGame.SetActive(true);
        //wait for Opponent
     
    }


    public void StartCountdown()
    {
        Debug.Log("Countdown");
        CountDownPanel.SetActive(true);
        StartCoroutine(CountDown(5));
        CountDownPanel.SetActive(false);
    }

    public void YouWin()
    {
        Debug.Log("UI: YouWin");
        StartGamePanel.SetActive(false);
        YouWinPanel.SetActive(true);
        
    }
    public void YouLose()
    {
        Debug.Log("UI: YouLose");
        StartGamePanel.SetActive(false);
        YouLosePanel.SetActive(true);

    }

    public void PressRestartinResult()
    {
        YouWinPanel.SetActive(false);
        YouLosePanel.SetActive(false);

        title.SetActive(true);
        MainPanel.SetActive(true);

    }

    private IEnumerator WaitingForPlayer()
    {
        yield return new WaitForSeconds(2f);
        
    }
    private IEnumerator CountDown(int count)
    {
        
        while (count > 0)
        {
            CountdownText.GetComponent<TextMeshPro>().text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
        }
        
    }

    
}
