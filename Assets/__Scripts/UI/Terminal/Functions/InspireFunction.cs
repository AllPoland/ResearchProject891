using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspireFunction : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private List<InspireNote> notes;
    [SerializeField] private Button startButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Slider progressSlider;

    [Space]
    [SerializeField] private ObjectPool indicatorPool;
    [SerializeField] private Transform indicatorParent;

    [Space]
    [SerializeField] private ScrollingText scrollingText;
    [SerializeField] private string winText;
    [SerializeField] private string loseText;

    [Header("Configuration")]
    [SerializeField] private float songLength = 1f;
    [SerializeField] private float startTime = 0f;
    [SerializeField] private float scrollSpeed = 50f;

    [Space]
    [SerializeField] private int minHitsForWin = 0;
    [SerializeField] private float maxTimingDeviance = 0.1f;

    [Header("Progression")]
    [SerializeField] private ProgressionRange influenceProgressionRange;
    [SerializeField] private int progressionOnComplete;
    [SerializeField] private int progressionTerminalInactive;

    private bool gameRunning = false;
    private float songProgress = 0f;

    private int _hits;
    private int hits
    {
        get => _hits;
        set
        {
            _hits = value;
            progressSlider.SetValueWithoutNotify(value);
        }
    }


    private void SetGameActive(bool active)
    {
        startButton.gameObject.SetActive(!active);
        resetButton.gameObject.SetActive(active);
        progressSlider.gameObject.SetActive(active);

        progressSlider.maxValue = minHitsForWin;

        songProgress = startTime;
        hits = 0;

        foreach(InspireNote note in notes)
        {
            note.wasHit = false;
            if(!active && note.transform)
            {
                indicatorPool.ReleaseObject(note.transform.gameObject);
                note.transform = null;
            }
            else if(active && !note.transform)
            {
                GameObject newIndicator = indicatorPool.GetObject();

                note.transform = (RectTransform)newIndicator.transform;
                note.transform.SetParent(indicatorParent);

                note.transform.localScale = Vector3.one;
                note.transform.localPosition = Vector3.zero;

                newIndicator.SetActive(true);
            }
        }

        gameRunning = active;

        if(gameRunning)
        {
            UpdateGame();
        }
    }


    public void StartGame()
    {
        SetGameActive(true);
    }


    private void SetProgression()
    {
        if(influenceProgressionRange.CheckInRange(ProgressionManager.ProgressionStage))
        {
            int newStage = TerminalScreen.TerminalActive ? progressionOnComplete : progressionTerminalInactive;
            ProgressionManager.ProgressionStage = newStage;
        }
    }


    private void UpdateWin()
    {
        if(hits >= minHitsForWin)
        {
            //The player hit enough notes to win
            scrollingText.SetText(winText);
            SetProgression();
        }
        else
        {
            scrollingText.SetText(loseText);
        }

        SetGameActive(false);
    }


    public void PlayNote(int noteType)
    {
        if(!gameRunning)
        {
            return;
        }

        foreach(InspireNote note in notes)
        {
            if(!note.wasHit && note.type == noteType && Mathf.Abs(note.time - songProgress) <= maxTimingDeviance)
            {
                //The player has hit a note
                hits++;

                //Disable the note's visual
                indicatorPool.ReleaseObject(note.transform.gameObject);
                note.transform = null;

                note.wasHit = true;
                return;
            }
        }
    }


    private void UpdateGame()
    {
        songProgress += Time.deltaTime;

        if(songProgress >= songLength)
        {
            UpdateWin();
            return;
        }

        foreach(InspireNote note in notes)
        {
            if(note.wasHit || !note.transform)
            {
                continue;
            }

            float timeDifference = note.time - songProgress;

            if(timeDifference < -maxTimingDeviance)
            {
                //This note has passed without getting hit
                indicatorPool.ReleaseObject(note.transform.gameObject);
                note.transform = null;

                note.wasHit = transform;
                continue;
            }

            float xPos = note.transform.sizeDelta.x * note.type;
            float yPos = timeDifference * scrollSpeed;

            note.transform.anchoredPosition = new Vector2(xPos, yPos);
        }
    }


    private void Update()
    {
        if(gameRunning)
        {
            UpdateGame();
        }
    }


    private void OnEnable()
    {
        SetGameActive(false);
    }


    private void OnDisable()
    {
        SetGameActive(false);
    }
}


[Serializable]
public class InspireNote
{
    public float time;
    public int type;

    [NonSerialized] public bool wasHit = false;
    [NonSerialized] public RectTransform transform;
}