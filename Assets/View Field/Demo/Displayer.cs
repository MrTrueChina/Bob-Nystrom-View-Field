using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MtC.Tools.FoV;
using UnityEngine.UI;

public class Displayer : MonoBehaviour
{

    /*
     *  需要能设置和移除墙的方法
     *  需要能显示可见和不可见、墙和不是墙的方法
     *  需要能修改观察者位置的方法
     */
    /*
     *  为了简单，可以考虑使用图片显示，玩家、墙、可见不可见区域使用不同的颜色代表
     *  可以考虑使用一个“光标”作为控制参考
     *  不同的按键有设置墙、移除墙、设置玩家位置的方法
     */
    [SerializeField]
    Image displayImage;
    [SerializeField]
    Color _mouseLightColor = new Color(0, 0, 0.8f);
    [SerializeField]
    Color _mouseDarkColor = new Color(0, 0, 0.4f);
    [SerializeField]
    Color _playerColor = new Color(0.5f, 0.5f, 0);
    [SerializeField]
    Color _visibleFloor = Color.white;
    [SerializeField]
    Color _invisibleFloor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField]
    Color _visibleWall = new Color(0.8f, 0.5f, 0);
    [SerializeField]
    Color _invisibleWall = new Color(0.4f, 0.2f, 0);

    const int WIDTH = 20;
    const int HEIGHT = 20;

    VisibleMap _visibleMap = new VisibleMap(WIDTH, HEIGHT);
    ViewField _viewField = new ViewField(WIDTH, HEIGHT);

    Vector2Int _mousePosition = new Vector2Int(WIDTH / 2, HEIGHT / 2);
    Vector2Int _playerPosition = new Vector2Int(WIDTH / 2, HEIGHT / 2);

    private void Update()
    {
        /*
         *  移动光标
         *  进行操作
         *  显示
         */
        MoveMouse();
        SetQuad();
        Display();
    }

    void MoveMouse()
    {
        /*
         *  if(上下左右)
         *      坐标移动
         *  限制范围
         */
        if (Input.GetKeyDown(KeyCode.UpArrow))
            _mousePosition.y++;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            _mousePosition.x++;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            _mousePosition.y--;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            _mousePosition.x--;

        _mousePosition.x = Mathf.Clamp(_mousePosition.x, 0, WIDTH - 1);
        _mousePosition.y = Mathf.Clamp(_mousePosition.y, 0, HEIGHT - 1);
    }

    void SetQuad()
    {
        /*
         *  放置墙
         *  移除墙
         *  移动玩家
         */
        if (Input.GetKeyDown(KeyCode.W))
            _visibleMap.SetTransparent(_mousePosition.x, _mousePosition.y, false);

        if (Input.GetKeyDown(KeyCode.E))
            _visibleMap.SetTransparent(_mousePosition.x, _mousePosition.y, true);

        if (Input.GetKeyDown(KeyCode.Q))
            _playerPosition.Set(_mousePosition.x, _mousePosition.y);
    }

    void Display()
    {
        /*
         *  更新视野
         *  获取图片
         *  把图片存入到显示图里
         */
        UpdateViewField();
        displayImage.sprite = GetDisplaySprite();
    }

    void UpdateViewField()
    {
        ViewFieldComputer fov = new ViewFieldComputer(_visibleMap);
        _viewField = fov.ComputeViewField(_playerPosition);
    }

    Sprite GetDisplaySprite()
    {
        /*
         *  获取空的图片
         *  
         *  遍历图片
         *      根据可视性图和视野获取颜色并存入
         *      
         *  应用
         *  
         *  返回
         */

        Texture2D texture = GetEmptyDisplayTexture();

        for (int x = 0; x < WIDTH; x++)
            for (int y = 0; y < HEIGHT; y++)
                texture.SetPixel(x, y, GetQuadColor(x, y));

        texture.Apply(); // 一定要记得 Apply，不然图像是不会更新的，而且获取像素的时候会返回新存入的像素，就好像见了鬼一样

        return TextureToSprite(texture);
    }

    Texture2D GetEmptyDisplayTexture()
    {
        Texture2D texture = new Texture2D(WIDTH, HEIGHT);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;

        return texture;
    }

    Color GetQuadColor(int x, int y)
    {
        /*
         *  光标 -> 闪烁的
         *  玩家
         *  可见的空地
         *  可见的墙
         *  不可见的空地
         *  不可见的墙
         */
        if (x == _mousePosition.x && y == _mousePosition.y)
            return GetMouseColor();

        if (x == _playerPosition.x && y == _playerPosition.y)
            return _playerColor;

        if (_viewField.IsVisible(x, y))
        {
            if (_visibleMap.IsTransparent(x, y))
            {
                return _visibleFloor;
            }
            else
            {
                return _visibleWall;
            }
        }
        else
        {
            if (_visibleMap.IsTransparent(x, y))
            {
                return _invisibleFloor;
            }
            else
            {
                return _invisibleWall;
            }
        }
    }

    Color GetMouseColor()
    {
        float cycleTime = 0.5f;

        float current = (Mathf.Sin(Time.time / cycleTime) + 1) / 2;

        return Color.Lerp(_mouseLightColor, _mouseDarkColor, current);
    }

    private static Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
