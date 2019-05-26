using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MtC.Tools.FoV
{
    public class Shadow
    {
        public float start
        {
            get { return _start; }
        }
        float _start;
        public float end
        {
            get { return _end; }
        }
        float _end;

        public Shadow(float start, float end)
        {
            _start = start;
            _end = end;
        }
    }
}
