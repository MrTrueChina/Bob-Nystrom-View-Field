using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MtC.Tools.FoV;

namespace OriginFov
{
    public class Fov
    {
        public ViewField viewField
        {
            get
            {
                return _viewField;
            }
        }
        ViewField _viewField;
        VisibleMap _visiableMap;

        public Fov(ViewField viewField, VisibleMap visibleMap)
        {
            _viewField = viewField;
            _visiableMap = visibleMap;
        }

        public void Refresh(Vector2 viewerPosition)
        {
            refreshOctant(new Octant(viewerPosition, Vector2.up, Vector2.right));
            refreshOctant(new Octant(viewerPosition, Vector2.right, Vector2.up));
            refreshOctant(new Octant(viewerPosition, Vector2.right, Vector2.down));
            refreshOctant(new Octant(viewerPosition, Vector2.down, Vector2.right));
            refreshOctant(new Octant(viewerPosition, Vector2.down, Vector2.left));
            refreshOctant(new Octant(viewerPosition, Vector2.left, Vector2.down));
            refreshOctant(new Octant(viewerPosition, Vector2.left, Vector2.up));
            refreshOctant(new Octant(viewerPosition, Vector2.up, Vector2.left));

            // 起始位置始终可见。
            _viewField.SetVisible(viewerPosition, true);
        }


        void refreshOctant(Octant octant)
        {
            ShadowLine shadowLine = new ShadowLine();

            //沿着主方向遍历每一行
            for (int mainStep = 1; _viewField.Contains(octant.GetPosition(mainStep, 0)); mainStep++)
                RefreshALine(octant, shadowLine, mainStep);
        }

        void RefreshALine(Octant octant, ShadowLine shadowLine, int mainStep)
        {
            for (int sideStep = 0; sideStep <= mainStep; sideStep++) // 注意这里的条件是[小于等于]，因为侧面方向和主方向要检测同样的长度才是一个完整的八分角，否则会少一条线
            {
                /*
                 *  if(越界了)
                 *      return
                 *  
                 *  if(阴影已经覆盖了整行）
                 */
                //超出地图则结束这一行的判断
                if (!_viewField.Contains(octant.GetPosition(mainStep, sideStep)))
                    return;

                if (shadowLine.IsFullShadow())
                    _viewField.SetVisible(octant.GetPosition(mainStep, sideStep), false); // 如果一整行都在阴影里，就不用计算可见性也不用增加阴影了，直接设为不可见就可以了
                else
                    RefreshAQuadVisibleAndShadow(octant, shadowLine, mainStep, sideStep);
            }
        }

        void RefreshAQuadVisibleAndShadow(Octant octant, ShadowLine shadowLine, int mainStep, int subStep)
        {
            Vector2 currentPosition = octant.GetPosition(mainStep, subStep);
            Shadow projection = ShadowLine.GetQuadProjection(mainStep, subStep); // 获取这个地块的投影

            // 设置这个地块的可见性
            _viewField.SetVisible(currentPosition, IsVisible(shadowLine, projection));

            // 添加阴影
            if (IsBlockView(shadowLine, currentPosition, projection))
                shadowLine.add(projection);
        }

        /// <summary>
        /// 判断一个地块是否可见
        /// </summary>
        /// <param name="shadowLine"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        bool IsVisible(ShadowLine shadowLine, Shadow projection)
        {
            return !shadowLine.AllInShadow(projection); // 如果这个地块的投影完全在阴影线的阴影里，则认为这个地块是不可见的
        }

        /// <summary>
        /// 判断一个地块是否遮挡视线
        /// </summary>
        /// <param name="shadowLine"></param>
        /// <param name="currentPosition"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        bool IsBlockView(ShadowLine shadowLine, Vector2 currentPosition, Shadow projection)
        {
            return !shadowLine.AllInShadow(projection) && !_visiableMap.IsTransparent(currentPosition);
            // 如果这个地块没有完全处于阴影中，并且这个地块不透明，说明会遮挡视线
        }
    }
}
