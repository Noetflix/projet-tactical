using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    public Text messageText;
    public Button confirmButton;
    public float autoCloseTime = 5f;

    Canvas parentCanvas;
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas == null)
        {
            Debug.LogError("DialogueBubble must be a child of a Canvas.");
        }
    }
    
    /// <summary>
    /// Active la bulle de début de combat et gère son affichage
    /// </summary>
    /// <param name="message"></param>
    /// <param name="worldTarget"></param>
    /// <param name="waitForConfirm"></param>
    /// <returns></returns>
    public IEnumerator ShowCoroutine(string message, Transform worldTarget, bool waitForConfirm = false)
    {
        Debug.Log("Showing dialogue bubble with message: " + message);
        gameObject.SetActive(true);
        messageText.text = message;

        if (waitForConfirm && confirmButton != null)
        {
            // Logique pour l'activation du bouton et l'attente de la confirmation
        }
        else
        {
            yield return new WaitForSeconds(autoCloseTime);
        }

        gameObject.SetActive(false);
        Debug.Log("Dialogue bubble hidden.");
    }
}
