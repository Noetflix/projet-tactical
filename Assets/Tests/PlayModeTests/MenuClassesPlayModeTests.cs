using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MenuClassesPlayModeTests
{
    private const string SceneName = "Level_01_Introduction";
    private const string PrefabPath = "Prefabs/PlayerUnitPrefab/Player Mage Variant";

    [UnityTest]
    public IEnumerator ChooseClass_LoadsScene_AndInstantiatesCorrectPrefab()
    {
        // --- Arrange ---
        // Charger d'abord une sc�ne "menu" de test si n�cessaire
        yield return SceneManager.LoadSceneAsync("MenuClasses", LoadSceneMode.Single);

        var menuClasses = new GameObject("MenuClasses").AddComponent<MenuClasses>();

        // --- Act ---
        // Appeler la m�thode pour choisir la classe
        menuClasses.ChooseClass("Mage");

        // Attendre que la sc�ne Level_01_Introduction soit charg�e
        yield return new WaitForSeconds(0.5f); // ou mieux: attendre activement
        Assert.AreEqual(SceneName, SceneManager.GetActiveScene().name);

        // --- Assert ---
        // V�rifier que le prefab correspondant existe dans la sc�ne
        var player = GameObject.Find("Player Mage Variant(Clone)");
        Assert.IsNotNull(player, "Le prefab 'Player Mage' n�a pas �t� instanci� dans la sc�ne.");
    }
}
