//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace OriginFov
//{
//    public class FovBackup
//    {
//        public ViewField viewField
//        {
//            get
//            {
//                return _viewField;
//            }
//        }
//        ViewField _viewField;
//        VisibleMap _visiableMap;

//        public FovBackup(ViewField viewField, VisibleMap visibleMap)
//        {
//            _viewField = viewField;
//            _visiableMap = visibleMap;
//        }

//        /// Updates the visible flags in [stage] given the [Hero]'s [pos].
//        /// 更新[Stage]中给定[Hero]的[pos]的可见标志。
//        public void refresh(Vector2 viewerPosition)
//        {
//            // Sweep through the octants.
//            // 遍历八个八分圆
//            for (int octant = 0; octant < 8; octant++)
//                refreshOctant(viewerPosition, octant);

//            // The starting position is always visible.
//            // 起始位置始终可见。
//            _viewField.SetVisible(viewerPosition, true);
//        }


//        List<Shadow> refreshOctant(Vector2 start, int octant, int maxRows = 999)
//        {
//            ShadowLine shadowLine = new ShadowLine();
//            bool fullShadow = false;

//            // Sweep through the rows ('rows' may be vertical or horizontal based on
//            // the incrementors). Start at row 1 to skip the center position.
//            // 扫过行（“行”可以是垂直的，也可以是水平的，以增量为基础）。从第1行开始跳过中间位置。
//            for (int row = 1; row < maxRows; row++)
//            {
//                // 主方向超出地图则结束主方向的循环
//                if (!_viewField.Contains(start + TransformOctant(row, 0, octant)))
//                    break;

//                // 遍历一行
//                for (int col = 0; col <= row; col++)
//                {
//                    Vector2 currentPosition = start + TransformOctant(row, col, octant);

//                    // If we've traversed out of bounds, bail on this row.
//                    // note: this improves performance, but works on the assumption that
//                    // the starting tile of the FOV is in bounds.
//                    // 侧面方向超出地图则结束这一行的循环
//                    if (!_viewField.Contains(currentPosition))
//                        break;

//                    // 如果一整行都在阴影里，就不需要详细判断了，直接设置不可见 If we know the entire row is in shadow, we don't need to be more specific.
//                    if (fullShadow)
//                    {
//                        _viewField.SetVisible(currentPosition, false);
//                    }
//                    else
//                    {
//                        Shadow projection = ProjectTile(row, col); // 获取这个地块的投影

//                        // Set the visibility of this tile.
//                        // 设置此图块的可见性。
//                        bool visible = !shadowLine.AllInShadow(projection); // 如果这个地块的投影完全在阴影线的阴影里，则认为这个地块是不可见的
//                        _viewField.SetVisible(currentPosition, visible);

//                        // Add any opaque tiles to the shadow map.
//                        // 向阴影贴图添加任何不透明瓷砖。
//                        if (visible && !_visiableMap.IsTransparent(currentPosition)) // 如果这个地块可见，并且这个地块阻挡视线，增加阴影。（不可见则说明已经完全遮蔽了，没有再增加阴影的必要了，如果修改可见标准这里也要变）
//                        {
//                            shadowLine.add(projection);
//                            fullShadow = shadowLine.isFullShadow;
//                        }
//                    }
//                }
//            }

//            return shadowLine.shadows;
//        }

//        Vector2 TransformOctant(int row, int col, int octant)
//        {
//            switch (octant)
//            {
//                case 0: return new Vector2(col, -row);
//                case 1: return new Vector2(row, -col);
//                case 2: return new Vector2(row, col);
//                case 3: return new Vector2(col, row);
//                case 4: return new Vector2(-col, row);
//                case 5: return new Vector2(-row, col);
//                case 6: return new Vector2(-row, -col);
//                case 7: return new Vector2(-col, -row);
//            }

//            throw new System.ArithmeticException("严重错误！传入了不正常的八分角下标");
//        }

//        /// Creates a [Shadow] that corresponds to the projected silhouette of the
//        /// given tile. This is used both to determine visibility (if any of the
//        /// projection is visible, the tile is) and to add the tile to the shadow map.
//        ///
//        /// The maximal projection of a square is always from the two opposing
//        /// corners. From the perspective of octant zero, we know the square is
//        /// above and to the right of the viewpoint, so it will be the top left and
//        /// bottom right corners.
//        /// 
//        /// 创建与给定图块的投影轮廓相对应的[阴影]。这既用于确定可见性（如果任何投影可见，则平铺为），也用于将平铺添加到阴影贴图。
//        /// 
//        /// 一个正方形的最大投影总是从两个相对的角开始。从八分之一零点的角度，我们知道这个正方形在视点的上方和右侧，所以它将是左上角和右下角。
//        Shadow ProjectTile(int row, int col) //row：向前，col：向侧面方向
//        {
//            // The top edge of row 0 is 2 wide.
//            // 第0行的上边缘为2宽。
//            float topLeft = col / (row + 2f);

//            // The bottom edge of row 0 is 1 wide.
//            // 第0行的下边缘为1宽。
//            float bottomRight = (col + 1f) / (row + 1f);

//            return new Shadow(topLeft, bottomRight, new Vector2(col, row + 2), new Vector2(col + 1, row + 1));
//        }
//    }
//}
