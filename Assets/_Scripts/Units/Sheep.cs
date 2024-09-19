using Nodes.Tiles;
using System.Collections;
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
        if (this.isActiveAndEnabled)
        {
            for (int mov = card.multiplierMin; mov <= card.multiplierMax; mov++)
            {
                if (card.movement.x != 0)
                {
                    Vector3 newPosition = transform.position + new Vector3(mov * card.movement.x, 0, 0f);
                    NodeBase node = GameplayManager.instance.FindTile(newPosition);
                    if (node != null && node._isWalkable == true)
                    {
                        GameObject g = Instantiate(prefabRange, newPosition, Quaternion.identity);
                        telePoints.Add(g);
                    }
                }

            }
            for (int mov = card.multiplierMin; mov <= card.multiplierMax; mov++)
            {
                if (card.movement.y != 0)
                {
                    Vector3 newPosition = transform.position + new Vector3(0, mov * card.movement.y, 0f);
                    NodeBase node = GameplayManager.instance.FindTile(newPosition);
                    if (node != null && node._isWalkable == true)
                    {
                        GameObject g = Instantiate(prefabRange, newPosition, Quaternion.identity);
                        telePoints.Add(g);
                    }
                }
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
}