using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class StringListWrapper
{
    public List<string> stringList;

    public StringListWrapper(List<string> strings)
    {
        stringList = strings;
    }
}

[DefaultExecutionOrder(-1)]
public class SavingManager : MonoBehaviour
{
    public static SavingManager instance;
    public List<string> stories;

    private string filePath;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        filePath = Application.persistentDataPath + "/savedStories.json";
        stories = LoadData();
    }
  
    public void OnAddStory(string story)
    {
        stories.Add(story);
        SaveData(stories);
    }

    public void SaveData(List<string> data)
    {
        // Create a wrapper object with the list
        StringListWrapper wrapper = new StringListWrapper(data);

        // Convert the wrapper object to JSON format
        string jsonData = JsonUtility.ToJson(wrapper);

        // Write the JSON data to a file
        File.WriteAllText(filePath, jsonData);
    }

    public List<string> LoadData()
    {
        List<string> loadedData = new List<string>();

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);

            // Convert the JSON data back to a wrapper object
            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(jsonData);

            // Retrieve the list from the wrapper object
            loadedData = wrapper.stringList;
        }
        else
        {
            Debug.LogWarning("No saved data found.");
        }

        return loadedData;
    }

}

