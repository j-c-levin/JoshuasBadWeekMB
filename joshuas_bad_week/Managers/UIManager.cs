using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using joshuas_bad_week.Config;
using joshuas_bad_week.Entities;
using joshuas_bad_week.Effects;

namespace joshuas_bad_week.Managers
{
    /// <summary>
    /// Manages UI rendering including health, timer, and win message
    /// </summary>
    public class UIManager
    {
        private SpriteFont _font;
        
        public void LoadContent(SpriteFont font)
        {
            _font = font;
        }
        
        public void Draw(SpriteBatch spriteBatch, Player player, GameStateManager gameStateManager, EnemyManager enemyManager, VisualEffects visualEffects, GameTime gameTime)
        {
            if (_font == null) return;
            
            // Draw health bar instead of just text
            DrawHealthBar(spriteBatch, player, visualEffects, gameTime);
            
            // Draw timer text with color changes
            DrawTimer(spriteBatch, gameStateManager, visualEffects);
            
            // Draw enemy count (top-left, below health)
            if (enemyManager != null)
            {
                string enemyText = $"Enemies: {enemyManager.TotalEnemyCount}";
                Vector2 enemyPosition = new Vector2(10, 60);
                spriteBatch.DrawString(_font, enemyText, enemyPosition, GameConfig.UITextColor);
            }
            
            // Draw win message (center) if game is won
            if (gameStateManager.IsGameWon)
            {
                DrawWinMessage(spriteBatch, visualEffects, gameTime);
            }
            
            // Draw game over message (center) if game is over
            if (gameStateManager.IsGameOver)
            {
                DrawGameOverMessage(spriteBatch, visualEffects, gameTime);
            }
        }
        
        private void DrawHealthBar(SpriteBatch spriteBatch, Player player, VisualEffects visualEffects, GameTime gameTime)
        {
            // Health bar background
            Rectangle healthBarBg = new Rectangle(10, 10, 200, 20);
            visualEffects.DrawBorder(spriteBatch, healthBarBg, GameConfig.HealthBarBackgroundColor, 2);
            
            // Health bar fill
            float healthRatio = (float)player.Health / GameConfig.InitialHealth;
            int fillWidth = (int)(196 * healthRatio); // 196 = 200 - 4 (border)
            Rectangle healthBarFill = new Rectangle(12, 12, fillWidth, 16);
            
            Color healthColor = visualEffects.GetHealthBasedColor(player.Health, GameConfig.InitialHealth, GameConfig.HealthBarFullColor);
            
            // Draw health bar with glow when low
            if (player.Health <= 3)
            {
                visualEffects.DrawGlow(spriteBatch, healthBarFill, healthColor, 1.5f);
                visualEffects.DrawPulse(spriteBatch, new Vector2(healthBarFill.Center.X, healthBarFill.Center.Y), 30, healthColor, gameTime, 4.0f);
            }
            
            // Fill the health bar (this will be drawn with a solid color)
            // We'll use a simple approach since we don't have a filled rectangle method
            for (int y = healthBarFill.Y; y < healthBarFill.Y + healthBarFill.Height; y++)
            {
                Rectangle line = new Rectangle(healthBarFill.X, y, healthBarFill.Width, 1);
                visualEffects.DrawBorder(spriteBatch, line, healthColor, 1);
            }
            
            // Health text
            string healthText = $"Health: {player.Health}/{GameConfig.InitialHealth}";
            Vector2 healthTextPos = new Vector2(220, 12);
            spriteBatch.DrawString(_font, healthText, healthTextPos, GameConfig.UITextColor);
        }
        
        private void DrawTimer(SpriteBatch spriteBatch, GameStateManager gameStateManager, VisualEffects visualEffects)
        {
            string timerText = $"Time: {gameStateManager.TimeRemainingSeconds}";
            Vector2 timerTextSize = _font.MeasureString(timerText);
            Vector2 timerPosition = new Vector2(GameConfig.ScreenWidth - timerTextSize.X - 10, 10);
            
            // Change color based on remaining time
            Color timerColor = gameStateManager.TimeRemainingSeconds <= 30 ? GameConfig.TimerWarningColor : 
                              gameStateManager.TimeRemainingSeconds <= 60 ? GameConfig.TimerCautionColor : 
                              GameConfig.UITextColor;
            
            spriteBatch.DrawString(_font, timerText, timerPosition, timerColor);
        }
        
        private void DrawWinMessage(SpriteBatch spriteBatch, VisualEffects visualEffects, GameTime gameTime)
        {
            string winText = "You Win!";
            Vector2 winTextSize = _font.MeasureString(winText);
            Vector2 winPosition = new Vector2(
                (GameConfig.ScreenWidth - winTextSize.X) / 2f,
                (GameConfig.ScreenHeight - winTextSize.Y) / 2f
            );
            
            // Add celebration effects
            Rectangle textBounds = new Rectangle((int)winPosition.X - 10, (int)winPosition.Y - 10, 
                                                (int)winTextSize.X + 20, (int)winTextSize.Y + 20);
            visualEffects.DrawGlow(spriteBatch, textBounds, GameConfig.WinMessageColor, 2.0f);
            visualEffects.DrawPulse(spriteBatch, new Vector2(textBounds.Center.X, textBounds.Center.Y), 100, GameConfig.WinMessageColor, gameTime, 2.0f);
            
            spriteBatch.DrawString(_font, winText, winPosition, GameConfig.WinMessageColor);
        }
        
        private void DrawGameOverMessage(SpriteBatch spriteBatch, VisualEffects visualEffects, GameTime gameTime)
        {
            string gameOverText = "Game Over!";
            Vector2 gameOverTextSize = _font.MeasureString(gameOverText);
            Vector2 gameOverPosition = new Vector2(
                (GameConfig.ScreenWidth - gameOverTextSize.X) / 2f,
                (GameConfig.ScreenHeight - gameOverTextSize.Y) / 2f
            );
            
            // Add dramatic effects
            Rectangle textBounds = new Rectangle((int)gameOverPosition.X - 10, (int)gameOverPosition.Y - 10, 
                                                (int)gameOverTextSize.X + 20, (int)gameOverTextSize.Y + 20);
            visualEffects.DrawGlow(spriteBatch, textBounds, GameConfig.GameOverColor, 2.0f);
            visualEffects.DrawPulse(spriteBatch, new Vector2(textBounds.Center.X, textBounds.Center.Y), 80, GameConfig.GameOverColor, gameTime, 3.0f);
            
            spriteBatch.DrawString(_font, gameOverText, gameOverPosition, GameConfig.GameOverColor);
        }
    }
}
