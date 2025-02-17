using UnityEngine;

public class Cover : MonoBehaviour
{
    public Transform[] coverPoints;

    public Vector2 GetNearestCoverPosition(Vector2 position) 
    {
        float distance = Mathf.Infinity;
        Vector2 coverPoint = Vector2.zero;

        foreach (Transform point in coverPoints) {
            float tempDistance = Vector2.Distance(position, point.position);
            if (tempDistance < distance) {
                distance = tempDistance;
                coverPoint = point.position;
            }
        }
        return coverPoint;
    }
}
