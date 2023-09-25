using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AINamesGenerator
{
    public static class Utils
    {
        [Serializable]
        private class NamesList { public List<string> names; }
        private static NamesList namesList;
        private static NamesList CurrentNamesList
        {
            get
            {
                if (namesList == null)
                {
                    TextAsset textAsset = Resources.Load("Names") as TextAsset;
                    namesList = JsonUtility.FromJson<NamesList>(textAsset.text);
                }
                return namesList;
            }
        }

        public static string GetRandomName() => CurrentNamesList.names[Random.Range(0, CurrentNamesList.names.Count)];
        public static List<string> GetRandomNames(int numberOfNames)
        {
            if (numberOfNames > CurrentNamesList.names.Count) throw new Exception("Asking for more random names than there actually are!");

            NamesList copy = new() { names = new List<string>(CurrentNamesList.names) };
            List<string> result = new();

            for (int i = 0; i < numberOfNames; i++)
            {
                int randomIndex = Random.Range(0, copy.names.Count);
                result.Add(copy.names[randomIndex]);
                copy.names.RemoveAt(randomIndex);
            }

            return result;
        }
    }
}