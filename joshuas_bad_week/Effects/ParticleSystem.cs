using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using joshuas_bad_week.Config;

namespace joshuas_bad_week.Effects
{
    /// <summary>
    /// Manages collections of particles for various visual effects
    /// </summary>
    public class ParticleSystem
    {
        private List<Particle> _particles;
        private Texture2D _particleTexture;
        private Random _random;
        
        public ParticleSystem()
        {
            _particles = new List<Particle>();
            _random = new Random();
        }
        
        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            // Create a small circular particle texture
            int size = GameConfig.ParticleTextureSize;
            _particleTexture = new Texture2D(graphicsDevice, size, size);
            Color[] data = new Color[size * size];
            
            // Create a circular gradient
            Vector2 center = new Vector2(size / 2f, size / 2f);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Vector2 pos = new Vector2(i, j);
                    float distance = Vector2.Distance(pos, center);
                    float alpha = Math.Max(0, 1f - (distance / (size / 2f)));
                    data[i * size + j] = Color.White * alpha;
                }
            }
            
            _particleTexture.SetData(data);
        }
        
        public void Update(GameTime gameTime)
        {
            // Update all particles
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                _particles[i].Update(gameTime);
                
                // Remove dead particles
                if (!_particles[i].IsAlive)
                {
                    _particles.RemoveAt(i);
                }
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_particleTexture == null) return;
            
            foreach (var particle in _particles)
            {
                particle.Draw(spriteBatch, _particleTexture);
            }
        }
        
        // Player trail effect
        public void AddPlayerTrail(Vector2 position, Color baseColor)
        {
            for (int i = 0; i < GameConfig.PlayerTrailParticleCount; i++)
            {
                Vector2 velocity = new Vector2(
                    (_random.NextSingle() - 0.5f) * GameConfig.PlayerTrailVelocityRange,
                    (_random.NextSingle() - 0.5f) * GameConfig.PlayerTrailVelocityRange
                );
                
                Color color = baseColor * GameConfig.PlayerTrailAlphaMultiplier;
                float scale = GameConfig.PlayerTrailScaleMin + _random.NextSingle() * (GameConfig.PlayerTrailScaleMax - GameConfig.PlayerTrailScaleMin);
                float life = GameConfig.PlayerTrailLifeMin + _random.NextSingle() * (GameConfig.PlayerTrailLifeMax - GameConfig.PlayerTrailLifeMin);
                
                var particle = new Particle(position, velocity, color, scale, life, ParticleType.PlayerTrail);
                particle.RotationSpeed = (_random.NextSingle() - 0.5f) * GameConfig.PlayerTrailRotationSpeedRange;
                _particles.Add(particle);
            }
        }
        
        // Enemy spawn effect
        public void AddSpawnBurst(Vector2 position, Color color, int count = -1)
        {
            int particleCount = count == -1 ? GameConfig.SpawnBurstParticleCount : count;
            for (int i = 0; i < particleCount; i++)
            {
                float angle = (float)(i * 2 * Math.PI / particleCount);
                float speed = GameConfig.SpawnBurstSpeedMin + _random.NextSingle() * (GameConfig.SpawnBurstSpeedMax - GameConfig.SpawnBurstSpeedMin);
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                
                float scale = GameConfig.SpawnBurstScaleMin + _random.NextSingle() * (GameConfig.SpawnBurstScaleMax - GameConfig.SpawnBurstScaleMin);
                float life = GameConfig.SpawnBurstLifeMin + _random.NextSingle() * (GameConfig.SpawnBurstLifeMax - GameConfig.SpawnBurstLifeMin);
                
                var particle = new Particle(position, velocity, color, scale, life, ParticleType.SpawnBurst);
                particle.RotationSpeed = (_random.NextSingle() - 0.5f) * GameConfig.SpawnBurstRotationSpeedRange;
                _particles.Add(particle);
            }
        }
        
        // Enhanced spawn effect with inward-flowing particles for better visibility
        public void AddEnhancedSpawnBurst(Vector2 edgePosition, Vector2 centerDirection, Color color, int count = 25)
        {
            // First, add the regular radial burst
            AddSpawnBurst(edgePosition, color, count / 2);
            
            // Then add particles that flow toward the screen center for visibility
            for (int i = 0; i < count / 2; i++)
            {
                // Create particles that flow toward screen center but with some spread
                Vector2 baseDirection = Vector2.Normalize(centerDirection);
                float spreadAngle = (_random.NextSingle() - 0.5f) * 0.8f; // Up to 0.8 radians spread
                float cos = (float)Math.Cos(spreadAngle);
                float sin = (float)Math.Sin(spreadAngle);
                Vector2 velocity = new Vector2(
                    baseDirection.X * cos - baseDirection.Y * sin,
                    baseDirection.X * sin + baseDirection.Y * cos
                ) * (80f + _random.NextSingle() * 60f); // Speed between 80-140
                
                float scale = 0.6f + _random.NextSingle() * 0.8f; // Scale between 0.6-1.4
                float life = 1.5f + _random.NextSingle() * 1.0f; // Life between 1.5-2.5 seconds
                
                var particle = new Particle(edgePosition, velocity, color, scale, life, ParticleType.SpawnBurst);
                particle.RotationSpeed = (_random.NextSingle() - 0.5f) * 6f;
                _particles.Add(particle);
            }
        }
        
        // Damage effect
        public void AddDamageEffect(Vector2 position, Color color)
        {
            for (int i = 0; i < GameConfig.DamageEffectParticleCount; i++)
            {
                float angle = _random.NextSingle() * MathF.PI * 2;
                float speed = GameConfig.DamageEffectSpeedMin + _random.NextSingle() * (GameConfig.DamageEffectSpeedMax - GameConfig.DamageEffectSpeedMin);
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                
                float scale = GameConfig.DamageEffectScaleMin + _random.NextSingle() * (GameConfig.DamageEffectScaleMax - GameConfig.DamageEffectScaleMin);
                float life = GameConfig.DamageEffectLifeMin + _random.NextSingle() * (GameConfig.DamageEffectLifeMax - GameConfig.DamageEffectLifeMin);
                
                var particle = new Particle(position, velocity, color, scale, life, ParticleType.DamageEffect);
                _particles.Add(particle);
            }
        }
        
        // Ambient background particles
        public void AddAmbientParticle(Vector2 screenSize)
        {
            // Only add if we don't have too many ambient particles
            int ambientCount = _particles.Count(p => p.Type == ParticleType.Ambient);
            if (ambientCount >= GameConfig.MaxAmbientParticles) return;
            
            Vector2 position = new Vector2(
                _random.NextSingle() * screenSize.X,
                _random.NextSingle() * screenSize.Y
            );
            
            Vector2 velocity = new Vector2(
                (_random.NextSingle() - 0.5f) * GameConfig.AmbientVelocityRange,
                (_random.NextSingle() - 0.5f) * GameConfig.AmbientVelocityRange
            );
            
            float alpha = GameConfig.AmbientAlphaMin + _random.NextSingle() * (GameConfig.AmbientAlphaMax - GameConfig.AmbientAlphaMin);
            Color color = Color.White * alpha;
            float scale = GameConfig.AmbientScaleMin + _random.NextSingle() * (GameConfig.AmbientScaleMax - GameConfig.AmbientScaleMin);
            float life = GameConfig.AmbientLifeMin + _random.NextSingle() * (GameConfig.AmbientLifeMax - GameConfig.AmbientLifeMin);
            
            var particle = new Particle(position, velocity, color, scale, life, ParticleType.Ambient);
            particle.RotationSpeed = (_random.NextSingle() - 0.5f) * GameConfig.AmbientRotationSpeedRange;
            _particles.Add(particle);
        }
        
        // Card projectile trail
        public void AddCardTrail(Vector2 position, Color color)
        {
            Vector2 velocity = new Vector2(
                (_random.NextSingle() - 0.5f) * GameConfig.CardTrailVelocityRange,
                (_random.NextSingle() - 0.5f) * GameConfig.CardTrailVelocityRange
            );
            
            float scale = GameConfig.CardTrailScaleMin + _random.NextSingle() * (GameConfig.CardTrailScaleMax - GameConfig.CardTrailScaleMin);
            float life = GameConfig.CardTrailLifeMin + _random.NextSingle() * (GameConfig.CardTrailLifeMax - GameConfig.CardTrailLifeMin);
            
            var particle = new Particle(position, velocity, color * GameConfig.CardTrailAlphaMultiplier, scale, life, ParticleType.CardTrail);
            _particles.Add(particle);
        }
        
        public int ParticleCount => _particles.Count;
    }
}
