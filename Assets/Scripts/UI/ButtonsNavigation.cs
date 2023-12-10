using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class ButtonsNavigation : MonoBehaviour, ISelectHandler, IDeselectHandler // required interface when using the OnSelect method.
{
    public void OnSelect(BaseEventData eventData)
    {
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void OnDeselect(BaseEventData data)
    {
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }
}