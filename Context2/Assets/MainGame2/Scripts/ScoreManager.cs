using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScoreManager : MonoBehaviour
{
    public static int[] totalScore;

    private void Start()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        totalScore = new int[playerCount];
    }
}
