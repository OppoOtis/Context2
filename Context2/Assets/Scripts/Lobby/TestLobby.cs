using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private float heartbeatTimer;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("signed in" + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        if (Input.GetKeyDown(KeyCode.L)) CreateLobby();
        if (Input.GetKeyDown(KeyCode.K)) ListLobbies();
        if (Input.GetKeyDown(KeyCode.J)) JoinLobby();
        
    }

    private async void HandleLobbyHeartbeat()
    {
        if(hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    private async void CreateLobby()
    {
        try
        {
            string lobbyname = "myLobby";
            int maxPlayers = 4;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyname, maxPlayers);

            hostLobby = lobby;
            Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers);
        } catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log(queryResponse.Results);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    private async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
