using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOffScreen : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] contributors;
    [SerializeField] private bool startPrimed = false;
    [SerializeField] public UnityEvent OnTriggered;

    private bool visible;
    private bool primed;


    public void Prime()
    {
        primed = true;
    }


    private void OnEnable()
    {
        if(startPrimed)
        {
            Prime();
        }

        visible = false;
    }


    private void Update()
    {
        bool newVisible = contributors.Any(x => x.isVisible);
        if(primed && visible && !newVisible)
        {
            OnTriggered?.Invoke();
            primed = false;
        }

        visible = newVisible;
    }
}