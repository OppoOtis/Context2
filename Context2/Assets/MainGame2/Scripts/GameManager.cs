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
    PhotonView photonView;
    int playerID;
    int playerCount;

    [SerializeField]
    private RoundObjects[] roundOBJ;
    [System.Serializable]
    public class RoundObjects
    {
        public GameObject[] objects;
    }

    public string[] round1Scenarios, round1PositiveOutcomes, round1NegativeOutcomes;
    public List<string> round1ScrambledScenarios;
    List<string> scenariosToReturnPerPlayer;
    public int[] round1PlayerChoices;
    public int[] round1PlayerVotes;
    public int roundSection = 0;
    public bool nextRound;
    public int currentPlayerShown = 0;
    public bool allOutcomesDone = false;

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        photonView = PhotonView.Get(this);
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        round1Scenarios = new string[playerCount];
        round1ScrambledScenarios = new List<string>(playerCount);
        round1PositiveOutcomes = new string[playerCount];
        round1NegativeOutcomes = new string[playerCount];
        round1PlayerChoices = new int[playerCount];
        round1PlayerVotes = new int[playerCount];
    }
    private void Start()
    {
        playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        if (PhotonNetwork.IsMasterClient)
        {
            nextSectionButton.SetActive(true);
        }
    }
    public void OnClickNextSection()
    {
        photonView.RPC("OnNextSection", RpcTarget.All);
    }
    [PunRPC]
    void OnNextSection()
    {
        roundSection++;
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
        }
        if (roundSection == 5)
        {
            photonView.RPC("RestartRound", RpcTarget.All);
        }
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
            photonView.RPC("SendRound1PlayerChoices", RpcTarget.MasterClient, (int)round1PlayerChoicesSlider.value, playerID);
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("TriggerScenarioPlayerShown", RpcTarget.All, (int)currentPlayerShown);

        }
        if (roundSection == 3)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("TriggerOutcomePlayerShown", RpcTarget.All, (int)currentPlayerShown);
            currentPlayerShown++;
            if (currentPlayerShown == playerCount)
            {
                allOutcomesDone = true;
            }
        }

    }
    //===============================================================================================================================
    public void Round1Scenarios()
    {
        string scenario = round1Scenario.text;
        photonView.RPC("SendRound1Scenarios", RpcTarget.MasterClient, scenario, playerID);
    }
    public void Round1Outcomes()
    {
        string positiveOutcome = round1PositiveOutcome.text;
        string negativeOutcome = round1NegativeOutcome.text;
        int playerChoice = 0;
        photonView.RPC("SendRound1Outcomes", RpcTarget.All, positiveOutcome, negativeOutcome, playerChoice, playerID);
    }

    public void PlaceVote()
    {
        photonView.RPC("SendRound1PlayerVote", RpcTarget.All, (int)round1PlayerVoteSlider.value, playerID);
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
    void SendRound1Outcomes(string out1, string out2, int choice, int pID)
    {
        string recievedString1 = out1;
        string recievedString2 = out2;
        int recievedChoice = choice;
        int recievedID = pID - 1;


        round1PositiveOutcomes[recievedID] = recievedString1;
        round1NegativeOutcomes[recievedID] = recievedString2;
        round1PlayerChoices[recievedID] = recievedChoice;
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
        int recievedValue = value;
        round1ScenarioTextShown.text = round1Scenarios[value];
    }

    [PunRPC]
    void TriggerOutcomePlayerShown(int value)
    {
        int recievedValue = value;
        round1PositiveOutcomeText.text = round1PositiveOutcomes[value];
        round1NegativeOutcomeText.text = round1NegativeOutcomes[value];
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
