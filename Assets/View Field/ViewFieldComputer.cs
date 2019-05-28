using UnityEngine;

namespace MtC.Tools.FoV
{
    public class ViewFieldComputer
    {
        /// <summary>
        /// 在八分圆的45°交界处，如果两个墙角对角在一起，将会由于计算时无法超出八分圆得知还有一个对角墙导致视线穿过对角位置。加一格额外的检测就可以解决这个问题
        /// </summary>
        const int LINE_PATCH = 1;

        ViewField _viewField;
        VisibleMap _visibleMap;

        public ViewFieldComputer(VisibleMap visibleMap)
        {
            _visibleMap = visibleMap;
            _viewField = new ViewField(_visibleMap.width, _visibleMap.height);
        }

        public ViewField ComputeViewField(Vector2 viewerPosition)
        {
            /*
             *  初始化视野
             *  计算视野
             *  返回视野
             */
            SetupViewField();
            DoComputeViewField(viewerPosition);
            return _viewField;
        }

        void SetupViewField()
        {
            _viewField.Fill(true);
        }

        void DoComputeViewField(Vector2 viewerPosition)
        {
            /*
             *  计算八个八分角的视野并合并到总视野中
             *  玩家肯定看得到自己，初始化可见，不用管
             */
            ComputeAndMergeAnOctant(new Octant(viewerPosition, Vector2.up, Vector2.right));
            ComputeAndMergeAnOctant(new Octant(viewerPosition, Vector2.right, Vector2.up));
            ComputeAndMergeAnOctant(new Octant(viewerPosition, Vector2.right, Vector2.down));
            ComputeAndMergeAnOctant(new Octant(viewerPosition, Vector2.down, Vector2.right));
            ComputeAndMergeAnOctant(new Octant(viewerPosition, Vector2.down, Vector2.left));
            ComputeAndMergeAnOctant(new Octant(viewerPosition, Vector2.left, Vector2.down));
            ComputeAndMergeAnOctant(new Octant(viewerPosition, Vector2.left, Vector2.up));
            ComputeAndMergeAnOctant(new Octant(viewerPosition, Vector2.up, Vector2.left));
        }

        void ComputeAndMergeAnOctant(Octant octant)
        {
            /*
             *  计算并合并第一行（因为要考虑到观察者的影响但又不能过度消耗计算量）
             * 
             *  向前走直到地图边界
             *      if(阴影没有完全覆盖八分角)
             *          计算并合并每一行
             *      else
             *          填充整行的阴影
             */
            bool isFullShadow = false;
            ShadowLine shadowLine = new ShadowLine();

            ComputeAndMergeStartLine(octant, shadowLine); // 因为观察者自身对视线的遮挡会导致各种问题，所以单独拿出一行来处理

            for (int mainStep = 1; _visibleMap.Contains(octant.GetPosition(mainStep, 0)); mainStep++) // 第一行单独处理了，这里从距离1开始
                if (!isFullShadow)
                    isFullShadow = ComputeAndMergeALineAndGetIsFullShadow(octant, mainStep, shadowLine);
                else
                    FillALineShadow(octant, mainStep);
        }

        void ComputeAndMergeStartLine(Octant octant, ShadowLine shadowLine)
        {
            int mainStep = 0;
            Vector2 currentPosition;
            for (int sideStep = 1; sideStep <= LINE_PATCH && _visibleMap.Contains(currentPosition = octant.GetPosition(mainStep, sideStep)); sideStep++) // 边缘方向从1开始，跳过观察者所在的位置
            {
                Shadow projection = ShadowLine.GetQuadProjection(mainStep, sideStep);
                DrawShadow(shadowLine, currentPosition, projection);
                UpdateShadowLine(currentPosition, shadowLine, projection);
            }
        }

        bool ComputeAndMergeALineAndGetIsFullShadow(Octant octant, int mainStep, ShadowLine shadowLine)
        {
            /*
             *  循环到一行结束或地图边界
             *  {
             *      获取投影
             *      通过投影判断可见度并存入视野
             *      通过投影判断是否遮挡视线并存入阴影线
             *  }
             *  返回阴影是否覆盖了整个八分角
             */
            Vector2 currentPosition;
            for (int sideStep = 0; sideStep <= mainStep + LINE_PATCH && _visibleMap.Contains(currentPosition = octant.GetPosition(mainStep, sideStep)); sideStep++)
            {
                Shadow projection = ShadowLine.GetQuadProjection(mainStep, sideStep);
                DrawShadow(shadowLine, currentPosition, projection);
                UpdateShadowLine(currentPosition, shadowLine, projection);
            }

            return shadowLine.IsFullShadow();
        }

        void DrawShadow(ShadowLine shadowLine, Vector2 currentPosition, Shadow projection)
        {
            if (!IsVisible(projection, shadowLine))
                _viewField.SetVisible(currentPosition, false);
            //因为阴影以八分圆为单位绘制，无论如何都会发生在边缘读取不到八分角外的墙而导致的判断不准确
            //在前面为了解决八分角边缘对角墙透视问题时增加了一格的判断用来补上这部分影子，但如果按照可见存true不可见存false的方法存储，则会不可避免的发生一部分计算正确的影子被不正确的可见覆盖
            //幸运的是这个算法在一开始就把整个视野设为可见， 因此只要在这里只绘制阴影就不会发生不正确可见覆盖不可见的问题了
        }

        bool IsVisible(Shadow projection, ShadowLine shadowLine)
        {
            return !shadowLine.Contains(projection); // 我觉得如果是一个经验丰富的观察者，就算只能看见一小部分也是能判断出看到了什么的，那就只要没有被完全遮挡就视为可见
        }

        void UpdateShadowLine(Vector2 position, ShadowLine shadowLine, Shadow projection)
        {
            if (IsBlockView(position, shadowLine, projection))
                shadowLine.AddShadow(projection);
        }

        bool IsBlockView(Vector2 position, ShadowLine shadowLine, Shadow shadow)
        {
            return !_visibleMap.IsTransparent(position) && !shadowLine.Contains(shadow); // 很明显的，只要一个东西没有被完全挡住，他又不是透明的，那他就一定会遮挡视线
        }

        void FillALineShadow(Octant octant, int mainStep)
        {
            /*
             *  循环到一行结束或地图边界
             *      填充阴影
             */
            Vector2 currentPosition;
            for (int sideStep = 0; sideStep <= mainStep && _visibleMap.Contains(currentPosition = octant.GetPosition(mainStep, sideStep)); sideStep++)
                _viewField.SetVisible(currentPosition, false);
            /*
             *  前面for判断里有一句 _visibleMap.Contains(currentPosition = octant.GetPosition(mainStep, sideStep))
             *  这一句是利用括号控制代码顺序，以括号为分隔，看成两个部分：
             *  第一部分，括号内的运算 currentPosition = octant.GetPosition(mainStep, sideStep) 运算后 currentPosition 就变为了这次循环的位置
             *  第二部分：括号外的运算，在经过第一部分后实际上变成了这样： _visibleMap.Contains(currentPosition)
             *  两步合到一起就完成了更新 currentPosition 并判断是否越界的效果
             */
        }
    }
}
