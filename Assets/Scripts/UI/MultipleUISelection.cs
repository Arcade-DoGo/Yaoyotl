using System.Collections.Generic;
using UnityEngine;

public class MultipleUISelection : MonoBehaviour
{
    public List<GameObject> selectableObjects;
    public static GameObject selectedGameObject;

    public void OnlyShowElement(int elementIndex) { for (int index = 0; index < selectableObjects.Count; index++) selectableObjects[index].SetActive(index == elementIndex); }
    public void ShowOtherElements(int elementIndex) { for (int index = 0; index < selectableObjects.Count; index++) selectableObjects[index].SetActive(index != elementIndex); }
    public void HideElements() { foreach (GameObject _gameObject in selectableObjects) _gameObject.SetActive(false); }
    public void SelectElement(int elementIndex) => selectedGameObject = selectableObjects[elementIndex];
    public bool IsElementActive(int elementIndex) => selectableObjects[elementIndex].activeSelf;
}