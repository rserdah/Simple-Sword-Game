using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScroll : MonoBehaviour
{
    [Serializable]
    public struct EntityIconPair
    {
        public enum Type
        {
            NEUTRAL, ALLY, ENEMY
        }
        public Transform entity;
        public RectTransform icon;
        public Type type;
    }

    public Vector2 panTo;
    public Vector2 coordinates;
    public float zoomScale = 1f;
    public Vector2 minMaxZoomScale = new Vector2(0.1f, 10f); //If zoomScale becomes 0, Material tiling becomes NaN, and then the tiling cannot change even if it is set to a number again

    public Vector2 mapSize;

    public Material mapMaterial;
    public RawImage mapImage;

    public bool invertX;
    public bool invertY;
    public float speed = 1f;

    public float zoomIncrement = 1f;
    public float panSpeed = 1f;

    Vector2 tiling, offset;
    public Vector2 normalizedCoordinates = new Vector2(0.5f, 0.5f); //(0.5, 0.5) is center

    public Camera miniMapCamera;
    public Transform player;
    public Camera playerCamera;
    public RectTransform playerIcon;
    public RectTransform map;
    public Transform horse;
    public RectTransform horseIcon;
    Quaternion mapRotation;

    public RectTransform iconPrefabs;
    public EntityIconPair[] entityIconPairs;


    private void Start()
    {
        for(int i = 0; i < entityIconPairs.Length; i++)
        {
            EntityIconPair e = entityIconPairs[i];

            if(e.entity)
            {
                switch(e.type)
                {
                    case EntityIconPair.Type.NEUTRAL:
                        e.icon = Instantiate((RectTransform)iconPrefabs.GetChild(0));
                        break;

                    case EntityIconPair.Type.ALLY:
                        e.icon = Instantiate((RectTransform)iconPrefabs.GetChild(1));
                        break;

                    case EntityIconPair.Type.ENEMY:
                        e.icon = Instantiate((RectTransform)iconPrefabs.GetChild(2));
                        break;
                }

                e.icon.parent = map;
                e.icon.gameObject.SetActive(true);

                WorldToMapPosition(e.entity.position, e.icon);
                e.icon.localScale = Vector3.one * 0.05f;

                entityIconPairs[i] = e;
            }
        }
    }

    private void Update()
    {
        if(mapMaterial || mapImage)
        {
            if(Input.mouseScrollDelta.y > 0)
            {
                zoomScale += zoomIncrement;
            }
            else if(Input.mouseScrollDelta.y < 0)
            {
                zoomScale += -zoomIncrement;
            }

            zoomScale = Mathf.Clamp(zoomScale, minMaxZoomScale.x, minMaxZoomScale.y);

            //Uncomment!!!!!!!!!!!!!!!!!!!
            //if(Input.GetKey(KeyCode.Mouse0))
            //{
            //    coordinates.x += Input.GetAxis("Mouse X") * panSpeed / zoomScale;
            //    coordinates.y += Input.GetAxis("Mouse Y") * panSpeed / zoomScale;
            //}



            /*
             To zoom in on center of map, increment the tiling by how much want to zoom in and then offset = (1 - tiling) * 0.5 (0.5 represents center b/c it is a normalizedCoordinate (0.5 is "center" of 0 to 1))
             Ex.
             Tiling (1, 1); Offset (0, 0) --> Tiling (2, 2); Offset (-0.5, -0.5)

             To zoom in on a coordinate, increment the tiling and then offset = (1 - tiling) * normalizedCoordinate
             Ex.
             Tiling (1, 1); Offset (0, 0); Normalized Coordinate (0.25, 0.75) (top left corner w/ 0.25 padding) --> Tiling (2, 2); Offset (-.25, -.75) (which is ([1 - 2] * .25, [1- 2] * .75))


             To map location of Transform on MiniMap, ((position - miniMapCamPosition) / miniMapCamSize) * (mapSize * 0.5) //miniMapCamSize is half the Camera width; multiply by half of mapSize b/c Map item icons will have their anchor/pivot in the 
             center of map
             */

            tiling = Vector3.Lerp(tiling, new Vector2((mapSize.y / mapSize.x) * (invertX ? -1 : 1), (mapSize.x / mapSize.x) * (invertY ? -1 : 1)) / zoomScale, Time.deltaTime * speed);

            ////Check!!!!!!!!!!!!
            //coordinates.x = (1 - tiling.x) * normalizedCoordinates.x;
            //coordinates.y = (1 - tiling.y) * normalizedCoordinates.y;

            //offset = coordinates;

            ////Uncomment!!!!!!!!!!!!!!!
            ////offset = Vector3.Lerp(offset, coordinates, Time.deltaTime * speed);

            panTo.x = (1 - tiling.x) * normalizedCoordinates.x - coordinates.x;
            panTo.y = (1 - tiling.y) * normalizedCoordinates.y - coordinates.y;

            offset = panTo;

            if(mapMaterial)
            {
                mapMaterial.mainTextureScale = tiling;
                mapMaterial.mainTextureOffset = offset;
            }
            else if(mapImage)
            {
                Rect uvRect = mapImage.uvRect;
                uvRect.x = offset.x;
                uvRect.y = offset.y;

                uvRect.width = tiling.x;
                uvRect.height = tiling.y;

                mapImage.uvRect = uvRect;
            }

            if(miniMapCamera)
            {
                if(player && playerIcon)
                {
                    //Vector2 anchoredPosition = Vector3.zero;
                    Vector3 playerCameraHeading = playerCamera.transform.forward;
                    playerCameraHeading.y = 0;

                    Quaternion lookRot = Quaternion.LookRotation(playerCameraHeading);

                    mapRotation.eulerAngles = new Vector3(0, 0, lookRot.eulerAngles.y);
                    map.rotation = mapRotation;

                    //anchoredPosition.x = ((player.position.x - miniMapCamera.transform.position.x) / miniMapCamera.orthographicSize) * (mapSize.x * 0.5f);
                    //anchoredPosition.y = ((player.position.z - miniMapCamera.transform.position.z) / miniMapCamera.orthographicSize) * (mapSize.y * 0.5f); //Use player's & miniMapCamera's position.z in calculating

                    //playerIcon.anchoredPosition = anchoredPosition;

                    //asdaskdjhkdjlsfhaslkjdfh
                            //playerIcon.anchoredPosition = WorldToMapPosition(player.position);
                    
                    WorldToMapPosition(player.position, playerIcon);


                    if(horse && horseIcon)
                    {
                        horseIcon.anchoredPosition = WorldToMapPosition(horse.position);

                        KeepIconUpright(horseIcon);
                    }

                    playerIcon.localRotation = Quaternion.Euler(0, 0, -player.transform.rotation.eulerAngles.y);
                }
            }
        }

        for(int i = 0; i < entityIconPairs.Length; i++)
        {
            EntityIconPair e = entityIconPairs[i];

            if(e.entity && e.icon)
            {
                //Original
                //WorldToMapPosition(e.entity.position, e.icon);

                //New
                e.icon.anchoredPosition = Vector3.Lerp(e.icon.anchoredPosition, WorldToMapPosition(e.entity.position), Time.deltaTime * 10f);
            }
        }
    }

    private Vector3 WorldToMapPosition(Vector3 worldPosition, RectTransform rectTransform = null)
    {
        Vector2 anchoredPosition = Vector3.zero;

        //The returned anchoredPosition is based on an anchor in the center of the map
        anchoredPosition.x = ((worldPosition.x - miniMapCamera.transform.position.x) / miniMapCamera.orthographicSize) * (mapSize.x * 0.5f);
        anchoredPosition.y = ((worldPosition.z - miniMapCamera.transform.position.z) / miniMapCamera.orthographicSize) * (mapSize.y * 0.5f); //Use worlPosition's & miniMapCamera's position.z in calculating

        //Keeps icons in correct position when zooming in
        anchoredPosition *= zoomScale;

        if(rectTransform)
        {
            rectTransform.anchoredPosition = anchoredPosition;
        }

        return anchoredPosition;
    }

    private void KeepIconUpright(RectTransform icon)
    {
        icon.localEulerAngles = new Vector3(0, 0, -mapRotation.eulerAngles.z);
    }
}
