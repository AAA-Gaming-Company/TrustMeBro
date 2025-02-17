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

    public CoverEntry EnterNearestCoverPoint(GameObject entity) {
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
            return null;
        }
        // Cover point is already occupied
        if (coverPointsOccupied[index]) {
            return null;
        }

        coverPointsOccupied[index] = true;
        entity.transform.position = coverPoints[index].position;

        return new CoverEntry(this, index);
    }

    public void ExitCover(int index) {
        coverPointsOccupied[index] = false;
    }

    public bool IsCovering() {
        for (int i = 0; i < coverPoints.Length; i++) {
            if (coverPointsOccupied[i]) {
                return true;
            }
        }

        return false;
    }
}

public class CoverEntry {
    public Cover cover;
    public int index;

    public CoverEntry(Cover cover, int index) {
        this.cover = cover;
        this.index = index;
    }

    public void ExitCover() {
        cover.ExitCover(index);
    }
}
