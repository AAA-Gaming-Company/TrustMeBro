using System.Collections.Generic;
using UnityEngine;

//I didn't like the default Unity stuff, so I made my own
public static class BetterPhysics2D {
    public static bool Linecast(PlayerController player, LinecastDirection direction, int layerMask, float distance) {
        Vector2[] startEnd = GetStartEnd(player, direction, distance);
        RaycastHit2D[] hits = Physics2D.LinecastAll(startEnd[0], startEnd[1], layerMask);
        return System.Array.FindAll(hits, x => x.collider.gameObject != player).Length > 0;
    }

    public static bool Linecast(PlayerController player, LinecastDirection direction, float distance) {
        Vector2[] startEnd = GetStartEnd(player, direction, distance);
        RaycastHit2D[] hits = Physics2D.LinecastAll(startEnd[0], startEnd[1]);
        return System.Array.FindAll(hits, x => x.collider.gameObject != player).Length > 0;
    }

    public static GameObject[] LinecastGameObject(PlayerController player, LinecastDirection direction, int layerMask, float distance) {
        Vector2[] startEnd = GetStartEnd(player, direction, distance);
        RaycastHit2D[] hits = Physics2D.LinecastAll(startEnd[0], startEnd[1], layerMask);
        return System.Array.ConvertAll(System.Array.FindAll(hits, x => x.collider.gameObject != player), x => x.collider.gameObject);
    }

    private static Vector2[] GetStartEnd(PlayerController player, LinecastDirection direction, float distance) {
        Vector2 start = Vector2.zero;
        Vector2 end = Vector2.zero;

        Collider2D collider2d = player.GetCollider();

        //Calculate what the relative forwards and backwards mean for this player
        if (direction == LinecastDirection.FORWARDS) {
            if (player.IsPlayerFlipped()) {
                direction = LinecastDirection.LEFT;
            } else {
                direction = LinecastDirection.RIGHT;
            }
        } else if (direction == LinecastDirection.BACKWARDS) {
            if (player.IsPlayerFlipped()) {
                direction = LinecastDirection.RIGHT;
            } else {
                direction = LinecastDirection.LEFT;
            }
        }

        switch (direction) {
            case LinecastDirection.UP:
                start = new Vector2(collider2d.bounds.min.x + 0.1f, collider2d.bounds.max.y);
                end = new Vector2(collider2d.bounds.max.x - 0.1f, collider2d.bounds.max.y + distance);
                break;
            case LinecastDirection.DOWN:
                start = new Vector2(collider2d.bounds.min.x + 0.1f, collider2d.bounds.min.y);
                end = new Vector2(collider2d.bounds.max.x - 0.1f, collider2d.bounds.min.y - distance);
                break;
            case LinecastDirection.LEFT:
                start = new Vector2(collider2d.bounds.min.x, collider2d.bounds.min.y + 0.1f);
                end = new Vector2(collider2d.bounds.min.x - distance, collider2d.bounds.max.y - 0.1f);
                break;
            case LinecastDirection.RIGHT:
                start = new Vector2(collider2d.bounds.max.x, collider2d.bounds.min.y + 0.1f);
                end = new Vector2(collider2d.bounds.max.x + distance, collider2d.bounds.max.y - 0.1f);
                break;
        }

        return new Vector2[] { start, end };
    }

    public static Collider2D[] OverlapConeAll(Vector2 point, Vector2 direction, float radius, float angle, int layerMask) {
        Collider2D[] hits = Physics2D.OverlapCircleAll(point, radius, layerMask);
        List<Collider2D> inCone = new List<Collider2D>();

        direction.Normalize();
        float halfAngle = angle / 2;

        foreach (Collider2D hit in hits) {
            Vector2 pointToCollider = ((Vector2) hit.transform.position - point).normalized;
            float dot = Vector2.Dot(pointToCollider, direction);
            if (dot >= Mathf.Cos(halfAngle)) {
                inCone.Add(hit);
            }
        }

        return inCone.ToArray();
    }
}

public enum LinecastDirection {
    UP,
    DOWN,
    LEFT,
    RIGHT,
    FORWARDS,
    BACKWARDS
}
