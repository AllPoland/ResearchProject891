using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartupSequence : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioSource source;
    [SerializeField] private Light screenLight;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI welcomeText;
    [SerializeField] private Image soupLogo;

    [Space]
    [SerializeField] private AudioSource backgroundSource;

    [Header("Configuration")]
    [SerializeField] private AudioClip startClip;

    [Space]
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float welcomeTextDelay = 1f;
    [SerializeField] private float welcomeTextTime = 1f;
    [SerializeField] private float soupLogoDelay = 1f;
    [SerializeField] private float soupLogoTime = 1f;
    [SerializeField] private float endDelay = 1f;

    [NonSerialized] public bool starting = false;


    private IEnumerator StartupCoroutine()
    {
        source.clip = startClip;
        source.Play();

        yield return new WaitForSeconds(startDelay);

        backgroundSource.Play();
        screenLight.enabled = true;
        background.enabled = true;
        starting = false;

        yield return new WaitForSeconds(welcomeTextDelay);

        welcomeText.enabled = true;

        yield return new WaitForSeconds(welcomeTextTime);

        welcomeText.enabled = false;

        yield return new WaitForSeconds(soupLogoDelay);

        soupLogo.gameObject.SetActive(true);

        yield return new WaitForSeconds(soupLogoTime);

        soupLogo.gameObject.SetActive(false);

        yield return new WaitForSeconds(endDelay);

        TerminalAudio.PlayTerminalSound(TerminalSoundType.Text);

        //Start the actual terminal stuff
        gameObject.SetActive(false);
        TerminalScreen.Instance.SetWindow(TerminalScreen.Instance.StartWindow);
    }


    public void PlayStartup()
    {
        starting = true;

        gameObject.SetActive(true);
        screenLight.enabled = false;
        background.enabled = false;
        welcomeText.enabled = false;
        soupLogo.gameObject.SetActive(false);

        StartCoroutine(StartupCoroutine());
    }
}