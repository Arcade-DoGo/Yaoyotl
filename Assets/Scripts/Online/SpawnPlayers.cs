using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;

namespace Online
{
    public class SpawnPlayers : MonoBehaviourPunCallbacks
    {
        [Header ("UI References")]
        public GameObject playerPrefab;
        public TextMeshProUGUI startText;
        public Transform[] spawnPosition;

        private void Awake()
        {
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition[PhotonNetwork.IsMasterClient ? 0 : 1].position, Quaternion.identity);
            if(GameManager.usingEditor) Debug.Log("Spawned " + player.GetComponent<ComponentsManager>().characterStats.playerName + " in " + spawnPosition[GameManager.players.Count].position);
            player.GetComponent<ComponentsManager>().inputManagement.enabled = false;
        }
        private void Start() => StartCoroutine(StartGame());
        private IEnumerator StartGame()
        {
            startText.text = "";
            yield return new WaitForSeconds(0.5f);
            for (int i = 3; i > 0; i--)
            {
                startText.text = i + "";
                yield return new WaitForSeconds(1f);
            }
            startText.text = "GO!";
            GameManager.EnablePlayers();
            MatchData.instance.StartTimer();
            yield return new WaitForSeconds(1f);
            startText.text = "";
        }
    }
}