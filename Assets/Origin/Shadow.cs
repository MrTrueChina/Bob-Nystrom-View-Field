using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 这个算法阴影的精妙之处在于对八分角和方格地图的组合应用
 * 
 * 以上偏右的八分角为例，这个八分角部分的地图是一个倒三角形，像这样：
 * 口口口口口
 * 口口口口
 * 口口口
 * 口口
 * 口
 * 
 * 在这个算法里，观察者位于最底下的位置，向上和向右是正方向，视野逐层向上计算，阴影则以横向记录被遮挡的部分。
 * 
 * 如何记录一个横向的区域？当做线就可以。如何记录一条线？只要记录起点和终点就可以。
 * 
 * 但如何才能记录起点和终点？
 * 
 * 视野是一个圆，向外的方向都是正方向。八分角是视野的八分之一，从最底下的位置到顶部的任何一个位置都是正方向。
 * 阴影要随着视野的计算逐层向上移动辅助计算，但每一行的长度不同，阴影不能直接记录起点终点的位置，否则就会在向上移动时出错。
 * 
 * 有没有一种可以记录起点终点位置的、不论在哪一行都相同的东西呢？
 * 
 * 答案当然是有的。
 * 
 * 斜率，斜率不会随着行的变化而变化
 * 
 * 当然这个斜率和数学的斜率不一样，它是从上方向到右方向的斜率，范围则是向上为0，向右上为1
 */

namespace OriginFov
{
    /// <summary>
    /// 阴影，即视线中被遮挡的部分
    /// </summary>
    class Shadow
    {
        public float startGradient
        {
            get { return _startGradient; }
            set { _startGradient = value; }
        }
        float _startGradient;
        public float endGradient
        {
            get { return _endGradient; }
            set { _endGradient = value; }
        }
        float _endGradient;

        Vector2 _startPos;
        Vector2 _endPos;

        public Shadow(float startGradient, float endGradient)
        {
            _startGradient = startGradient;
            _endGradient = endGradient;
        }

        /// <summary>
        /// 当传入的Shadow被这个阴影完全覆盖时返回true
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool contains(Shadow other)
        {
            return _startGradient <= other._startGradient && _endGradient >= other._endGradient;
        }
    }
}