using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerUnitTests
{
    private PlayerUnit player;
    private GameObject enemyGO;
    private EnemyUnit enemy;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Création du joueur
        var playerGO = new GameObject("Player");
        player = playerGO.AddComponent<PlayerUnit>();

        var charClass = ScriptableObject.CreateInstance<CharacterClassData>();
        charClass.className = "Guerrier";
        charClass.baseAttack = 15;
        player.characterClass = charClass;

        // Création de l'ennemi
        enemyGO = new GameObject("Enemy");
        enemy = enemyGO.AddComponent<EnemyUnit>();
        enemy.health = 50;

        yield return null; // Attente d'une frame pour PlayModeTest
    }

    [UnityTest]
    public IEnumerator UseBaseAttack_ReducesEnemyHealth()
    {
        int initialHealth = enemy.health;

        player.UseBasiqueAttack(enemyGO);

        Assert.AreEqual(initialHealth - player.characterClass.baseAttack, enemy.health);
        yield return null;
    }

    [UnityTest]
    public IEnumerator UseSpecialAttack_ReducesEnemyHealth()
    {
        int initialHealth = enemy.health;

        var attack = new Attack { attackName = "Cri de guerre", damage = 0, manaCost = 5, type = Attack.AttackType.Buff };
        player.UseSpecialAttack(attack, enemyGO);

        if (attack.type == Attack.AttackType.Buff)
        {
            // Pas de dégâts infligés
            Assert.AreEqual(initialHealth, enemy.health);
        }
        else
        {
            Assert.AreEqual(initialHealth - (player.characterClass.baseAttack + attack.damage), enemy.health);
        }

        yield return null;
    }
}
