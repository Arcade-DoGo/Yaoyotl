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
        usernameInput.text = AINamesGenerator.Utils.GetRandomName();
        UpdateNickName();
    }

    public void UpdateNickName() => PhotonNetwork.NickName = usernameInput.text;
}