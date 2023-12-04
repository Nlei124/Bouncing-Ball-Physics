using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bouncing_Ball_Physics
{
    
    public class Game1 : Game
    {
        
        public int WindowW;
        public int WindowH;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<BaseObject> balls;
        private Texture2D circleT;
        private SpriteFont font;
        private float fontHeight;
        private double energy;
        private double momentum;
        private MouseState prevMState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }
        public void OnResize(Object sender, EventArgs e)
        {
            if (balls[0] != null)
            {
                balls[0].WWidth = _graphics.GraphicsDevice.Viewport.Width;
                balls[0].WHeight = _graphics.GraphicsDevice.Viewport.Height;
            }
            foreach (BaseObject b in balls)
            {
                b.WWidth = _graphics.GraphicsDevice.Viewport.Width;
                b.WHeight = _graphics.GraphicsDevice.Viewport.Height;
            }
            WindowW = _graphics.GraphicsDevice.Viewport.Width;
            WindowH = _graphics.GraphicsDevice.Viewport.Height;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            balls = new List<BaseObject>();
            WindowW = _graphics.PreferredBackBufferWidth;
            WindowH = _graphics.PreferredBackBufferHeight;
            //_graphics.PreferredBackBufferWidth = WindowW;
            //_graphics.PreferredBackBufferHeight = WindowH;
            //_graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            circleT = Content.Load<Texture2D>("Circle");
            
            balls.Add(new BaseObject(circleT, 50, 300, 50, new Vector2(120, 50)));
            balls.Add(new BaseObject(circleT, 300, 400, 20, new Vector2(100, 0)));
            /*balls.Add(new BaseObject(circleT, 500, 100, 20, new Vector2(100, 500)));
            balls.Add(new BaseObject(circleT, 300, 200, 50, new Vector2(300, 500)));
            balls.Add(new BaseObject(circleT, 300, 20, 40, new Vector2(300, 300)));
            balls.Add(new BaseObject(circleT, 350, 20, 15, new Vector2(700, 600)));
            balls.Add(new BaseObject(circleT, 350, 100, 15, new Vector2(300, 600)));
            balls.Add(new BaseObject(circleT, 500, 200, 30, new Vector2(-300, 600)));
            */
            


            font = Content.Load<SpriteFont>("ArialRound15");
            fontHeight = font.MeasureString("A").Y;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MouseState mState = Mouse.GetState();
            if(mState.LeftButton == ButtonState.Pressed && prevMState.LeftButton == ButtonState.Released)
            {
                balls.Add(new BaseObject(circleT, mState.X, mState.Y, 30, new Vector2()));
            }

            // TODO: Add your update logic here
            foreach (BaseObject b in balls)
            {
                b.Update(gameTime);
            }
            for (int i = 0; i < balls.Count; i++)
            {
                
                for (int j = i+1; j < balls.Count; j++)
                {
                    if (balls[i].CheckCollide(balls[j]))
                    {
                        balls[i].CollideWith(balls[j]);
                        //TODO: try to flag collided objects
                        //and resolve collisions in later step
                    }
                   
                }
            }
            prevMState = mState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            energy = 0;
            momentum = 0;

            foreach (BaseObject b in balls)
            {
                b.Draw(_spriteBatch);
                energy += b.Energy;
                momentum += b.Momentum;
            }

            //_spriteBatch.DrawString(font, String.Format(balls[0].X.ToString()), new Vector2(), Color.Black);
            _spriteBatch.DrawString(font, String.Format("Ball 1 Redness" + balls[0].Redness.ToString()), new Vector2(), Color.Black);
            _spriteBatch.DrawString(font, String.Format("KE: {0:F4}", energy/10000000), new Vector2(0, fontHeight), Color.Black);
            _spriteBatch.DrawString(font, String.Format("Mom: {0:F4}", momentum/10000), new Vector2(0, fontHeight*2), Color.Black);

            //MouseState mState = Mouse.GetState();
            //_spriteBatch.Draw(circleT, new Rectangle(mState.X - 25, mState.Y -25, 50, 50), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
