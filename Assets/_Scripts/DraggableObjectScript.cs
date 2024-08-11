using Nodes.Tiles;
using UnityEngine;

public class DraggableObjectScript : MonoBehaviour
{
    public CardSO cardData;
    public DragControllerScript DragControllerScript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tile"))
        {
            Debug.Log("Tile");
            if (other.GetComponent<NodeBase>()._tileUnit != null && other.GetComponent<NodeBase>()._tileUnit.gameObject.tag == "Sheep")
            {
                Debug.Log("Sheep");
                DragControllerScript.DestroyBoth();
            }
        }
    }
}
