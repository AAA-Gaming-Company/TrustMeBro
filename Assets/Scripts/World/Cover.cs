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

        Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        Vector3 coverPosition = coverPoints[index].position;
        // Make sure that the entity is at the correct height, its feet should be at the cover point's height
        coverPosition.y = coverPosition.y + entity.GetComponent<Collider2D>().bounds.extents.y;
        // Make sure that the entity is at the correct Z position
        coverPosition.z = entity.transform.position.z;
        entity.transform.position = coverPosition;

        return new CoverEntry(this, coverPoints[index].position, index, rb != null);
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

    public void ExitCover(GameObject entity, CoverEntry entry) {
        coverPointsOccupied[entry.index] = false;

        Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.AddForce(new Vector2(0f, 0.01f), ForceMode2D.Impulse);
        }
        if (entry.rigidbody && rb == null) {
            throw new System.Exception("Rigidbody2D is null when a gravity scale was set!");
        }
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
        Gizmos.color = Color.red;
        foreach (Transform coverPoint in coverPoints) {
            Gizmos.DrawCube(coverPoint.position, new Vector3(0.1f, 0.1f, 0.1f));
        }
    }
}

public class CoverEntry {
    public Cover cover;
    public Vector3 coverPoint;
    public int index;
    public bool rigidbody;

    public CoverEntry(Cover cover, Vector3 coverPoint, int index, bool rigidbody) {
        this.cover = cover;
        this.coverPoint = coverPoint;
        this.index = index;
        this.rigidbody = rigidbody;
    }

    public void ExitCover(GameObject entity) {
        cover.ExitCover(entity, this);
    }
}
