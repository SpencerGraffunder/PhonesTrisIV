using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PhonesTrisIV
{
    public abstract class State
    {
        public static State CurrentState { get; private set; }
        static State PreviousState;
        public State()
        {
            
        }

        public static void SwitchState(StateType newState)
        {
            PreviousState = CurrentState;
            CurrentState = PhonestrisIV.States[newState];
        }

        public virtual void LoadContent(ContentManager Content, SpriteBatch spriteBatch) { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}