using Photon.Pun;
using UnityEngine;

namespace Online
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerOnline : MonoBehaviour
    {
        private PhotonView photonView;
        private void Awake() => photonView = GetComponent<PhotonView>();
        public bool IsView() => photonView.IsMine;
    }
}