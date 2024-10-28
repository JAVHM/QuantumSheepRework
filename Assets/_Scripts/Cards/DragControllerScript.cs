using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DragControllerScript : MonoBehaviour
{
    public GameObject draggablePrefab; // Prefab del objeto que se generará y arrastrará

    public CardSO cardData;
    private ObjectSpawner objectSpawner;


    public (bool, GameObject) HandleMouseDown()
    {
        GameplayManager.onMouseDown.Invoke(cardData);
        GameObject currentDraggable = CreateDraggableObject();
        return (true, currentDraggable);
    }

    public (bool, GameObject) HandleMouseUp(GameObject currentDraggable)
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

        return (false, null);
    }

    private GameObject CreateDraggableObject()
    {
        GameObject currentDraggable = Instantiate(draggablePrefab);
        currentDraggable.GetComponent<DraggableObjectScript>().DragControllerScript = this;
        currentDraggable.transform.position = GetMouseWorldPosition();
        currentDraggable.GetComponent<DraggableObjectScript>().Init(cardData);
        // AudioManager.instance.Play("take energy");

        return currentDraggable;
    }

    public void DragObject(GameObject currentDraggable)
    {
        currentDraggable.transform.position = GetMouseWorldPosition();
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
        this.gameObject.GetComponent<Image>().sprite = cardData.sprite;
    }

    // Método que se llama cuando el objeto colisiona con otro objeto
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si el objeto colisionado es el segundo objeto (puedes usar su tag, layer o nombre)
        if (collision.gameObject.name == "ButtonCard(Clone)") // Reemplaza con el nombre de tu segundo objeto
        {
            //print("DETECT");
        }
    }
}
