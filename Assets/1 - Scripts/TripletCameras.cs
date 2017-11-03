using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TripletCameras : MonoBehaviour
{
    [SerializeField] private Camera _otherWorldCamera;
    [SerializeField] private Camera _alphaMaskCamera;
    [SerializeField] private int _alphaMaskDownResFactor;

    private Material material;

    void Start()
    {
        material = new Material(Shader.Find("Hidden/AlphaMask"));
        SetLightUniverseTex();
        SetAlphaMaskRT();
    }

    private void SetLightUniverseTex()
    {
        if (_otherWorldCamera.targetTexture != null)
        {
            RenderTexture temp = _otherWorldCamera.targetTexture;
            _otherWorldCamera.targetTexture = null;
            DestroyImmediate(temp);
        }

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);

        renderTexture.name = "LightUniverseTex";
        renderTexture.filterMode = FilterMode.Bilinear;
        _otherWorldCamera.targetTexture = renderTexture;

        Shader.SetGlobalTexture("_LightUniverseTex", _otherWorldCamera.targetTexture);
    }

    private void SetAlphaMaskRT()
    {
        if (_alphaMaskCamera.targetTexture != null)
        {
            RenderTexture temp = _alphaMaskCamera.targetTexture;
            _alphaMaskCamera.targetTexture = null;
            DestroyImmediate(temp);
        }

        RenderTexture renderTexture = new RenderTexture(
                Camera.main.pixelWidth >> _alphaMaskDownResFactor,
                Camera.main.pixelHeight >> _alphaMaskDownResFactor,
                8);

        renderTexture.name = "AlphaMaskTex";
        renderTexture.antiAliasing = 4;
        renderTexture.depth = 0;
        renderTexture.filterMode = FilterMode.Bilinear;
        _alphaMaskCamera.targetTexture = renderTexture;

        Shader.SetGlobalTexture("_AlphaMaskTex", _alphaMaskCamera.targetTexture);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (material == null) { return; }
        Graphics.Blit(src, dst, material);
    }
}
