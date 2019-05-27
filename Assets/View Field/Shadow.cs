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
            set { _start = value; }
        }
        float _start;
        public float end
        {
            get { return _end; }
            set { _end = value; }
        }
        float _end;

        public Shadow(float start, float end)
        {
            _start = start;
            _end = end;
        }

        public bool Contions(Shadow shadow)
        {
            return _start <= shadow._start && _end >= shadow._end;
        }
    }
}
