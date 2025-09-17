using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // La cible que la cam�ra doit suivre
    public bool lockPosition = false; // Verrouiller la position

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z); // Suivre la cible en gardant la position Z de la cam�ra
        }
    }

    public void ModeOpenWorldOrCombat(bool isOnCombat)
    {
        if(!isOnCombat)
            Camera.main.orthographicSize = 4; // Ajuster la taille de la cam�ra pour le mode monde ouvert
        else
            Camera.main.orthographicSize = 5; // Ajuster la taille de la cam�ra pour le mode combat
    }

    public void LockOn(Transform newTarget)
    {
        target = newTarget;
        lockPosition = true;
    }

    public void Unlock()
    {
        lockPosition = false;
    }
}
