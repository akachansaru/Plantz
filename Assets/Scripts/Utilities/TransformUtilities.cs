using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Resources.Scripts.Utilities
{
    class TransformUtilities
    {
        /// <summary>
        /// Returns the local scale that the global scale was translated to
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="globalScale"></param>
        /// <returns></returns>
        public static Vector3 SetGlobalScale(Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(
                globalScale.x / transform.lossyScale.x,
                globalScale.y / transform.lossyScale.y,
                globalScale.z / transform.lossyScale.z);
            return transform.localScale;
        }
    }
}
