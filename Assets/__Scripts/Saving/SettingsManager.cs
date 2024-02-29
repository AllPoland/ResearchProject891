using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static Settings _currentSettings;
    public static Settings CurrentSettings
    {
        get => _currentSettings;

        set
        {
            _currentSettings = value;
            Loaded = _currentSettings != null;
        }
    }

    public static event Action<string> OnSettingsUpdated;
    public static event Action OnSettingsReset;

    //This is a yucky, hacky pass-through for outside scripts to force settings to save
    //without needing a reference, and without making the whole thing a singleton
    public static Action SaveSettingsStatic;

    public static bool Loaded { get; private set; }

    private const string settingsFile = "UserSettings.json";

    [SerializeField] private List<SerializedOption<bool>> defaultBools;
    [SerializeField] private List<SerializedOption<int>> defaultInts;
    [SerializeField] private List<SerializedOption<float>> defaultFloats;
    [SerializeField] private List<SerializedOption<Color>> defaultColors;


    private bool saving;


    private async Task WriteFileAsync(string text, string path)
    {
        await File.WriteAllTextAsync(path, text);
    }


    private IEnumerator SaveSettingsCoroutine()
    {
        saving = true;

        string filePath = Path.Combine(Application.persistentDataPath, settingsFile);
        //Need to use newtonsoft otherwise dictionaries don't serialize
        string json = JsonConvert.SerializeObject(CurrentSettings);

        Task writeTask = WriteFileAsync(json, filePath);
        yield return new WaitUntil(() => writeTask.IsCompleted);

        if(writeTask.Exception != null)
        {
            Debug.LogWarning($"Failed to save settings with error: {writeTask.Exception.Message}, {writeTask.Exception.StackTrace}");
        }

        saving = false;
    }


    public void SaveSettings()
    {
        if(saving)
        {
            Debug.LogWarning("Trying to save settings when already saving!");
            return;
        }

        StartCoroutine(SaveSettingsCoroutine());
    }


    private void LoadSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, settingsFile);
        
        if(!File.Exists(filePath))
        {
            Debug.Log("Settings file doesn't exist. Using defaults.");
            CurrentSettings = Settings.GetDefaultSettings();
            SaveSettings();

            OnSettingsUpdated?.Invoke("all");
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            CurrentSettings = JsonConvert.DeserializeObject<Settings>(json);
        }
        catch(Exception err)
        {
            Debug.LogWarning($"Failed to load settings with error: {err.Message}, {err.StackTrace}");
            CurrentSettings = Settings.GetDefaultSettings();

            //It *should* be impossible for this to happen on first startup, since the file wouldn't exist
            //So the user will have already seen the static lights warning
            SetRule("staticlightswarningacknowledged", true, false);
            SaveSettings();
        }

        OnSettingsUpdated?.Invoke("all");
    }


    public static bool GetBool(string name)
    {
        bool value;

        if(!Loaded)
        {
            Debug.LogWarning($"Setting {name} was accessed before settings loaded!");
            return false;
        }

        if(CurrentSettings.Bools.TryGetValue(name, out value))
        {
            return value;
        }
        else if(Settings.DefaultSettings.Bools.TryGetValue(name, out value))
        {
            return value;
        }
        else return false;
    }


    public static int GetInt(string name)
    {
        int value;

        if(!Loaded)
        {
            Debug.LogWarning($"Setting {name} was accessed before settings loaded!");
            return 0;
        }

        if(CurrentSettings.Ints.TryGetValue(name, out value))
        {
            return value;
        }
        else if(Settings.DefaultSettings.Ints.TryGetValue(name, out value))
        {
            return value;
        }
        else return 0;
    }


    public static float GetFloat(string name)
    {
        float value;

        if(!Loaded)
        {
            Debug.LogWarning($"Setting {name} was accessed before settings loaded!");
            return 0;
        }

        if(CurrentSettings.Floats.TryGetValue(name, out value))
        {
            return value;
        }
        else if(Settings.DefaultSettings.Floats.TryGetValue(name, out value))
        {
            return value;
        }
        else return 0f;
    }


    public static Color GetColor(string name)
    {
        float r = GetFloat(name + ".r");
        float g = GetFloat(name + ".g");
        float b = GetFloat(name + ".b");
        return new Color(r, g, b);
    }


    public static void SetRule(string name, bool value, bool notify = true)
    {
        Dictionary<string, bool> rules = CurrentSettings.Bools;
        if(rules.ContainsKey(name))
        {
            rules[name] = value;
        }
        else
        {
            rules.Add(name, value);
        }

        if(notify)
        {
            OnSettingsUpdated?.Invoke(name);
        }
    }


    public static void SetRule(string name, int value, bool notify = true)
    {
        Dictionary<string, int> rules = CurrentSettings.Ints;
        if(rules.ContainsKey(name))
        {
            rules[name] = value;
        }
        else
        {
            rules.Add(name, value);
        }

        if(notify)
        {
            OnSettingsUpdated?.Invoke(name);
        }
    }


    public static void SetRule(string name, float value, bool notify = true, bool round = true)
    {
        if(round)
        {
            value = (float)Math.Round(value, 3);
        }

        Dictionary<string, float> rules = CurrentSettings.Floats;
        if(rules.ContainsKey(name))
        {
            rules[name] = value;
        }
        else
        {
            rules.Add(name, value);
        }

        if(notify)
        {
            OnSettingsUpdated?.Invoke(name);
        }
    }


    public static void SetRule(string name, Color value, bool notify = true)
    {
        //Color values need to be set as separate floats
        SetRule(name + ".r", value.r, false, false);
        SetRule(name + ".g", value.g, false, false);
        SetRule(name + ".b", value.b, false, false);

        if(notify)
        {
            OnSettingsUpdated?.Invoke(name);
        }
    }


    public static void SetDefaults()
    {
        bool staticLightsWarningAcknowledged = GetBool("staticlightswarningacknowledged");
        bool replayMode = GetBool("replaymode");

        CurrentSettings = Settings.GetDefaultSettings();
        
        //Some settings should still persist or else they'll be annoying
        SetRule("staticlightswarningacknowledged", staticLightsWarningAcknowledged, false);
        SetRule("replaymode", replayMode, false);

        OnSettingsReset?.Invoke();
        OnSettingsUpdated?.Invoke("all");
    }


    private void Awake()
    {
        //Update the default settings
        Settings.DefaultSettings.Bools = Settings.SerializedOptionsToDictionary<bool>(defaultBools);
        Settings.DefaultSettings.Ints = Settings.SerializedOptionsToDictionary<int>(defaultInts);
        Settings.DefaultSettings.Floats = Settings.SerializedOptionsToDictionary<float>(defaultFloats);
        Settings.DefaultSettings.AddColorRules(defaultColors);

        SaveSettingsStatic = SaveSettings;
        LoadSettings();
    }
}


