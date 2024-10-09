using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefab;
    public int numberOfObjects = 5;
    public float spawnInterval = 0.5f;
    public Vector2 launchDirection = Vector2.down;
    public float launchSpeed = 5f;
    public Vector2 gizmoLineLength = new Vector2(0, -5f);
    public CardSO[] cards;
    public CardManager cardManager;
    public LevelManager levelManager;

    private void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    private System.Collections.IEnumerator SpawnObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnObject()
    {
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);

        CardSO selectedCard = cardManager.GetRandomCard();

        if (selectedCard != null)
        {
            DragControllerScript dragControl = obj.GetComponent<DragControllerScript>();
            dragControl.Init(selectedCard, this);

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = launchDirection * launchSpeed;
            }
        }
    }

    // Función para reintroducir una carta en las disponibles
    public void ReturnCardToAvailable(CardSO card)
    {
        cardManager.ReturnCard(card);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)gizmoLineLength);
    }
}