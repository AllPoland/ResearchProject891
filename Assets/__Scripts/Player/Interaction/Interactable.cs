using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public static List<Interactable> Interactables = new List<Interactable>();

    //Called to let the player know they can't interact with something anymore
    public static event Action<Interactable> OnInteractableDisabled;

    [SerializeField] public string Prompt;

    [Space]
    [SerializeField] public float MaxDistance = 1f;
    [SerializeField] public float MaxLookAngle = 45f;
    [SerializeField] public float MaxPositionAngle = 180f;

    [Space]
    [SerializeField] public UnityEvent OnInteract;

    [NonSerialized] public float DistanceFromPlayer;


    public void Interact()
    {
        OnInteract?.Invoke();
    }


    private void OnEnable()
    {
        Interactables.Add(this);
    }


    private void OnDisable()
    {
        Interactables.Remove(this);
        OnInteractableDisabled?.Invoke(this);
    }
}