using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using CustomClasses;

namespace Online
{
    public class LobbyManager : InstanceOnlineClass<LobbyManager>
    {        
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
            roomItemList = new();
            roomListPanel.SetActive(ConnectToServer.instance.connectionType == ConnectionType.PublicMatch);
            matchRoomPanel.SetActive(ConnectToServer.instance.connectionType == ConnectionType.PrivateMatch);
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby() => ConnectToServer.instance.SetLoadingText("Connected to lobby!");
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
        public void SetRoomName(TMP_InputField textField) => roomName = textField.text;
        public void JoinOrCreateRoom()
        {
            if(!string.IsNullOrWhiteSpace(roomName))
            {
                ConnectToServer.instance.SetLoadingText("Connecting to room...");
                if(ConnectToServer.instance.connectionType == ConnectionType.PrivateMatch) PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(){MaxPlayers = ConnectToServer.instance.maxPlayers, IsVisible = false}, TypedLobby.Default);
                else PhotonNetwork.CreateRoom(roomName, new RoomOptions(){MaxPlayers = ConnectToServer.instance.maxPlayers, IsVisible = true}, TypedLobby.Default);
            }
            else ConnectToServer.instance.SetLoadingText("Please enter a room rame");
        }

        public void JoinRoom(string _roomName)
        {
            ConnectToServer.instance.SetLoadingText("Connecting to room...");
            PhotonNetwork.JoinRoom(_roomName);
        }
    }
}