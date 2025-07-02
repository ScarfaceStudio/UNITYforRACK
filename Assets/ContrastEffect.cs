using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ContrastEffect : MonoBehaviour
{
    public Shader contrastShader;
    private Material contrastMaterial;

    [Range(0.5f, 2.0f)]
    public float contrast = 1.2f;

    void Start()
    {
        if (contrastShader == null)
            contrastShader = Shader.Find("Hidden/ContrastEffect");
        if (contrastShader != null)
            contrastMaterial = new Material(contrastShader);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (contrastMaterial != null)
        {
            contrastMaterial.SetFloat("_Contrast", contrast);
            Graphics.Blit(src, dest, contrastMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
