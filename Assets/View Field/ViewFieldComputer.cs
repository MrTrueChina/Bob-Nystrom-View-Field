using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MtC.Tools.FoV
{
    public class ViewFieldComputer
    {
        static VisibleMap _visibleMap;
        static Vector2 _viewerPosition;
        static ViewField _viewField;

        /// <summary>
        /// 根据传入的可视地图和位置计算视野
        /// </summary>
        /// <param name="map"></param>
        /// <param name="viewerPosition"></param>
        /// <returns></returns>
        public static ViewField Compute(VisibleMap visibleMap, Vector2 viewerPosition)
        {
            try
            {
                SetupComputer(visibleMap, viewerPosition);
                DoCompute();
                return _viewField;
            }
            finally
            {
                ClearComputer();
            }
        }

        static void SetupComputer(VisibleMap visibleMap, Vector2 position)
        {
            _visibleMap = visibleMap;
            _viewField = GetOriginViewField();
            _viewerPosition = position;
        }

        static ViewField GetOriginViewField()
        {
            ViewField viewField = new ViewField(_visibleMap.width, _visibleMap.height);

            viewField.Fill(true); // 这个算法的关键就在于默认视野是全部可见的，然后在视野上绘制出不可见的部分

            return viewField;
        }

        static void ClearComputer()
        {
            _visibleMap = null;
            _viewField = null;
            _viewerPosition = new Vector2(); // Vector2 不能为 null，但保留引用有可能导致误操作，用一个新的顶上去
        }

        static void DoCompute()
        {
            /*
             *  遍历八个八分圆
             *  {
             *      计算这个八分圆上的视野
             *      把这个八分圆上的视野合并到总视野中
             *  }
             */
            ComputeAndMergeAnOctant(Vector2.up, Vector2.right);
            ComputeAndMergeAnOctant(Vector2.right, Vector2.up);
            ComputeAndMergeAnOctant(Vector2.right, Vector2.down);
            ComputeAndMergeAnOctant(Vector2.down, Vector2.right);
            ComputeAndMergeAnOctant(Vector2.down, Vector2.left);
            ComputeAndMergeAnOctant(Vector2.left, Vector2.down);
            ComputeAndMergeAnOctant(Vector2.left, Vector2.up);
            ComputeAndMergeAnOctant(Vector2.up, Vector2.left);
        }

        static void ComputeAndMergeAnOctant(Vector2 mainDirection, Vector2 rowDirection)
        {
            /*
             *  沿主方向直到边界
             *      计算每一行的视野
             */
            for (Vector2 currentLineStart = mainDirection; _viewField.Contains(currentLineStart); currentLineStart += rowDirection)
                ComputeAndMergeALine(currentLineStart, rowDirection);
        }

        static void ComputeAndMergeALine(Vector2 startPosition, Vector2 rowDirection)
        {
            //TODO：计算一个横线的视野并合并到总视野中
        }
    }
}
