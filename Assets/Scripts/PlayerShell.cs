using UnityEngine;
using UnityTools.DataManagement;

[RequireComponent(typeof(MeshRenderer))]
public class PlayerShell : MonoBehaviour
{

    [SerializeField]
    private Material brokenMaterial;
    [SerializeField]
    private Color emissionColor;

    private bool animateEmission = false;
    private float emissionStartTime;
    private MeshRenderer meshRenderer;
    private float frequency;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        frequency = ConstantsManager.BrokenShellEmissionFrequency;
    }

    private void Update()
    {
        float t = Time.time - emissionStartTime;
        float emission = Mathf.Sin(t * frequency);
        meshRenderer.material.SetColor("_EmissionColor", emissionColor * (2 + emission));
    }

    public void ActivateBrokenMaterial()
    {
        meshRenderer.material = brokenMaterial;
        emissionStartTime = Time.time;
        animateEmission = true;
    }
}
