using System;
using Events;
using UnityEngine;

namespace GamePlayLogic
{
    [Serializable]
    public class Chip
    {
        [field: SerializeField] public Block CurrentBlock { get; set; }

        [field: SerializeField] public int LastPosition { get; set; }

        [field: SerializeField] public int Position { get; set; } = -1;
        [field: SerializeField] public int MoveIndex { get; set; } = -1;

        [field: SerializeField] public bool IsAtHome { get; set; }

        [field: SerializeField]
        public bool HasReachedEnd{ get; private set; } // Assuming 51 is the end position in Ludo

        [field: SerializeField] public int Index { get; set; } // New property
        [field: SerializeField] public int PlayerIndex { get; set; } // New property
        [field: SerializeField] public bool IsPlayable  { get; set; } // New property
        
        public Chip(int index, int playerIndex)
        {
            IsAtHome = true;
            Position = -1;
            Index = index;
            PlayerIndex = playerIndex;
        }
        
        public void MoveFromHome(int startBlockIndex)
        {
            MoveIndex = 0;
            // Move the chip
            Position = startBlockIndex;
            LastPosition = startBlockIndex;
            CurrentBlock = GameController.Instance.Board.Blocks[Position];
            CurrentBlock.Occupy(this);
            IsAtHome = false;
            Debug.Log("<color=green>Chip moved from home</color>");
            EventManager.Broadcast(new OnChipMovedEvent() { Chip = this });
        }

        public void MoveToHome()
        {
            // Move the chip
            Position = -1;
            MoveIndex = -1;
            LastPosition = 0;
            CurrentBlock = null;
            IsAtHome = true;
            Debug.Log("<color=green>Chip moved to home</color>");
            //EventManager.Broadcast(new OnChipMovedEvent() { Chip = this, MoveToStartHome = true });
        }

        public void Move(int spaces,bool hitToOtherChip)
        {
            //if (CanMove(startBlockIndex,spaces))
            {
                bool moveToEnd = false;
                int blockNumber = GameModel.Instance.BlockNumber - 1;
                LastPosition = Position;

                if (CurrentBlock != null)
                {
                    CurrentBlock.UnOccupy();
                }

                MoveIndex =  Mathf.Clamp( MoveIndex + spaces, 0, blockNumber);;
                // Move the chip
                Position += spaces;
                int index = Position >= blockNumber - 1
                    ? (Position % blockNumber)
                    : Position;

                if (MoveIndex >= blockNumber)
                {
                    moveToEnd = true;
                    
                    int playerIndex = -1;
#if Debugging
                    if (GameModel.Instance.PlayerNumber == 1 && GameModel.Instance.UsePlayerIndexForPosition)
                    {
                        playerIndex = 0;
                    }
                    else
                    {
                        playerIndex = PlayerIndex;
                    }
#else
               playerIndex = PlayerIndex;
#endif
                    
                    if (index > GameController.Instance.Board.EndHomeWithColors[playerIndex].BlockHomes.Count - 1)
                    {
                        index = GameController.Instance.Board.EndHomeWithColors[playerIndex].BlockHomes.Count - 1;
                    }

                    HasReachedEnd = true;
                    CurrentBlock = GameController.Instance.Board.EndHomeWithColors[playerIndex].BlockHomes[index];
                    GameController.Instance.Board.EndHomeWithColors[playerIndex].BlockHomes.RemoveAt(index);
                }
                else
                    CurrentBlock = GameController.Instance.Board.Blocks[index];

                CurrentBlock.Occupy(this);
                Debug.Log("<color=yellow>Chip moved from " + LastPosition + " to " + Position + "</color>");
                EventManager.Broadcast(new OnChipMovedEvent() { Chip = this, MoveToStartHome = false,
                    MoveToEndHome = moveToEnd,HitToOtherChip = hitToOtherChip , TotalDicesRoll = spaces});
            }
        }
    }
}