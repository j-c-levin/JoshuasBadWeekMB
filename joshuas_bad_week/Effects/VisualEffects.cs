using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using joshuas_bad_week.Config;

namespace joshuas_bad_week.Effects
{
    /// <summary>
    /// Handles screen effects like shake, glow, and background rendering
    /// </summary>
    public class VisualEffects
    {
        private Vector2 _screenShakeOffset;
        private float _screenShakeIntensity;
        private float _screenShakeDuration;
        private Random _random;
        private Texture2D _pixelTexture;
        
        public Vector2 ScreenShakeOffset => _screenShakeOffset;
        
        public VisualEffects()
        {
            _random = new Random();
            _screenShakeOffset = Vector2.Zero;
            _screenShakeIntensity = 0f;
            _screenShakeDuration = 0f;
        }
        
        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            // Create a 1x1 white pixel texture for drawing shapes
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }
        
        public void Update(GameTime gameTime)
        {
            UpdateScreenShake(gameTime);
        }
        
        private void UpdateScreenShake(GameTime gameTime)
        {
            if (_screenShakeDuration > 0)
            {
                _screenShakeDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if (_screenShakeDuration > 0)
                {
                    // Generate random shake offset
                    _screenShakeOffset = new Vector2(
                        (_random.NextSingle() - 0.5f) * _screenShakeIntensity,
                        (_random.NextSingle() - 0.5f) * _screenShakeIntensity
                    );
                }
                else
                {
                    _screenShakeOffset = Vector2.Zero;
                    _screenShakeIntensity = 0f;
                }
            }
        }
        
        public void TriggerScreenShake(float intensity, float duration)
        {
            _screenShakeIntensity = intensity;
            _screenShakeDuration = duration;
        }
        
        // Draw animated gradient background
        public void DrawBackground(SpriteBatch spriteBatch, GameTime gameTime, int screenWidth, int screenHeight)
        {
            if (_pixelTexture == null) return;
            
            // Create animated gradient effect
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            
            // Draw multiple layers for depth
            for (int layer = 0; layer < 3; layer++)
            {
                float layerTime = time * (0.5f + layer * 0.2f);
                
                for (int y = 0; y < screenHeight; y += 4)
                {
                    for (int x = 0; x < screenWidth; x += 4)
                    {
                        // Calculate distance from center
                        Vector2 center = new Vector2(screenWidth / 2f, screenHeight / 2f);
                        Vector2 pos = new Vector2(x, y);
                        float distance = Vector2.Distance(pos, center);
                        float maxDistance = Vector2.Distance(Vector2.Zero, center);
                        float normalizedDistance = distance / maxDistance;
                        
                        // Create animated color based on position and time
                        float colorValue = (float)(Math.Sin(layerTime + normalizedDistance * 3) * 0.1f + 0.1f);
                        
                        Color baseColor = layer switch
                        {
                            0 => new Color(0.05f, 0.05f, 0.15f), // Deep blue
                            1 => new Color(0.1f, 0.05f, 0.2f),  // Purple
                            _ => new Color(0.15f, 0.1f, 0.25f)  // Lighter purple
                        };
                        
                        Color finalColor = baseColor * (colorValue + 0.3f);
                        
                        Rectangle rect = new Rectangle(x, y, 4, 4);
                        spriteBatch.Draw(_pixelTexture, rect, finalColor);
                    }
                }
            }
        }
        
        // Draw glow effect around a rectangle
        public void DrawGlow(SpriteBatch spriteBatch, Rectangle bounds, Color color, float intensity = 1.0f)
        {
            if (_pixelTexture == null) return;
            
            int glowSize = (int)(10 * intensity);
            
            // Draw multiple layers of glow
            for (int i = glowSize; i > 0; i--)
            {
                float alpha = (1.0f - (float)i / glowSize) * 0.3f * intensity;
                Color glowColor = color * alpha;
                
                Rectangle glowRect = new Rectangle(
                    bounds.X - i,
                    bounds.Y - i,
                    bounds.Width + i * 2,
                    bounds.Height + i * 2
                );
                
                // Draw glow border
                DrawBorder(spriteBatch, glowRect, glowColor, 1);
            }
        }
        
