using UnityEngine;

namespace MtC.Tools.FoV
{
    /// <summary>
    /// 二维bool数组封装类，提供Vector2的访问接口
    /// </summary>
    public abstract class BoolMap
    {
        protected bool[,] _quads;

        public int width
        {
            get
            {
                return _quads.GetLength(0);
            }
        }

        public int height
        {
            get
            {
                return _quads.GetLength(1);
            }
        }

        public BoolMap(int width, int height)
        {
            _quads = new bool[width, height];
        }

        public bool[,] GetBoolArray()
        {
            return _quads;
        }

        protected bool Get(int x, int y)
        {
            return _quads[x, y];
        }

        protected void Set(int x, int y, bool value)
        {
            _quads[x, y] = value;
        }

        public void Fill(bool value)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _quads[x, y] = value;
        }

        /// <summary>
        /// 判断一个点在不在二维数组的范围里
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Contains(Vector2 position)
        {
            return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
        }
    }
}
