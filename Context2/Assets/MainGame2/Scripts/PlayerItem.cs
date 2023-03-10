using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    public TMP_Text playerName;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    public Image playerAvatar;
    public TMP_Text playerScoreText;
    public int playerScore = 0;
    public Material[] avatars;

    Player player;
    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);
    }

    public void OnClickCharacter(int i)
    {
        playerProperties["playerAvatar"] = i;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("playerAvatar"))
        {
            playerAvatar.material = avatars[(int)player.CustomProperties["playerAvatar"]];
            playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];

        }
        else
        {
            playerProperties["playerAvatar"] = Random.Range(0, avatars.Length);
        }
    }

    public void SetPlayerItem(Player player)
    {
        playerAvatar.material = avatars[(int)player.CustomProperties["playerAvatar"]];
        playerName.text = player.NickName;
    }

    public void ShowScore()
    {
        playerScoreText.text = playerScore.ToString();
        playerScoreText.gameObject.SetActive(true);
    }

    public void HideScore()
    {
        playerScoreText.gameObject.SetActive(false);
    }
}
