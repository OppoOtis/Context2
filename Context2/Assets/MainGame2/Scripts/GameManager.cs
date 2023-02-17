using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField input;
    PhotonView photonView;

    private void Awake()
    {
        photonView = PhotonView.Get(this);
    }
    public void OnClick()
    {
        string testText = input.text;
        photonView.RPC("SendString", RpcTarget.MasterClient, testText);
    }

    [PunRPC]
    void SendString(string doodookaka)
    {
        string recieved_string = doodookaka;
        Debug.Log(recieved_string);
    }
}
