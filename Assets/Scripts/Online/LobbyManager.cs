using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using System;

namespace Online
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        public static LobbyManager instance;
        
        [Header ("Online Setup")]
        [Tooltip ("Whether the players need to type the same room name to connect (private match), or choose from a room list (public match)")]
        public bool matchRoomName;

        [Header ("Lobby")]
        [Tooltip ("Panel if matchRoomName is true")]
        public GameObject matchRoomPanel;
        [Tooltip ("Panel if matchRoomName is false")]
        public GameObject roomListPanel;

        [Header ("Room List")]
        [Tooltip ("Prefab of a room item in room list")]
        public RoomItem roomItemPrefab;
        [Tooltip ("Transform parent where a room item should be added")]
        public Transform contentObject;
        [Tooltip ("Time of separation to update the room list")]
        public float timeBetweenUpdates = 1.5f;

        [Tooltip ("String indicating the name of the room")]
        [NonSerialized] public string roomName;

        [Tooltip ("Private room list reference")]
        private List<RoomItem> roomItemList;
        [Tooltip ("Private time counter to update room list")]
        private float nextUpdateTime;
        
        private void Start() 
        {
            instance = this;
            roomItemList = new();
            roomListPanel.SetActive(false);
            matchRoomPanel.SetActive(false);
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            ConnectToServer.instance.SetLoadingText("");
            if(matchRoomName) matchRoomPanel.SetActive(true);
            else roomListPanel.SetActive(true);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if(Time.time >= nextUpdateTime)
            {
                UpdateRoomList(roomList);
                nextUpdateTime = Time.time + timeBetweenUpdates;
            }
        }

        private void UpdateRoomList(List<RoomInfo> list)
        {
            foreach (RoomItem item in roomItemList) Destroy(item.gameObject);
            roomItemList.Clear();

            foreach (RoomInfo room in list)
            {
                RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
                newRoom.SetRoomName(room.Name);
                roomItemList.Add(newRoom);
            }
        }

        public void CreateRroom()
        {
            if(!string.IsNullOrWhiteSpace(roomName))
            {
                ConnectToServer.instance.SetLoadingText("Connecting to room...");
                if(matchRoomName) PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(){MaxPlayers = ConnectToServer.instance.maxPlayers, IsVisible = matchRoomName}, TypedLobby.Default);
                else PhotonNetwork.CreateRoom(roomName, new RoomOptions(){MaxPlayers = ConnectToServer.instance.maxPlayers, IsVisible = matchRoomName}, TypedLobby.Default);
            }
            else ConnectToServer.instance.SetLoadingText("Please enter a room rame");
        }

        public void JoinRoom(string _roomName)
        {
            ConnectToServer.instance.SetLoadingText("Connecting to room...");
            PhotonNetwork.JoinRoom(_roomName);
        }
        public void SetRoomName(TMP_InputField textField) => roomName = textField.text;
    }
}