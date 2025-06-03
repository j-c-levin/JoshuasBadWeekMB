using Microsoft.Xna.Framework;

namespace joshuas_bad_week.Config
{
    /// <summary>
    /// Centralized configuration for all game settings
    /// </summary>
    public static class GameConfig
    {
        // Screen Settings
        public static readonly int ScreenWidth = 800;
        public static readonly int ScreenHeight = 600;
        
        // Player Settings
        public static readonly int PlayerSize = 20;
        public static readonly float PlayerSpeed = 200f; // pixels per second
        public static readonly Color PlayerColor = Color.Yellow;
        
        // Game Settings
        public static readonly int GameDurationSeconds = 120;
        public static readonly int InitialHealth = 10;
        
        // UI Settings
        public static readonly Color UITextColor = Color.White;
        public static readonly Vector2 HealthTextPosition = new Vector2(10, 10);
        public static readonly Vector2 WinTextPosition = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
        
        // Effect Colors
        public static readonly Color DamageEffectColor = Color.Red;
        public static readonly Color PlayerLowHealthColor = Color.Red;
        public static readonly Color JoelChargingColor = Color.White;
        public static readonly Color WinMessageColor = Color.Gold;
        public static readonly Color GameOverColor = Color.Red;
        public static readonly Color TimerWarningColor = Color.Red;
        public static readonly Color TimerCautionColor = Color.Orange;
        public static readonly Color HealthLowColor = Color.Red;
        public static readonly Color HealthMediumColor = Color.Orange;
        public static readonly Color HealthBarBackgroundColor = Color.Gray;
        public static readonly Color HealthBarFullColor = Color.Green;
        public static readonly Color BackgroundColor = Color.Black;
        
        // Input Settings
        public static readonly float DiagonalMovementNormalizer = 0.707f; // 1/sqrt(2) to normalize diagonal movement
        
        // Kezia Enemy Settings
        public static readonly int KeziaWidth = 20;
        public static readonly int KeziaHeight = 10;
        public static readonly float KeziaSpeed = 120f; // pixels per second
        public static readonly Color KeziaColor = Color.Red;
        public static readonly float KeziaTrackingDuration = 5.0f; // seconds to track player
        public static readonly float KeziaTurnRate = 2.0f; // radians per second
        public static readonly int KeziaDamage = 1;
        public static readonly float KeziaSpawnDistance = 10f; // pixels outside screen border
        
        // Kezia Spawning Settings
        public static readonly float InitialSpawnInterval = 2.0f; // seconds between spawns initially
        public static readonly float MinSpawnInterval = 0.3f; // fastest spawn rate
        public static readonly float DifficultyRampDuration = 90f; // seconds to reach max difficulty
        
        // Joel Enemy Settings
        public static readonly int JoelWidth = 30;
        public static readonly int JoelHeight = 15;
        public static readonly float JoelSpeed = 100f; // pixels per second
        public static readonly Color JoelColor = Color.DeepSkyBlue;
        public static readonly float JoelApproachDistance = 50f; // pixels from screen edge to stop
        public static readonly float JoelTrackingDuration = 8.0f; // seconds in tracking mode
        public static readonly float JoelTurnRate = 5.0f; // radians per second
        public static readonly float JoelCardFireRate = 1.5f; // seconds between cards
        public static readonly int JoelDamage = 1;
        public static readonly float JoelSpawnDistance = 10f; // pixels outside screen border
        
        // Card Projectile Settings
        public static readonly int CardWidth = 6;
        public static readonly int CardHeight = 4;
        public static readonly float CardSpeed = 150f; // pixels per second
        public static readonly Color CardColor = Color.LightGreen;
        public static readonly int CardDamage = 1;
        
        // Visual Effects Settings
        public static readonly int PlayerGlowSize = 15; // pixels
        public static readonly float PlayerGlowIntensity = 1.5f;
        public static readonly float PlayerLowHealthGlowIntensity = 2.5f;
        
        public static readonly int KeziaGlowSize = 12; // pixels
        public static readonly float KeziaGlowIntensity = 1.8f;
        
        public static readonly int JoelGlowSize = 14; // pixels
        public static readonly float JoelGlowIntensity = 1.4f;
        
        public static readonly int CardGlowSize = 8; // pixels
        public static readonly float CardGlowIntensity = 1.0f;
        
        public static readonly float GlowAlphaMultiplier = 0.4f; // Base alpha for glow effects
        
        // Particle System Settings
        public static readonly int ParticleTextureSize = 4;
        public static readonly int MaxAmbientParticles = 50;
        
        // Player Trail Particles
        public static readonly int PlayerTrailParticleCount = 3;
        public static readonly float PlayerTrailVelocityRange = 20f;
        public static readonly float PlayerTrailScaleMin = 0.5f;
        public static readonly float PlayerTrailScaleMax = 1.0f;
        public static readonly float PlayerTrailLifeMin = 0.5f;
        public static readonly float PlayerTrailLifeMax = 1.0f;
        public static readonly float PlayerTrailRotationSpeedRange = 4f;
        public static readonly float PlayerTrailAlphaMultiplier = 0.8f;
        
        // Enemy Spawn Burst Particles
        public static readonly int SpawnBurstParticleCount = 15;
        public static readonly float SpawnBurstSpeedMin = 50f;
        public static readonly float SpawnBurstSpeedMax = 150f;
        public static readonly float SpawnBurstScaleMin = 0.3f;
        public static readonly float SpawnBurstScaleMax = 1.0f;
        public static readonly float SpawnBurstLifeMin = 0.8f;
        public static readonly float SpawnBurstLifeMax = 1.2f;
        public static readonly float SpawnBurstRotationSpeedRange = 8f;
        
        // Damage Effect Particles
        public static readonly int DamageEffectParticleCount = 8;
        public static readonly float DamageEffectSpeedMin = 80f;
        public static readonly float DamageEffectSpeedMax = 120f;
        public static readonly float DamageEffectScaleMin = 0.4f;
        public static readonly float DamageEffectScaleMax = 1.0f;
        public static readonly float DamageEffectLifeMin = 0.3f;
        public static readonly float DamageEffectLifeMax = 0.6f;
        
        // Ambient Particles
        public static readonly float AmbientVelocityRange = 10f;
        public static readonly float AmbientScaleMin = 2f;
        public static readonly float AmbientScaleMax = 4f;
        public static readonly float AmbientLifeMin = 5f;
        public static readonly float AmbientLifeMax = 15f;
        public static readonly float AmbientAlphaMin = 0.1f;
        public static readonly float AmbientAlphaMax = 0.3f;
        public static readonly float AmbientRotationSpeedRange = 1f;
        
        // Card Trail Particles
        public static readonly float CardTrailVelocityRange = 30f;
        public static readonly float CardTrailScaleMin = 1f;
        public static readonly float CardTrailScaleMax = 3f;
        public static readonly float CardTrailLifeMin = 0.3f;
        public static readonly float CardTrailLifeMax = 0.5f;
        public static readonly float CardTrailAlphaMultiplier = 0.6f;
    }
}
