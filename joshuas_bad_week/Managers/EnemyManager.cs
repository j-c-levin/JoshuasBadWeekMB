using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using joshuas_bad_week.Config;
using joshuas_bad_week.Entities;
using joshuas_bad_week.Effects;

namespace joshuas_bad_week.Managers
{
    /// <summary>
    /// Manages all enemies (Kezia and Joel) including spawning, updating, and collision detection
    /// </summary>
    public class EnemyManager
    {
        private List<Kezia> _keziaEnemies;
        private List<Joel> _joelEnemies;
        private List<Card> _cards;
        private Random _random;
        private float _spawnTimer;
        private float _gameTime;
        private GraphicsDevice _graphicsDevice;
        private List<Vector2> _spawnPositions; // Track spawn positions for effects
        private List<SpawnWarning> _spawnWarnings; // Track pre-spawn warnings
        private List<SpawnWarning.EnemySpawnType> _spawnTypes; // Track enemy types for correct colors
        
        public IReadOnlyList<Kezia> KeziaEnemies => _keziaEnemies;
        public IReadOnlyList<Joel> JoelEnemies => _joelEnemies;
        public IReadOnlyList<Card> Cards => _cards;
        public int TotalEnemyCount => _keziaEnemies.Count(e => e.IsAlive) + _joelEnemies.Count(e => e.IsAlive);
        
        public EnemyManager()
        {
            _keziaEnemies = new List<Kezia>();
            _joelEnemies = new List<Joel>();
            _cards = new List<Card>();
            _random = new Random();
            _spawnTimer = 0f;
            _gameTime = 0f;
            _spawnPositions = new List<Vector2>();
            _spawnWarnings = new List<SpawnWarning>();
            _spawnTypes = new List<SpawnWarning.EnemySpawnType>();
        }
        
        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            // Load content for all existing enemies
            foreach (var enemy in _keziaEnemies)
            {
                enemy.LoadContent(graphicsDevice);
            }
            
            foreach (var enemy in _joelEnemies)
            {
                enemy.LoadContent(graphicsDevice);
            }
            
            foreach (var card in _cards)
            {
                card.LoadContent(graphicsDevice);
            }
        }
        
