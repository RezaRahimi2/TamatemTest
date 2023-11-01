using System;
using Cysharp.Threading.Tasks;
using Events;
using UnityEngine;

namespace GamePlayLogic
{
    [Serializable]
    public class Dice
    {
        [SerializeField]
        public byte LastRoll { get; private set; }

        private RandomNumberService randomNumberService;

        public Dice()
        {
            randomNumberService = new RandomNumberService();
        }

        public async UniTask<byte> RollDice()
        {
            LastRoll = await randomNumberService.GetRandomNumber();
            return LastRoll;
        }
    }

}