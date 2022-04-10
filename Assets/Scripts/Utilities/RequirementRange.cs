using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class RequirementRange
    {
        //public float Low { get; set; }
        //public float High { get; set; }

        public float Ave { get; private set; }
        public float Low { get; set; }
        public float High { get; set; }

        //public float Spread { get; set; } // change this to private

        public RequirementRange(float low, float high, float variancePercent)
        {
            Low = low + Random.Range(-low * variancePercent, low * variancePercent);
            High = high + Random.Range(-high * variancePercent, high * variancePercent);
            Ave = (Low + High) / 2;
        }

        //public RequirementRange(float ave, float spread, float variancePercent)
        //{
        //    Ave = ave + UnityEngine.Random.Range(-ave * variancePercent, ave * variancePercent);
        //    Spread = spread + UnityEngine.Random.Range(-spread * variancePercent, spread * variancePercent);
        //    Low = Ave - Spread;
        //    High = Ave + Spread;
        //}
    }
}