        public void Update(GameTime gameTime, Vector2 playerPosition, bool isGameActive)
        {
            if (!isGameActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _gameTime += deltaTime;
            _spawnTimer += deltaTime;
            
            // Update spawn warnings
            for (int i = _spawnWarnings.Count - 1; i >= 0; i--)
            {
                _spawnWarnings[i].Update(deltaTime);
                if (_spawnWarnings[i].IsComplete)
                {
                    // Spawn the enemy when warning completes
                    if (_spawnWarnings[i].EnemyType == SpawnWarning.EnemySpawnType.Kezia)
                    {
                        SpawnKeziaAtPosition(_spawnWarnings[i].SpawnPosition);
                    }
                    else
                    {
                        SpawnJoelAtPosition(_spawnWarnings[i].SpawnPosition, _spawnWarnings[i].SpawnSide);
                    }
                    _spawnWarnings.RemoveAt(i);
                }
            }
            
            // Spawn new enemies based on difficulty curve
            float currentSpawnInterval = CalculateCurrentSpawnInterval();
            if (_spawnTimer >= currentSpawnInterval)
            {
                InitiateSpawnWarning();
                _spawnTimer = 0f;
            }
            
            // Update all Kezia enemies
            for (int i = _keziaEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _keziaEnemies[i];
                if (enemy.IsAlive)
                {
                    enemy.Update(gameTime, playerPosition);
                    // Remove immediately if it became dead during update
                    if (!enemy.IsAlive)
                    {
                        _keziaEnemies.RemoveAt(i);
                    }
                }
                else
                {
                    // Remove dead enemies immediately
                    _keziaEnemies.RemoveAt(i);
                }
            }
            
            // Update all Joel enemies
            for (int i = _joelEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _joelEnemies[i];
                if (enemy.IsAlive)
                {
                    enemy.Update(gameTime, playerPosition, _cards, _graphicsDevice);
                    // Remove immediately if it became dead during update
                    if (!enemy.IsAlive)
                    {
                        _joelEnemies.RemoveAt(i);
                    }
                }
                else
                {
                    // Remove dead enemies immediately
                    _joelEnemies.RemoveAt(i);
                }
            }
            
            // Update all cards
            for (int i = _cards.Count - 1; i >= 0; i--)
            {
                var card = _cards[i];
                if (card.IsAlive)
                {
                    card.Update(gameTime);
                    // Remove immediately if it became dead during update
                    if (!card.IsAlive)
                    {
                        _cards.RemoveAt(i);
                    }
                }
                else
                {
                    // Remove dead cards immediately
                    _cards.RemoveAt(i);
                }
            }
        }
        
        private float CalculateCurrentSpawnInterval()
        {
            // Calculate difficulty progression from initial to minimum spawn interval
            float difficultyProgress = Math.Min(_gameTime / GameConfig.DifficultyRampDuration, 1.0f);
            
            // Use smooth curve for difficulty progression (ease-in)
            difficultyProgress = difficultyProgress * difficultyProgress;
            
            return MathHelper.Lerp(
                GameConfig.InitialSpawnInterval,
                GameConfig.MinSpawnInterval,
                difficultyProgress
            );
        }
        
        private void InitiateSpawnWarning()
        {
            // Randomly choose between Kezia and Joel (50/50 chance)
            if (_random.Next(2) == 0)
            {
                InitiateKeziaSpawnWarning();
            }
            else
            {
                InitiateJoelSpawnWarning();
            }
        }
        
        private void InitiateKeziaSpawnWarning()
        {
            Vector2 spawnPosition = GetRandomSpawnPosition(GameConfig.KeziaSpawnDistance);
            Vector2 warningPosition = GetScreenEdgePosition(spawnPosition);
            
            var warning = new SpawnWarning(
                spawnPosition, 
                warningPosition, 
                SpawnWarning.EnemySpawnType.Kezia,
                1.0f // 1 second warning time
            );
            _spawnWarnings.Add(warning);
        }
        
        private void InitiateJoelSpawnWarning()
        {
            int spawnSide = _random.Next(4);
            Vector2 spawnPosition = GetRandomSpawnPositionForSide(spawnSide, GameConfig.JoelSpawnDistance);
            Vector2 warningPosition = GetScreenEdgePosition(spawnPosition);
            
            var warning = new SpawnWarning(
                spawnPosition, 
                warningPosition, 
                SpawnWarning.EnemySpawnType.Joel,
                1.0f, // 1 second warning time
                spawnSide
            );
            _spawnWarnings.Add(warning);
        }
        
        private Vector2 GetScreenEdgePosition(Vector2 spawnPosition)
        {
            // Convert off-screen spawn position to nearest screen edge position
            Vector2 edgePosition = spawnPosition;
            
            if (spawnPosition.X < 0) // Left edge
                edgePosition.X = 5;
            else if (spawnPosition.X > GameConfig.ScreenWidth) // Right edge
                edgePosition.X = GameConfig.ScreenWidth - 5;
                
            if (spawnPosition.Y < 0) // Top edge
                edgePosition.Y = 5;
            else if (spawnPosition.Y > GameConfig.ScreenHeight) // Bottom edge
                edgePosition.Y = GameConfig.ScreenHeight - 5;
                
            return edgePosition;
        }
        
        private void SpawnKeziaAtPosition(Vector2 spawnPosition)
        {
            // Calculate initial rotation to point toward screen center
            Vector2 screenCenter = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
            Vector2 directionToCenter = screenCenter - spawnPosition;
            float initialRotation = (float)Math.Atan2(directionToCenter.Y, directionToCenter.X);
            
            var newKezia = new Kezia(spawnPosition, initialRotation);
            
            if (_graphicsDevice != null)
            {
                newKezia.LoadContent(_graphicsDevice);
            }
            
            _keziaEnemies.Add(newKezia);
            _spawnPositions.Add(GetScreenEdgePosition(spawnPosition)); // Use visible position for effects
            _spawnTypes.Add(SpawnWarning.EnemySpawnType.Kezia); // Track enemy type for correct color
        }
        
        private void SpawnJoelAtPosition(Vector2 spawnPosition, int spawnSide)
        {
            var newJoel = new Joel(spawnPosition, spawnSide);
            
            if (_graphicsDevice != null)
            {
                newJoel.LoadContent(_graphicsDevice);
            }
            
            _joelEnemies.Add(newJoel);
            _spawnPositions.Add(GetScreenEdgePosition(spawnPosition)); // Use visible position for effects
            _spawnTypes.Add(SpawnWarning.EnemySpawnType.Joel); // Track enemy type for correct color
        }
        
        private Vector2 GetRandomSpawnPosition(float spawnDistance)
        {
            // Choose a random side of the screen to spawn from
            int side = _random.Next(4); // 0=top, 1=right, 2=bottom, 3=left
            return GetRandomSpawnPositionForSide(side, spawnDistance);
        }
        
        private Vector2 GetRandomSpawnPositionForSide(int side, float spawnDistance)
        {
            return side switch
            {
                0 => new Vector2( // Top
                    _random.Next(0, GameConfig.ScreenWidth),
                    -spawnDistance
                ),
                1 => new Vector2( // Right
                    GameConfig.ScreenWidth + spawnDistance,
                    _random.Next(0, GameConfig.ScreenHeight)
                ),
                2 => new Vector2( // Bottom
                    _random.Next(0, GameConfig.ScreenWidth),
                    GameConfig.ScreenHeight + spawnDistance
                ),
                3 => new Vector2( // Left
                    -spawnDistance,
                    _random.Next(0, GameConfig.ScreenHeight)
                ),
                _ => Vector2.Zero
            };
        }
        
        public bool CheckCollisions(Rectangle playerBounds)
        {
            // Check collisions with Kezia enemies
            foreach (var enemy in _keziaEnemies)
            {
                if (enemy.CheckCollision(playerBounds))
                {
                    enemy.Destroy(); // Remove enemy on collision
                    return true;
                }
            }
            
            // Check collisions with Joel enemies
            foreach (var enemy in _joelEnemies)
            {
                if (enemy.CheckCollision(playerBounds))
                {
                    enemy.Destroy(); // Remove enemy on collision
                    return true;
                }
            }
            
            // Check collisions with cards
            foreach (var card in _cards)
            {
                if (card.CheckCollision(playerBounds))
                {
                    card.Destroy(); // Remove card on collision
                    return true;
                }
            }
            
            return false;
        }
        
        public void LoadContentForNewEnemies(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            // Load content for any enemies that don't have it yet
            foreach (var enemy in _keziaEnemies)
            {
                enemy.LoadContent(graphicsDevice);
            }
            
            foreach (var enemy in _joelEnemies)
            {
                enemy.LoadContent(graphicsDevice);
            }
            
            foreach (var card in _cards)
            {
                card.LoadContent(graphicsDevice);
            }
        }
        
        public void Draw(SpriteBatch spriteBatch, VisualEffects visualEffects, ParticleSystem particleSystem, GameTime gameTime)
        {
            // Draw spawn warnings
            foreach (var warning in _spawnWarnings)
            {
                warning.Draw(spriteBatch, visualEffects, particleSystem, gameTime);
            }
            
            // Add spawn effects for new enemies with correct colors
            for (int i = 0; i < _spawnPositions.Count; i++)
            {
                Vector2 spawnPos = _spawnPositions[i];
                SpawnWarning.EnemySpawnType enemyType = _spawnTypes[i];
                
                // Calculate direction from spawn position toward screen center for enhanced effect
                Vector2 screenCenter = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
                Vector2 directionToCenter = screenCenter - spawnPos;
                
                // Use the correct enemy color based on type
                Color spawnColor = enemyType == SpawnWarning.EnemySpawnType.Kezia ? GameConfig.KeziaColor : GameConfig.JoelColor;
                
                // Use enhanced spawn burst with inward-flowing particles in the correct color
                particleSystem.AddEnhancedSpawnBurst(spawnPos, directionToCenter, spawnColor, 30);
                
                // Add a bright glow effect at the spawn location with correct color
                visualEffects.DrawGlow(spriteBatch, new Rectangle((int)spawnPos.X - 20, (int)spawnPos.Y - 20, 40, 40), spawnColor, 3.0f);
                
                // Add a pulse effect for extra visibility with correct color
                visualEffects.DrawPulse(spriteBatch, spawnPos, 25, spawnColor, gameTime, 8.0f);
            }
            _spawnPositions.Clear();
            _spawnTypes.Clear(); // Clear both lists together
            
            // Draw all Kezia enemies
            foreach (var enemy in _keziaEnemies)
            {
                if (enemy.IsAlive)
                {
                    enemy.Draw(spriteBatch, visualEffects, particleSystem, gameTime);
                }
            }
            
            // Draw all Joel enemies
            foreach (var enemy in _joelEnemies)
            {
                if (enemy.IsAlive)
                {
                    enemy.Draw(spriteBatch, visualEffects, particleSystem, gameTime);
                }
            }
            
            // Draw all cards
            foreach (var card in _cards)
            {
                if (card.IsAlive)
                {
                    card.Draw(spriteBatch, visualEffects, particleSystem, gameTime);
                }
            }
        }
        
        public void Reset()
        {
            _keziaEnemies.Clear();
            _joelEnemies.Clear();
            _cards.Clear();
            _spawnTimer = 0f;
            _gameTime = 0f;
            _spawnWarnings.Clear();
            _spawnPositions.Clear();
            _spawnTypes.Clear();
        }
        
        public void Clear()
        {
            _keziaEnemies.Clear();
            _joelEnemies.Clear();
            _cards.Clear();
            _spawnWarnings.Clear();
            _spawnPositions.Clear();
            _spawnTypes.Clear();
        }
    }
    
