using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Online
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        [Header ("UI References")]
        [Tooltip ("Panel for player 2 character")] public GameObject player2Selection;
        [Tooltip ("Text to display the player list inside a room")] public TextMeshProUGUI playerList;

        [Tooltip ("Room custom properties")] private Hashtable roomCustomProperties;

        private void Start() 
        {
            player2Selection.SetActive(false);
            CharacterSelection.instance.SetCharacter(0);
            UpdatePlayerList();
            
            // Set room custom properties
            if(PhotonNetwork.IsMasterClient)
            {
                roomCustomProperties = new() { { "Stage", 0 } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomCustomProperties);
            }

            if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                player2Selection.SetActive(true);
                player2Selection.GetComponent<MultipleUISelection>().OnlyShowElement(GetOtherCustomProperty(PhotonNetwork.PlayerListOthers[0]));
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ConnectToServer.instance.SetLoadingText(newPlayer.NickName + " joined!");
            player2Selection.SetActive(true);
            UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(Player newPlayer) 
        {
            ConnectToServer.instance.SetLoadingText(newPlayer.NickName + " left");
            player2Selection.SetActive(false);
            UpdatePlayerList();
        }

        public void UpdatePlayerList()
        {
            if(playerList)
            {
                playerList.text = "-------------\n";
                foreach (Player player in PhotonNetwork.PlayerList) 
                    playerList.text += player.ActorNumber + ": <color=" 
                        + (player.IsMasterClient ? "green" : "white") + ">" + player.NickName 
                        + (PhotonNetwork.LocalPlayer == player ? " (Me)" : "") + "</color>\n";
                playerList.text += "-------------";
            }
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { if(changedProps.ContainsKey("playerCharacter") && targetPlayer != PhotonNetwork.LocalPlayer) { player2Selection.GetComponent<MultipleUISelection>().OnlyShowElement(GetOtherCustomProperty(targetPlayer)); }}
        public int GetOtherCustomProperty(Player _player) => (int) _player.CustomProperties["playerCharacter"];
    }
}