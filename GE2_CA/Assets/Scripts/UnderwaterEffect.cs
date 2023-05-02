using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEffect : MonoBehaviour
{
    public Material underwaterMaterial; // The material to apply to the camera when underwater
    public float underwaterLevel = 0f; // The height of the water surface
    public float waterSurfaceOffset = 0.1f; // The distance between the camera and the water surface
    public Color underwaterFogColor = new Color(0f, 0.3f, 0.4f, 0.5f); // The color of the underwater fog
    public float underwaterFogDensity = 0.1f; // The density of the underwater fog
    public float underwaterFogStartDistance = 0.1f; // The start distance of the underwater fog
    public float underwaterFogEndDistance = 10f; // The end distance of the underwater fog

    private Camera cam;
    private bool isUnderwater;
    private Color originalFogColor;
    private float originalFogDensity;
    private float originalFogStartDistance;
    private float originalFogEndDistance;

    void Start()
    {
        cam = GetComponent<Camera>();
        originalFogColor = RenderSettings.fogColor;
        originalFogDensity = RenderSettings.fogDensity;
        originalFogStartDistance = RenderSettings.fogStartDistance;
        originalFogEndDistance = RenderSettings.fogEndDistance;
    }

    // The following blocks of code checks if the player is under water level and renders the fog settings if they are under water or not and proceeds to call out the corresponding functions
    void Update()
    {
        if (transform.position.y < underwaterLevel)
        {
            if (!isUnderwater)
            {
                StartUnderwater();
            }
        }
        else
        {
            if (isUnderwater)
            {
                StopUnderwater();
            }
        }
    }

    void StartUnderwater()
    {
        isUnderwater = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = underwaterFogColor;
        cam.depthTextureMode |= DepthTextureMode.Depth;
        cam.transform.position += Vector3.up * waterSurfaceOffset;
        cam.SetReplacementShader(underwaterMaterial.shader, "");

        RenderSettings.fogColor = underwaterFogColor;
        RenderSettings.fogDensity = underwaterFogDensity;
        RenderSettings.fogStartDistance = underwaterFogStartDistance;
        RenderSettings.fogEndDistance = underwaterFogEndDistance;
        RenderSettings.fog = true;
    }

    void StopUnderwater()
    {
        isUnderwater = false;
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.depthTextureMode = DepthTextureMode.None;
        cam.transform.position -= Vector3.up * waterSurfaceOffset;
        cam.ResetReplacementShader();

        RenderSettings.fogColor = originalFogColor;
        RenderSettings.fogDensity = originalFogDensity;
        RenderSettings.fogStartDistance = originalFogStartDistance;
        RenderSettings.fogEndDistance = originalFogEndDistance;
        RenderSettings.fog = true;
    }
}