    /// <summary>
    /// Represents a pre-spawn warning effect
    /// </summary>
    public class SpawnWarning
    {
        public enum EnemySpawnType
        {
            Kezia,
            Joel
        }
        
        public Vector2 SpawnPosition { get; private set; }
        public Vector2 WarningPosition { get; private set; }
        public EnemySpawnType EnemyType { get; private set; }
        public int SpawnSide { get; private set; }
        public bool IsComplete => _timer >= _duration;
        
        private float _timer;
        private float _duration;
        
        public SpawnWarning(Vector2 spawnPosition, Vector2 warningPosition, EnemySpawnType enemyType, float duration, int spawnSide = -1)
        {
            SpawnPosition = spawnPosition;
            WarningPosition = warningPosition;
            EnemyType = enemyType;
            SpawnSide = spawnSide;
            _duration = duration;
            _timer = 0f;
        }
        
        public void Update(float deltaTime)
        {
            _timer += deltaTime;
        }
        
        public void Draw(SpriteBatch spriteBatch, VisualEffects visualEffects, ParticleSystem particleSystem, GameTime gameTime)
        {
            float progress = _timer / _duration;
            float intensity = Math.Max(0.3f, 1.0f - progress); // Keep minimum visibility
            
            // Warning color based on enemy type - use proper game config colors
            Color warningColor = EnemyType == EnemySpawnType.Kezia ? GameConfig.KeziaColor : GameConfig.JoelColor;
            Color pulseColor = warningColor * intensity;
            
            // Increasing urgency effects as spawn approaches
            float urgency = progress; // 0 to 1 as spawn approaches
            float pulseSpeed = 4.0f + (urgency * 8.0f); // Pulse faster as spawn approaches
            
            // Multi-layered pulsing warning effect at screen edge
            visualEffects.DrawPulse(spriteBatch, WarningPosition, 15 + (urgency * 15), pulseColor, gameTime, pulseSpeed);
            visualEffects.DrawPulse(spriteBatch, WarningPosition, 25 + (urgency * 20), pulseColor * 0.6f, gameTime, pulseSpeed * 0.7f);
            
            // Growing portal/rift effect
            float portalSize = 8 + (progress * 40); // Grows from 8 to 48 pixels
            visualEffects.DrawGlow(spriteBatch, 
                new Rectangle((int)WarningPosition.X - (int)portalSize/2, (int)WarningPosition.Y - (int)portalSize/2, (int)portalSize, (int)portalSize), 
                pulseColor, 2.0f + urgency);
            
            // Add a border effect that contracts as spawn approaches
            float borderSize = 60 - (progress * 30); // Shrinks from 60 to 30 pixels
            Rectangle borderRect = new Rectangle(
                (int)WarningPosition.X - (int)borderSize/2, 
                (int)WarningPosition.Y - (int)borderSize/2, 
                (int)borderSize, 
                (int)borderSize);
            visualEffects.DrawBorder(spriteBatch, borderRect, pulseColor * 0.4f, 3);
            
            // Enhanced particle effects
            if (_timer % 0.08f < 0.04f) // More frequent particles as urgency increases
            {
                Vector2 screenCenter = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
                Vector2 direction = Vector2.Normalize(screenCenter - WarningPosition);
                
                // Create multiple particle streams flowing inward
                for (int i = 0; i < 2 + (int)(urgency * 4); i++) // More particles as spawn approaches
                {
                    Vector2 offset = new Vector2(
                        (_random.Next(-15, 16)), 
                        (_random.Next(-15, 16))
                    );
                    
                    // Some particles flow inward, others swirl around the warning position
                    Vector2 particleVelocity;
                    if (i % 3 == 0) // Every third particle swirls
                    {
                        float angle = (float)(_timer * 3.0f + i * 0.5f);
                        particleVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 30f;
                    }
                    else // Others flow inward
                    {
                        particleVelocity = direction * (40f + urgency * 40f);
                    }
                    
                    particleSystem.AddPlayerTrail(WarningPosition + offset, pulseColor * 0.9f);
                }
                
                // Add some sparks emanating from the warning position
                for (int i = 0; i < (int)(2 + urgency * 3); i++)
                {
                    float sparkAngle = _random.NextSingle() * MathF.PI * 2;
                    Vector2 sparkVelocity = new Vector2(
                        (float)Math.Cos(sparkAngle),
                        (float)Math.Sin(sparkAngle)
                    ) * (20f + _random.NextSingle() * 40f);
                    
                    Vector2 sparkOffset = sparkVelocity * 0.1f; // Start slightly away from center
                    particleSystem.AddPlayerTrail(WarningPosition + sparkOffset, warningColor * 0.7f);
                }
            }
        }
        
        private Random _random = new Random();
    }
}
