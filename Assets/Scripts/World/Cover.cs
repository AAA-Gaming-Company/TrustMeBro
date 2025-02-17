using UnityEngine;

public class Cover : MonoBehaviour {
    public Transform[] coverPoints;
    private bool[] coverPointsOccupied;

    private void Start() {
        // Initialize the cover points as unoccupied
        coverPointsOccupied = new bool[coverPoints.Length];
        for (int i = 0; i < coverPoints.Length; i++) {
            coverPointsOccupied[i] = false;
        }
    }

    public void EnterNearestCoverPoint(GameObject entity) {
        Vector2 position = entity.transform.position;

        float distance = float.PositiveInfinity;
        int index = -1;

        for (int i = 0; i < coverPoints.Length; i++) {
            float currentDistance = Vector2.Distance(position, coverPoints[i].position);
            if (currentDistance < distance) {
                distance = currentDistance;
                index = i;
            }
        }

        // No cover points found
        if (index == -1) {
            return;
        }
        // Cover point is already occupied
        if (coverPointsOccupied[index]) {
            return;
        }

        coverPointsOccupied[index] = true;
        entity.transform.position = coverPoints[index].position;
    }
}
