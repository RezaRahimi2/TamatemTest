using UnityEngine;

namespace Tools
{
    public class BoardBlockGenerator:MonoBehaviour
    {
        [SerializeField] public GameObject ChipPrefab;
        [SerializeField] public BoxCollider BlockCollider;
        [SerializeField] public float GapBetweenBlocks = 0.001f;
        [SerializeField] public Transform BlocksParent;

    }
}