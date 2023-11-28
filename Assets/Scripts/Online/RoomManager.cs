using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using CustomClasses;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Online
{
    public class RoomManager : InstanceOnlineClass<RoomManager>
    {
        [Header ("UI References")]
        [Tooltip ("Player 1 ready Text")] public TextMeshProUGUI player1ReadyText;
        [Tooltip ("Player 2 ready Text")] public TextMeshProUGUI player2ReadyText;
        [Tooltip ("Player 3 ready Text")] public TextMeshProUGUI player3ReadyText;
        [Tooltip ("Player 4 ready Text")] public TextMeshProUGUI player4ReadyText;

        public Sprite[] sprites;
        public string[] names;
        public CharacterIcon[] characterIcons;

        [Tooltip ("Player 1 ready Button")] public Button player1ReadyButton;
        [Tooltip ("Text to display the player list inside a room")] public TextMeshProUGUI playerList;
        [Tooltip ("Panel for player 2 character")] public GameObject player2Selection;
        public UnityEvent OnReady;
        public bool InitOnEnabled = true, rematch = false;

        [Tooltip ("Room custom properties")] private Hashtable roomCustomProperties;

        private void Start() 
        {
            player1ReadyButton.interactable = false;
            if(InitOnEnabled) Init();

            if(player2Selection) player2Selection.SetActive(false);
            if(player1ReadyText) player1ReadyText.gameObject.SetActive(false);
            if(player2ReadyText) player2ReadyText.gameObject.SetActive(false);
            if(player3ReadyText) player3ReadyText.gameObject.SetActive(false);
            if(player4ReadyText) player4ReadyText.gameObject.SetActive(false);

            if(characterIcons.Length > 0)
            {
                if(PhotonNetwork.IsConnected)
                {
                    for (int i = 0; i < characterIcons.Length; i++)
                        characterIcons[i].gameObject.SetActive(i < PhotonNetwork.PlayerList.Length);
                }
                else
                {
                    for (int i = 0; i < characterIcons.Length; i++) characterIcons[i].gameObject.SetActive(false);
                    characterIcons[0].gameObject.SetActive(true);
                }
            }
        }
        public void Init()
        {
            UpdatePlayerList();
            SetReady(false);
            
            if(PhotonNetwork.IsConnected)
            {
                // Set room custom properties
                if(PhotonNetwork.IsMasterClient)
                {
                    roomCustomProperties = new() { { "Stage", 0 } };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(roomCustomProperties);
                }
                else
                {
                    player1ReadyButton.interactable = true;
                    if(player2Selection)
                    {
                        player2Selection.SetActive(true);
                        player2Selection.GetComponent<MultipleUISelection>().
                            OnlyShowElements(GetOtherCustomProperty(PhotonNetwork.PlayerListOthers[0]));
                    }
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ConnectToServer.instance.SetLoadingText(newPlayer.NickName + " joined!");
            if(player2Selection) 
            {
                player2Selection.SetActive(true);
                player2Selection.GetComponent<MultipleUISelection>().
                    OnlyShowElements(0);
            }
            UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(Player newPlayer) 
        {
            ConnectToServer.instance.SetLoadingText(newPlayer.NickName + " left");
            if(player2Selection) player2Selection.SetActive(false);
            UpdatePlayerList();
        }

        public void UpdatePlayerList()
        {
            if(playerList)
            {
                if(PhotonNetwork.PlayerList.Count() > 1) player1ReadyButton.interactable = true;
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
            player1ReadyText.gameObject.SetActive(true);
            if(PhotonNetwork.IsConnected) SetReady(true);
            else
            {
                OnReady?.Invoke();
                // AllPlayersReady();
            }
        }

        private void SetReady(bool value)
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(ConnectToServer.READY)) PhotonNetwork.LocalPlayer.CustomProperties.Add(ConnectToServer.READY, value);
            PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.READY] = value;
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { ConnectToServer.READY, value } });
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) 
        {
            // print("PlayerPropertiesUpdate!");
            bool isLocalPlayer = targetPlayer == PhotonNetwork.LocalPlayer;
            if(changedProps.ContainsKey(ConnectToServer.PLAYERCHARACTER))
            {
                if(player2Selection)
                {
                    if(!isLocalPlayer) 
                        player2Selection.GetComponent<MultipleUISelection>()
                            .OnlyShowElements(GetOtherCustomProperty(targetPlayer));
                }
                else
                {
                    SetPlayerCharacter(targetPlayer.ActorNumber - 1, (int)changedProps[ConnectToServer.PLAYERCHARACTER]);
                }
            }
            else if(changedProps.ContainsKey(ConnectToServer.READY))
            {
                if((bool) changedProps[ConnectToServer.READY] == true)
                {
                    List<Player> playersToCheck = PhotonNetwork.PlayerListOthers.ToList().FindAll(player => player.CustomProperties.ContainsKey(ConnectToServer.READY));
                    if(!isLocalPlayer)
                    {
                        if(player2ReadyText) player2ReadyText.gameObject.SetActive(true);
                        if(player3ReadyText) player3ReadyText.gameObject.SetActive(true);
                        if(player4ReadyText) player4ReadyText.gameObject.SetActive(true);
                    }
                    else if(playersToCheck.Count > 0)
                    {
                        List<Player> playersNotReady = playersToCheck.FindAll(player => (bool) player.CustomProperties[ConnectToServer.READY] == false);
                        if(playersNotReady.Count == 0)
                        {
                            ConnectToServer.instance.SetLoadingText("Starting match...");
                            AllPlayersReady();
                        }
                        else
                        {
                            ConnectToServer.instance.SetLoadingText("Waiting for opponent...");
                        }
                    }
                }
            }
            else if(changedProps.ContainsKey(ConnectToServer.STOCKS))
            {
                if(!isLocalPlayer)
                {
                    CharacterStats stats = GameManager.players[targetPlayer.ActorNumber - 1];
                    stats.stocks = (int) PhotonNetwork.PlayerListOthers[0].CustomProperties[ConnectToServer.STOCKS];
                    // print("PROPERTIES UPDATE ONLINE: " + stats.playerName + " " + stats.stocks + " " + stats.damage);
                    MatchData.instance.UpdatePlayersData(stats);
                }
            }
            else if(changedProps.ContainsKey(ConnectToServer.DAMAGE))
            {
                if(!isLocalPlayer)
                {
                    CharacterStats stats = GameManager.players[targetPlayer.ActorNumber - 1];
                    stats.damage = (float) PhotonNetwork.PlayerListOthers[0].CustomProperties[ConnectToServer.DAMAGE];
                    // print("PROPERTIES UPDATE ONLINE: " + stats.playerName + " " + stats.stocks + " " + stats.damage);
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

        private int GetOtherCustomProperty(Player _player) => (int) _player.CustomProperties[ConnectToServer.PLAYERCHARACTER];
        public void StartGame() => StartCoroutine(LoadGameScene());
        private IEnumerator LoadGameScene()
        {
            yield return new WaitForSeconds(1f);
            // PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name == "GameplayScene" ? "MenuScene" : "GameplayScene");
            PhotonNetwork.LoadLevel("GameplayScene");
        }
    }
}