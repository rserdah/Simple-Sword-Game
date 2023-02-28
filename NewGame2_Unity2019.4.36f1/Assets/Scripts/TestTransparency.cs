using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TestTransparency : MonoBehaviour
{
    public bool test;

    public Texture2D texture1;
    public Texture2D texture2;
    private Texture2D result;

    public string fileName = null;

    public float threshold = 0.05f;
    public TextureImporterType textureType = TextureImporterType.Default;

    private int width;
    private int height;

    private Color[] pixels1_1d;
    private Color[] pixels2_1d;
    private Color[] resultPixels;

    public Color[][] pixels1;
    public Color[][] pixels2;


    public void Update()
    {
        if(test)
        {
            if(texture1.width == texture2.width && texture1.height == texture2.height)
            {
                pixels1_1d = texture1.GetPixels();
                pixels2_1d = texture2.GetPixels();

                width = texture1.width;
                height = texture1.height;

                result = new Texture2D(width, height, TextureFormat.RGBA32, false);
                resultPixels = new Color[width * height];

                //for(int i = 0; i < width; i++)
                //{
                //    for(int j = 0; j < height; j++)
                //    {
                //        pixels1[i][j] = pixels1_1d


                //    }
                //}
                
                for(int i = 0; i < width * height; i++)
                {
                    //if(pixels1_1d[i] == Color.white && pixels2_1d[i] == Color.black)
                    if(ColorsEqual(pixels1_1d[i], Color.white, threshold) && ColorsEqual(pixels2_1d[i], Color.black, threshold))
                    {
                        resultPixels[i] = Color.clear;
                    }
                    else
                    {
                        resultPixels[i] = pixels1_1d[i];
                    }
                }

                result.SetPixels(resultPixels);


                //if(fileName != null)
                {
                    byte[] bytes = null;
                    string fileName = null;

                    bytes = result.EncodeToPNG();
                    fileName = getPNGFileName();

                    System.IO.File.WriteAllBytes(fileName, bytes);


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
                }
            }
            else
            {
                Debug.LogError("Textures don't match in size.");
            }


            test = false;
        }
    }

    private string getPNGFileName()
    {
        return string.Format("{0}/Renders/render_{1}x{2}_{3}.png",
            Application.dataPath,
            width,
            height,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    } //getPNGFileName()

    public bool ColorsEqual(Color col1, Color col2, float threshold)
    {
        if(Mathf.Abs(col1.r - col2.r) >= threshold)
            return false;
        if(Mathf.Abs(col1.g - col2.g) >= threshold)
            return false;
        if(Mathf.Abs(col1.b - col2.b) >= threshold)
            return false;

        return true;
    }
}
