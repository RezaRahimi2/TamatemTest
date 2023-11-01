using GamePlayLogic;
using UnityEngine;

namespace GamePlayView
{
    public class BlockView:MonoBehaviour
    {
        [field: SerializeField]public Block Block { get; private set; }

        public void Initialize(Block block)
        {
            Block = block;
        }
    }
}