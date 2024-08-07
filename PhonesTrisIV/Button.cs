using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace PhonesTrisIV
{
    public class Button
    {
        public Rectangle DisplayRectangle;
        public Rectangle TouchRectangle;
        public Texture2D Texture;
        public Vector2 CenterPoint;
        public bool IsPressed = false;
        bool JustPressed = false;
        bool JustReleased = false;
        public int PlayerNumber;
        public ButtonTypes ButtonType;

        public Button(Rectangle displayRectangle, Rectangle touchRectangle, Vector2 centerPoint, ButtonTypes buttonType)
        {
            DisplayRectangle = displayRectangle;
            TouchRectangle = touchRectangle;
            CenterPoint = centerPoint;
            ButtonType = buttonType;
        }

        public void HandleTouch(TouchCollection touches)
        {
            var wasPressed = IsPressed;
            IsPressed = false;
            foreach (var t in touches)
            {
                if (TouchRectangle.Contains(t.Position))
                {
                    IsPressed = true;
                }
            }
            if (!wasPressed && IsPressed)
            {
                JustPressed = true;
            }
            if (wasPressed && !IsPressed)
            {
                JustReleased = true;
            }
        }

        public bool WasJustPressed()
        {
            if (JustPressed)
            {
                JustPressed = false;
                return true;
            }
            return false;
        }

        public bool WasJustReleased()
        {
            if (JustReleased)
            {
                JustReleased = false;
                return true;
            }
            return false;
        }
    }
}
