using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class ExtraHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private List<Graphic> targets;

    private Selectable selectable;
    private ColorBlock colorBlock => selectable.colors;

    private bool hovered;
    private bool clicked;


    private void UpdateColor()
    {
        Color newColor;
        if(clicked)
        {
            newColor = colorBlock.pressedColor;
        }
        else if(hovered)
        {
            newColor = colorBlock.highlightedColor;
        }
        else
        {
            newColor = colorBlock.normalColor;
        }

        foreach(Graphic target in targets)
        {
            target.color = newColor;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        UpdateColor();
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        UpdateColor();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        clicked = true;
        UpdateColor();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
        UpdateColor();
    }


    private void OnEnable()
    {
        if(!selectable)
        {
            selectable = GetComponent<Selectable>();
        }

        hovered = false;
        clicked = false;
        UpdateColor();
    }
}