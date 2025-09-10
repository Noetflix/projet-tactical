using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))] // Assure que le GameObject a un composant Camera
public class CameraTilemapFit : MonoBehaviour
{

    public Tilemap tilemap; // R�f�rence au Tilemap

    // Start is called before the first frame update
    void Start()
    {
        FitCameraToTilemap();
    }

    public void FitCameraToTilemap() 
    { 
        if(tilemap == null) // Si le Tilemap n'est pas assign�
        {
            Debug.LogError("Tilemap reference is missing!");
            return;
        }

        Camera cam = GetComponent<Camera>(); // R�cup�re le composant Camera

        Bounds  bounds = tilemap.localBounds; // R�cup�re les limites locales du Tilemap

        transform.position = new Vector3(
            bounds.center.x, // Centre en x
            bounds.center.y, // Centre en y
            transform.position.z // Garde la position z actuelle
            );

        float  aspectRatio = (float)Screen.width / (float)Screen.height; // Calcule le ratio d'aspect de l'�cran

        float tilemapWidth = bounds.size.x; // Largeur du Tilemap
        float tilemapHeight = bounds.size.y; // Hauteur du Tilemap

        float sizeBasedOnWidth = tilemapWidth / (2f * aspectRatio); // Taille de la cam�ra bas�e sur la largeur
        float sizeBasedOnHeight = tilemapHeight / 2f; // Taille de la cam�ra bas�e sur la hauteur

        cam.orthographicSize = Mathf.Max(sizeBasedOnWidth, sizeBasedOnHeight); // Fixe la taille orthographique de la cam�ra
    }
}
