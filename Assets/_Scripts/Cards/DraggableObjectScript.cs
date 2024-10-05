using Nodes.Tiles;
using UnityEngine;

public class DraggableObjectScript : MonoBehaviour
{
    public CardSO cardData;
    public DragControllerScript DragControllerScript;

    public void Init(CardSO cardSO)
    {
        cardData = cardSO;
        gameObject.GetComponent<SpriteRenderer>().sprite = cardData.sprite;
    }
}
