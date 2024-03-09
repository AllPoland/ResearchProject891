using UnityEngine;
using UnityEngine.Events;

public class LookTrigger : MonoBehaviour
{
    [SerializeField] private Vector2 playerAngleMinMax;
    [SerializeField] private bool convertAngleToPositive;
    [SerializeField] private bool primeOnEnable = true;

    [Space]
    [SerializeField] public UnityEvent OnTriggered;

    private bool hasTriggered = true;


    private void Trigger()
    {
        OnTriggered?.Invoke();
        hasTriggered = true;
    }


    private void CheckTrigger()
    {
        Transform playerTransform = PlayerController.Instance.transform;

        float playerAngle = playerTransform.eulerAngles.y % 360f;

        if(convertAngleToPositive)
        {
            //Check the angle as only positive values
            //(Centered around 180 degrees)
            if(playerAngle < 0f)
            {
                playerAngle += 360f;
            }

            if(playerAngle >= playerAngleMinMax.x && playerAngle <= playerAngleMinMax.y)
            {
                Trigger();
                return;
            }
        }
        else
        {
            //Check the angle as positive and negative values
            //(Centered around 0 degrees)
            if(playerAngle > 180f)
            {
                playerAngle -= 360f;
            }
            else if(playerAngle < -180f)
            {
                playerAngle += 360f;
            }

            if(playerAngle >= playerAngleMinMax.x && playerAngle <= playerAngleMinMax.y)
            {
                Trigger();
                return;
            }
        }
    }


    public void Prime()
    {
        hasTriggered = false;
    }


    public void Disarm()
    {
        hasTriggered = true;
    }


    private void OnEnable()
    {
        if(primeOnEnable)
        {
            Prime();
        }
    }


    private void Update()
    {
        if(!hasTriggered)
        {
            CheckTrigger();
        }
    }
}