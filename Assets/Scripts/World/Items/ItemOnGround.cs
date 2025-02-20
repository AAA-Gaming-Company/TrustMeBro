using MoreMountains.Feedbacks;
using UnityEngine;

public class ItemOnGround : MonoBehaviour {
    public static float floatStrength = 0.008f;
    public static float floatSpeed = 5f;

    private WeaponType weapon;

    private void FixedUpdate() {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + Mathf.Sin(Time.time * ItemOnGround.floatSpeed) * ItemOnGround.floatStrength, this.transform.position.z);
    }

    public void Setup(WeaponType weapon) {
        this.weapon = weapon;

        //Add a collider to the item
        CircleCollider2D newCollider = this.gameObject.AddComponent<CircleCollider2D>();
        newCollider.isTrigger = true;
        newCollider.radius = 0.5f;

        //Add a sprite renderer to the item
        SpriteRenderer newSpriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
        newSpriteRenderer.sprite = this.weapon.displaySprite;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.PickupItem(this.weapon);

            NotificationBuilder.Builder()
                .WithMessages("You picked up a " + this.weapon.displayName + "!")
                .WithSpeakerImage(this.weapon.displaySprite)
                .WithSpeakerName("Narrator")
                .WithTimeToDisplay(2f)
                .BuildAndDisplay();

            Destroy(this.gameObject);
        }
    }
}
