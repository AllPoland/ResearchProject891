using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ProgressionMaterialSwap : ProgressionUpdater
{
    [SerializeField] private Material enabledMaterial;
    [SerializeField] private Material disabledMaterial;

    private Renderer targetRenderer;


    public override void Enable()
    {
        targetRenderer.material = enabledMaterial;
    }


    public override void Disable()
    {
        targetRenderer.material = disabledMaterial;
    }

    private void OnEnable()
    {
        if(!targetRenderer)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }
}