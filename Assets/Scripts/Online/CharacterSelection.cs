using CustomClasses;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Online
{
    public class CharacterSelection : InstanceOnlineClass<CharacterSelection>
    {
        public void RandomCharacter() => SetCharacter(Random.Range(0, RoomManager.instance.names.Length)); 
        public void SetCharacter(int _index) 
        {
            if(_index < RoomManager.instance.names.Length)
            {
                if(PhotonNetwork.IsConnected)
                {
                    if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(ConnectToServer.PLAYERCHARACTER)) PhotonNetwork.LocalPlayer.CustomProperties.Add(ConnectToServer.PLAYERCHARACTER, _index);
                    PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.PLAYERCHARACTER] = _index;
                    PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { ConnectToServer.PLAYERCHARACTER, _index } });
                }
                else
                {
                    RoomManager.instance.SetPlayerCharacter(0, _index);
                }
            }
        }
    }
}