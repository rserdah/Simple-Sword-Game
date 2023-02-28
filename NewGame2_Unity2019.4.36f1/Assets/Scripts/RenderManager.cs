using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Not sure who to credit/if copied code from a website?????????????????????????
/// </summary>
[ExecuteAlways]
public class RenderManager : MonoBehaviour
{
    public Render renderCam;
    public TextureImporterType textureType = TextureImporterType.Default;

    public bool clickToRender;
    public bool pressSpaceToRender;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || clickToRender)
        {
            renderCam.callRender(textureType);

            clickToRender = false;
        }
    }
}
