using System.IO;
using UnityEngine;

public class WindowBehaviours : MonoBehaviour
{
    private static GameObject CurrentWindow = null;
    private GameObject OpenedWindow = null;

    [SerializeField] private GameObject profilePrefab = null;
    [SerializeField] private Transform profileList = null;

    [Header("References")]
    [SerializeField] private GameObject[] Windows;

    private void Awake()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Generations");
        DirectoryInfo dirInfo = new System.IO.DirectoryInfo(filePath);
        if (!dirInfo.Exists)
        {
            dirInfo.Create();
        }

        filePath = Path.Combine(Application.persistentDataPath, "Profiles");
        dirInfo = new DirectoryInfo(filePath);
        if (!dirInfo.Exists)
        {
            dirInfo.Create();
        }

        SetActiveWindow(Windows[0]);
    }

    private void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Profiles");
        DirectoryInfo dirInfo = new DirectoryInfo(filePath);
        var profiles = dirInfo.GetFiles();
        GameObject obj = null;
        CharacterProfile profile;
        string tempPath = null;

        foreach(var f in profiles)
        {
            tempPath = Path.Combine(filePath, f.Name);
            profile = FileReadWrite.ReadFromBinaryFile<CharacterProfile>(tempPath);

            obj = Instantiate(profilePrefab, profileList);

            obj.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = profile.ProfileName;
        }
    }

    public void SetActiveWindow(GameObject newWindow)
    {
        CurrentWindow = newWindow;
    }

    public void ChangeActiveWindow(int window)
    {
        CurrentWindow?.SetActive(false);
        Windows[window]?.SetActive(true);
        CurrentWindow = Windows[window];
    }

    public void OpenWindow(GameObject newWindow)
    {
        newWindow?.SetActive(true);
        OpenedWindow = newWindow;
    }
}
