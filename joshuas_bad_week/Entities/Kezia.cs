using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using joshuas_bad_week.Config;
using joshuas_bad_week.Effects;

namespace joshuas_bad_week.Entities
{
    /// <summary>
    /// Kezia enemy entity - a pill/capsule shaped enemy that tracks the player
    /// </summary>
    public class Kezia
    {
        private Vector2 _position;
        private Vector2 _velocity;
        private float _rotation;
        private float _lifeTimer;
        private bool _isTrackingPlayer;
        private Texture2D _texture;
        private Rectangle _bounds;
        
        public Vector2 Position => _position;
        public float Rotation => _rotation;
        public bool IsAlive { get; private set; }
        public Rectangle Bounds => _bounds;
        
        public Kezia(Vector2 startPosition, float initialRotation = 0f)
        {
            _position = startPosition;
            _velocity = Vector2.Zero;
            _rotation = initialRotation;
            _lifeTimer = 0f;
            _isTrackingPlayer = true;
            IsAlive = true;
            
            UpdateBounds();
        }
        
        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            // Create a 1x1 white pixel texture for the capsule
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
        }
        
        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            if (!IsAlive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _lifeTimer += deltaTime;
            
            // Check if we should stop tracking the player
            if (_lifeTimer >= GameConfig.KeziaTrackingDuration)
            {
                _isTrackingPlayer = false;
            }
            
            // Update rotation and velocity
            if (_isTrackingPlayer)
            {
                // Calculate desired direction to player
                Vector2 directionToPlayer = playerPosition - _position;
                if (directionToPlayer != Vector2.Zero)
                {
                    directionToPlayer.Normalize();
                    float targetRotation = (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X);
                    
                    // Smoothly rotate towards target
                    float rotationDifference = MathHelper.WrapAngle(targetRotation - _rotation);
                    float maxRotationChange = GameConfig.KeziaTurnRate * deltaTime;
                    
                    if (Math.Abs(rotationDifference) <= maxRotationChange)
                    {
                        _rotation = targetRotation;
                    }
                    else
                    {
                        _rotation += Math.Sign(rotationDifference) * maxRotationChange;
                    }
                }
            }
            
            // Update velocity based on current rotation
            _velocity = new Vector2(
                (float)Math.Cos(_rotation) * GameConfig.KeziaSpeed,
                (float)Math.Sin(_rotation) * GameConfig.KeziaSpeed
            );
            
            // Update position
            _position += _velocity * deltaTime;
            
            // Update collision bounds
            UpdateBounds();
            
            // Check if enemy has moved off screen and should be destroyed
            // Only destroy if no longer tracking player (has had chance to move onto screen)
            if (IsOffScreen() && !_isTrackingPlayer)
            {
                IsAlive = false;
            }
        }
        
        private void UpdateBounds()
        {
            _bounds = new Rectangle(
                (int)(_position.X - GameConfig.KeziaWidth / 2),
                (int)(_position.Y - GameConfig.KeziaHeight / 2),
                GameConfig.KeziaWidth,
                GameConfig.KeziaHeight
            );
        }
        
        private bool IsOffScreen()
        {
            float margin = Math.Max(GameConfig.KeziaWidth, GameConfig.KeziaHeight);
            return _position.X < -margin ||
                   _position.X > GameConfig.ScreenWidth + margin ||
                   _position.Y < -margin ||
                   _position.Y > GameConfig.ScreenHeight + margin;
        }
        
        public bool CheckCollision(Rectangle playerBounds)
        {
            return IsAlive && _bounds.Intersects(playerBounds);
        }
        
        public void Destroy()
        {
            IsAlive = false;
        }
        
        public void Draw(SpriteBatch spriteBatch, VisualEffects visualEffects, ParticleSystem particleSystem, GameTime gameTime)
        {
            if (!IsAlive || _texture == null) return;
            
            Vector2 origin = new Vector2(0.5f, 0.5f);
            Rectangle destinationRectangle = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                GameConfig.KeziaWidth,
                GameConfig.KeziaHeight
            );
            
            // Add menacing glow effect that rotates with the entity
            visualEffects.DrawGlowRotated(spriteBatch, _position, GameConfig.KeziaWidth, GameConfig.KeziaHeight, _rotation, GameConfig.KeziaColor, GameConfig.KeziaGlowSize, GameConfig.KeziaGlowIntensity);
            
            // Add pulsing effect
            visualEffects.DrawPulse(spriteBatch, _position, GameConfig.KeziaWidth, GameConfig.KeziaColor, gameTime, 3.0f);
            
            // Add trailing particles when moving
            if (_velocity.Length() > 10f)
            {
                particleSystem.AddPlayerTrail(_position, GameConfig.KeziaColor * 0.6f);
            }
            
            // Draw the pill/capsule shape
            spriteBatch.Draw(
                _texture,
                destinationRectangle,
                null,
                GameConfig.KeziaColor,
                _rotation,
                origin,
                SpriteEffects.None,
                0f
            );
        }
    }
}
