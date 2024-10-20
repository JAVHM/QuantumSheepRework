using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using System;
using UnityEngine;

public class DragControllerScript : MonoBehaviour
{
    public GameObject draggablePrefab; // Prefab del objeto que se generará y arrastrará

    private GameObject currentDraggable;
    private bool isDragging = false;
    public CardSO cardData;
    private ObjectSpawner objectSpawner;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isDragging)
            HandleMouseUp();

        if (Input.GetMouseButtonDown(0) && Time.timeScale == 1f)
            HandleMouseDown();

        if (isDragging)
            DragObject();

        
    }

    private void HandleMouseDown()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            CreateDraggableObject();
            GameplayManager.onMouseDown.Invoke(cardData);
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
                bool isOnTile = GameplayManager.instance.Check(currentDraggable);
                GameplayManager.onMouseUp.Invoke();
                if (isOnTile)
                {
                    GameplayManager.onUnitMove.Invoke();
                    objectSpawner.SpawnObject();
                    objectSpawner.ReturnCardToAvailable(cardData);
                    Destroy(this.gameObject);
                }
                Destroy(currentDraggable);  
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
            currentDraggable.GetComponent<DraggableObjectScript>().Init(cardData);
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

    public void Init(CardSO cardSO, ObjectSpawner objSpawner)
    {
        cardData = cardSO;
        objectSpawner = objSpawner;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = cardData.sprite;
    }
}
