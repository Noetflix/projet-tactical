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
        // Charger d'abord une scène "menu" de test si nécessaire
        yield return SceneManager.LoadSceneAsync("MenuClasses", LoadSceneMode.Single);

        var menuClasses = new GameObject("MenuClasses").AddComponent<MenuClasses>();

        // --- Act ---
        // Appeler la méthode pour choisir la classe
        menuClasses.ChooseClass("Mage");

        // Attendre que la scène Level_01_Introduction soit chargée
        yield return new WaitForSeconds(0.5f); // ou mieux: attendre activement
        Assert.AreEqual(SceneName, SceneManager.GetActiveScene().name);

        // --- Assert ---
        // Vérifier que le prefab correspondant existe dans la scène
        var player = GameObject.Find("Player Mage Variant(Clone)");
        Assert.IsNotNull(player, "Le prefab 'Player Mage' n’a pas été instancié dans la scène.");
    }
}
