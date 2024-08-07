using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PhonesTrisIV
{
    public class PhonestrisIV : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Dictionary<StateType, State> States { get; private set; }

        public PhonestrisIV()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            States = new Dictionary<StateType, State>
            {
                { StateType.PLAY, new PlayState() },
                { StateType.MENU, new MenuState() },
            };
        }

        protected override void Initialize()
        {
            State.SwitchState(StateType.MENU);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (State s in States.Values)
            {
                s.LoadContent(Content, _spriteBatch);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            State.CurrentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            
            State.CurrentState.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
