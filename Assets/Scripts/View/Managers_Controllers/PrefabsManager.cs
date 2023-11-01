using Cysharp.Threading.Tasks;
using GamePlayView;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlayView
{
    public class PrefabsManager : Singleton<PrefabsManager>
    {
        [Header("Game")]
        public PlayerView PlayerView;
        public ChipView ChipViewPrefab;

        public void Initialize(ChipView chipViewPrefab)
        {
            ChipViewPrefab = chipViewPrefab;
        }

    }

}