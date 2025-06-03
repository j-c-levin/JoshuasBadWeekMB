using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using joshuas_bad_week.Config;
using joshuas_bad_week.Effects;

namespace joshuas_bad_week.Entities
{
    /// <summary>
    /// Joel enemy entity - a larger rectangular enemy that approaches, tracks, and fires cards
    /// </summary>
    public class Joel
    {
        public enum JoelState
        {
            Approaching,
            Tracking,
            Retreating
        }
        
        private Vector2 _position;
        private Vector2 _velocity;
        private float _rotation;
        private JoelState _state;
        private float _stateTimer;
        private float _cardFireTimer;
        private Texture2D _texture;
        private Rectangle _bounds;
        private bool _isChargingUp;
        private float _chargeTimer;
        
        // Spawn information for retreat calculation
        private int _spawnSide; // 0=top, 1=right, 2=bottom, 3=left
        private Vector2 _spawnPosition;
        private Vector2 _targetPosition;
        
        public Vector2 Position => _position;
        public float Rotation => _rotation;
        public bool IsAlive { get; private set; }
        public Rectangle Bounds => _bounds;
        public JoelState State => _state;
        
        public Joel(Vector2 startPosition, int spawnSide)
        {
            _position = startPosition;
            _spawnPosition = startPosition;
            _spawnSide = spawnSide;
            _velocity = Vector2.Zero;
            _rotation = 0f;
            _state = JoelState.Approaching;
            _stateTimer = 0f;
            _cardFireTimer = 0f;
            _isChargingUp = false;
            _chargeTimer = 0f;
            IsAlive = true;
            
            // Calculate target position for approach phase
            CalculateTargetPosition();
            
            // Set initial rotation to face inward
            SetApproachRotation();
            
            UpdateBounds();
        }
        
        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            // Create a 1x1 white pixel texture for the rectangle
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
        }
        
