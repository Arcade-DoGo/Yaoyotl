using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour
{
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public void SetIcon(Sprite _icon) => characterIcon.sprite = _icon;
    public void SetName(string _name) => characterName.text = _name;
}