        // Draw glow effect centered on a position with specific dimensions
        public void DrawGlowCentered(SpriteBatch spriteBatch, Vector2 center, int width, int height, Color color, int glowSize, float intensity = 1.0f)
        {
            if (_pixelTexture == null) return;
            
            int actualGlowSize = (int)(glowSize * intensity);
            
            // Draw multiple layers of glow
            for (int i = actualGlowSize; i > 0; i--)
            {
                float alpha = (1.0f - (float)i / actualGlowSize) * GameConfig.GlowAlphaMultiplier * intensity;
                Color glowColor = color * alpha;
                
                Rectangle glowRect = new Rectangle(
                    (int)(center.X - width / 2f - i),
                    (int)(center.Y - height / 2f - i),
                    width + i * 2,
                    height + i * 2
                );
                
                // Draw glow border
                DrawBorder(spriteBatch, glowRect, glowColor, 1);
            }
        }
        
        // Draw rotation-aware glow effect using circular approach
        public void DrawGlowRotated(SpriteBatch spriteBatch, Vector2 center, int width, int height, float rotation, Color color, int glowSize, float intensity = 1.0f)
        {
            if (_pixelTexture == null) return;
            
            int actualGlowSize = (int)(glowSize * intensity);
            
            // For rotated entities, use a circular glow approach to avoid rectangular holes
            float maxDimension = Math.Max(width, height);
            float glowRadius = maxDimension / 2f;
            
            // Draw multiple layers of circular glow
            for (int i = actualGlowSize; i > 0; i--)
            {
                float alpha = (1.0f - (float)i / actualGlowSize) * GameConfig.GlowAlphaMultiplier * intensity;
                Color glowColor = color * alpha;
                
                float currentRadius = glowRadius + i;
                DrawCircleOutline(spriteBatch, center, currentRadius, glowColor);
            }
        }
        
        // Draw rotated rectangular glow that follows entity rotation (for player low health warning)
        public void DrawGlowCenteredRotated(SpriteBatch spriteBatch, Vector2 center, int width, int height, float rotation, Color color, int glowSize, float intensity = 1.0f)
        {
            if (_pixelTexture == null) return;
            
            int actualGlowSize = (int)(glowSize * intensity);
            
            // Draw multiple layers of rotated rectangular glow
            for (int i = actualGlowSize; i > 0; i--)
            {
                float alpha = (1.0f - (float)i / actualGlowSize) * GameConfig.GlowAlphaMultiplier * intensity;
                Color glowColor = color * alpha;
                
                // Create expanded rectangle for this glow layer
                int expandedWidth = width + i * 2;
                int expandedHeight = height + i * 2;
                
                // Draw rotated rectangle border
                DrawRotatedRectangleBorder(spriteBatch, center, expandedWidth, expandedHeight, rotation, glowColor);
            }
        }
        
        // Helper method to draw a rotated rectangle border
        private void DrawRotatedRectangleBorder(SpriteBatch spriteBatch, Vector2 center, int width, int height, float rotation, Color color)
        {
            if (_pixelTexture == null) return;
            
            // Calculate the four corners of the rotated rectangle
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;
            
            Vector2[] corners = new Vector2[4]
            {
                new Vector2(-halfWidth, -halfHeight), // Top-left
                new Vector2(halfWidth, -halfHeight),  // Top-right
                new Vector2(halfWidth, halfHeight),   // Bottom-right
                new Vector2(-halfWidth, halfHeight)   // Bottom-left
            };
            
            // Rotate and translate corners
            for (int i = 0; i < 4; i++)
            {
                float cos = (float)Math.Cos(rotation);
                float sin = (float)Math.Sin(rotation);
                
                float rotatedX = corners[i].X * cos - corners[i].Y * sin;
                float rotatedY = corners[i].X * sin + corners[i].Y * cos;
                
                corners[i] = center + new Vector2(rotatedX, rotatedY);
            }
            
            // Draw the four edges of the rectangle
            for (int i = 0; i < 4; i++)
            {
                Vector2 start = corners[i];
                Vector2 end = corners[(i + 1) % 4];
                DrawLine(spriteBatch, start, end, color);
            }
        }
        
