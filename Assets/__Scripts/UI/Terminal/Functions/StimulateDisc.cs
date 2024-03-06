using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class StimulateDisc : MonoBehaviour
{
    //Icons are sorted 0 - top, 1 - right, 2 - bottom, 3 - left
    public StimulateChargeIcon[] chargeIcons = new StimulateChargeIcon[4];

    //0 - neutral, 1 - positive, 2 - negative
    [SerializeField] public int charge;

    [Space]
    [SerializeField] private AudioClip startClip;
    [SerializeField] private AudioClip endClip;

    [Space]
    [SerializeField] private StimulateDisc upDisc;
    [SerializeField] private StimulateDisc downDisc;
    [SerializeField] private StimulateDisc leftDisc;
    [SerializeField] private StimulateDisc rightDisc;

    [Space]
    [SerializeField] public Image UpBar;
    [SerializeField] public Image DownBar;
    [SerializeField] public Image LeftBar;
    [SerializeField] public Image RightBar;

    [Space]
    [SerializeField] private Button leftRotateButton;
    [SerializeField] private Button rightRotateButton;

    [Space]
    [SerializeField] private float iconDistance = 50;
    [SerializeField] private float turnTime = 0.25f;

    [Space]
    [SerializeField] public UnityEvent OnRotate;

    private RectTransform rectTransform;

    private bool active = true;
    private bool canRotate => active && !chargeIcons.Any(x => x?.Rotating ?? true);


    private void DisableConnectionBars()
    {
        UpBar.enabled = false;
        DownBar.enabled = false;
        LeftBar.enabled = false;
        RightBar.enabled = false;

        if(upDisc)
        {
            upDisc.DownBar.enabled = false;
        }
        if(downDisc)
        {
            downDisc.UpBar.enabled = false;
        }
        if(leftDisc)
        {
            leftDisc.RightBar.enabled = false;
        }
        if(rightDisc)
        {
            rightDisc.LeftBar.enabled = false;
        }
    }


    private IEnumerator RotateCoroutine(float endAngle, Action callback)
    {
        //Plays a rotating animation to make it seem like the icons are rotating
        //Before running a callback that actually updates the icons

        //Play a sound at the start
        TerminalAudio.PlayTerminalSound(TerminalSoundType.Notable, startClip);

        //Bring all the icons as children
        UpdateIconPositions();
        foreach(StimulateChargeIcon chargeIcon in chargeIcons)
        {
            chargeIcon.Rotating = true;
            chargeIcon.SetOffColor();
        }

        DisableConnectionBars();

        endAngle %= 360f;

        float t = 0f;
        while(t < 1f)
        {
            float angle = Mathf.Lerp(0f, endAngle, t);
            rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);

            t += Time.deltaTime / turnTime;
            yield return null;
        }

        rectTransform.localEulerAngles = Vector3.zero;

        foreach(StimulateChargeIcon chargeIcon in chargeIcons)
        {
            chargeIcon.Rotating = false;
        }

        callback();
    }


    private void UpdateIconPositions()
    {
        for(int i = 0; i < chargeIcons.Length; i++)
        {
            float x = iconDistance;
            float y = iconDistance;
            switch(i)
            {
                case 0:
                    x = 0f;
                    break;
                case 1:
                    y = 0f;
                    break;
                case 2:
                    x = 0f;
                    y = -y;
                    break;
                case 3:
                    x = -x;
                    y = 0f;
                    break;
            }

            RectTransform targetTransform = (RectTransform)chargeIcons[i].transform;
            targetTransform.SetParent(rectTransform, true);
            targetTransform.anchoredPosition = new Vector2(x, y);
        }
    }


    public void UpdateIcons(bool updateGame = true)
    {
        chargeIcons[0].UpdatePosition(upDisc, charge);
        bool useUpBar = chargeIcons[0].charge != 0 && chargeIcons[0].correctPos;
        UpBar.enabled = useUpBar;
        if(upDisc)
        {
            upDisc.chargeIcons[2] = chargeIcons[0];
            upDisc.DownBar.enabled = useUpBar;
        }

        chargeIcons[2].UpdatePosition(downDisc, charge);
        bool useDownBar = chargeIcons[2].charge != 0 && chargeIcons[2].correctPos;
        DownBar.enabled = useDownBar;
        if(downDisc)
        {
            downDisc.chargeIcons[0] = chargeIcons[2];
            downDisc.UpBar.enabled = useDownBar;
        }

        chargeIcons[3].UpdatePosition(leftDisc, charge);
        bool useLeftBar = chargeIcons[3].charge != 0 && chargeIcons[3].correctPos;
        LeftBar.enabled = useLeftBar;
        if(leftDisc)
        {
            leftDisc.chargeIcons[1] = chargeIcons[3];
            leftDisc.RightBar.enabled = useLeftBar;
        }

        chargeIcons[1].UpdatePosition(rightDisc, charge);
        bool useRightBar = chargeIcons[1].charge != 0 && chargeIcons[1].correctPos;
        RightBar.enabled = useRightBar;
        if(rightDisc)
        {
            rightDisc.chargeIcons[3] = chargeIcons[1];
            rightDisc.LeftBar.enabled = useRightBar;
        }

        UpdateIconPositions();
        if(updateGame)
        {
            TerminalAudio.PlayTerminalSound(TerminalSoundType.Notable, endClip);
            OnRotate?.Invoke();
        }
    }


    private void RotateIconsLeft()
    {
        StimulateChargeIcon first = chargeIcons[0];
        for(int i = 1; i < chargeIcons.Length; i++)
        {
            chargeIcons[i - 1] = chargeIcons[i];
        }
        chargeIcons[chargeIcons.Length - 1] = first;

        UpdateIcons();
    }

    
    private void RotateIconsRight()
    {
        StimulateChargeIcon last = chargeIcons[chargeIcons.Length - 1];
        for(int i = chargeIcons.Length - 2; i >= 0; i--)
        {
            chargeIcons[i + 1] = chargeIcons[i];
        }
        chargeIcons[0] = last;

        UpdateIcons();
    }


    public void Activate()
    {
        active = true;

        leftRotateButton.gameObject.SetActive(true);
        rightRotateButton.gameObject.SetActive(true);
    }


    public void Deactivate()
    {
        active = false;

        leftRotateButton.gameObject.SetActive(false);
        rightRotateButton.gameObject.SetActive(false);
    }


    public void TurnLeft()
    {
        if(!canRotate)
        {
            return;
        }

        StartCoroutine(RotateCoroutine(90f, RotateIconsLeft));
    }


    public void TurnRight()
    {
        if(!canRotate)
        {
            return;
        }

        StartCoroutine(RotateCoroutine(-90f, RotateIconsRight));
    }


    private void OnEnable()
    {
        if(!rectTransform)
        {
            rectTransform = (RectTransform)transform;
        }
    }
}