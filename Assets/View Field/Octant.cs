using UnityEngine;

namespace MtC.Tools.FoV
{
    class Octant
    {
        public Vector2 center
        {
            get { return _center; }
        }
        Vector2 _center;
        public Vector2 forwwardDirection
        {
            get { return _forwardDirection; }
        }
        Vector2 _forwardDirection;
        public Vector2 sideDirection
        {
            get { return _sideDirectin; }
        }
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
