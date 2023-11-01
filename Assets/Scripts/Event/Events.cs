using System.Collections.Generic;
using GamePlayLogic;
using GamePlayView;
using UnityEngine;
using UnityEngine.Serialization;

namespace Events
{
    public struct OnDiceAnimationStartedEvent
    {
        // This event doesn't need to pass any data
    }

    public struct OnDiceAnimationFinishedEvent
    {
        // This event doesn't need to pass any data
    }
    
    public struct OnChipAnimationStartedEvent
    {
        // This event doesn't need to pass any data
    }

    public struct OnChipAnimationFinishedEvent
    {
        // This event doesn't need to pass any data
    }

    public struct OnUIChangingStartedEvent
    {
        // This event doesn't need to pass any data
    }

    public struct OnUIChangingEndEvent
    {
        // This event doesn't need to pass any data
    }
    
    public struct OnWaitingForInputStartedEvent
    {
        // This event doesn't need to pass any data
    }

    public struct OnWaitingForInputFinishedEvent
    {
        // This event doesn't need to pass any data
    }
    
    public struct OnChipMovedEvent
    {
        public Chip Chip;
        public int TotalDicesRoll;
        public bool MoveToStartHome;
        public bool MoveToEndHome;
        public bool HitToOtherChip;

        public OnChipMovedEvent(Chip chip, int totalDicesRoll, bool moveToStartHome, bool moveToEndHome, bool hitToOtherChip)
        {
            Chip = chip;
            TotalDicesRoll = totalDicesRoll;
            MoveToStartHome = moveToStartHome;
            MoveToEndHome = moveToEndHome;
            HitToOtherChip = hitToOtherChip;
        }
    }

    public struct OnStartDiceRollEvent
    {
    }
    
    
    public struct OnDiceRollEvent
    {
        public int PlayerIndex;

        public OnDiceRollEvent(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
    
    public struct OnDiceRolledEvent
    {
        public int PlayerIndex;
        public int Dice1LastRoll;
        public int Dice2LastRoll;

        public OnDiceRolledEvent(int playerIndex,int dice1LastRoll, int dice2LastRoll)
        {
            Dice1LastRoll = dice1LastRoll;
            Dice2LastRoll = dice2LastRoll;
            PlayerIndex = playerIndex;
        }
    }

    public struct OnPlayerSetPlayableChipsEvent
    {
        public int PlayerIndex;
        public bool ToPlay;

        public OnPlayerSetPlayableChipsEvent(int playerIndex,bool toPlay)
        {
            PlayerIndex = playerIndex;
            ToPlay = toPlay;
        }
    }

    public struct OnStartGameEvent
    {
        public Board Board;
        public List<PlayerBase> Players;

        public OnStartGameEvent(ref Board board,ref List<PlayerBase> players)
        {
            Board = board;
            Players = players;
        }
    }

    public struct OnSetPlayerTurnEvent
    {
        public int PlayerIndex;

        public OnSetPlayerTurnEvent(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
    
    public struct OnWinnerEvent
    {
        public int PlayerIndex;
    }
    
    public struct OnChipTapEvent
    {
        public bool MoveToStartHome;
        public int StartHomeIndex;
        public GameObject ChipGameObject;
    }
   
    public struct OnHitChipEvent
    {
        public int PlayerIndex;
        public int HitChipIndex;

        public OnHitChipEvent(int playerIndex, int hitChipIndex)
        {
            PlayerIndex = playerIndex;
            HitChipIndex = hitChipIndex;
        }
    }
    
    public struct OnBlockChipEvent
    {
        public Chip Chip;
        public Chip BlockChip;
    }
}