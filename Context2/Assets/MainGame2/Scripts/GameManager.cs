using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{

    public TMP_InputField round1Scenario, round1PositiveOutcome, round1NegativeOutcome;
    public TMP_Text round1ScenarioText, round1ScenarioTextShown, round1PositiveOutcomeText, round1NegativeOutcomeText;
    public Slider round1PlayerChoicesSlider;
    public Slider round1PlayerVoteSlider;
    public GameObject nextSectionButton;
    public int[] roundTimers;
    public float timer;
    public bool timerIsRunning;
    public TMP_Text timerText;
    PhotonView photonView;
    int playerID;
    int playerCount;

    public Transform resetPosition;
    public Transform playerReviewPosition;
    public Transform[] playerPositiveChoices;
    public Transform[] playerNegativeChoices;

    [SerializeField]
    private RoundObjects[] roundOBJ;
    [System.Serializable]
    public class RoundObjects
    {
        public GameObject[] objects;
    }

    public GameObject playerItem;
    public PlayerItem playerItemPrefab;
    public Transform playerParent;
    public List<GameObject> playerItemsList;
    public List<GameObject> redonePlayerItemsList;
    public GameObject[] playerScorePositions;
    public GameObject shuffleScreen;
    public GameObject[] shuffleParents;

    public GameObject waitPlayerVote;
    public GameObject waitForPlayers;
    public GameObject[] selectedButtons;
    public GameObject positivePanelHighlight;
    public GameObject negativePanelHighlight;

    public string[] round1Scenarios, round1PositiveOutcomes, round1NegativeOutcomes;
    public List<string> round1ScrambledScenarios;
    List<string> scenariosToReturnPerPlayer;
    public int[] round1PlayerChoices;
    public int[] round1PlayerVotes;
    public int roundSection = 0;
    public bool nextRound;
    public int currentPlayerShown = 0;
    public bool allOutcomesDone = false;

    public int everyoneSubmitted = 0;

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        playerItemsList = new List<GameObject>();
        redonePlayerItemsList = new List<GameObject>();
        photonView = PhotonView.Get(this);
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        round1Scenarios = new string[playerCount];
        round1ScrambledScenarios = new List<string>(playerCount);
        round1PositiveOutcomes = new string[playerCount];
        round1NegativeOutcomes = new string[playerCount];
        round1PlayerChoices = new int[playerCount];
        round1PlayerVotes = new int[playerCount];
        for(int i = 0; i < playerCount; i++)
        {
            round1PlayerVotes[i] = -1;
        }
        timerIsRunning = true;
        timer = 90;
    }
    private void Start()
    {
        playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        if (PhotonNetwork.IsMasterClient)
        {
            nextSectionButton.SetActive(true);
            
        }
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerParent);
            newPlayerItem.SetPlayerItem(player.Value);
            playerItemsList.Add(newPlayerItem.gameObject);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                photonView.RPC("SendPlayerItemsList", RpcTarget.All, player.Key, player.Value.NickName);
            }
        }
        Debug.Log(PhotonNetwork.CurrentRoom.Players);
    }
    public void OnClickNextSection()
    {
        photonView.RPC("OnNextSection", RpcTarget.All);
        everyoneSubmitted = 0;
    }
    [PunRPC]
    void OnNextSection()
    {
        roundSection++;
        waitForPlayers.SetActive(false);
        if (roundSection == 4)
        {
            if (!allOutcomesDone)
            {
                roundSection = 2;
                foreach (GameObject obj in roundOBJ[3].objects)
                {
                    obj.SetActive(false);
                }
            }
            else
            {
                for(int i = 0; i < playerCount; i++)
                {
                    redonePlayerItemsList[i].transform.position = playerScorePositions[i].transform.position;
                    redonePlayerItemsList[i].GetComponent<PlayerItem>().ShowScore();
                }
            }

        }
        if (roundSection == 5)
        {
            for (int i = 0; i < playerCount; i++)
            {
                redonePlayerItemsList[i].GetComponent<PlayerItem>().HideScore();
            }
            photonView.RPC("RestartRound", RpcTarget.All);
            return;
        }
        timer = roundTimers[roundSection];
        timerIsRunning = true;
        foreach (GameObject obj in roundOBJ[roundSection].objects)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in roundOBJ[roundSection - 1].objects)
        {
            obj.SetActive(false);
        }
        if (roundSection == 1)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            scenariosToReturnPerPlayer = new List<string>();
            round1ScrambledScenarios = round1Scenarios.ToList();
            for (int playerNumber = 0; playerNumber < playerCount; playerNumber++)
            {
                if (playerNumber == playerCount - 1 && round1ScrambledScenarios.Contains(round1Scenarios[playerCount - 1]))
                {
                    scenariosToReturnPerPlayer.Insert(scenariosToReturnPerPlayer.Count - 2, round1Scenarios[playerCount - 1]);
                    break;
                }
                string savedCurrentPlayerScenario = "placeHolderText";
                if (round1ScrambledScenarios.Contains(round1Scenarios[playerNumber]))
                {
                    savedCurrentPlayerScenario = round1Scenarios[playerNumber];
                    round1ScrambledScenarios.Remove(savedCurrentPlayerScenario);
                }
                string stringToReturn = round1ScrambledScenarios[Random.Range(0, round1ScrambledScenarios.Count)];
                if (savedCurrentPlayerScenario != "placeHolderText")
                    round1ScrambledScenarios.Add(savedCurrentPlayerScenario);
                round1ScrambledScenarios.Remove(stringToReturn);
                scenariosToReturnPerPlayer.Add(stringToReturn);
            }
            for (int playerNumber = 0; playerNumber < playerCount; playerNumber++)
            {
                Debug.Log(scenariosToReturnPerPlayer[playerNumber]);
                photonView.RPC("SendRound1Scenario", RpcTarget.All, scenariosToReturnPerPlayer[playerNumber], playerNumber + 1);
            }
        }
        if (roundSection == 2)
        {
            for (int i = 0; i < playerCount; i++)
            {
                round1PlayerVotes[i] = -1;
            }
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("TriggerScenarioPlayerShown", RpcTarget.All, (int)currentPlayerShown);

        }
        if (roundSection == 3)
        {
            foreach(GameObject ob in selectedButtons)
            {
                ob.SetActive(false);
            }
            waitPlayerVote.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("TriggerOutcomePlayerShown", RpcTarget.All, (int)currentPlayerShown);
            }
            currentPlayerShown++;
            if (currentPlayerShown == playerCount)
            {
                allOutcomesDone = true;
            }
        }
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                bool triggerShuffle = false;
                if (roundSection == 0)
                {
                    Round1Scenarios(false);
                    /*
                    Debug.Log("test");
                    triggerShuffle = true;
                    waitForPlayers.SetActive(false);
                    foreach (GameObject obj in roundOBJ[0].objects)
                    {
                        obj.SetActive(false);
                    }
                    shuffleScreen.SetActive(true);
                    for(int i = 0; i < playerCount; i++)
                    {
                        redonePlayerItemsList[i].transform.parent = shuffleParents[i].transform;
                        redonePlayerItemsList[i].transform.localPosition = new Vector3(0, 0, 0);
                    }
                    */
                }
                if (roundSection == 1)
                {
                    Round1Outcomes(false);
                }
                timer = 0;
                timerIsRunning = false;
                if (PhotonNetwork.IsMasterClient && triggerShuffle == false)
                {
                    OnClickNextSection();
                }
            }
        }
        DisplayTime(timer);
        if(everyoneSubmitted == playerCount && PhotonNetwork.IsMasterClient)
        {
            everyoneSubmitted = 0;
            OnClickNextSection();
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
    //===============================================================================================================================
    public void Round1Scenarios(bool clicked)
    {
        string scenario = round1Scenario.text;
        photonView.RPC("SendRound1Scenarios", RpcTarget.MasterClient, scenario, playerID);
        if (clicked)
        {
            clickSubmit();
        }
    }
    public void Round1Outcomes(bool clicked)
    {
        string positiveOutcome = round1PositiveOutcome.text;
        string negativeOutcome = round1NegativeOutcome.text;
        photonView.RPC("SendRound1Outcomes", RpcTarget.All, positiveOutcome, negativeOutcome, playerID);
        Debug.Log(playerID);
        if (clicked)
        {
            clickSubmit();
        }
    }
    public void clickSubmit()
    {
        foreach (GameObject obj in roundOBJ[roundSection].objects)
        {
            obj.SetActive(false);
        }
        waitForPlayers.SetActive(true);
        photonView.RPC("GetPlayerSubmit", RpcTarget.MasterClient);
    }

    public void FinishShuffle()
    {
        for (int i = 0; i < playerCount; i++)
        {
            redonePlayerItemsList[i].transform.parent = playerParent;
            redonePlayerItemsList[i].transform.position = playerParent.position;
        }
        shuffleScreen.SetActive(false);
        OnClickNextSection();
    }
    [PunRPC]
    void GetPlayerSubmit()
    {
        everyoneSubmitted++;
    }
    public void PlaceChoice(int value)
    {
        photonView.RPC("SendRound1PlayerChoices", RpcTarget.All, value, playerID);
    }

    public void PlaceVote(int value)
    {
        photonView.RPC("SendRound1PlayerVote", RpcTarget.All, value, playerID);
    }

    [PunRPC]
    void SendPlayerItemsList(int value, string name)
    {
        foreach(GameObject player in playerItemsList)
        {
            if(player.GetComponent<PlayerItem>().playerName.text == name)
            {
                redonePlayerItemsList.Add(player);
            }
        }
    }

    [PunRPC]
    void SendRound1Scenarios(string input, int pID)
    {
        string recievedString = input;
        int recievedID = pID - 1;
        round1Scenarios[recievedID] = input;
    }

    [PunRPC]
    void SendRound1Scenario(string input, int pID)
    {
        string recievedString = input;
        if (playerID == pID)
        {
            round1ScenarioText.text = recievedString;
        }
        round1Scenarios[pID - 1] = recievedString;
    }

    [PunRPC]
    void SendRound1Outcomes(string out1, string out2, int pID)
    {
        string recievedString1 = out1;
        string recievedString2 = out2;
        //int recievedChoice = choice;
        int recievedID = pID - 1;


        round1PositiveOutcomes[recievedID] = recievedString1;
        round1NegativeOutcomes[recievedID] = recievedString2;
        //round1PlayerChoices[recievedID] = recievedChoice;
    }

    [PunRPC]
    void SendRound1PlayerChoices(int value, int pID)
    {
        int recievedValue = value;
        int recievedID = pID - 1;
        round1PlayerChoices[recievedID] = recievedValue;
    }

    [PunRPC]
    void TriggerScenarioPlayerShown(int value)
    {
        foreach(GameObject player in redonePlayerItemsList)
        {
            player.transform.position = resetPosition.position;
        }
        int recievedValue = value;
        if(playerID-1 == recievedValue)
        {
            foreach (GameObject obj in roundOBJ[roundSection].objects)
            {
                obj.SetActive(false);
            }
            waitPlayerVote.SetActive(true);
        }
        else
        {
            waitPlayerVote.SetActive(false);
            round1ScenarioTextShown.text = round1Scenarios[recievedValue];
            redonePlayerItemsList[recievedValue].transform.position = playerReviewPosition.position;
        }
        
    }

    [PunRPC]
    void TriggerOutcomePlayerShown(int value)
    {
        int recievedValue = value;
        round1PositiveOutcomeText.text = round1PositiveOutcomes[value];
        round1NegativeOutcomeText.text = round1NegativeOutcomes[value];
        positivePanelHighlight.SetActive(false);
        negativePanelHighlight.SetActive(false);
        for (int i = 0; i < playerCount; i++)
        {
            if (round1PlayerChoices[recievedValue] == round1PlayerVotes[i] && recievedValue != i)
            {
                int score = redonePlayerItemsList[i].GetComponent<PlayerItem>().playerScore;
                score++;
                redonePlayerItemsList[i].GetComponent<PlayerItem>().playerScore = score;
                //ScoreManager.totalScore[i] = score;
            }
            if(round1PlayerVotes[i] == 0)
            {
                redonePlayerItemsList[i].transform.position = playerPositiveChoices[i].position;
                
            }else if(round1PlayerVotes[i] == 1)
            {
                redonePlayerItemsList[i].transform.position = playerNegativeChoices[i].position;
                
            }
            if (i == recievedValue)
            {
                if(round1PlayerChoices[recievedValue] == 0)
                {
                    redonePlayerItemsList[i].transform.position = playerPositiveChoices[8].position;
                    positivePanelHighlight.SetActive(true);
                }
                else if (round1PlayerChoices[recievedValue] == 1)
                {
                    redonePlayerItemsList[i].transform.position = playerNegativeChoices[8].position;
                    negativePanelHighlight.SetActive(true);
                }
            }
        }
    }

    [PunRPC]
    void SendRound1PlayerVote(int value, int pID)
    {
        int recievedValue = value;
        int recievedID = pID - 1;
        round1PlayerVotes[recievedID] = recievedValue;
    }

    [PunRPC]
    void RestartRound()
    {
        roundSection = 0;
        PhotonNetwork.LoadLevel("Game");
    }
    //===============================================================================================================================

}
