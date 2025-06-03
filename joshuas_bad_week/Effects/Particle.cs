using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace joshuas_bad_week.Effects
{
    /// <summary>
    /// Individual particle with position, velocity, color, and lifetime
    /// </summary>
    public class Particle
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Color Color { get; set; }
        public Color OriginalColor { get; set; }  // Store original color for proper fading
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }
        public float Life { get; set; }
        public float MaxLife { get; set; }
        public ParticleType Type { get; set; }
        public bool IsAlive => Life > 0;
        
        public Particle(Vector2 position, Vector2 velocity, Color color, float scale, float life, ParticleType type = ParticleType.Ambient)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            OriginalColor = color;  // Store the original color
            Scale = scale;
            Life = life;
            MaxLife = life;
            Type = type;
            Rotation = 0f;
            RotationSpeed = 0f;
        }
        
        public void Update(GameTime gameTime)
        {
            if (!IsAlive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update position
            Position += Velocity * deltaTime;
            
            // Update rotation
            Rotation += RotationSpeed * deltaTime;
            
            // Update life
            Life -= deltaTime;
            
            // Fade out over time - use original color and apply life ratio as alpha
            float lifeRatio = Life / MaxLife;
            Color = OriginalColor * lifeRatio;
        }
        
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            if (!IsAlive) return;
            
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            
            spriteBatch.Draw(
                texture,
                Position,
                null,
                Color,
                Rotation,
                origin,
                Scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
