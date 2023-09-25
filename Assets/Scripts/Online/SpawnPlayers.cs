using UnityEngine;
using Photon.Pun;

namespace Online
{
    public class SpawnPlayers : MonoBehaviourPunCallbacks
    {
        public GameObject playerPrefab;
        public Transform[] spawnPosition;

        void Start() => InstantiatePlayer();
        private void InstantiatePlayer()
        {
            Instantiate(playerPrefab, spawnPosition[PhotonNetwork.PlayerList.Length].position, Quaternion.identity);
        }
    }
}