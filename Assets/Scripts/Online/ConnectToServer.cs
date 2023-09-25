using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

[Serializable]
public enum ConnectionType { SingleRoom, RandomMatch, PrivateMatch, PublicMatch }
namespace Online
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        public static ConnectToServer instance;

        [Header ("UI References")]
        [Tooltip ("TextField to indicate Online Connection status. Optional")]
        public TextMeshProUGUI loadingText;

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

        [Tooltip ("Game Version for Photon \nDO NOT CHANGE UNLESS NECCESARY")]
        private readonly string gameVersion = "0.1";

        private void Start()
        {
            instance = this;
            SetLoadingText("");
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
                    LobbyManager.instance.matchRoomName = false;
                    break;
                case ConnectionType.PublicMatch:
                    SetLoadingText("Connecting to lobby...");
                    MultipleUISelection.instance.OnlyShowElement(2);
                    LobbyManager.instance.matchRoomName = true;
                    break;
                default:
                    break;
            }
        }

        public override void OnJoinedRoom()
        {
            SetLoadingText("Connected to room " + PhotonNetwork.CurrentRoom.Name + "!");
            OnConnectedEvent?.Invoke();

            // Hashtable roomCustomProperties = new() { { "Character", 0 } };
            // PhotonNetwork.CurrentRoom.SetCustomProperties(roomCustomProperties);
        }

        public void SetConnectionType(int _type)=> connectionType = (ConnectionType)Enum.GetValues(connectionType.GetType()).GetValue(_type);
        public void SetLoadingText(string _text) { if(loadingText) loadingText.text = _text; }
    }
}