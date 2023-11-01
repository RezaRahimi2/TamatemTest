using System;
using System.Collections.Generic;
using DG.Tweening;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePlayView
{
    public class DiceView : MonoBehaviour
    {
        [SerializeField] public List<DiceRotation> diceRotations;

        public void RollDice(int lastRoll,int diceIndex,Action<int> finishCallback)
        {
            // Find the DiceRotation that corresponds to the rolled number
            DiceRotation diceRotation = diceRotations.Find(dr => dr.Number == lastRoll);

            if (diceRotations != null)
            {
                // Throw the dice up to y = 2 with a random rotation
                transform.DOLocalRotate(
                    new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), .5f,
                    RotateMode.FastBeyond360);
                transform.DOLocalMoveY(.2f, .5f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                        // When the first two tweens are complete, start the next set of tweens
                        transform.DOLocalMoveY(.02f, .5f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            // Bounce the dice up a little bit when it hits the ground
                            transform.DOLocalMoveY(.03f, .2f).SetEase(Ease.OutQuad).OnComplete(() =>
                            {
                                transform.DOLocalMoveY(.02f, .2f).SetEase(Ease.InQuad).OnComplete(() =>
                                {
                                    // When the dice hits the ground, start the next set of tweens
                                });
                            });
                        });
                        transform.DOLocalRotate(
                            new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), .5f,
                            RotateMode.FastBeyond360).OnComplete(() =>
                        {
                            transform.DOLocalRotate(diceRotation.Rotation, .3f).SetEase(Ease.OutBounce).OnComplete(() =>
                            {
                                // Start the sequence
                                finishCallback?.Invoke(diceIndex);
                            });
                        });
                });


            }
        }
    }
}