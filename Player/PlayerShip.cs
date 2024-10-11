using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collisions;
using StateManagement;
using System.Data.Common;

namespace Player
{
    public class PlayerShip
    {
        // the texture of the ship
        private Texture2D _texture;

        // the bounding triangle for the ship
        private BoundingTriangle _hitbox;

        // the angle of the ship
        private float _angle;

        /// <summary>
        /// The position of the spaceship.
        /// </summary>
        public Vector2 Position = new Vector2(400, 300);

        /// <summary>
        /// The current health of the spaceship
        /// </summary>
        public int Health = 300;

        /// <summary>
        /// The speed modifier of the ship
        /// </summary>
        public float Speed = 75;

        /// <summary>
        /// Determines if the ship is deaed (health <= 0)
        /// </summary>
        public bool Dead;

        /// <summary>
        /// The hitbox of the ship
        /// </summary>
        public BoundingTriangle Hitbox => _hitbox;

        /// <summary>
        /// Loads the content for the player ship sprite
        /// </summary>
        /// <param name="content">The content manager to load the sprite with</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("playerShip3_blue");

            float scale = 0.5f;
            Vector2 origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            Vector2 topVertex = new Vector2(Position.X, Position.Y - origin.Y * scale);
            Vector2 leftVertex = new Vector2(Position.X - origin.X * scale, Position.Y + origin.Y * scale);
            Vector2 rightVertex = new Vector2(Position.X + origin.X * scale, Position.Y + origin.Y * scale);

            _hitbox = new BoundingTriangle(topVertex, leftVertex, rightVertex);
        }

        /// <summary>
        /// Updates the player ship based on how much gametime has elapsed
        /// </summary>
        /// <param name="gameTime">the current gametime of the game</param>
        public void HandleInput(GameTime gameTime, InputState input, int playerIndex)
        {
            var keyboardState = input.CurrentKeyboardStates[playerIndex];

            Vector2 movement = Vector2.Zero;

            if(keyboardState.IsKeyDown(Keys.W)){
                movement.Y -= 1;
            }
            if(keyboardState.IsKeyDown(Keys.S)){
                movement.Y += 1;
            }
            if(keyboardState.IsKeyDown(Keys.A)){
                movement.X -= 1;
            }
            if(keyboardState.IsKeyDown(Keys.D)){
                movement.X += 1;
            }

            if(movement != Vector2.Zero){
                movement.Normalize();

                Position += movement * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(movement.Y < 0 || movement.X != 0){
                    _angle = (float)Math.Atan2(movement.Y, movement.X) + MathHelper.PiOver2;
                }
                else{
                    _angle = 0;
                }
            }
            else{
                _angle = 0; // if no input, the ship should be facing the top of the screen regardless
            }

            float scale = 0.5f;
            Vector2 origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            Vector2 topVertex = new Vector2(Position.X, Position.Y - origin.Y * scale);
            Vector2 leftVertex = new Vector2(Position.X - origin.X * scale, Position.Y + origin.Y * scale);
            Vector2 rightVertex = new Vector2(Position.X + origin.X * scale, Position.Y + origin.Y * scale);

            topVertex = RotatePoint(topVertex, Position, _angle);
            leftVertex = RotatePoint(leftVertex, Position, _angle);
            rightVertex = RotatePoint(rightVertex, Position, _angle);

            _hitbox.Point1 = topVertex;
            _hitbox.Point2 = leftVertex;
            _hitbox.Point3 = rightVertex;

            //Console.WriteLine("Position : " + Position);
            //Console.WriteLine("p1 : " + _hitbox.Point1);
            //Console.WriteLine("p2 : " + _hitbox.Point2);
            //Console.WriteLine("p3 : " + _hitbox.Point3);

        }

        private Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle){
            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);

            Vector2 translatedPoint = point - pivot;
            Vector2 rotatedPoint = new Vector2(
                translatedPoint.X * cosTheta - translatedPoint.Y * sinTheta,
                translatedPoint.X * sinTheta + translatedPoint.Y * cosTheta
            );

            return rotatedPoint + pivot;
        }

        /// <summary>
        /// Draws the sprite
        /// </summary>
        /// <param name="gameTime">The time in the game</param>
        /// <param name="spriteBatch">The spritebatch to draw with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(_texture.Width / 2, _texture.Height / 2);

            if(Health <= 0) spriteBatch.Draw(_texture, Position, null, Color.Red, _angle, origin, 0.5f, SpriteEffects.None, 0);

            else spriteBatch.Draw(_texture, Position, null, Color.White, _angle, origin, 0.5f, SpriteEffects.None, 0);            
        }

        /// <summary>
        /// Handles decreasing the health when the ship has taken damage
        /// </summary>
        /// <param name="damage">the amount of damage to take</param>
        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Dead = true;
            }
        }

        
    }
}