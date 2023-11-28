using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using CustomClasses;

namespace Online
{
    [Serializable]
    public enum ConnectionType { SingleRoom, RandomMatch, PrivateMatch, PublicMatch }
    public class ConnectToServer : InstanceOnlineClass<ConnectToServer>
    {
        [Header ("UI References")]
        [Tooltip ("TextField to indicate Online Connection status. Optional")]
        public TextMeshProUGUI loadingText;
        [Tooltip ("Button to go back")]
        public GameObject backButton;
        [Tooltip ("Input field for the username")]
        public GameObject usernamePanel;
        [Tooltip ("Button list that contains the different online modes")]
        public Button[] onlineModesButtons;
        [Tooltip ("Selection script for the different UI panels")]
        public MultipleUISelection multipleUISelection;

        [Header ("Online Setup")]
        [Tooltip ("Connection type used for matchmaking")]
        public ConnectionType connectionType;
        [Tooltip ("Max players per room")]
        [Min (1)] public int maxPlayers;
        [Tooltip ("Automatically sync all players Scene?")]
        public bool automaticallySyncScene;

        [Tooltip ("Game Version for Photon \nDO NOT CHANGE UNLESS NECCESARY")]
        private readonly string gameVersion = "0.1";

        [Header ("RPC Names")]
        public static string PLAYERCHARACTER = "PlayerCharacter";
        public static string STOCKS = "Stocks";
        public static string DAMAGE = "Damage";
        public static string READY = "Ready";

        private void Start()
        {
            SetLoadingText("");
            if(PhotonNetwork.InRoom)
            {
                multipleUISelection.OnlyShowElements(3);
            }
            else
            {
                backButton.SetActive(false);
                multipleUISelection.OnlyShowElements(0);
                foreach (Button button in onlineModesButtons) button.interactable = false;
                usernamePanel.SetActive(false);
            }
        }

        public void Connect()
        {
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
            usernamePanel.SetActive(true);
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
                case ConnectionType.PrivateMatch: case ConnectionType.PublicMatch:
                    SetLoadingText("Connecting to lobby...");
                    multipleUISelection.OnlyShowElements(2);
                    break;
                default:
                    break;
            }
        }

        public override void OnJoinedRoom()
        {
            SetLoadingText("Connected to room " + PhotonNetwork.CurrentRoom.Name + "!");
            multipleUISelection.OnlyShowElements(3);
        }

        public void OnClickLeave()
        {
            if(multipleUISelection.IsElementActive(3))
            {
                if(PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
                multipleUISelection.OnlyShowElements(connectionType == ConnectionType.SingleRoom || connectionType == ConnectionType.RandomMatch ? 1 : 2);
            }
            else if(multipleUISelection.IsElementActive(2))
            {
                if(PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();
                multipleUISelection.OnlyShowElements(1);
            }
            else if(multipleUISelection.IsElementActive(1))
            {
                if(PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
                multipleUISelection.OnlyShowElements(0);
                backButton.SetActive(false);
            }
        }

        public void SetConnectionType(int _type) => connectionType = (ConnectionType)Enum.GetValues(connectionType.GetType()).GetValue(_type);
        public void SetLoadingText(string _text) { if(loadingText) loadingText.text = _text; }
    }
}