/*
 * Laser.cs
 * 
 * Created by: Aidan Harries
 * Date: 9/15/23
 * Project: Proj1
 * 
 * Description: Represents a laser shot by the player's ship in the game.
 * Responsible for updating its position, and rendering the laser sprite.
 * 
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Proj1
{
    /// <summary>
    /// Class representing a laser shot in the game.
    /// </summary>
    public class Laser
    {
        // Constant for laser speed
        const float SPEED = 1000;

        // Public fields for laser attributes
        public Vector2 position;
        public Vector2 direction;
        public float angle;

        // Texture for the laser
        Texture2D texture;

        /// <summary>
        /// Constructor to initialize the laser.
        /// </summary>
        /// <param name="startPosition">Initial position of the laser.</param>
        /// <param name="startDirection">Initial direction of the laser.</param>
        /// <param name="angle">Initial angle of the laser.</param>

        public Laser(Vector2 startPosition, Vector2 startDirection, float angle)
        {
            position = startPosition;
            direction = startDirection;
            this.angle = angle;
        }

        /// <summary>
        /// Loads the laser texture.
        /// </summary>
        /// <param name="content">The game's ContentManager for loading assets.</param>

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("laser"); 
        }

        /// <summary>
        /// Updates the laser's position based on its speed and direction.
        /// </summary>
        /// <param name="gameTime">Game time.</param>

        public void Update(GameTime gameTime)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += direction * SPEED * t;
        }

        /// <summary>
        /// Draws the laser on screen.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">SpriteBatch for drawing.</param>

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 1.5f, SpriteEffects.None, 0);
        }
    }
}
