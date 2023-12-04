using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using CustomClasses;
using UnityEngine.Events;
using System.Collections.Generic;

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
        public UnityEvent OnReady;
        public bool InitOnEnabled = true, rematch = false;

        [Tooltip ("Room custom properties")] private Hashtable roomCustomProperties;

        private void Start() 
        {
            player1ReadyButton.interactable = false;
            if(InitOnEnabled) Init();
            SetReady(false);

            foreach (TextMeshProUGUI readyText in readyTexts)
                if(readyText) readyText.gameObject.SetActive(false);
            
            if(characterIcons.Length > 0)
            {
                for (int i = 0; i < characterIcons.Length; i++) 
                    characterIcons[i].gameObject.SetActive(PhotonNetwork.IsConnected && i < PhotonNetwork.PlayerList.Length);
                characterIcons[0].gameObject.SetActive(true);
            }
        }
        public void Init()
        {
            UpdatePlayerList();
            
            if(PhotonNetwork.IsConnected)
            {
                if(PhotonNetwork.IsMasterClient) // Set room custom properties
                {
                    roomCustomProperties = new() { { "Stage", 0 } };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(roomCustomProperties);
                }
                else player1ReadyButton.interactable = true;
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
            if(PhotonNetwork.IsConnected) SetReady(true);
            else OnReady?.Invoke();
        }

        private void SetReady(bool value)
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(ConnectToServer.READY)) PhotonNetwork.LocalPlayer.CustomProperties.Add(ConnectToServer.READY, value);
            PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.READY] = value;
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { ConnectToServer.READY, value } });
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) 
        {
            if(changedProps.ContainsKey(ConnectToServer.PLAYERCHARACTER))
                SetPlayerCharacter(targetPlayer.ActorNumber - 1, (int)changedProps[ConnectToServer.PLAYERCHARACTER]);
            else if(changedProps.ContainsKey(ConnectToServer.READY))
            {
                if((bool) changedProps[ConnectToServer.READY] == true)
                {
                    List<Player> playersToCheck = PhotonNetwork.PlayerList.ToList().FindAll(player => player.CustomProperties.ContainsKey(ConnectToServer.READY));
                    readyTexts[targetPlayer.ActorNumber].gameObject.SetActive(true);
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
            if(rematch) OnReady?.Invoke();
            else StartGame();
        }

        public void SetPlayerText(int playerIndex, string _text) { if(characterIcons.Length > 0) characterIcons[playerIndex].SetName(_text); }
        public void StartGame() => StartCoroutine(LoadGameScene());
        private IEnumerator LoadGameScene()
        {
            yield return new WaitForSeconds(1f);
            // PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name == "GameplayScene" ? "MenuScene" : "GameplayScene");
            PhotonNetwork.LoadLevel("GameplayScene");
        }
    }
}