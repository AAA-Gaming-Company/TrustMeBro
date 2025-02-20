using MoreMountains.Feedbacks;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {
    public WeaponType itemToSpawn;
    public MMF_Player pickupFeedback;

    private void Start() {
        // Create an empty game object to hold the item
        GameObject item = new GameObject();
        item.transform.position = this.transform.position;
        item.name = this.itemToSpawn.displayName + " World Item";

        ItemOnGround itemOnGround = item.AddComponent<ItemOnGround>();
        itemOnGround.Setup(this.itemToSpawn, this.pickupFeedback);

        Destroy(this.gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, 0.2f);
    }
}
