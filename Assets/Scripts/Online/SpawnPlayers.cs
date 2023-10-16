using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;

namespace Online
{
    public class SpawnPlayers : MonoBehaviourPunCallbacks
    {
        public static SpawnPlayers instance;
        [Header ("Prefabs")]
        public GameObject[] playerPrefabs;
        public GameObject playerPrefabOffline;
        [Min(1)] public int playersToSpawn = 2;
        public Transform[] spawnPosition;
        [Header ("UI References")]
        public TextMeshProUGUI startText;

        private void Awake() => instance = this;
        private void Start() 
        {
            if(PhotonNetwork.IsConnected) SpawnPlayerOnline();
            else 
            {
                for (int i = 0; i < playersToSpawn; i++) SpawnPlayerOffline(i);
                StartGame();
            }
            CheckForPunchBags();
        }

        public void SpawnPlayerOnline()
        {
            string prefabName = playerPrefabs[(int) PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.PLAYERCHARACTER]].name;
            GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPosition[PhotonNetwork.IsMasterClient ? 0 : 1].position, Quaternion.Euler(new Vector3(0f, 135f, 0f)));
            if(GameManager.usingEditor) Debug.Log("Spawned " + player.GetComponent<ComponentsManager>().characterStats.playerName + " in " + spawnPosition[GameManager.players.Count].position);
            player.GetComponent<ComponentsManager>().inputManagement.enabled = false;
        }

        public void SpawnPlayerOffline(int index)
        {
            GameObject player = Instantiate(playerPrefabOffline, spawnPosition[index].position, Quaternion.Euler(new Vector3(0, 135, 0)));
            CharacterStats stats = player.GetComponent<ComponentsManager>().characterStats;
            if(index != 0)
            {
                player.GetComponent<CharacterMovement>().enabled = false;
                player.GetComponent<Attack>().enabled = false;
            }
            stats.playerNumber = index;
            stats.playerName = "Player " + (index + 1);
            GameManager.players.Add(stats);
            if(GameManager.usingEditor) Debug.Log("Spawned " + stats.playerName + " in " + spawnPosition[index].position + " in list " + GameManager.players.Count);
        }

        public void CheckForPunchBags()
        {
            foreach (GameObject punchBag in GameObject.FindGameObjectsWithTag("PunchBag"))
            {
                CharacterStats stats = punchBag.GetComponent<ComponentsManager>().characterStats;
                stats.playerNumber = GameManager.players.Count - 1;
                stats.playerName = "PunchBag " + GameManager.players.Count;
                GameManager.players.Add(stats);
            }
        }

        public void StartGame() => StartCoroutine(StartGameRoutine());
        private IEnumerator StartGameRoutine()
        {
            if(PhotonNetwork.IsConnected)
            {
                if(startText) startText.text = "";
                yield return new WaitForSeconds(0.5f);
                for (int i = 3; i > 0; i--)
                {
                    if(startText) startText.text = i + "";
                    yield return new WaitForSeconds(1f);
                }
            }
            if(startText) startText.text = "GO!";
            GameManager.EnablePlayers();
            MatchData.instance.StartTimer();
            yield return new WaitForSeconds(1f);
            if(startText) startText.text = "";
        }
    }
}