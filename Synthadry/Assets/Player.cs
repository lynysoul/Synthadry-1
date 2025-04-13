using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;

    void Start()
    {
        if (!gameObject.CompareTag("Player"))
        {
            Debug.LogError("Игровой объект должен иметь тег 'Player'!");
        }
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0f, 100f); 

        Debug.Log("Получен урон: " + amount + ", Текущее здоровье: " + health);
        if (health <= 0f)
        {
            Die();
        }
    }

    public float GetHealth()
    {
        return health;
    }

    private void Die()
    {
        Debug.Log("Игрок умер!");
        gameObject.SetActive(false);
    }
}
