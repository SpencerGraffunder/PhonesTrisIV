using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace PhonesTrisIV
{
    class MenuState : State
    {
        Texture2D backgroundTex;
        public MenuState()
        {
            TouchPanel.EnabledGestures =
                GestureType.Tap;
        }

        public override void LoadContent(ContentManager Content, SpriteBatch spriteBatch)
        {

            backgroundTex = Content.Load<Texture2D>("background");

        }

        private void HandleTouchInput()
        {
            TouchCollection touches = TouchPanel.GetState();
            if (touches.Count > 0)
            {
                State.SwitchState(StateType.PLAY);
                ((PlayState)State.CurrentState).Reset(2, 5);
            }
        }

        public override void Update(GameTime gameTime)
        {
            HandleTouchInput();

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            int w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            int boardWidth = 20;
            double tileSize = h / boardWidth;
            int boardHeight = w / (int)tileSize + 2;
            double centeringOffset = w % tileSize / 2;

            // board
            for (int row = -1; row < boardHeight; row++)
            {
                for (int col = 0; col < boardWidth; col++)
                {
                    _spriteBatch.Draw(backgroundTex, new Rectangle((int)((row) * tileSize + centeringOffset), (int)(h - ((col + 1) * tileSize)), (int)tileSize, (int)tileSize), Color.White);
                }
            }

            base.Draw(_spriteBatch);
        }
    }
}
