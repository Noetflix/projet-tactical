using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        health = Mathf.Max(health, 0);
    }
}
