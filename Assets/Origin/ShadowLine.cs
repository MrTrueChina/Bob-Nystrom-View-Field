using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriginFov
{
    class ShadowLine
    {
        public List<Shadow> shadows
        {
            get { return _shadows; }
        }
        List<Shadow> _shadows = new List<Shadow>();

        /// <summary>
        /// 判断一个阴影是否完全包含在阴影线里
        /// </summary>
        /// <param name="projection"></param>
        /// <returns></returns>
        public bool AllInShadow(Shadow projection)
        {
            foreach (Shadow shadow in _shadows)
                if (shadow.contains(projection))
                    return true;
            return false;
        }

        /// <summary>
        /// 将阴影以不重叠的方式添加到阴影列表里，如果有重叠的阴影则进行合并 Add [shadow] to the list of non-overlapping shadows. May merge one or more shadows.
        /// </summary>
        /// <param name="shadow"></param>
        public void add(Shadow shadow)
        {
            // 在有序阴影列表中找出新的阴影应该插入的位置 Figure out where to slot the new shadow in the sorted list.
            int index = 0;
            for (; index < _shadows.Count; index++)
                // Stop when we hit the insertion point.
                if (_shadows[index].startGradient >= shadow.startGradient)
                    break;

            // The new shadow is going here. See if it overlaps the previous or next.
            // 获取在前面的和新阴影重叠的阴影，没有前面重叠的阴影则保持null
            Shadow overlappingPrevious = null; // overlappingPrevious：重叠的上一个
            if (index > 0 && _shadows[index - 1].endGradient > shadow.startGradient)
                overlappingPrevious = _shadows[index - 1];

            // 获取在后面的和新阴影重叠的阴影，没有后面重叠的阴影则保持null
            Shadow overlappingNext = null; // overlappingNext：重叠的下一个
            if (index < _shadows.Count && _shadows[index].startGradient < shadow.endGradient)
                overlappingNext = _shadows[index];

            // Insert and unify with overlapping shadows.
            // 根据前后重叠阴影进行合并
            if (overlappingNext != null)
            {
                if (overlappingPrevious != null)
                {
                    // Overlaps both, so unify one and delete the other.
                    overlappingPrevious.endGradient = overlappingNext.endGradient;
                    _shadows.RemoveAt(index);
                }
                else
                {
                    // Only overlaps the next shadow, so unify it with that.
                    overlappingNext.startGradient = shadow.startGradient;
                }
            }
            else
            {
                if (overlappingPrevious != null)
                {
                    // Only overlaps the previous shadow, so unify it with that.
                    overlappingPrevious.endGradient = shadow.endGradient;
                }
                else
                {
                    // Does not overlap anything, so insert.
                    _shadows.Insert(index, shadow);
                }
            }
        }

        public bool IsFullShadow()
        {
            return _shadows.Count == 1 && _shadows[0].startGradient == 0 && _shadows[0].endGradient == 1;
            /*
             * 这个算法是基于作者设计的[起点斜率最小为0，终点斜率最大为1]规则写出的，在这个规则下起点斜率0终点斜率1代表阴影已经覆盖了整个八分角
             * 
             * 而如果将计算斜率的算法改为标准算法，这个条件就必须更改，因为标准算法的范围可以小于0、可以大于1，而且有时候没有覆盖满就已经达到1了
             */
        }

        /// <summary>
        /// 计算一个地块的投影
        /// </summary>
        /// <param name="forwardStep"></param>
        /// <param name="sideStep"></param>
        /// <returns></returns>
        public static Shadow GetQuadProjection(int forwardStep, int sideStep)
        {
            /*
             * 是不是并不需要考虑计算什么，只要做到左上角在x=0的时候值为0，右下角在x=y的时候值为1，两者都是向右增加就行？
             */

            /*
             * 求地块的投影（就是假设这个地块不透明，它会产生的阴影）
             * 
             * 假设这是正上偏右的八分角，假设玩家的视野由玩家所在地块的中心发出
             * 
             * 那么一个不透明的地块所产生的阴影应该是通过左上角求出起点、右下角求出终点，因为右上角观察者根本看不见，左下角在视野中则在左上角和右下角之间。
             * 当然这有个特殊情况：地块在八分角最左侧，此时观察者只能看到左下右下两个角，按理来说应该使用左下角为准求起点。
             * 但实际上这个位置用左边哪个角求都一样，因为更左边没有地块了，而不管用那个角计算结果都能完全遮住正后方的地块。
             */

            /*
             * 需要正上是0，右上是1的斜率计算公式
             * 
             * 假设左下角地块中心坐标为(0,0)
             * 假设要计算的是(1,1)的阴影，则左上角的坐标就是(0.5,1.5)
             * 已知(x=0,y=0)(x=0.5,y=0.5)，求直线方程 y=ax+b
             * 结果是 y=3x，x/y = 1/3
             * 
             * 假设要计算的是(1,2)的阴影，则左上角的坐标是(0.5,3.5)
             * 已知(x=0,y=0)(x=0.5,y=3.5)
             * 结果是 y=7x，x/y = 1/7
             * 
             * 已知(前方向长度=1,侧方向长度=1,y/x=1/3)(前方向长度=2,侧方向长度=1,y/x=1/7)，求前方向长度、侧方向长度、x/y三者关系
             * 最后的结果是这样的：x/y = (侧方向长度 * 2 + 1) / (前方向长度 * 2 - 1)
             * 
             * topLeft = (sideStep * 2 - 1) / (forwardStep * 2 + 1f)
             * 
             * 然而！作者留下了一个更加简洁、更加神奇的算式：
             */
            float topLeft = sideStep / (forwardStep + 2f); // 注意这个2是float，不然就是两个int的除法，直接得到0
            /*
             *  作者唯一留下的信息：
             *  
             *  The top edge of row 0 is 2 wide.
             *  
             *  谁能告诉我这个算式是怎么回事？取近似简化吗？
             */

            /*
             * 右下角的斜率计算，如果按照标准的计算方式，公式是这样的
             * 
             * bottomRight = (sideStep * 2 + 1) / (forwardStep * 2 - 1f);
             * 
             * 作者当然也给出了简单公式：
             */
            float bottomRight = (sideStep + 1) / (forwardStep + 1f);
            /*
             * 然后同样只留下一句话：
             * 
             * The bottom edge of row 0 is 1 wide.
             * 
             * 不过这次的公式我有点看明白了，是为了达到[最大斜率为1，不在最右边斜率不为1]的效果写出的简化公式
             * 有了这个最大斜率为1的设定，在阴影线判断是否全覆盖时就可以使用[等于1]的条件
             * 而如果使用了标准公式，判断全覆盖时的条件则要改为[大于1]，因为位于(0,1)的障碍的右下角透出的阴影斜率正好是1，但观察者应该还能通过(1,1)的空隙看到后面的方块
             * 没错，标准公式的斜率范围，最小小于0、最大大于1
             * 
             * 当然作为简化公式，这个公式的结果和标准公式有偏差，简化公式让观察者能看到更多标准公式看不到的地块，具体有多多呢？大概有1/3不可见地块都成了可见的了 -_-||
             * 
             * 牢记图形学至理名言：如果它看上去是对的，那么它就是对的
             */

            return new Shadow(topLeft, bottomRight);
        }
    }
}
