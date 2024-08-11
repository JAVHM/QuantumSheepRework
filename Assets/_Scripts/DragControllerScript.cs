using Nodes.Tiles;
using UnityEngine;

public class DragControllerScript : MonoBehaviour
{
    public GameObject draggablePrefab; // Prefab del objeto que se generará y arrastrará

    private GameObject currentDraggable;
    private bool isDragging = false;
    public CardSO cardData;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale == 1f)
            HandleMouseDown();

        if (isDragging)
            DragObject();

        if (Input.GetMouseButtonUp(0))
            HandleMouseUp();
    }

    private void HandleMouseDown()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            CreateDraggableObject();
            isDragging = true;
        }
    }

    private void HandleMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            if (currentDraggable != null)
            {
                Debug.Log("A");
                //currentDraggable.gameObject.GetComponent<CircleCollider2D>().enabled = true;
                Check(currentDraggable);
                Destroy(currentDraggable, 0.5f);
                currentDraggable = null;
            }
        }
    }

    private void CreateDraggableObject()
    {
        if (draggablePrefab != null)
        {
            currentDraggable = Instantiate(draggablePrefab);
            currentDraggable.GetComponent<DraggableObjectScript>().DragControllerScript = this;
            currentDraggable.transform.position = GetMouseWorldPosition();
            // AudioManager.instance.Play("take energy");
        }
    }

    private void DragObject()
    {
        if (currentDraggable != null)
        {
            currentDraggable.transform.position = GetMouseWorldPosition();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10f; // Ajusta esta distancia si es necesario
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void DestroyBoth()
    {
        Destroy(this.gameObject, 0.5f);
    }

    public void Check(GameObject currentDraggable)
    {
        Vector3 roundedPosition = new Vector3(
                Mathf.Round(currentDraggable.transform.position.x),
                Mathf.Round(currentDraggable.transform.position.y),
                Mathf.Round(currentDraggable.transform.position.z)
            );

        Collider2D collider = Physics2D.OverlapPoint(roundedPosition, 9);
        currentDraggable.transform.position = roundedPosition;
        if (collider != null)
        {
            Debug.Log("Ya hay un objeto en la posición: " + roundedPosition);
            if (collider.gameObject.GetComponent<NodeBase>()._tileUnit != null && collider.gameObject.GetComponent<NodeBase>()._tileUnit._team == 1)
            {
                Debug.Log("Sheep");
            }
        }
    }

    public void Init(CardSO cardSO)
    {
        cardData = cardSO;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = cardData.sprite;
    }
}
