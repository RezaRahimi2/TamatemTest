using System.Collections.Generic;
using GamePlayLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlayView
{
    public class BoardView : Singleton<BoardView>
    {
        [field: SerializeField] public List<BlockView> BlockViews { get; private set; }
        [field: SerializeField]public List<BlockWithPlayerColorView> StartHomeWithColorViews;
        public List<BlockWithPlayerColorView> EndHomeWithColors;

        [SerializeField] private Transform blocksParent; // The parent of the blocks in the UI

        public void Initialize(Board board)
        {
            BlockViews = new List<BlockView>();

            for (var i = 0; i < StartHomeWithColorViews.Count; i++)
            {
                var startHomeWithColor = StartHomeWithColorViews[i];
                startHomeWithColor.HomeBlockViews = new List<HomeBlockView>();
                for (var j = 0; j < startHomeWithColor.Parent.childCount; j++)
                {
                    BlockView blockView = startHomeWithColor.Parent.GetChild(j).GetComponent<BlockView>();
                    startHomeWithColor.HomeBlockViews.Add(new HomeBlockView(-1, blockView));
                    blockView.Block.Occupy(null);
                    blockView.Initialize(new Block(-1));
                }
                StartHomeWithColorViews[i] = startHomeWithColor;
            }
            
            for (var i = 0; i < EndHomeWithColors.Count; i++)
            {
                var mEndHomeWithColor = EndHomeWithColors[i];
                mEndHomeWithColor.HomeBlockViews = new List<HomeBlockView>();
                for (var j = 0; j < mEndHomeWithColor.Parent.childCount; j++)
                {
                    int blockIndex = 52 + j;
                    BlockView blockView = mEndHomeWithColor.Parent.GetChild(j).GetComponent<BlockView>();
                    mEndHomeWithColor.HomeBlockViews.Add(new HomeBlockView(blockIndex, blockView));
                    blockView.Initialize(new Block(blockIndex));
                }
                EndHomeWithColors[i] = mEndHomeWithColor;
            }

            // Initialize the BlockViews list with the blocks in the UI
            for (int i = 0; i < blocksParent.childCount; i++)
            {
                Transform child = blocksParent.GetChild(i);
                Block block = board.Blocks[i];
                BlockView blockView = child.GetComponent<BlockView>();
                blockView.Initialize(block);
                BlockViews.Add(blockView);
            }
        }

        public BlockView GetBlockViewByBlock(Block block,int playerIndex,bool endBlock)
        {
            return !endBlock
                ? BlockViews.Find(x => x.Block == block)
                : EndHomeWithColors[playerIndex].HomeBlockViews.Find(x => x.BlockView.Block == block).BlockView;
        }
    }
}