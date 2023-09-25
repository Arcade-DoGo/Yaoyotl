using Photon.Pun;
using TMPro;

public class SelectRandomName : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    private void Start() { if(!string.IsNullOrEmpty(PhotonNetwork.NickName)) usernameInput.text = PhotonNetwork.NickName; }
    public void CheckUsername()
    {
        if(!string.IsNullOrWhiteSpace(usernameInput.text)) PhotonNetwork.NickName = usernameInput.text;
        else SetRandomName();
    }

    public void SetRandomName()
    {
        PhotonNetwork.NickName = AINamesGenerator.Utils.GetRandomName();
        usernameInput.text = PhotonNetwork.NickName;
    }
}