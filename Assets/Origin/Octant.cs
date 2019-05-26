using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriginFov
{
    /// <summary>
    /// 八分圆，用于辅助视野计算时的位置计算
    /// </summary>
    class Octant
    {
        public Vector2 center
        {
            get { return _center; }
        }
        Vector2 _center;
        Vector2 _forwardDirection;
        Vector2 _sideDirectin;

        public Octant(Vector2 center, Vector2 forwardDirection, Vector2 sideDirection)
        {
            _center = center;
            _forwardDirection = forwardDirection;
            _sideDirectin = sideDirection;
        }

        public Vector2 GetPosition(int forwardDirection, int sideDistance)
        {
            return center + _forwardDirection * forwardDirection + _sideDirectin * sideDistance;
        }
    }
}
