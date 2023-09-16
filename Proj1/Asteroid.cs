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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Proj1
{
    public class Asteroid
    {
        // Fields and Properties
        Texture2D texture;
        Vector2 position;
        Vector2 velocity;
        Game game;
        float radius;
        bool insideViewport;

        // Explosion animation fields
        Texture2D explosionTexture;
        const float TIME_PER_FRAME = 0.1f;  // 10 frames per second
        int frameCount = 8;

        // Public states
        public bool isExploding = false;
        public bool toBeRemoved = false;
        public bool isInvulnerable = false;
        public int currentFrame;
        public float timeSinceLastFrame;

        /// <summary>
        /// Constructor to initialize the asteroid.
        /// </summary>
        /// <param name="game">Game instance.</param>
        public Asteroid(Game game)
        {
            this.game = game;
            var viewport = game.GraphicsDevice.Viewport;
            var rand = new Random();

            // Randomly decide which edge (top, bottom, left, right) to start from
            int edge = rand.Next(4);

            switch (edge)
            {
                case 0: // Top edge
                    position = new Vector2(rand.Next(viewport.Width), -50);
                    break;
                case 1: // Bottom edge
                    position = new Vector2(rand.Next(viewport.Width), viewport.Height + 50);
                    break;
                case 2: // Left edge
                    position = new Vector2(-50, rand.Next(viewport.Height));
                    break;
                case 3: // Right edge
                    position = new Vector2(viewport.Width + 50, rand.Next(viewport.Height));
                    break;
            }

            // Velocity pointing towards the center of the screen
            Vector2 center = new Vector2(viewport.Width / 2, viewport.Height / 2);
            this.velocity = Vector2.Normalize(center - position) * 100;  // Speed is 100 units per second

            this.radius = 48f;
            this.insideViewport = false;
        }

        /// <summary>
        /// Loads the asteroid and explosion textures.
        /// </summary>
        /// <param name="content">Content manager.</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("asteroid");
            explosionTexture = content.Load<Texture2D>("asteroid_explode");
            radius = texture.Width / 2; // Update the radius based on the loaded texture
        }

        /// <summary>
        /// Updates the asteroid's position and handles bouncing logic.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            var viewport = game.GraphicsDevice.Viewport;

            // Update position
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Once it enters the viewport, it will bounce inside
            if (!insideViewport &&
                position.X + radius * 2 > 0 && position.X < viewport.Width &&
                position.Y + radius * 2 > 0 && position.Y < viewport.Height)
            {
                insideViewport = true;
            }

            if (insideViewport)
            {
                if ((position.X <= 0 && velocity.X < 0) || (position.X + radius * 2 >= viewport.Width && velocity.X > 0))
                {
                    velocity.X = -velocity.X;
                }
                if ((position.Y <= 0 && velocity.Y < 0) || (position.Y + radius * 2 >= viewport.Height && velocity.Y > 0))
                {
                    velocity.Y = -velocity.Y;
                }
            }

            if (isExploding)
            {
                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceLastFrame >= TIME_PER_FRAME)
                {
                    currentFrame++;
                    timeSinceLastFrame = 0;
                    if (currentFrame >= frameCount)
                    {
                        // Reset and stop the animation
                        isExploding = false;
                        currentFrame = 0;

                        // Mark the asteroid for removal
                        toBeRemoved = true; 
                    }
                }
            }
        }

        /// <summary>
        /// Draws the asteroid or its explosion, if applicable.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isExploding)
            {
                spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, (int)radius * 2, (int)radius * 2), Color.White);
            }
            else
            {
                int frameSize = 48; // size of each frame
                Rectangle sourceRect = new Rectangle(currentFrame * frameSize, 0, frameSize, frameSize);
                spriteBatch.Draw(explosionTexture, new Rectangle((int)position.X, (int)position.Y, frameSize, frameSize), sourceRect, Color.White);
            }
        }

        /// <summary>
        /// Checks for collision between the asteroid and a laser.
        /// </summary>
        /// <param name="laser">Laser object to check collision with.</param>
        /// <returns>True if collision occurs, else false.</returns>
        public bool CheckCollision(Laser laser)
        {
            if (isInvulnerable || isExploding)
            {
                return false;
            }
            float distance = Vector2.Distance(this.position + new Vector2(radius, radius), laser.position);
            return distance < this.radius;
        }

        /// <summary>
        /// Checks for collision between the asteroid and the player's ship.
        /// </summary>
        /// <param name="player">PlayerShip object to check collision with.</param>
        /// <returns>True if collision occurs, else false.</returns>
        public bool CheckCollision(PlayerShip player)
        {
            Rectangle asteroidRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            Rectangle playerRect = new Rectangle((int)player.position.X, (int)player.position.Y, player.texture.Width, player.texture.Height);

            return asteroidRect.Intersects(playerRect);
        }

        /// <summary>
        /// Sets the velocity of the asteroid.
        /// </summary>
        /// <param name="newVelocity">New velocity vector.</param>
        public void SetVelocity(Vector2 newVelocity)
        {
            this.velocity = newVelocity;
        }

    }
}
