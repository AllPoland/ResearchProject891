using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public static event Action<Interactable> OnTargetChanged;

    [NonSerialized] public List<Interactable> inRangeInteractables = new List<Interactable>();

    
    private Interactable GetBestInteractable()
    {
        if(inRangeInteractables.Count == 0)
        {
            return null;
        }

        Interactable bestChoice = null;
        foreach(Interactable interactable in inRangeInteractables)
        {
            if(bestChoice == null)
            {
                bestChoice = interactable;
            }
            else if(interactable.DistanceFromPlayer < bestChoice.DistanceFromPlayer)
            {
                //This interactable is closer, so it should be picked instead
                bestChoice = interactable;
            }
        }

        return bestChoice;
    }


    private void UpdateInteractables()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        //I made these names 2 minutes ago and I'm already regretting my life choices
        foreach(Interactable interactable in Interactable.Interactables)
        {
            Vector3 interactablePosition = interactable.transform.position;
            Vector3 positionDifference = interactablePosition - position;
            positionDifference.y = 0f;

            interactable.DistanceFromPlayer = positionDifference.magnitude;
            if(interactable.DistanceFromPlayer > interactable.MaxDistance)
            {
                //This interactable is out of range
                inRangeInteractables.Remove(interactable);
                continue;
            }

            Quaternion direction = Quaternion.LookRotation(positionDifference);
            interactable.AngleDifference = Mathf.Abs(Quaternion.Angle(rotation, direction));
            if(interactable.AngleDifference > interactable.MaxLookAngle)
            {
                //We're looking away from this interactable
                inRangeInteractables.Remove(interactable);
                continue;
            }

            //This interactable is in range and we're looking at it
            if(!inRangeInteractables.Contains(interactable))
            {
                inRangeInteractables.Add(interactable);
            }
        }
    }


    private void HandleInteractableDisabled(Interactable interactable)
    {
        inRangeInteractables.Remove(interactable);
    }


    private void HandleInteract()
    {
        UpdateInteractables();

        Interactable target = GetBestInteractable();
        OnTargetChanged?.Invoke(target);

        if(target != null && Input.GetButtonDown("Interact"))
        {
            //The player wants to interact with the thing
            target.Interact();
        }
    }


    private void Update()
    {
        if(!TerminalScreen.TerminalActive)
        {
            HandleInteract();
        }
    }


    private void Start()
    {
        Interactable.OnInteractableDisabled += HandleInteractableDisabled;
    }
}