        public void Update(GameTime gameTime, Vector2 playerPosition, List<Card> cards, GraphicsDevice graphicsDevice)
        {
            if (!IsAlive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _stateTimer += deltaTime;
            
            switch (_state)
            {
                case JoelState.Approaching:
                    UpdateApproaching(deltaTime);
                    break;
                    
                case JoelState.Tracking:
                    UpdateTracking(deltaTime, playerPosition, cards, graphicsDevice);
                    break;
                    
                case JoelState.Retreating:
                    UpdateRetreating(deltaTime);
                    break;
            }
            
            // Update position
            _position += _velocity * deltaTime;
            
            // Update collision bounds
            UpdateBounds();
        }
        
        private void UpdateApproaching(float deltaTime)
        {
            // Move toward target position
            Vector2 directionToTarget = _targetPosition - _position;
            float distanceToTarget = directionToTarget.Length();
            
            if (distanceToTarget <= 5f) // Close enough to target
            {
                _state = JoelState.Tracking;
                _stateTimer = 0f;
                _velocity = Vector2.Zero;
            }
            else
            {
                // Continue moving toward target
                directionToTarget.Normalize();
                _velocity = directionToTarget * GameConfig.JoelSpeed;
            }
        }
        
        private void UpdateTracking(float deltaTime, Vector2 playerPosition, List<Card> cards, GraphicsDevice graphicsDevice)
        {
            // Stop moving
            _velocity = Vector2.Zero;
            
            // Rotate to face player with SHORT axis (perpendicular to long axis)
            // Joel's short axis should point at the player, so we add 90 degrees to the direction
            Vector2 directionToPlayer = playerPosition - _position;
            if (directionToPlayer != Vector2.Zero)
            {
                directionToPlayer.Normalize();
                float targetRotation = (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X);
                // Add 90 degrees so the short axis points at player instead of long axis
                targetRotation += MathHelper.PiOver2;
                
                // Find the shortest rotation path to avoid 360-degree spins
                float rotationDifference = targetRotation - _rotation;
                
                // Normalize the rotation difference to [-π, π] range
                while (rotationDifference > MathHelper.Pi)
                    rotationDifference -= MathHelper.TwoPi;
                while (rotationDifference < -MathHelper.Pi)
                    rotationDifference += MathHelper.TwoPi;
                
                float maxRotationChange = GameConfig.JoelTurnRate * deltaTime;
                
                if (Math.Abs(rotationDifference) <= maxRotationChange)
                {
                    _rotation = targetRotation;
                }
                else
                {
                    _rotation += Math.Sign(rotationDifference) * maxRotationChange;
                }
            }
            
            // Fire cards periodically with charge-up effect
            _cardFireTimer += deltaTime;
            
            // Start charging up before firing
            if (_cardFireTimer >= GameConfig.JoelCardFireRate - 0.5f && !_isChargingUp)
            {
                _isChargingUp = true;
                _chargeTimer = 0f;
            }
            
            if (_isChargingUp)
            {
                _chargeTimer += deltaTime;
            }
            
            if (_cardFireTimer >= GameConfig.JoelCardFireRate)
            {
                FireCard(cards, graphicsDevice);
                _cardFireTimer = 0f;
                _isChargingUp = false;
                _chargeTimer = 0f;
            }
            
            // Check if tracking duration is over
            if (_stateTimer >= GameConfig.JoelTrackingDuration)
            {
                _state = JoelState.Retreating;
                _stateTimer = 0f;
                SetRetreatRotation();
            }
        }
        
        private void UpdateRetreating(float deltaTime)
        {
            // Move back toward spawn position and beyond
            Vector2 directionToSpawn = _spawnPosition - _position;
            if (directionToSpawn != Vector2.Zero)
            {
                directionToSpawn.Normalize();
                _velocity = directionToSpawn * GameConfig.JoelSpeed;
            }
            
            // Check if we've moved off screen
            if (IsOffScreen())
            {
                IsAlive = false;
            }
        }
        
        private void FireCard(List<Card> cards, GraphicsDevice graphicsDevice)
        {
            // Fire card in the direction Joel's short axis is pointing
            // Since _rotation represents the long axis orientation, subtract 90 degrees to get short axis direction
            float cardDirection = _rotation - MathHelper.PiOver2;
            var card = new Card(_position, cardDirection);
            card.LoadContent(graphicsDevice);
            cards.Add(card);
        }
        
        private void CalculateTargetPosition()
        {
            float distance = GameConfig.JoelApproachDistance;
            
            _targetPosition = _spawnSide switch
            {
                0 => new Vector2(_position.X, distance), // Top
                1 => new Vector2(GameConfig.ScreenWidth - distance, _position.Y), // Right
                2 => new Vector2(_position.X, GameConfig.ScreenHeight - distance), // Bottom
                3 => new Vector2(distance, _position.Y), // Left
                _ => _position
            };
        }
        
        private void SetApproachRotation()
        {
            // Set rotation to minimize the difference when transitioning to tracking
            // Joel's short axis should roughly point toward screen center initially
            Vector2 screenCenter = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
            Vector2 directionToCenter = screenCenter - _position;
            if (directionToCenter != Vector2.Zero)
            {
                directionToCenter.Normalize();
                float centerRotation = (float)Math.Atan2(directionToCenter.Y, directionToCenter.X);
                // Add 90 degrees so the short axis points toward center
                _rotation = centerRotation + MathHelper.PiOver2;
            }
            else
            {
                // Fallback to original logic if position is exactly at center
                _rotation = _spawnSide switch
                {
                    0 => 0f, // Top
                    1 => MathHelper.PiOver2, // Right
                    2 => 0f, // Bottom
                    3 => MathHelper.PiOver2, // Left
                    _ => 0f
                };
            }
        }
        
        private void SetRetreatRotation()
        {
            // Face outward based on spawn side, Joel moves along SHORT axis
            // Same orientation as approach since Joel moves the same way
            _rotation = _spawnSide switch
            {
                0 => 0f, // Top - long axis horizontal, moving up
                1 => MathHelper.PiOver2, // Right - long axis vertical, moving right
                2 => 0f, // Bottom - long axis horizontal, moving down
                3 => MathHelper.PiOver2, // Left - long axis vertical, moving left
                _ => 0f
            };
        }
        
        private void UpdateBounds()
        {
            _bounds = new Rectangle(
                (int)(_position.X - GameConfig.JoelWidth / 2),
                (int)(_position.Y - GameConfig.JoelHeight / 2),
                GameConfig.JoelWidth,
                GameConfig.JoelHeight
            );
        }
        
        private bool IsOffScreen()
        {
            float margin = Math.Max(GameConfig.JoelWidth, GameConfig.JoelHeight);
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
                GameConfig.JoelWidth,
                GameConfig.JoelHeight
            );
            
            // Add intimidating aura that follows rotation
            visualEffects.DrawGlowRotated(spriteBatch, _position, GameConfig.JoelWidth, GameConfig.JoelHeight, _rotation, GameConfig.JoelColor, GameConfig.JoelGlowSize, GameConfig.JoelGlowIntensity);
            
            // Add charging up effect when about to fire
            if (_isChargingUp)
            {
                float chargeIntensity = _chargeTimer / 0.5f; // 0.5 second charge time
                visualEffects.DrawPulse(spriteBatch, _position, GameConfig.JoelWidth, GameConfig.JoelChargingColor, gameTime, 8.0f * chargeIntensity);
                
                // Add charge particles
                if (_chargeTimer > 0.1f)
                {
                    particleSystem.AddPlayerTrail(_position, GameConfig.JoelChargingColor * chargeIntensity);
                }
            }
            
            // Add blue particle effects
            if (_state == JoelState.Tracking)
            {
                particleSystem.AddPlayerTrail(_position, GameConfig.JoelColor * 0.4f);
            }
            
            spriteBatch.Draw(
                _texture,
                destinationRectangle,
                null,
                GameConfig.JoelColor,
                _rotation,
                origin,
                SpriteEffects.None,
                0f
            );
        }
    }
}
