using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlayLogic
{
    // This class represents the game board
    [Serializable]
    public class Board
    {
        // List of blocks on the board
        [field:SerializeField] public List<Block> Blocks { get; set; }

        // List of blocks with player colors
        [SerializeField] public List<BlockWithPlayerColor> EndHomeWithColors;

        // Constructor for the Board class
        public Board()
        {
            // Initialize the list of blocks
            Blocks = new List<Block>();

            // Add blocks to the list based on the block number from the game model
            for (int i = 0; i < GameModel.Instance.BlockNumber; i++)
            {
                Blocks.Add(new Block(i));
            }
        
            // Initialize the list of blocks with player colors
            EndHomeWithColors = new List<BlockWithPlayerColor>();

            // Add blocks with player colors to the list based on the player number from the game model
            for (int i = 0; i < GameModel.Instance.PlayerNumber; i++)
            {
                BlockWithPlayerColor endHomeWithColor = new BlockWithPlayerColor();
            
                // Set the player color name
                endHomeWithColor.PlayerColorName = (PlayerColorName)i;

                // Initialize the list of block homes
                endHomeWithColor.BlockHomes = new List<Block>();
            
                // Add block homes to the list based on the home number from the game model
                for (int j = 0; j < GameModel.Instance.HomeNumber; j++)
                {
                    endHomeWithColor.BlockHomes.Add(new Block(GameModel.Instance.BlockNumber + j));
                }
            
                // Add the block with player color to the list
                EndHomeWithColors.Add(endHomeWithColor);
            }
        }
    }

}