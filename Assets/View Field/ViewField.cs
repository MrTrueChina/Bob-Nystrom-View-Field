using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MtC.Tools.FoV
{
    /// <summary>
    /// 视野地图，储存地图上各个位置是否可见的地图
    /// </summary>
    public class ViewField : BoolMap
    {
        public ViewField(int width, int height) : base(width, height)
        {
            Fill(true);
        }

        //public bool[,] GetBoolViewField()
        //{
        //    return _quads;
        //}

        public bool IsVisible(Vector2 position)
        {
            return IsVisible((int)position.x, (int)position.y);
        }

        public bool IsVisible(int x, int y)
        {
            return _quads[x, y];
        }

        public void SetVisible(Vector2 position, bool visible)
        {
            SetVisible((int)position.x, (int)position.y, visible);
        }

        public void SetVisible(int x, int y, bool visible)
        {
            Set(x, y, visible);
        }
    }
}
