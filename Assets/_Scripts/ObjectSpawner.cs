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

    private List<CardSO> availableCards;

    private void Start()
    {
        // Inicializa la lista de cartas disponibles
        availableCards = new List<CardSO>(cards);
        StartCoroutine(SpawnObjects());
    }

    private System.Collections.IEnumerator SpawnObjects()
    {
        for (int i = 0; i < numberOfObjects && availableCards.Count > 0; i++)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnObject()
    {
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);

        // Seleccionar una carta aleatoria y removerla de la lista
        int randomIndex = Random.Range(0, availableCards.Count);
        CardSO selectedCard = availableCards[randomIndex];
        availableCards.RemoveAt(randomIndex);

        DragControllerScript dragControl = obj.GetComponent<DragControllerScript>();
        dragControl.Init(selectedCard, this);

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = launchDirection * launchSpeed;
        }
    }

    // Función para reintroducir una carta en las disponibles
    public void ReturnCardToAvailable(CardSO card)
    {
        // Verificar si la carta ya no está en la lista para evitar duplicados
        if (!availableCards.Contains(card))
        {
            availableCards.Add(card);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)gizmoLineLength);
    }
}
