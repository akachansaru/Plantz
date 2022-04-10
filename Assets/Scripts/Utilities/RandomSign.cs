using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class RandomSign
    {
        public static int Sign()
        {
            return Random.Range(0, 2) * 2 - 1;
        }
    }
}
