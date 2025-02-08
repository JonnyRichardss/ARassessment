using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Newtonsoft.Json;
using System.Linq;
public class BadgeManager : MonoBehaviour
{
    private static BadgeManager _instance;
    public static BadgeManager Instance {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Badge manager accessed with no game object attached!");
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
    public static string[] progressionNames =
    {
        "INB",
        "Labs",
        "CSS"
    };
    public static int numProgression
    {
        get
        {
            if (_instance == null) return 0;
            int output = 0;
            foreach (KeyValuePair<string, bool> kvp in _instance.badgesSeen)
            {
                if (progressionNames.Contains(kvp.Key) && kvp.Value) output++;
            }
            return output;
        }
    }
    
    public static bool loadSucceded = false;
    public static bool hasReset = false;
    public XRReferenceImageLibrary library;
    private Dictionary<string, bool> badgesSeen; 
    private static string encryptionKey = "XORencryption";
    public bool useEncryption = true;
    public float LockTimer = 5.0f;
    public GameObject Lock;
    public AssistantController CRT;
    public AudioSource found;
    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
        {
            Debug.LogError("Badge manager has multiple instances!");
            Destroy(gameObject);
            return;
        }
        if (!library)
        {
            Debug.LogError("Badge Manager has no library!");
            Destroy(gameObject);
            return;
        }
        Lock.SetActive(false);
        LoadData();
    }
    private static void SaveData()
    {
        string jsonString = JsonConvert.SerializeObject(Instance.badgesSeen);
        string path = Application.persistentDataPath + "/saveGame.json";
        jsonString = EncryptDecrypt(jsonString);
        System.IO.File.WriteAllText(path, jsonString);
    }
    private static void LoadData()
    {
        Debug.LogWarning("Beginning JSON load!");//debugging on mobile is a pain i have to filter out infos theres too many
        //load badges seen in from file
        string path = Application.persistentDataPath + "/saveGame.json";
        string jsonString = "";

        //new storage incase load fails for any reason
        Dictionary<string,bool> libraryImages = new Dictionary<string, bool>();
        foreach(var image in Instance.library)
        {
            libraryImages.Add(image.name, false);
        }

        if (!File.Exists(path))
        {
            Debug.LogWarning("No JSON file found! Resetting badges!");
            Instance.badgesSeen = libraryImages;
            return;
        }
        try
        {
            jsonString = File.ReadAllText(path);
            jsonString = EncryptDecrypt(jsonString);
            Dictionary<string, bool> loadedDict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(jsonString);
            if (!(loadedDict.Count == libraryImages.Count && loadedDict.Keys.All(libraryImages.ContainsKey)))
            {
                //we need to reset if the saved version is "old" compared to our current set of tracking images
                Debug.LogWarning("Loaded badges do not match current image library! Resetting badges!");
                Instance.badgesSeen = libraryImages;
                return;
            }
            else
            {
                Instance.badgesSeen = loadedDict;
                loadSucceded = true;
            }
        }
        catch(JsonReaderException)
        {
            Debug.LogError("Invalid JSON loaded! Resetting badges!");
            Instance.badgesSeen = libraryImages;
        }
    }
    public static void ResetBadges()
    {
        hasReset = true;
        foreach(XRReferenceImage image in Instance.library)
        {
            Instance.badgesSeen[image.name] = false;
        }
        SaveData();
    }
    public static bool IsFound(string name)
    {
        return Instance.badgesSeen[name];//if i call a wrong name i *WANT* the exception to appear so i can find it
    }
    public static void RegisterFound(string name)
    {
        if ((Instance.badgesSeen[name])) return;
        Instance.found.Play();
        RDG.Vibration.Vibrate(100, -1, true);
        IEnumerator c = Instance.LockAnim();
        Instance.StartCoroutine(c);
        Instance.badgesSeen[name] = true;//again the exception is a big red flag pointing to whatever called this function
        SaveData();
        if (numProgression >= 3) Instance.CRT.ShowCompletion();
        
    }
    private static string EncryptDecrypt(string data)
    {
        if (!Instance.useEncryption) return data;//toggleable encrypt for debug

        string output = "";
        for (int i = 0; i < data.Length; i++)
        {
            output += (char)(data[i] ^ encryptionKey[i % encryptionKey.Length]);
        }
        return output;
    }
    IEnumerator LockAnim()
    {
        Instance.Lock.SetActive(true);
        yield return new WaitForSeconds(Instance.LockTimer);
        Instance.Lock.SetActive(false);
        yield break;
    }
}
