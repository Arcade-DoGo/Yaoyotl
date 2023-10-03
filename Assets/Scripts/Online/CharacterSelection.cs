using ExitGames.Client.Photon;
using Photon.Pun;

namespace Online
{
    public class CharacterSelection : MonoBehaviourPunCallbacks
    {
        public static CharacterSelection instance;
        private void Awake() => instance = this;
        [PunRPC] public void SetCharacter(int _index) 
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerCharacter")) PhotonNetwork.LocalPlayer.CustomProperties.Add("playerCharacter", _index);
            PhotonNetwork.LocalPlayer.CustomProperties["playerCharacter"] = _index;
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "playerCharacter", _index } });
        }
    }
}