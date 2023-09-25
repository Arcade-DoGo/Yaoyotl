using Photon.Pun;
using TMPro;

public class SelectRandomName : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    
    private void Start() 
    { 
        if(!string.IsNullOrEmpty(PhotonNetwork.NickName)) usernameInput.text = PhotonNetwork.NickName;
        else SetRandomName();
    }

    public void SetRandomName()
    {
        PhotonNetwork.NickName = AINamesGenerator.Utils.GetRandomName();
        usernameInput.text = PhotonNetwork.NickName;
    }
}