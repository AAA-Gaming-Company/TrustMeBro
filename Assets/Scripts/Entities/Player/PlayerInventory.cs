using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class PlayerInventory {
    public List<ItemStack> items = new List<ItemStack>();
    public int currentSelectedWeapon = -1;

    private UnityEvent inventoryChangedEvent = new UnityEvent();

    public void RegisterInventoryChangedEvent(UnityAction action) {
        if (this.inventoryChangedEvent == null) {
            this.inventoryChangedEvent = new UnityEvent();
        }

        this.inventoryChangedEvent.AddListener(action);
    }

    public void AddItem(WeaponType weaponType, int amount = 1) {
        if (this.items.Count == 0) {
            this.currentSelectedWeapon = 0;
        }

        ItemStack item = this.items.Find(i => i.weaponType == weaponType);
        if (item == null) {
            this.items.Add(new ItemStack { weaponType = weaponType, amount = amount });
        } else {
            item.amount += amount;
        }

        this.inventoryChangedEvent.Invoke();
    }

    public bool RemoveItem(WeaponType weaponType, int amount) {
        ItemStack item = this.items.Find(i => i.weaponType == weaponType);
        if (item == null) {
            return false;
        }

        if (item.amount < amount) {
            return false;
        }

        item.amount -= amount;
        if (item.amount <= 0) { //Shouldn't be less than 0, but just in case
            this.items.Remove(item);
            this.currentSelectedWeapon -= 1;
        }

        this.inventoryChangedEvent.Invoke();

        return true;
    }

    public bool HasItem(WeaponType weaponType, int amount = 1) {
        ItemStack item = this.items.Find(i => i.weaponType == weaponType);
        if (item == null) {
            return false;
        }

        return item.amount >= amount;
    }

    public WeaponType GetSelectedWeapon() {
        if (this.items.Count == 0 || this.currentSelectedWeapon < 0 || this.currentSelectedWeapon >= this.items.Count) {
            return null;
        }

        return this.items[this.currentSelectedWeapon].weaponType;
    }

    public bool CycleSelectedWeapon(bool forward) {
        if (this.items.Count == 0) {
            this.currentSelectedWeapon = -1;
            return false;
        }

        if (forward) {
            this.currentSelectedWeapon += 1;
            if (this.currentSelectedWeapon >= this.items.Count) {
                this.currentSelectedWeapon = 0;
            }
        } else {
            this.currentSelectedWeapon -= 1;
            if (this.currentSelectedWeapon < 0) {
                this.currentSelectedWeapon = this.items.Count - 1;
            }
        }

        return true;
    }
}

[System.Serializable]
public class ItemStack {
    public WeaponType weaponType;
    public int amount;
}
