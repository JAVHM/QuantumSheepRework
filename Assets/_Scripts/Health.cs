using Pathfinding._Scripts.Units;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject textDamage;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Method to take damage
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        GameObject textInst = Instantiate(textDamage, transform.position + new Vector3(0.5f, 0.5f,0), Quaternion.identity);
        textInst.GetComponent<TextMeshPro>().text = amount.ToString();
        Destroy(textInst, 2f);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    // Method to heal
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    // Method to check if the player is dead
    private void Die()
    {
        if(GetComponent<Unit>()._unitType == UnitType.Sheep)
        {
            GameplayManager.instance.GameOver();
            UnitsManager.Instance.RemoveAndDestroyPlayerUnits(this.GetComponent<Unit>());
        }
        else
            UnitsManager.Instance.RemoveAndDestroyNpcUnits(this.GetComponent<Unit>());
    }

    // Method to get the current health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
