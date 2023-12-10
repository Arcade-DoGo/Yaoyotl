using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using CustomClasses;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Online
{
    public class RoomManager : InstanceOnlineClass<RoomManager>
    {
        [Header ("UI References")]
        [Tooltip ("Players ready Text")] public TextMeshProUGUI[] readyTexts;
        [Tooltip ("Players character Icons")] public Sprite[] sprites;
        [Tooltip ("Players character Names")] public string[] names;
        [Tooltip ("Character icon Sprites")] public CharacterIcon[] characterIcons;
        [Tooltip ("Player 1 ready Button")] public Button player1ReadyButton;
        [Tooltip ("Text to display the player list inside a room")] public TextMeshProUGUI playerList;
        private bool rematch = false;

        [Tooltip ("Room custom properties")] private Hashtable roomCustomProperties;

        private void Start() 
        {
            rematch = false;
            SendPlayerProperty(new Hashtable(){{ConnectToServer.READY, false}});

            foreach (TextMeshProUGUI readyText in readyTexts)
                if(readyText) readyText.gameObject.SetActive(false);
            
            if(characterIcons.Length > 0)
            {
                for (int i = 0; i < characterIcons.Length; i++) 
                    characterIcons[i].gameObject.SetActive(PhotonNetwork.IsConnected && i < PhotonNetwork.PlayerList.Length);
                characterIcons[0].gameObject.SetActive(true);
            }
        }

        public void ShowRoom()
        {
            UpdatePlayerList();
            
            if(PhotonNetwork.IsConnected)
            {
                if(PhotonNetwork.IsMasterClient) // Set room custom properties
                {
                    roomCustomProperties = new() { { "Stage", 0 } };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(roomCustomProperties);
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ConnectToServer.instance.SetLoadingText(newPlayer.NickName + " joined!");
            UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(Player newPlayer) 
        {
            ConnectToServer.instance.SetLoadingText(newPlayer.NickName + " left");
            UpdatePlayerList();
        }

        public void UpdatePlayerList()
        {
            if(playerList)
            {
                player1ReadyButton.interactable = true;
                playerList.text = "-------------\n";
                foreach (Player player in PhotonNetwork.PlayerList) 
                    playerList.text += player.ActorNumber + ": <color=" 
                        + (player.IsMasterClient ? "green" : "white") + ">" + player.NickName 
                        + (PhotonNetwork.LocalPlayer == player ? " (Me)" : "") + "</color>\n";
                playerList.text += "-------------";
            }
        }

        public void Ready(bool isRematch)
        {
            rematch = isRematch;
            Ready();
        }

        public void Ready()
        {
            if(PhotonNetwork.IsConnected)
            {
                SendPlayerProperty(new Hashtable(){{ConnectToServer.READY, true}});
            }
            else
            {
                // Multiplayer offline
                CameraManager.instance.SetGameplayView();
            }
        }

        public void SendPlayerProperty(Hashtable customProperties)
        {
            foreach (var key in customProperties.Keys)
            {
                if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(key)) PhotonNetwork.LocalPlayer.CustomProperties.Add(key, customProperties[key]);
                PhotonNetwork.LocalPlayer.CustomProperties[key] = customProperties[key];
                // Debug.Log("Set " + key + " as " + customProperties[key]);
            }
            PhotonNetwork.SetPlayerCustomProperties(customProperties);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) 
        {
            if(changedProps.ContainsKey(ConnectToServer.PLAYERCHARACTER))
                SetPlayerCharacter(targetPlayer.ActorNumber - 1, (int)changedProps[ConnectToServer.PLAYERCHARACTER]);
            else if(changedProps.ContainsKey(ConnectToServer.READY))
            {
                // Debug.Log(targetPlayer + " ready: " + (bool) changedProps[ConnectToServer.READY]);
                if((bool) changedProps[ConnectToServer.READY] == true)
                {
                    readyTexts[targetPlayer.ActorNumber].gameObject.SetActive(true);
                    List<Player> playersToCheck = PhotonNetwork.PlayerList.ToList().FindAll(player => player.CustomProperties.ContainsKey(ConnectToServer.READY));
                    if(playersToCheck.Count > 0)
                    {
                        List<Player> playersNotReady = playersToCheck.FindAll(player => (bool) player.CustomProperties[ConnectToServer.READY] == false);
                        if(playersNotReady.Count == 0)
                        {
                            ConnectToServer.instance.SetLoadingText("Starting match...");
                            AllPlayersReady();
                        }
                        else ConnectToServer.instance.SetLoadingText("Waiting for opponent...");
                    }
                    else
                    {
                        ConnectToServer.instance.SetLoadingText("Starting match...");
                        AllPlayersReady();
                    }
                }
            }
            else if(changedProps.ContainsKey(ConnectToServer.STOCKS) | changedProps.ContainsKey(ConnectToServer.DAMAGE))
            {
                if(targetPlayer != PhotonNetwork.LocalPlayer)
                {
                    CharacterStats stats = GameManager.players[targetPlayer.ActorNumber - 1];
                    if(changedProps.ContainsKey(ConnectToServer.STOCKS)) stats.stocks = (int) PhotonNetwork.PlayerListOthers[0].CustomProperties[ConnectToServer.STOCKS];
                    if(changedProps.ContainsKey(ConnectToServer.DAMAGE)) stats.damage = (float) PhotonNetwork.PlayerListOthers[0].CustomProperties[ConnectToServer.DAMAGE];
                    MatchData.instance.UpdatePlayersData(stats);
                }
            }
        }

        public void SetPlayerCharacter(int playerIndex, int characterIndex)
        {
            if(characterIcons.Length > 0)
            {
                characterIcons[playerIndex].SetIcon(sprites[characterIndex]);
                characterIcons[playerIndex].SetName(names[characterIndex]);
            }
        }

        private void AllPlayersReady()
        {
            if(SceneManager.GetActiveScene().name == "GameplayScene")
            {
                if(rematch) CameraManager.instance.SetCharacterSelectionView();
                else CameraManager.instance.SetGameplayView();
            }
            else LoadGameScene();
        }

        public void SetPlayerText(int playerIndex, string _text) { if(characterIcons.Length > 0) characterIcons[playerIndex].SetName(_text); }
        public void LoadGameScene() => StartCoroutine(LoadGameSceneRoutine());
        private IEnumerator LoadGameSceneRoutine()
        {
            yield return new WaitForSeconds(1f);
            PhotonNetwork.LoadLevel("GameplayScene");
        }
    }
}