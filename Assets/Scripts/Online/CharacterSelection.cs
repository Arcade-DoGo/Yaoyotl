using ExitGames.Client.Photon;
using Photon.Pun;

namespace Online
{
    public class CharacterSelection : MonoBehaviourPunCallbacks
    {
        public static CharacterSelection instance;
        private void Awake() => instance = this;
        public void SetCharacter(int _index) 
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(ConnectToServer.PLAYERCHARACTER)) PhotonNetwork.LocalPlayer.CustomProperties.Add(ConnectToServer.PLAYERCHARACTER, _index);
            PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.PLAYERCHARACTER] = _index;
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { ConnectToServer.PLAYERCHARACTER, _index } });
        }
    }
}