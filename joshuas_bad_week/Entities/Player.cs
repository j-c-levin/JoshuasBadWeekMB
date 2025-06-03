using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using joshuas_bad_week.Config;
using joshuas_bad_week.Effects;

namespace joshuas_bad_week.Entities
{
    /// <summary>
    /// Player entity that handles movement, rotation, and rendering
    /// </summary>
    public class Player
    {
        private Vector2 _position;
        private Vector2 _velocity;
        private float _rotation;
        private Texture2D _texture;
        private Rectangle _bounds;
        private Vector2 _lastPosition;
        private float _trailTimer;

        public Vector2 Position => _position;
        public float Rotation => _rotation;
        public int Health { get; private set; }
        public Rectangle Bounds => _bounds;

        public Player(Vector2 startPosition)
        {
            _position = startPosition;
            _lastPosition = startPosition;
            _velocity = Vector2.Zero;
            _rotation = 0f;
            Health = GameConfig.InitialHealth;
            _trailTimer = 0f;

            // Set up collision bounds
            _bounds = new Rectangle(
                (int)_position.X - GameConfig.PlayerSize / 2,
                (int)_position.Y - GameConfig.PlayerSize / 2,
                GameConfig.PlayerSize,
                GameConfig.PlayerSize
            );
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            // Create a 1x1 white pixel texture for the square
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            Vector2 inputDirection = GetInputDirection(keyboardState);

            if (inputDirection != Vector2.Zero)
            {
                // Normalize diagonal movement to prevent faster diagonal speed
                if (inputDirection.X != 0 && inputDirection.Y != 0)
                {
                    inputDirection *= GameConfig.DiagonalMovementNormalizer;
                }

                // Calculate rotation to face movement direction
                _rotation = (float)Math.Atan2(inputDirection.Y, inputDirection.X);

                // Update velocity
                _velocity = inputDirection * GameConfig.PlayerSpeed;
            }
            else
            {
                _velocity = Vector2.Zero;
            }

            // Update position
            Vector2 newPosition = _position + _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply boundary constraints
            newPosition = ApplyBoundaryConstraints(newPosition);

            _lastPosition = _position;
            _position = newPosition;

            // Update collision bounds
            _bounds.X = (int)_position.X - GameConfig.PlayerSize / 2;
            _bounds.Y = (int)_position.Y - GameConfig.PlayerSize / 2;
        }

        private Vector2 GetInputDirection(KeyboardState keyboardState)
        {
            Vector2 direction = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.Down))
                direction.Y += 1;
            if (keyboardState.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.Right))
                direction.X += 1;

            return direction;
        }

        private Vector2 ApplyBoundaryConstraints(Vector2 newPosition)
        {
            float halfSize = GameConfig.PlayerSize / 2f;

            // Constrain X position
            if (newPosition.X - halfSize < 0)
                newPosition.X = halfSize;
            else if (newPosition.X + halfSize > GameConfig.ScreenWidth)
                newPosition.X = GameConfig.ScreenWidth - halfSize;

            // Constrain Y position
            if (newPosition.Y - halfSize < 0)
                newPosition.Y = halfSize;
            else if (newPosition.Y + halfSize > GameConfig.ScreenHeight)
                newPosition.Y = GameConfig.ScreenHeight - halfSize;

            return newPosition;
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public void Draw(SpriteBatch spriteBatch, VisualEffects visualEffects, ParticleSystem particleSystem, GameTime gameTime)
        {
            if (_texture != null)
            {
                // Add trail effect when moving
                if (_velocity.Length() > 10f)
                {
                    _trailTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_trailTimer >= 0.05f)
                    {
                        Color healthColor = visualEffects.GetHealthBasedColor(Health, GameConfig.InitialHealth, GameConfig.PlayerColor);
                        particleSystem.AddPlayerTrail(_position, healthColor);
                        _trailTimer = 0f;
                    }
                }

                Vector2 origin = new Vector2(0.5f, 0.5f);
                Rectangle destinationRectangle = new Rectangle(
                    (int)_position.X,
                    (int)_position.Y,
                    GameConfig.PlayerSize,
                    GameConfig.PlayerSize
                );

                // Get health-based color
                Color playerColor = visualEffects.GetHealthBasedColor(Health, GameConfig.InitialHealth, GameConfig.PlayerColor);

                // Draw glow effect - use rotation only when moving to avoid initial artifacts
                float glowIntensity = Health <= 3 ? GameConfig.PlayerLowHealthGlowIntensity : GameConfig.PlayerGlowIntensity;
                visualEffects.DrawGlowCentered(spriteBatch, _position, GameConfig.PlayerSize, GameConfig.PlayerSize, playerColor, GameConfig.PlayerGlowSize, glowIntensity);

                spriteBatch.Draw(
                    _texture,
                    destinationRectangle,
                    null,
                    playerColor,
                    _rotation,
                    origin,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
