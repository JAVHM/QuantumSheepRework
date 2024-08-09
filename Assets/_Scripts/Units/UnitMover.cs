using Nodes.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    public IEnumerator MoveAlongPath(List<NodeBase> path, float speed)
    {
        foreach (var node in path)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = node.transform.position;
            float journey = 0f;

            while (journey < 1f)
            {
                journey += Time.deltaTime * speed;
                transform.position = Vector3.Lerp(startPosition, endPosition, journey);
                yield return null;
            }
        }
    }
}
