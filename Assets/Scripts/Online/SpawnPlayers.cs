using UnityEngine;
using Photon.Pun;
using CustomClasses;

namespace Online
{
    public class SpawnPlayers : InstanceOnlineClass<SpawnPlayers>
    {
        public GameObject[] playerPrefabs, playerPrefabsOffline;
        public Transform[] spawnPosition;
        public GameObject playerStatsUIPrefab;
        public Transform playerStatsUIContainer;
        [Min(1)] public int playersToSpawn = 2;

        protected override void Awake() // Wait to start
        {
            base.Awake();
            enabled = false;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            // Clear playerStatsUI in scene
            int _childCount = playerStatsUIContainer.childCount;
            if(_childCount > 0) for (int i = 0; i < _childCount; i++) Destroy(playerStatsUIContainer.GetChild(i).gameObject);

            if(PhotonNetwork.IsConnected)
            {
                SpawnPlayerOnline();
                if(PhotonNetwork.PlayerList.Length == 1 && playersToSpawn > 1)
                    for (int i = 1; i < playersToSpawn; i++) SpawnPlayerOffline(i);
            }
            else
            {
                for (int i = 0; i < playersToSpawn; i++) SpawnPlayerOffline(i);
                GameplayManager.instance.StartGame();
            }
            CheckForPunchBags();
        }

        public void SpawnPlayerOnline()
        {
            PlayerStatsUIElements statsUI = Instantiate(playerStatsUIPrefab, playerStatsUIContainer).GetComponent<PlayerStatsUIElements>();
            MatchData.instance.playerStatsUI.Add(statsUI);
            
            string prefabName = playerPrefabs[(int) PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.PLAYERCHARACTER]].name;
            Vector3 spawnPoint = spawnPosition[PhotonNetwork.IsMasterClient ? 0 : 1].position;
            GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPoint,
                                Quaternion.Euler(new Vector3(0f, spawnPoint.x < 0 ? 90f : 270f, 0f)));

            // if(GameManager.usingEditor) Debug.Log("Spawned " + player.GetComponent<ComponentsManager>().characterStats.playerName + " in " + spawnPosition[GameManager.players.Count].position);
            player.GetComponent<ComponentsManager>().characterStats.playerStatsUI = statsUI;
            player.GetComponent<ComponentsManager>().characterStats.isFacingRight = spawnPoint.x < 0;
            player.GetComponent<ComponentsManager>().inputManagement.enabled = false;
        }

        public void SpawnPlayerOffline(int index)
        {
            PlayerStatsUIElements statsUI = Instantiate(playerStatsUIPrefab, playerStatsUIContainer).GetComponent<PlayerStatsUIElements>();
            MatchData.instance.playerStatsUI.Add(statsUI);
            
            Vector3 spawnPoint = spawnPosition[index].position;
            GameObject player = Instantiate(playerPrefabsOffline[GameManager.currentPlayer], spawnPoint,
                                Quaternion.Euler(new Vector3(0f, spawnPoint.x < 0 ? 90f : 270f, 0f)));

            CharacterStats stats = player.GetComponent<ComponentsManager>().characterStats;
            stats.playerStatsUI = statsUI;
            if(index != 0)
            {
                player.GetComponent<ComponentsManager>().characterStats.NPC = true;
                player.GetComponent<ComponentsManager>().inputManagement.enableInputs = false;
            }
            stats.SetPlayerInfo("Player " + (index + 1), index);
            // if(GameManager.usingEditor) Debug.Log("Spawned " + stats.playerName + " in " + spawnPosition[index].position + " in list " + GameManager.players.Count);
        }

        public void CheckForPunchBags()
        {
            foreach (GameObject punchBag in GameObject.FindGameObjectsWithTag("PunchBag"))
            {
                PlayerStatsUIElements statsUI = Instantiate(playerStatsUIPrefab, playerStatsUIContainer).GetComponent<PlayerStatsUIElements>();
                MatchData.instance.playerStatsUI.Add(statsUI);

                CharacterStats stats = punchBag.GetComponent<ComponentsManager>().characterStats;
                stats.playerStatsUI = statsUI;
                stats.SetPlayerInfo("PunchBag " + GameManager.players.Count, GameManager.players.Count - 1);
            }
        }
    }
}