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
    public TMP_Text round1ScenarioText;
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

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        photonView = PhotonView.Get(this);
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        round1Scenarios = new string[playerCount];
        round1ScrambledScenarios = new List<string>(playerCount);
        round1PositiveOutcomes = new string[playerCount];
        round1NegativeOutcomes = new string[playerCount];
        round1PlayerChoices = new int[playerCount];
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
        photonView.RPC("SendRound1Outcomes", RpcTarget.MasterClient, positiveOutcome, negativeOutcome, playerChoice, playerID);
    }

    [PunRPC]
    void SendRound1Scenarios(string input, int pID)
    {
        string recievedString = input;
        int recievedID = pID-1;
        round1Scenarios[recievedID] = input;
    }
    //hoi joppe
    [PunRPC]
    void SendRound1Scenario(string input, int pID)
    {
        if(playerID == pID)
        {
            string recievedString = input;
            round1ScenarioText.text = recievedString;
        }
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
        int recievedID = pID-1;
        round1PlayerChoices[recievedID] = recievedValue;
    }
    //===============================================================================================================================

}
