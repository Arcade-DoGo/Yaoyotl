using System;
using System.Collections;
using CustomClasses;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// Class that manages the reconnection in case of a disconnection of the Photon PUN 2 net
/// </summary>

namespace Online
{
    public class ReconnectionHandler : InstanceOnlineClass<ReconnectionHandler>
    {
        [NonSerialized] public bool connected;
        public override void OnDisconnected(DisconnectCause cause) => StartCoroutine(MainReconnect());
        public IEnumerator MainReconnect()
        {
            connected = false;
            while (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
            {
                if (GameManager.usingEditor) Debug.Log("Waiting for client to be fully disconnected..", this);
                yield return new WaitForSeconds(0.2f);
            }

            if (GameManager.usingEditor) Debug.Log("Client is disconnected!", this);
            if (!PhotonNetwork.ReconnectAndRejoin())
            {
                if (PhotonNetwork.Reconnect())
                {
                    if (GameManager.usingEditor) Debug.Log("Successful reconnected!", this);
                    if(ConnectToServer.ROOMNAME != null) PhotonNetwork.RejoinRoom(ConnectToServer.ROOMNAME);
                    connected = true;
                }
            }
            else
            {
                if (GameManager.usingEditor) Debug.Log("Successful reconnected and joined!", this);
                yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
                if(ConnectToServer.ROOMNAME != null) PhotonNetwork.JoinRoom(ConnectToServer.ROOMNAME);
                connected = true;
            }
        }
    }
}