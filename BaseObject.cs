using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bouncing_Ball_Physics
{
    class BaseObject
    {
        
        private static int WindowW = 800;
        private static int WindowH = 480;
        //fields
        private Texture2D circleTexture;
        private Vector2 curPosition;
        private double radius;
        private Vector2 velocity;
        //private Vector2 acceleration;
        private float redness;

        public BaseObject(Texture2D texture, Vector2 pos, double r)
        {
            circleTexture = texture;
            curPosition = pos;
            radius = r;
            velocity = new Vector2(0, 0);
            //acceleration = new Vector2(0, 0);
            redness = 0;
        }

        public BaseObject(Texture2D texture, float xPos, float yPos, double r)
        {
            circleTexture = texture;
            curPosition = new Vector2(xPos, yPos);
            radius = r;
            velocity = new Vector2(0, 0);
            //acceleration = new Vector2(0, 0);
            redness = 0;
        }
        public BaseObject(Texture2D texture, float xPos, float yPos, double r, Vector2 vel)
        {
            circleTexture = texture;
            curPosition = new Vector2(xPos, yPos);
            radius = r;
            velocity = vel;
            //acceleration = acc;
            redness = 0;
        }

        public double X
        {
            get { return curPosition.X; }
        }
        public double Y
        {
            get { return curPosition.Y; }
        }
        public double Mass
        {
            get { return radius * radius; }
        }
        public double Energy
        {
            get { return 0.5 * Mass * velocity.LengthSquared(); }
        }
        public double Momentum
        {
            get { return Mass * velocity.Length(); }
        }
        public int WWidth
        {
            set { WindowW = value; }
        }
        public int WHeight
        {
            set { WindowH = value; }
        }

        public int Redness
        {
            get { return (int) redness; }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(circleTexture,
                new Rectangle((int)(curPosition.X - radius), (int)(curPosition.Y - radius)
                , (int)(radius * 2), (int)(radius * 2))
                , new Color((int)redness, 10, 10));
        }

        public void Update(GameTime gameTime)
        {
            Vector2 prevPosition = curPosition;
            UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds);
            curPosition = KeepInBounds(prevPosition, gameTime.ElapsedGameTime.TotalSeconds);
            redness -= (float) gameTime.ElapsedGameTime.TotalSeconds * 100;
        }
        public void UpdatePosition(double time)
        {
            //velocity.X += (float)(acceleration.X * time);
            //velocity.Y += (float)(acceleration.Y * time);
            curPosition.X += (float)(velocity.X * time);
            curPosition.Y += (float)(velocity.Y * time);
        }

        public Vector2 KeepInBounds(Vector2 prevPos, double time)
        {
            if (curPosition.X - radius < 0)
            {
                //IncreaseRedness();
                #region comment
                /*
                double distFromWall = 0 - (prevPos.X - radius);
                //Use quadratic formula to get the time it took to hit the wall
                double timeToWall = (-velocity.X + Math.Sqrt(velocity.X * velocity.X
                    - 4 * 0.5 * acceleration.X * -distFromWall))
                    / (0.5 * acceleration.X);
                curPosition = prevPos;
                UpdatePosition(timeToWall);
                velocity.X *= -1;
                UpdatePosition(time - timeToWall);
                */
                #endregion
                double distFromWall = 0 - (prevPos.X - radius);
                double timeToWall = distFromWall / velocity.X;
                curPosition = prevPos;
                UpdatePosition(timeToWall);
                velocity.X *= -1;
                Vector2 lp = curPosition;
                UpdatePosition(time - timeToWall);
                return KeepInBounds(lp, time - timeToWall);
            }
            if (curPosition.X + radius > WindowW)
            {
                //IncreaseRedness();
                #region commented out
                /*
                double distFromWall = WindowW - (prevPos.X + radius);
                //Use quadratic formula to get the time it took to hit the wall
                double timeToWall = (-velocity.X + Math.Sqrt(velocity.X * velocity.X 
                    - 4 * 0.5 * acceleration.X * -distFromWall))
                    / (0.5 * acceleration.X);
                curPosition = prevPos;
                UpdatePosition(timeToWall);
                velocity.X *= -1;
                UpdatePosition(time - timeToWall);
                */
                #endregion
                double distFromWall = WindowW - (prevPos.X + radius);
                double timeToWall = distFromWall / velocity.X;
                curPosition = prevPos;
                UpdatePosition(timeToWall);
                velocity.X *= -1;
                Vector2 lp = curPosition;
                UpdatePosition(time - timeToWall);
                return KeepInBounds(lp, time - timeToWall);
            }
            if (curPosition.Y - radius < 0)
            {
                //IncreaseRedness();
                double distFromWall = 0 - (prevPos.Y - radius);
                double timeToWall = distFromWall / velocity.Y;
                curPosition = prevPos;
                UpdatePosition(timeToWall);
                velocity.Y *= -1;
                Vector2 lp = curPosition;
                UpdatePosition(time - timeToWall);
                return KeepInBounds(lp, time - timeToWall);
            }
            if (curPosition.Y + radius > WindowH)
            {
                //IncreaseRedness();
                double distFromWall = WindowH - (prevPos.Y + radius);
                double timeToWall = distFromWall / velocity.Y;
                curPosition = prevPos;
                UpdatePosition(timeToWall);
                velocity.Y *= -1;
                Vector2 lp = curPosition;
                UpdatePosition(time - timeToWall);
                return KeepInBounds(lp, time - timeToWall);
            }
            else
            {
                return curPosition;
            }
        }

        public void CollideWith(BaseObject other)
        {

            IncreaseRedness();
            other.IncreaseRedness();
            
            Vector2 normalVector = other.curPosition - curPosition;
            normalVector.Normalize();
            Vector2 tanVector = new Vector2(-normalVector.Y, normalVector.X);

            float INormVel = Vector2.Dot(normalVector, velocity);
            float IOtherNormVel = Vector2.Dot(normalVector, other.velocity);

            Vector2 FNormVel = Vector2.Multiply(normalVector, (float)(INormVel * (Mass - other.Mass) / (Mass + other.Mass) + IOtherNormVel * (Mass * 2) / (Mass + other.Mass)));
            Vector2 FOtherNormVel = Vector2.Multiply(normalVector, (float)(IOtherNormVel * (other.Mass - Mass) / (Mass + other.Mass) + INormVel * (other.Mass * 2) / (Mass + other.Mass)));

            velocity = FNormVel + Vector2.Multiply(tanVector, Vector2.Dot(tanVector, velocity));
            other.velocity = FOtherNormVel + Vector2.Multiply(tanVector, Vector2.Dot(tanVector, other.velocity));

            //Adding a "separating force"
            //proximity is the sum of both radius - distance between centers 
            double proximity = radius + other.radius - (Distance(curPosition, other.curPosition));
            curPosition += Vector2.Multiply(curPosition-other.curPosition, (float)(.01 * proximity * Mass /(Mass + other.Mass)));
            other.curPosition += Vector2.Multiply(other.curPosition- curPosition, (float) ( 0.01 * proximity * other.Mass / (Mass + other.Mass)));
        }

        public bool CheckCollide(BaseObject other)
        {
            if(radius + other.radius >= Distance(curPosition, other.curPosition))
            {
                return true;
            }
            return false;
        }

        public double Distance(Vector2 point1, Vector2 point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        private void IncreaseRedness()
        {
            /*
            if(redness < 255)
            {
                redness += 10;
            }*/
            redness = 200;
        }
    }
}
