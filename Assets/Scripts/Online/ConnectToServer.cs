using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

namespace Online
{
    [Serializable]
    public enum ConnectionType { SingleRoom, RandomMatch, PrivateMatch, PublicMatch }
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        public static ConnectToServer instance;

        [Header ("UI References")]
        [Tooltip ("TextField to indicate Online Connection status. Optional")]
        public TextMeshProUGUI loadingText;
        [Tooltip ("Text to display the player list inside a room")]
        public TextMeshProUGUI playerList;
        [Tooltip ("Button to go back")]
        public GameObject backButton;
        [Tooltip ("Button list that contains the different online modes")]
        public Button[] onlineModesButtons;

        [Header ("Online Setup")]
        [Tooltip ("Connection type used for matchmaking")]
        public ConnectionType connectionType;
        [Tooltip ("Max players per room")]
        [Min (1)] public int maxPlayers;
        [Tooltip ("Automatically sync all players Scene?")]
        public bool automaticallySyncScene;


        [Header ("Online Connection Events")]
        [Tooltip ("Actions to do when starting to connect to Online Server")]
        public UnityEvent OnStartConnection;
        [Tooltip ("Actions to do when connection is stablished to Server")]
        public UnityEvent OnConnectedEvent;

        [Tooltip ("Room custom properties")]
        private Hashtable roomCustomProperties;        
        [Tooltip ("Game Version for Photon \nDO NOT CHANGE UNLESS NECCESARY")]
        private readonly string gameVersion = "0.1";

        private void Start()
        {
            instance = this;
            SetLoadingText("");
            backButton.SetActive(false);
            foreach (Button button in onlineModesButtons) button.interactable = false;
            roomCustomProperties = new() { { "Stage", 0 } };
        }

        public void Connect()
        {
            OnStartConnection?.Invoke();
            SetLoadingText("Connecting to server...");
            if(!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.AutomaticallySyncScene = automaticallySyncScene;
            }
            else 
            {
                if(connectionType == ConnectionType.SingleRoom) OnJoinedRoom();
                else if(connectionType == ConnectionType.RandomMatch) PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: new RoomOptions(){MaxPlayers = maxPlayers});
                else OnJoinedLobby();
            }
        }

        public override void OnConnectedToMaster() 
        {
            SetLoadingText("Connected to server!");
            foreach (Button button in onlineModesButtons) button.interactable = true;
        }

        public void OnClickOnlineMode()
        {
            switch (connectionType)
            {
                case ConnectionType.SingleRoom:
                    SetLoadingText("Connecting to room...");
                    PhotonNetwork.JoinOrCreateRoom("RoomGame", new RoomOptions(){MaxPlayers = maxPlayers}, TypedLobby.Default);
                    break;
                case ConnectionType.RandomMatch:
                    SetLoadingText("Connecting to room...");
                    PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: new RoomOptions(){MaxPlayers = maxPlayers});
                    break;
                case ConnectionType.PrivateMatch:
                    SetLoadingText("Connecting to lobby...");
                    MultipleUISelection.instance.OnlyShowElement(2);
                    break;
                case ConnectionType.PublicMatch:
                    SetLoadingText("Connecting to lobby...");
                    MultipleUISelection.instance.OnlyShowElement(2);
                    break;
                default:
                    break;
            }
        }

        public override void OnJoinedRoom()
        {
            SetLoadingText("Connected to room " + PhotonNetwork.CurrentRoom.Name + "!");
            MultipleUISelection.instance.OnlyShowElement(3);
            OnConnectedEvent?.Invoke();
            UpdatePlayerList();

            // Set custom properties
            if(PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.SetCustomProperties(roomCustomProperties);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) => UpdatePlayerList();
        public override void OnPlayerLeftRoom(Player newPlayer) => UpdatePlayerList();
        public void UpdatePlayerList()
        {
            if(playerList)
            {
                playerList.text = "-------------\n";
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    playerList.text += player.ActorNumber + ": <color=" + (player.IsMasterClient ? "green" : "white") + ">" + player.NickName + "</color>";
                    playerList.text += player.CustomProperties["playerHealth"] + "\n";
                }
                playerList.text += "-------------";
            }
        }

        public void OnClickLeave()
        {
            if(MultipleUISelection.instance.IsElementActive(3))
            {
                if(PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
                if(connectionType == ConnectionType.SingleRoom || connectionType == ConnectionType.RandomMatch)
                    MultipleUISelection.instance.OnlyShowElement(1);
                else
                    MultipleUISelection.instance.OnlyShowElement(2);
            }
            else if(MultipleUISelection.instance.IsElementActive(2))
            {
                if(PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();
                MultipleUISelection.instance.OnlyShowElement(1);
            }
            else if(MultipleUISelection.instance.IsElementActive(1))
            {
                if(PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
                backButton.SetActive(false);
                MultipleUISelection.instance.OnlyShowElement(0);
            }
        }

        public void SetConnectionType(int _type) => connectionType = (ConnectionType)Enum.GetValues(connectionType.GetType()).GetValue(_type);
        public void SetLoadingText(string _text) { if(loadingText) loadingText.text = _text; }
    }
}