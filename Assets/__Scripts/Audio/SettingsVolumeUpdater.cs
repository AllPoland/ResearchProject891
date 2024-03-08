using UnityEngine;

public class SettingsVolumeUpdater : MonoBehaviour
{
    [SerializeField] private VolumeHandler globalVolumeHandler;
    [SerializeField] private VolumeHandler sfxVolumeHandler;
    [SerializeField] private VolumeHandler ambientVolumeHandler;


    private void OnSettingsUpdated(string setting)
    {
        bool allSettings = setting == "all";
        if(allSettings || setting == "volume")
        {
            globalVolumeHandler.SetVolume(SettingsManager.GetFloat("volume"));
        }
        // if(allSettings || setting == "sfxvolume")
        // {
        //     sfxVolumeHandler.SetVolume(SettingsManager.GetFloat("sfxvolume"));
        // }
        // if(allSettings || setting == "ambientvolume")
        // {
        //     ambientVolumeHandler.SetVolume(SettingsManager.GetFloat("ambientvolume"));
        // }
    }


    private void Start()
    {
        SettingsManager.OnSettingsUpdated += OnSettingsUpdated;
        if(SettingsManager.Loaded)
        {
            OnSettingsUpdated("all");
        }
    }
}