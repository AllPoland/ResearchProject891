using System;
using UnityEngine;
using UnityEngine.UI;

public class StimulateChargeIcon : MonoBehaviour
{
    [NonSerialized] public bool Rotating;
    [NonSerialized] public int ParentCharge;
    public bool correctPos => charge == 0 || (!isInside && charge == ParentCharge);

    //0 - neutral, 1 - positive, 2 - negative
    [SerializeField] public int charge;

    [SerializeField] private Image image;
    [SerializeField] private Color correctColor = Color.white;
    [SerializeField] private Color incorrectColor = Color.grey;

    private bool isInside;


    public void UpdatePosition(bool inside, int parentCharge)
    {
        isInside = inside;
        ParentCharge = parentCharge;
        image.color = correctPos ? correctColor : incorrectColor;
    }


    public void SetOffColor()
    {
        image.color = incorrectColor;
    }

    
    private void LateUpdate()
    {
        //Keep the icon right-side up
        transform.eulerAngles = Vector3.zero;
    }
}