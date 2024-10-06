using Nodes.Tiles;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public GameObject prefabRange;
    private List<GameObject> telePoints = new List<GameObject>();

    public void Start()
    {
        GameplayManager.onMouseDown += ShowRange;
        GameplayManager.onMouseUp += DestroyAndClearList;
    }

    public void OnDestroy()
    {
        GameplayManager.onMouseDown -= ShowRange;
        GameplayManager.onMouseUp -= DestroyAndClearList;
    }

    public void ShowRange(CardSO card)
    {
        if (!this.isActiveAndEnabled) return;

        CheckMovement(card, card.movement.x, Vector3.right);
        CheckMovement(card, card.movement.y, Vector3.up);
    }

    private void CheckMovement(CardSO card, float axisValue, Vector3 direction)
    {
        if (axisValue == 0) return;

        for (int mov = card.multiplierMin; mov <= card.multiplierMax; mov++)
        {
            Vector3 newPosition = transform.position + direction * (mov * axisValue);
            NodeBase node = GameplayManager.instance.FindTile(newPosition);

            if (node != null && node._isWalkable)
            {
                SetPreviewInstance(newPosition);
            }
        }
    }

    public void DestroyAndClearList()
    {
        foreach (GameObject obj in telePoints)
        {
            Destroy(obj);
        }

        telePoints.Clear();
    }

    void SetPreviewInstance(Vector3 newPosition)
    {
        GameObject g = Instantiate(prefabRange, newPosition, Quaternion.identity);
        SpriteRenderer newSpriteRenderer = g.GetComponent<SpriteRenderer>();
        newSpriteRenderer.sprite = GetComponentInChildren<SpriteRenderer>().sprite;
        Color newColor = newSpriteRenderer.color;
        newColor.a = 100f / 255f;
        newSpriteRenderer.color = newColor;
        telePoints.Add(g);
    }
}