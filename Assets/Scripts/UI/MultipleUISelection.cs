using System.Collections.Generic;
using System.Linq;
using CustomClasses;
using UnityEngine;

public class MultipleUISelection : InstanceClass<MultipleUISelection>
{
    public List<GameObject> selectableObjects;
    public static GameObject selectedGameObject;

    # region OnlyShowElements
    // Set active only called elements, the others will be inactive
    public void OnlyShowElements(params string[] elements) => OnlyShowElements(GetElementsIndexByName(elements));
    public void OnlyShowElements(string element) => OnlyShowElements(GetElementIndexByName(element));
    public void OnlyShowElements(int element) => OnlyShowElements(new int[]{element});
    public void OnlyShowElements(params int[] elements) 
    {
        for (int index = 0; index < selectableObjects.Count; index++)
            selectableObjects[index].SetActive(elements.Contains(index));
    }
    #endregion
    
    # region ShowOtherElements
    // Set active only elements that are not called, the others will be active
    public void ShowOtherElements(params string[] elements) => ShowOtherElements(GetElementsIndexByName(elements));
    public void ShowOtherElements(string element) => ShowOtherElements(GetElementIndexByName(element));
    public void ShowOtherElements(int element) => ShowOtherElements(new int[]{element});
    public void ShowOtherElements(params int[] elements)
    {
        for (int index = 0; index < selectableObjects.Count; index++)
            selectableObjects[index].SetActive(!elements.Contains(index));
    }
    #endregion
    
    # region HideElements
    // Hide only specified elements (all if no argument is given)
    public void HideElements(params string[] elements) => HideElements(GetElementsIndexByName(elements));
    public void HideElements(string element) => HideElements(GetElementIndexByName(element));
    public void HideElements(int element) => HideElements(new int[]{element});
    public void HideElements(params int[] elements)
    {
        bool isEmpty = elements.Length == 0;
        for (int index = 0; index < selectableObjects.Count; index++)
        {
            if(elements.Contains(index) || isEmpty)
                selectableObjects[index].SetActive(false);
        }
    }
    public void HideElements()
    {
        for (int index = 0; index < selectableObjects.Count; index++)
            selectableObjects[index].SetActive(false);
    }
    #endregion
    
    # region SelectElement
    // Set selectedGameObject with value given
    public void SelectElement(string elementName) => SelectElement(GetElementIndexByName(elementName));
    public void SelectElement(int elementIndex) => selectedGameObject = selectableObjects[elementIndex];
    #endregion
    
    # region IsElementActive
    // Check if value given is active
    public bool IsElementActive(string elementName) => IsElementActive(GetElementIndexByName(elementName));
    public bool IsElementActive(int elementIndex) => selectableObjects[elementIndex].activeSelf;
    #endregion
    
    # region GetElementIndexByName
    // Get element index by its name if in selectableObjects list
    public int GetElementIndexByName(string _name) => selectableObjects.IndexOf(selectableObjects.Find(element => element.name == _name));
    public int[] GetElementsIndexByName(string[] _names)
    {
        List<int> indexes = new();
        for (int index = 0; index < selectableObjects.Count; index++)
            if(_names.Contains(selectableObjects[index].name)) indexes.Add(index);
        
        return indexes.ToArray();
    }
    #endregion
}