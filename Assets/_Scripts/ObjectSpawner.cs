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

    private void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    private System.Collections.IEnumerator SpawnObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);

            DragControllerScript dragControl = obj.GetComponent<DragControllerScript>();
            dragControl.Init(cards[Random.Range(0, cards.Length)]);

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = launchDirection * launchSpeed;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)gizmoLineLength);
    }
}