[Serializable]
public class Settings
{
    public Dictionary<string, bool> Bools;
    public Dictionary<string, int> Ints;
    public Dictionary<string, float> Floats;


    public Settings()
    {
        Bools = new Dictionary<string, bool>();
        Ints = new Dictionary<string, int>();
        Floats = new Dictionary<string, float>();
    }


    public void AddColorRule(string name, Color color)
    {
        bool success = Floats.TryAdd(name + ".r", color.r);
        success &= Floats.TryAdd(name + ".g", color.g);
        success &= Floats.TryAdd(name + ".b", color.b);
        if(!success)
        {
            Debug.LogWarning($"Failed to add setting '{name}'. Is it a duplicate?");
        }
    }


    public void AddColorRules(IEnumerable<SerializedOption<Color>> colors)
    {
        foreach(SerializedOption<Color> color in colors)
        {
            AddColorRule(color.Name, color.Value);
        }
    }


    public static Settings DefaultSettings = new Settings();
    public static Settings GetDefaultSettings()
    {
        //Provides a deep copy of the default settings I hate reference types I hate reference types I hate reference types I hate reference types I hate reference types
        Settings settings = new Settings
        {
            Bools = new Dictionary<string, bool>(),
            Ints = new Dictionary<string, int>(),
            Floats = new Dictionary<string, float>()
        };

        foreach(var key in DefaultSettings.Bools)
        {
            settings.Bools.Add(key.Key, key.Value);
        }

        foreach(var key in DefaultSettings.Ints)
        {
            settings.Ints.Add(key.Key, key.Value);
        }

        foreach(var key in DefaultSettings.Floats)
        {
            settings.Floats.Add(key.Key, key.Value);
        }

        return settings;
    }


    public static Dictionary<string, T> SerializedOptionsToDictionary<T>(List<SerializedOption<T>> options)
    {
        Dictionary<string, T> dictionary = new Dictionary<string, T>();

        foreach(SerializedOption<T> option in options)
        {
            bool success = dictionary.TryAdd(option.Name, option.Value);

            if(!success)
            {
                Debug.LogWarning($"Failed to add setting '{option.Name}'. Is it a duplicate?");
            }
        }

        return dictionary;
    }
}


[Serializable]
public struct SerializedOption<T>
{
    public string Name;
    public T Value;
}