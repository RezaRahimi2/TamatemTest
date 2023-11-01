using System;
using System.Collections.Generic;
using GamePlayLogic;
using GamePlayView;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct DiceRotation
{
    public int Number;
    public Vector3 Rotation;
}

[Serializable]
public struct BlockWithPlayerColor
{
    [SerializeField]
    public PlayerColorName PlayerColorName;
    [SerializeField]
    public List<Block> BlockHomes;

    public BlockWithPlayerColor(PlayerColorName playerColorName, List<Block> blockHomes)
    {
        PlayerColorName = playerColorName;
        BlockHomes = blockHomes;
    }
}


[Serializable]
public class BlockWithPlayerColorView
{
    [SerializeField]
    public PlayerColorName PlayerColorName;
    [SerializeField] 
    public Transform Parent;
    [SerializeField]
    public List<HomeBlockView> HomeBlockViews;
}

[Serializable]
public class HomeBlockView
{
    [SerializeField]
    public int Number;
    [SerializeField]
    public BlockView BlockView;

    public HomeBlockView(int number, BlockView blockView)
    {
        Number = number;
        BlockView = blockView;
    }
}