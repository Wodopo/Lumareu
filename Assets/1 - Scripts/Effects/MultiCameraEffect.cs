using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCameraEffect : MonoBehaviour
{
    private Material material;

    private void Awake()
    {
        if (material == null)
            material = new Material(Shader.Find("Hidden/AmplifyAlphaMask"));
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (material == null)
            return;
        Graphics.Blit(src, dst, material);
    }
}
