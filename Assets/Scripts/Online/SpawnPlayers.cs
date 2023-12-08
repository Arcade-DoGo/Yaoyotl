using UnityEngine;
using Photon.Pun;
using CustomClasses;

namespace Online
{
    public class SpawnPlayers : InstanceOnlineClass<SpawnPlayers>
    {
        [Header ("Prefabs")]
        public GameObject[] playerPrefabs;
        public GameObject playerPrefabOffline;
        [Min(1)] public int playersToSpawn = 2;
        public Transform[] spawnPosition;
        
        protected override void Awake() // Wait to start
        {
            base.Awake();
            enabled = false;
        }

        private void Start() 
        {
            if(PhotonNetwork.IsConnected) SpawnPlayerOnline();
            else 
            {
                for (int i = 0; i < playersToSpawn; i++) SpawnPlayerOffline(i);
                GameplayManager.instance.StartGame();
            }
            CheckForPunchBags();
        }

        public void SpawnPlayerOnline()
        {
            string prefabName = playerPrefabs[(int) PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.PLAYERCHARACTER]].name;
            Vector3 spawnPoint = spawnPosition[PhotonNetwork.IsMasterClient ? 0 : 1].position;
            GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPoint, 
                                Quaternion.Euler(new Vector3(0f, spawnPoint.x < 0 ? 90f : 270f, 0f)));

            if(GameManager.usingEditor) Debug.Log("Spawned " + player.GetComponent<ComponentsManager>().characterStats.playerName + " in " + spawnPosition[GameManager.players.Count].position);
            player.GetComponent<ComponentsManager>().inputManagement.enabled = false;
        }

        public void SpawnPlayerOffline(int index)
        {
            Vector3 spawnPoint = spawnPosition[index].position;
            GameObject player = Instantiate(playerPrefabOffline, spawnPoint, 
                                Quaternion.Euler(new Vector3(0f, spawnPoint.x < 0 ? 90f : 270f, 0f)));

            CharacterStats stats = player.GetComponent<ComponentsManager>().characterStats;
            if(index != 0)
            {
                player.GetComponent<ComponentsManager>().inputManagement.enableInputs = false;
                player.GetComponent<CharacterMovement>().enabled = false;
                player.GetComponent<Attack>().enabled = false;
            }
            stats.playerNumber = index;
            stats.playerName = "Player " + (index + 1);
            if(GameManager.usingEditor) Debug.Log("Spawned " + stats.playerName + " in " + spawnPosition[index].position + " in list " + GameManager.players.Count);
        }

        public void CheckForPunchBags()
        {
            foreach (GameObject punchBag in GameObject.FindGameObjectsWithTag("PunchBag"))
            {
                CharacterStats stats = punchBag.GetComponent<ComponentsManager>().characterStats;
                stats.playerNumber = GameManager.players.Count - 1;
                stats.playerName = "PunchBag " + GameManager.players.Count;
            }
        }
    }
}