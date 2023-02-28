using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Not sure who to credit/if copied code from a website?????????????????????????
/// </summary>
[RequireComponent(typeof(Camera))]
public class Render : MonoBehaviour
{
    public Camera renderCam;

    private TextureImporterType textureType = TextureImporterType.Default;

    int resWidth = 256;
    int resHeight = 256;

    private void Awake()
    {
        Debug.Log("renderCam's active/deactive state: " + renderCam.enabled);

        try
        {
            if(!renderCam)
                renderCam = GetComponent<Camera>();
        }
        catch(Exception e) { Debug.LogWarning("Render Component must be attached to a Camera GameObject."); }

        //Maybe meant to say if(renderCam.targetTexture == null) ????????????
        if(renderCam == null) //Why check if null and then try to access something null ????????
            renderCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        else
        {
            resWidth = renderCam.targetTexture.width;
            resHeight = renderCam.targetTexture.height;
        }

        renderCam.gameObject.SetActive(false);
        Debug.Log("Deactivated renderCam.");
    } //Awake()

    public void callRender(TextureImporterType textureType = TextureImporterType.Default)
    {
        renderCam.gameObject.SetActive(true);

        this.textureType = textureType;

        //Call Awake() and LateUpdate() if callRender() is called when not in Play Mode (b/c they won't be called automatically)
        if(!Application.isPlaying)
        {
            Awake();
            LateUpdate();
        }
    } //callRender()

    private string getPNGFileName()
    {
        return string.Format("{0}/Renders/render_{1}x{2}_{3}.png",
            Application.dataPath,
            resWidth,
            resHeight,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    } //getPNGFileName()

    private string getTGAFileName()
    {
        return string.Format("{0}/Renders/render_{1}x{2}_{3}.tga",
            Application.dataPath,
            resWidth,
            resHeight,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    } //getTGAFileName()

    void LateUpdate()
    {
        //if(renderCam.gameObject.activeInHierarchy)
        {
            Texture2D renderImage = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            renderCam.Render();
            RenderTexture.active = renderCam.targetTexture;
            renderImage.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

            byte[] bytes = null;
            string fileName = null;

            //Render PNG
            bytes = renderImage.EncodeToPNG();
            fileName = getPNGFileName();
            System.IO.File.WriteAllBytes(fileName, bytes);
            //Debug.Log("Rendered PNG: " + fileName);

            string relativePath = "Assets" + fileName.Substring(Application.dataPath.Length);

            //Use ImportAssetOptions to make it a Sprite (and have settings in this class to decide what image type to make it)
            //Use ImportAssetOptions to make it a Sprite (and have settings in this class to decide what image type to make it)
            //Use ImportAssetOptions to make it a Sprite (and have settings in this class to decide what image type to make it)
            //Use ImportAssetOptions to make it a Sprite (and have settings in this class to decide what image type to make it)
            //Use ImportAssetOptions to make it a Sprite (and have settings in this class to decide what image type to make it)
            //Use ImportAssetOptions to make it a Sprite (and have settings in this class to decide what image type to make it)
            //Use ImportAssetOptions to make it a Sprite (and have settings in this class to decide what image type to make it)
            //Use ImportAssetOptions to make it a Sprite (and have settings in this class to decide what image type to make it)

            AssetDatabase.ImportAsset(relativePath); //FIRST IMPORT, THERE IS ANOTHER IMPORT BELOW; Import so image shows up immediately
            Texture2D image = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);
            Debug.Log("Rendered PNG: " + fileName, image);

            //Modified from FlyingOstriche's answer at Unity Answers (https://answers.unity.com/questions/474654/how-to-set-a-texture-resource-to-texture-type-gui.html)
            TextureImporter importer = AssetImporter.GetAtPath(relativePath) as TextureImporter;
            importer.textureType = textureType;
            AssetDatabase.WriteImportSettingsIfDirty(relativePath);

            AssetDatabase.ImportAsset(relativePath); //Import again b/c even though it shows the type as Sprite, it's not accepted in Image Component until it is reimported again

            //Render TGA
            /*bytes = renderImage.EncodeToTGA();
            fileName = getTGAFileName();
            System.IO.File.WriteAllBytes(fileName, bytes);
            Debug.Log("Rendered TGA: " + fileName);*/

            renderCam.gameObject.SetActive(false);
        }
    } //LateUpdate()
} //Render
