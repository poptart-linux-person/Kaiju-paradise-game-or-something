using UnityEngine;
using KaijuGame.World;

namespace KaijuGame.Items
{
    public sealed class KeycardItem : PhysicalItem
    {
        [SerializeField] private KeycardLevel accessLevel = KeycardLevel.Blue;

        public KeycardLevel AccessLevel => accessLevel;

        public override bool Use(GameObject user)
        {
            if (user == null) return false;
            var inventory = user.GetComponentInParent<KeycardInventory>();
            if (inventory == null) return false;
            inventory.Add(accessLevel);
            return true;
        }
    }
}
