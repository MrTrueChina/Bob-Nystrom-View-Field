using UnityEngine;

namespace MtC.Tools.FoV
{
    /// <summary>
    /// 可视性地图，储存地图上各个位置是否有遮挡视线的物体的地图
    /// </summary>
    public class VisibleMap : BoolMap
    {
        // 在二维bool数组里，true代表这个地块不遮挡视线，false代表遮挡视线
        public VisibleMap(int width, int height) : base(width, height)
        {
            Fill(true);
        }

        public void SetTransparent(int x, int y, bool transparent)
        {
            _quads[x, y] = transparent;
        }

        public bool IsTransparent(Vector2 position)
        {
            return IsTransparent((int)position.x, (int)position.y);
        }

        public bool IsTransparent(int x, int y)
        {
            return _quads[x, y];
        }
    }
}