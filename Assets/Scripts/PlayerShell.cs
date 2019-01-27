using UnityEngine;

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

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        float freq = 1;
        float t = Time.time - emissionStartTime;
        float emission = Mathf.Sin(t * freq);
        meshRenderer.material.SetColor("_EmissionColor", emissionColor * (2 + emission));
    }

    public void ActivateBrokenMaterial()
    {
        meshRenderer.material = brokenMaterial;
        emissionStartTime = Time.time;
        animateEmission = true;
    }
}
