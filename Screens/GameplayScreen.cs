using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StateManagement;
using ParticleSystem;
using Player;

namespace Screens
{
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen
    {
        // The content manager for the game
        private ContentManager _content;

        // The font for the game
        private SpriteFont _gameFont;

        // The position of the player
        private Vector2 _playerPosition = new Vector2(100, 100);
        
        // Random number generator
        private readonly Random _random = new Random();

        // The pause alpha
        private float _pauseAlpha;

        // The stars particle system
        private StarsParticleSystem _stars;

        // The asteroid particle system
        private AsteroidParticleSystem _asteroids;

        // The explosion particle system
        private ExplosionParticleSystem _explosions;

        private PlayerShip _player;

        // The inputs for a pause action
        private readonly InputAction _pauseAction;

        private bool _isZooming;

        private float _zoomTimer;

        private const float ZoomDuration = 0.5f;

        private Particle _collidedParticle;

        private Vector2 _zoomOrigin;

        private float _zoomScale;

        private bool _gamePaused;

        private SoundManager _sounds = new SoundManager();
        

        /// <summary>
        /// Constructor for the GameplayScreen
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Escape }, true);
        }

        // Load graphics content for the game
        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _gameFont = _content.Load<SpriteFont>("Baskervville-SC");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            _player = new PlayerShip();

            _player.LoadContent(_content);

            _stars = new StarsParticleSystem(ScreenManager, new Rectangle(0, -20, 800, 10));
            //stars.DrawOrder = 0;
            //ScreenManager.Game.Components.Add(stars);

            _asteroids = new AsteroidParticleSystem(ScreenManager, new Rectangle(0, -20, 800, 10));
            //asteroids.DrawOrder = 1;
            //ScreenManager.Game.Components.Add(asteroids);

            _explosions = new ExplosionParticleSystem(ScreenManager, 20);

            _sounds.LoadContent(_content);
            _sounds.UpdateVolume();
        }

        /// <summary>
        /// Deactivates the GameplayScreen
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
        }

        /// <summary>
        /// Unloads the content for the GameplayScreen
        /// </summary>
        public override void Unload()
        {
            _content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen){
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            } 
            else{
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);
            }

            //PauseParticles();
                

            if (IsActive)
            {
                // Update logic goes here
                //ResumeParticles();

                if(_isZooming){
                    _zoomTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _zoomScale = MathHelper.Lerp(1.0f, 2.0f, _zoomTimer / ZoomDuration);

                    if(_zoomTimer >= ZoomDuration){
                        _isZooming = false;
                        _zoomScale = 1.0f;
                    }
                }

                if(_gamePaused){
                    _explosions.Update(gameTime);

                    if(_player.Dead){
                        _sounds.PlayLoseSound();
                        ScreenManager.AddScreen(new LoseScreen(_asteroids.NumParticles), ControllingPlayer);
                    }
                    
                    return;
                }

                _stars.Update(gameTime);
                _asteroids.Update(gameTime);
                _explosions.Update(gameTime);
                CheckForPlayerCollision();
            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                //PauseParticles();
            }
            else
            {
                // Handle game input here
                _player.HandleInput(gameTime, input, playerIndex);
            }
        }

        /// <summary>
        /// Draws the GameplayScreen and make calls to its components to draw themselves
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            if(_isZooming){
                _gamePaused = true;
                Matrix zoomMatrix = Matrix.CreateTranslation(-_zoomOrigin.X, -_zoomOrigin.Y, 0) *
                                    Matrix.CreateScale(_zoomScale) *
                                    Matrix.CreateTranslation(_zoomOrigin.X, _zoomOrigin.Y, 0);
                
                spriteBatch.Begin(transformMatrix: zoomMatrix);
            }
            else{
                _gamePaused = false;
                spriteBatch.Begin();
            }


            _stars.Draw(gameTime, spriteBatch);
            _asteroids.Draw(gameTime, spriteBatch);
            _player.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            _explosions.Draw(gameTime, spriteBatch);
        }

        public void CheckForPlayerCollision(){
            // the particle that the player collided with
            Particle p;


            if(_asteroids.CheckForPlayerCollision(_player.Hitbox, out p)){
                _player.TakeDamage(100);
                _isZooming = true;
                _zoomTimer = 0;
                _collidedParticle = p;
                _zoomOrigin = p.Position;
                _zoomScale = 1.0f;

                _explosions.PlaceExplosion(p.Position);
                _sounds.PlayDamageSound();
            }


        }

    }
}