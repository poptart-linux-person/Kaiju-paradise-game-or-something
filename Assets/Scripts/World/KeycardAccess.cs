using System.Collections.Generic;
using UnityEngine;

namespace KaijuGame.World
{
    public enum KeycardLevel
    {
        None = 0,
        Blue = 1,
        Yellow = 2,
        Red = 3,
        Black = 4
    }

    public sealed class KeycardInventory : MonoBehaviour
    {
        private readonly HashSet<KeycardLevel> cards = new();

        public bool Has(KeycardLevel level)
        {
            if (level == KeycardLevel.None) return true;
            foreach (var card in cards)
                if ((int)card >= (int)level) return true;
            return false;
        }

        public void Add(KeycardLevel level)
        {
            if (level != KeycardLevel.None)
                cards.Add(level);
        }

        public void Clear() => cards.Clear();
    }
}
