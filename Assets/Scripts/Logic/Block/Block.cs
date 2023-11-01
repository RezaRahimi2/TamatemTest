using System;
using UnityEngine;

namespace GamePlayLogic
{
    [Serializable]
    public class Block
    {
        [field: SerializeField]public int Index { get;private set; } // The index of the block on the board
        [field: SerializeField]public bool IsOccupied { get; private set; }
        [field: SerializeField]public bool IsBlocked { get;private set; } // New property
        [field: SerializeField]public Chip OccupyingChip { get;private set; }

        public Block(int index)
        {
            Index = index;
            IsOccupied = false;
            IsBlocked = false; // Initialize the new property
            OccupyingChip = null;
        }

        public void Occupy(Chip chip)
        {
            OccupyingChip = chip;
            IsOccupied = true;
        }

        public void UnOccupy()
        {
            OccupyingChip = null;
            IsOccupied = false;
        }

        public void Blocking()
        {
            IsBlocked = true;
        }
        
        public void UnBlocking()
        {
            IsBlocked = false;
        }
    }
}