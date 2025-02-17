using UnityEngine;

public class Cover : MonoBehaviour
{
    private Transform coverPoint;

    void Start() 
    {
        coverPoint = transform.Find("Cover Point").transform;
    }

    public Vector2 GetCoverPosition() 
    {
        return coverPoint.position;
    }
}
