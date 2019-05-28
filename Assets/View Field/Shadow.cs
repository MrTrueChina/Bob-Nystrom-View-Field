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

        public override string ToString()
        {
            return "(" + _start + "," + _end + ")";
        }
    }
}
