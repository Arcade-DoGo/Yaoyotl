using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

namespace Online
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        [Header ("UI References")]
        [Tooltip ("Player 1 ready Text")] public TextMeshProUGUI player1ReadyText;
        [Tooltip ("Player 2 ready Text")] public TextMeshProUGUI player2ReadyText;
        [Tooltip ("Player 1 ready Button")] public Button player1ReadyButton;
        [Tooltip ("Text to display the player list inside a room")] public TextMeshProUGUI playerList;
        [Tooltip ("Panel for player 2 character")] public GameObject player2Selection;

        [Tooltip ("Room custom properties")] private Hashtable roomCustomProperties;

        private void Start() 
        {
            player1ReadyText.text = "";
            player2ReadyText.text = "";
            player1ReadyButton.enabled = false;
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
                if(PhotonNetwork.PlayerList.Count() == 2) player1ReadyButton.enabled = true;
                playerList.text = "-------------\n";
                foreach (Player player in PhotonNetwork.PlayerList) 
                    playerList.text += player.ActorNumber + ": <color=" 
                        + (player.IsMasterClient ? "green" : "white") + ">" + player.NickName 
                        + (PhotonNetwork.LocalPlayer == player ? " (Me)" : "") + "</color>\n";
                playerList.text += "-------------";
            }
        }

        public void Ready() 
        {
            player1ReadyText.text = "READY!";
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(ConnectToServer.READY)) PhotonNetwork.LocalPlayer.CustomProperties.Add(ConnectToServer.READY, true);
            PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.READY] = true;
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { ConnectToServer.READY, true } });
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) 
        { 
            if(targetPlayer != PhotonNetwork.LocalPlayer)
            {
                if(changedProps.ContainsKey(ConnectToServer.PLAYERCHARACTER)) 
                    player2Selection.GetComponent<MultipleUISelection>().OnlyShowElement(GetOtherCustomProperty(targetPlayer)); 
                else if(changedProps.ContainsKey(ConnectToServer.READY))
                    player2ReadyText.text = "READY!";
            }
            bool startGame = true;
            foreach (Player player in PhotonNetwork.PlayerList) if(!player.CustomProperties.ContainsKey(ConnectToServer.READY)) startGame = false;
            if(startGame)
            {
                ConnectToServer.instance.SetLoadingText("Starting match...");
                if(PhotonNetwork.IsMasterClient) StartCoroutine(LoadGameScene());
            }
        }

        private int GetOtherCustomProperty(Player _player) => (int) _player.CustomProperties[ConnectToServer.PLAYERCHARACTER];

        private IEnumerator LoadGameScene()
        {
            yield return new WaitForSeconds(1f);
            PhotonNetwork.LoadLevel("GameplayTesting");
        }
    }
}