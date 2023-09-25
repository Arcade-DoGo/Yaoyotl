using UnityEngine;
using TMPro;

namespace Online
{
    public class RoomItem : MonoBehaviour
    {
        public TextMeshProUGUI roomName;
        public void SetRoomName(string _roomName) => roomName.text = _roomName;
        public void OnClickItem() => LobbyManager.instance.JoinRoom(roomName.text);
    }
}