using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspireFunction : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private List<InspireNote> notes;
    [SerializeField] private Button startButton;
    [SerializeField] private Slider progressSlider;

    [Header("Configuration")]
    [SerializeField] private int minHitsForWin = 0;

    [Space]
    [SerializeField] private float maxTimingDeviance = 0.1f;

    private bool gameRunning = false;
    private float songProgress = 0f;


    private void SetGameActive(bool active)
    {
        startButton.gameObject.SetActive(!active);
        progressSlider.gameObject.SetActive(active);

        songProgress = 0f;
        foreach(InspireNote note in notes)
        {
            note.wasHit = false;
        }

        gameRunning = active;
    }


    public void CancelGame()
    {
        if(!gameRunning)
        {
            return;
        }

        SetGameActive(false);
    }


    public void StartGame()
    {
        if(gameRunning)
        {
            return;
        }

        SetGameActive(true);
    }


    public void PlayNote(int noteType)
    {
        
    }


    private void UpdateGame()
    {

    }


    private void Update()
    {
        if(gameRunning)
        {
            UpdateGame();
        }
    }


    private void OnDisable()
    {
        CancelGame();
    }
}


[Serializable]
public class InspireNote
{
    public float time;
    public int noteType;

    [NonSerialized] public bool wasHit = false;
}