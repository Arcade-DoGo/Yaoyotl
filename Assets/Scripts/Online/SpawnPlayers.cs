using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;

namespace Online
{
    public class SpawnPlayers : MonoBehaviourPunCallbacks
    {
        [Header ("Player prefabs")]
        public GameObject playerPrefab;
        public GameObject playerPrefabOffline;
        [Min(1)] public int playersToSpawn = 2;
        public Transform[] spawnPosition;
        [Header ("UI References")]
        public TextMeshProUGUI startText;

        private void Awake()
        {
            if(PhotonNetwork.IsConnected) SpawnPlayerOnline();
            else for (int i = 0; i < playersToSpawn; i++) SpawnPlayerOffline(i);
            CheckForPunchBags();
        }

        private void Start() => StartCoroutine(StartGame());

        public void SpawnPlayerOnline()
        {
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition[PhotonNetwork.IsMasterClient ? 0 : 1].position, Quaternion.identity);
            if(GameManager.usingEditor) Debug.Log("Spawned " + player.GetComponent<ComponentsManager>().characterStats.playerName + " in " + spawnPosition[GameManager.players.Count].position);
            player.GetComponent<ComponentsManager>().inputManagement.enabled = false;
        }

        public void SpawnPlayerOffline(int index)
        {
            GameObject player = Instantiate(playerPrefabOffline, spawnPosition[index].position, Quaternion.identity);
            CharacterStats stats = player.GetComponent<ComponentsManager>().characterStats;
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

        private IEnumerator StartGame()
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