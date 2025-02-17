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

    public CoverEntry EnterCover(GameObject entity, int index) {
        if (coverPointsOccupied[index]) {
            return null;
        }

        coverPointsOccupied[index] = true;
        entity.transform.position = coverPoints[index].position;

        return new CoverEntry(this, index);
    }

    public CoverEntry EnterCover(GameObject entity, Vector3 position) {
        int index = -1;
        for (int i = 0; i < coverPoints.Length; i++) {
            if (coverPoints[i].position == position) {
                index = i;
                break;
            }
        }

        if (index == -1) {
            Debug.LogError("Cover point not found!");
            return null;
        }

        return this.EnterCover(entity, index);
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

    public Vector3 NearestCoverPointPos(Vector3 position, GameObject other = null) {
        float distance = float.PositiveInfinity;
        int index = -1;

        if (other == null) {
            for (int i = 0; i < coverPoints.Length; i++) {
                float currentDistance = Vector2.Distance(position, coverPoints[i].position);
                if (currentDistance < distance) {
                    distance = currentDistance;
                    index = i;
                }
            }
        } else {
            float closestDistanceToPlayer = float.PositiveInfinity;
            Vector3 otherPosition = other.transform.position;

            for (int i = 0; i < coverPoints.Length; i++) {
                float currentDistance = Vector2.Distance(position, coverPoints[i].position);
                float distanceToOther = Vector2.Distance(otherPosition, coverPoints[i].position);

                if (currentDistance < distance) {
                    distance = currentDistance;
                    index = i;
                }
                if (distanceToOther < closestDistanceToPlayer) {
                    closestDistanceToPlayer = distanceToOther;
                }   
            }

            if (Vector2.Distance(coverPoints[index].position, otherPosition) == closestDistanceToPlayer) {
                return Vector3.positiveInfinity;
            }
        }

        // No cover points found
        if (index == -1) {
            return Vector3.positiveInfinity;
        }

        return coverPoints[index].position;
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        foreach (Transform coverPoint in coverPoints) {
            Gizmos.DrawCube(coverPoint.position, new Vector3(0.1f, 0.1f, 0.1f));
        }
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
