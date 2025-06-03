using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using joshuas_bad_week.Config;
using joshuas_bad_week.Effects;

namespace joshuas_bad_week.Entities
{
    /// <summary>
    /// Card projectile entity fired by Joel enemies
    /// </summary>
    public class Card
    {
        private Vector2 _position;
        private Vector2 _velocity;
        private float _rotation;
        private Texture2D _texture;
        private Rectangle _bounds;
        private float _trailTimer;
        private float _spinSpeed;
        
        public Vector2 Position => _position;
        public bool IsAlive { get; private set; }
        public Rectangle Bounds => _bounds;
        
        public Card(Vector2 startPosition, float direction)
        {
            _position = startPosition;
            _rotation = direction;
            _trailTimer = 0f;
            _spinSpeed = 8f; // radians per second for spinning animation
            _velocity = new Vector2(
                (float)Math.Cos(direction) * GameConfig.CardSpeed,
                (float)Math.Sin(direction) * GameConfig.CardSpeed
            );
            IsAlive = true;
            
            UpdateBounds();
        }
        
        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            // Create a 1x1 white pixel texture for the card
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
        }
        
        public void Update(GameTime gameTime)
        {
            if (!IsAlive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update spinning animation
            _rotation += _spinSpeed * deltaTime;
            
            // Update position
            _position += _velocity * deltaTime;
            
            // Update collision bounds
            UpdateBounds();
            
            // Check if card has moved off screen and should be destroyed
            if (IsOffScreen())
            {
                IsAlive = false;
            }
        }
        
        private void UpdateBounds()
        {
            _bounds = new Rectangle(
                (int)(_position.X - GameConfig.CardWidth / 2),
                (int)(_position.Y - GameConfig.CardHeight / 2),
                GameConfig.CardWidth,
                GameConfig.CardHeight
            );
        }
        
        private bool IsOffScreen()
        {
            float margin = Math.Max(GameConfig.CardWidth, GameConfig.CardHeight);
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
            
            // Add trail effect from center of card
            _trailTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_trailTimer >= 0.03f)
            {
                // Emit particles from the center of the card (which is already _position)
                particleSystem.AddCardTrail(_position, GameConfig.CardColor);
                _trailTimer = 0f;
            }
            
            Vector2 origin = new Vector2(0.5f, 0.5f);
            Rectangle destinationRectangle = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                GameConfig.CardWidth,
                GameConfig.CardHeight
            );
            
            // Add subtle glow that follows rotation
            visualEffects.DrawGlowRotated(spriteBatch, _position, GameConfig.CardWidth, GameConfig.CardHeight, _rotation, GameConfig.CardColor, GameConfig.CardGlowSize, GameConfig.CardGlowIntensity);
            
            spriteBatch.Draw(
                _texture,
                destinationRectangle,
                null,
                GameConfig.CardColor,
                _rotation,
                origin,
                SpriteEffects.None,
                0f
            );
        }
    }
}
