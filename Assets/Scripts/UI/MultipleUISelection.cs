using System.Collections.Generic;
using UnityEngine;

public class MultipleUISelection : MonoBehaviour
{
    public static MultipleUISelection instance;
    public List<GameObject> selectableObjects;
    public static GameObject selectedGameObject;

    private void Start() 
    {
        instance = this;
        OnlyShowElement(0);
    }
    public void OnlyShowElement(int elementIndex)
    {
        for (int index = 0; index < selectableObjects.Count; index++) selectableObjects[index].SetActive(index == elementIndex);
    }
    
    public void ShowOtherElements(int elementIndex)
    {
        for (int index = 0; index < selectableObjects.Count; index++) selectableObjects[index].SetActive(index != elementIndex);
    }

    public void SelectElement(int elementIndex) => selectedGameObject = selectableObjects[elementIndex];
    public bool IsElementActive(int elementIndex) => selectableObjects[elementIndex].activeSelf;
}