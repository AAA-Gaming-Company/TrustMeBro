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

    public Vector2 NearestCoverPointPos(Vector2 position, GameObject other = null) {
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
            float closestDistanceToPlayer = Mathf.Infinity;
            Vector2 otherPosition = other.transform.position; 

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
            Debug.Log(closestDistanceToPlayer);
            Debug.Log(Vector2.Distance(coverPoints[index].position, otherPosition));
            if (Vector2.Distance(coverPoints[index].position, otherPosition) == closestDistanceToPlayer) {
                Debug.Log("Cover is the nearest to the player");
                return Vector2.positiveInfinity;
            }
            
        }
        // No cover points found
        if (index == -1) {
            return Vector2.positiveInfinity;
        }

        return coverPoints[index].position;
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