        // Helper method to draw a line between two points
        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            if (_pixelTexture == null) return;
            
            Vector2 delta = end - start;
            float distance = delta.Length();
            
            if (distance < 1f) return;
            
            Vector2 direction = delta / distance;
            
            // Draw pixels along the line
            for (float d = 0; d < distance; d += 0.5f)
            {
                Vector2 pos = start + direction * d;
                Rectangle rect = new Rectangle((int)Math.Round(pos.X), (int)Math.Round(pos.Y), 1, 1);
                spriteBatch.Draw(_pixelTexture, rect, color);
            }
        }
        
        // Draw a border around a rectangle
        public void DrawBorder(SpriteBatch spriteBatch, Rectangle bounds, Color color, int thickness = 1)
        {
            if (_pixelTexture == null) return;
            
            // Top
            spriteBatch.Draw(_pixelTexture, new Rectangle(bounds.X, bounds.Y, bounds.Width, thickness), color);
            // Bottom
            spriteBatch.Draw(_pixelTexture, new Rectangle(bounds.X, bounds.Bottom - thickness, bounds.Width, thickness), color);
            // Left
            spriteBatch.Draw(_pixelTexture, new Rectangle(bounds.X, bounds.Y, thickness, bounds.Height), color);
            // Right
            spriteBatch.Draw(_pixelTexture, new Rectangle(bounds.Right - thickness, bounds.Y, thickness, bounds.Height), color);
        }
        
        // Draw pulsing effect
        public void DrawPulse(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, GameTime gameTime, float speed = 2.0f)
        {
            if (_pixelTexture == null) return;
            
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            float pulse = (float)(Math.Sin(time * speed) * 0.5f + 0.5f);
            
            float currentRadius = radius * (0.8f + pulse * 0.4f);
            Color pulseColor = color * (0.3f + pulse * 0.4f);
            
            // Draw concentric circles for pulse effect
            for (int i = 0; i < 5; i++)
            {
                float ringRadius = currentRadius * (1.0f - i * 0.2f);
                DrawCircleOutline(spriteBatch, center, ringRadius, pulseColor * (1.0f - i * 0.2f));
            }
        }
        
        // Draw circle outline (approximated with rectangles)
        private void DrawCircleOutline(SpriteBatch spriteBatch, Vector2 center, float radius, Color color)
        {
            if (_pixelTexture == null) return;
            
            // Use a more sophisticated circle drawing algorithm to prevent artifacts
            int segments = Math.Max(32, (int)(radius * 0.5f)); // More segments for better approximation
            float angleStep = MathF.PI * 2 / segments;
            
            // Use Bresenham-like approach for better circle approximation
            for (int i = 0; i < segments; i++)
            {
                float angle = i * angleStep;
                
                // Calculate multiple points around this angle to fill gaps
                for (float offset = -0.5f; offset <= 0.5f; offset += 0.5f)
                {
                    float adjustedAngle = angle + (offset * angleStep * 0.3f);
                    Vector2 pos = center + new Vector2(
                        (float)Math.Cos(adjustedAngle) * radius,
                        (float)Math.Sin(adjustedAngle) * radius
                    );
                    
                    // Round to nearest pixel to prevent sub-pixel gaps
                    Vector2 roundedPos = new Vector2(
                        MathF.Round(pos.X),
                        MathF.Round(pos.Y)
                    );
                    
                    Rectangle rect = new Rectangle((int)roundedPos.X, (int)roundedPos.Y, 1, 1);
                    spriteBatch.Draw(_pixelTexture, rect, color);
                }
            }
        }
        
        // Draw health-based color for player
        public Color GetHealthBasedColor(int currentHealth, int maxHealth, Color baseColor)
        {
            float healthRatio = (float)currentHealth / maxHealth;
            
            if (healthRatio > 0.6f)
                return baseColor; // Full health color
            else if (healthRatio > 0.3f)
                return Color.Lerp(GameConfig.HealthMediumColor, baseColor, (healthRatio - 0.3f) / 0.3f); // Orange transition
            else
                return Color.Lerp(GameConfig.HealthLowColor, GameConfig.HealthMediumColor, healthRatio / 0.3f); // Red when low
        }
    }
}

