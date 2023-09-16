/*
 * Proj1.cs
 * 
 * Created by: Aidan Harries
 * Date: 9/15/23
 * Project: Proj1
 * 
 * Description: Main class for the Proj1 game. Manages game initialization,
 * content loading, updates, and rendering. Includes functionality for 
 * collision detection and game state updates.
 * 
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Proj1
{
    public class Proj1 : Game
    {
        // Instance variables for managing game graphics and objects
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private PlayerShip playerShip;
        private Asteroid asteroid;

        /// <summary>
        /// Constructor - Initializes a new instance of the Proj1 game.
        /// </summary>
        public Proj1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initialize - Initializes the game objects and settings.
        /// </summary>
        protected override void Initialize()
        {
            playerShip = new PlayerShip(this);
            asteroid = new Asteroid(this);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent - Loads the game content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerShip.LoadContent(Content);
            asteroid.LoadContent(Content);
        }

        /// <summary>
        /// Update - Updates the game state.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Exit the game if the user presses the 'Back' button or 'Escape' key
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update player ship and asteroid positions and states
            playerShip.Update(gameTime);
            asteroid.Update(gameTime);

            // Laser-Asteroid collision detection
            Laser collidedLaser = null;
            foreach (var laser in playerShip.GetLasers())
            {
                if (asteroid.CheckCollision(laser))
                {
                    collidedLaser = laser;
                    break;
                }
            }

            if (collidedLaser != null)
            {
                playerShip.RemoveLaser(collidedLaser);

                // Check if already exploding or invulnerable
                if (!asteroid.isExploding && !asteroid.isInvulnerable)
                {
                    asteroid.isInvulnerable = true; // Make the asteroid invulnerable to further hits
                    asteroid.isExploding = true;  // Start the explosion animation
                    asteroid.currentFrame = 0;  // Reset the current frame
                    asteroid.timeSinceLastFrame = 0;  // Reset the time counter

                    // Stop the asteroid from moving
                    asteroid.SetVelocity(Vector2.Zero);
                }
            }

            // Collision detection for player ship
            if (asteroid.CheckCollision(playerShip))
            {
                // Check if already exploding or invulnerable
                if (!asteroid.isExploding && !asteroid.isInvulnerable)
                {
                    // Set the ship to be red
                    playerShip.SetToBeRed();

                    asteroid.isInvulnerable = true; // Make the asteroid invulnerable to further hits
                    asteroid.isExploding = true;    // Start the explosion animation
                    asteroid.currentFrame = 0;      // Reset the current frame
                    asteroid.timeSinceLastFrame = 0;  // Reset the time counter

                    // Stop the asteroid from moving
                    asteroid.SetVelocity(Vector2.Zero);
                }
            }

            // Check if the asteroid is marked for removal
            if (asteroid.toBeRemoved)
            {
                asteroid = new Asteroid(this);
                asteroid.LoadContent(Content);
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// Draw - Draws the game objects.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            playerShip.Draw(gameTime, spriteBatch);
            asteroid.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}