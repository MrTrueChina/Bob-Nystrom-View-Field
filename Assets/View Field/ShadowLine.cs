using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MtC.Tools.FoV
{
    public class ShadowLine
    {
        List<Shadow> _shadows = new List<Shadow>();

        /// <summary>
        /// 以不重复的方式向阴影线中加入阴影，重叠的阴影会合并为一个
        /// </summary>
        /// <param name="newShadow"></param>
        public void AddShadow(Shadow newShadow)
        {
            /*
             *  确定阴影要插入的位置
             *  根据位置可以找到前后的阴影
             *  如果前面的阴影和这个阴影重合，则要合并
             *  如果后面的阴影和这个阴影重合，也要合并
             */
            int insertIndex = GetInsertIndex(newShadow);
            Shadow previousOverlappingShadow = GetPreviousOverlappingShadow(insertIndex, newShadow);
            Shadow nextOverlappingShadow = GetNextOverlappingShadow(insertIndex, newShadow);

            if (previousOverlappingShadow != null)
            {
                if (nextOverlappingShadow != null)
                    MergePreviousAndNewAndNextShadow(insertIndex, previousOverlappingShadow, nextOverlappingShadow);
                else
                    MergePreviousAndNewShadow(newShadow, previousOverlappingShadow);
            }
            else
            {
                if (nextOverlappingShadow != null)
                    MergeNewAndNextShadow(newShadow, nextOverlappingShadow);
                else
                    InsertNewShadow(insertIndex, newShadow);
            }
        }

        int GetInsertIndex(Shadow newShadow)
        {
            /*
             *  以起点为准
             *  遍历阴影列表
             *      if(当前阴影的起点在新阴影的起点后面)
             *          返回这个下标
             *          
             *  遍历完了也没找到，说明要加在最后面，返回阴影列表长度
             */
            int insertIndex = 0;

            for (; insertIndex < _shadows.Count; insertIndex++)
                if (_shadows[insertIndex].start > newShadow.start)
                    return insertIndex;

            return insertIndex; // 循环结束的条件是 insertIndex >= _shadows.Count，因为是++的，所以循环结束后的 insertIndex 就是列表长度
        }

        Shadow GetPreviousOverlappingShadow(int insertIndex, Shadow newShadow)
        {
            if(insertIndex > 0)
            {
                Shadow prevousShadow = _shadows[insertIndex - 1];
                if (prevousShadow.end >= newShadow.start)
                    return prevousShadow;
            }
            return null;
        }

        Shadow GetNextOverlappingShadow(int insertIndex, Shadow newShadow)
        {
            if(insertIndex < _shadows.Count)
            {
                Shadow nextShadow = _shadows[insertIndex]; // 获取后一个重叠的阴影时新的阴影还没有插入列表，所以插入下标指向的就是后一个阴影
                if (nextShadow.start <= newShadow.end)
                    return nextShadow;
            }
            return null;
        }

        void MergePreviousAndNewAndNextShadow(int insertIndex, Shadow previousOverlappingShadow, Shadow nextOverlappingShadow)
        {
            /*
             *  合并前中后的情况，说明前中后三个阴影范围相连
             *  前阴影的结尾移动到后的结尾处
             *  把后移除掉
             *  中本来就不在列表里，不去管
             */
            previousOverlappingShadow.end = nextOverlappingShadow.end;
            _shadows.RemoveAt(insertIndex); // 插入下标在插入新阴影之前就是下一个阴影的下标
        }

        void MergePreviousAndNewShadow(Shadow newShadow, Shadow previousOverlappingShadow)
        {
            /*
             *  前阴影结尾移到新阴影结尾处
             *  新阴影不在列表里不用管
             */
            previousOverlappingShadow.end = newShadow.end;
        }

        void MergeNewAndNextShadow(Shadow newShadow, Shadow nextOverlappingShadow)
        {
            /*
             *  后阴影的起点移动到新阴影的起点
             *  新阴影不管
             */
            nextOverlappingShadow.start = newShadow.start;
        }

        void InsertNewShadow(int insertIndex, Shadow newShadow)
        {
            _shadows.Insert(insertIndex, newShadow);
        }

        /// <summary>
        /// 判断阴影是否已经覆盖了整个横线的范围
        /// </summary>
        /// <returns></returns>
        public bool IsFullShadow()
        {
            /*
             *  首先，阴影覆盖整个区域，整个区域就只剩下一个从头到尾的阴影了，阴影数量为1
             *  设计上最左边的障碍物的起点应该是小于0的，同时不在最左边的障碍物起点都大于0，因此全覆盖阴影起点要小于0
             *  设计上最右边的障碍物终点应该大于1，但如果玩家紧贴障碍物，是会产生等于1的终点的，因此全覆盖阴影必须是大于1，不能等于
             */
            return _shadows.Count == 1 && _shadows[0].start < 0 && _shadows[0].end > 1;
        }

        /// <summary>
        /// 获取一个地块的投影
        /// </summary>
        /// <param name="forwardStep"></param>
        /// <param name="sideStep"></param>
        /// <returns></returns>
        public static Shadow GetQuadProjection(int forwardStep, int sideStep)
        {
            /*
             *  计算起点斜率
             *  计算终点斜率
             *  返回投影
             */
            float topLeft = (sideStep * 2 - 1) / (forwardStep * 2 + 1f); // 注意这个2是float，不然就是两个int的除法，直接得到0
            float bottomRight = (sideStep * 2 + 1) / (forwardStep * 2 - 1f);
            return new Shadow(topLeft, bottomRight);
        }

        /// <summary>
        /// 返回这个阴影线是否完全包含指定阴影
        /// </summary>
        /// <param name="shadow"></param>
        /// <returns></returns>
        public bool Contains(Shadow shadow)
        {
            /*
             *  阴影线对阴影是不重叠储存的，能连接的阴影肯定都合并了，所以如果完全包含参数阴影，必然是列表里有一个阴影完全包含了参数阴影
             */
            foreach (Shadow shadowInList in _shadows)
                if (shadowInList.Contions(shadow))
                    return true;
            return false;
        }
    }
}
