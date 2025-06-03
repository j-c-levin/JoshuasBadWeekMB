using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using joshuas_bad_week.Config;
using joshuas_bad_week.Entities;
using joshuas_bad_week.Managers;
using joshuas_bad_week.Effects;

namespace joshuas_bad_week;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private GameStateManager _gameStateManager;
    private UIManager _uiManager;
    private EnemyManager _enemyManager;
    private SpriteFont _defaultFont;
    private ParticleSystem _particleSystem;
    private VisualEffects _visualEffects;
    private float _ambientParticleTimer;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        // Set screen size from config
        _graphics.PreferredBackBufferWidth = GameConfig.ScreenWidth;
        _graphics.PreferredBackBufferHeight = GameConfig.ScreenHeight;
    }

    protected override void Initialize()
    {
        // Initialize game managers and entities
        _gameStateManager = new GameStateManager();
        _uiManager = new UIManager();
        _enemyManager = new EnemyManager();
        _particleSystem = new ParticleSystem();
        _visualEffects = new VisualEffects();
        _ambientParticleTimer = 0f;
        
        // Create player at center of screen
        Vector2 playerStartPosition = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
        _player = new Player(playerStartPosition);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Load font
        _defaultFont = Content.Load<SpriteFont>("DefaultFont");
        
        // Load content for game objects
        _player.LoadContent(GraphicsDevice);
        _uiManager.LoadContent(_defaultFont);
        _enemyManager.LoadContent(GraphicsDevice);
        _particleSystem.LoadContent(GraphicsDevice);
        _visualEffects.LoadContent(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        // Update visual effects and particles
        _visualEffects.Update(gameTime);
        _particleSystem.Update(gameTime);
        
        // Add ambient particles periodically
        _ambientParticleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_ambientParticleTimer >= 0.1f)
        {
            _particleSystem.AddAmbientParticle(new Vector2(GameConfig.ScreenWidth, GameConfig.ScreenHeight));
            _ambientParticleTimer = 0f;
        }
        
        // Update game state (timer, win condition)
        _gameStateManager.Update(gameTime);
        
        // Only update player and enemies if game is active (not won or game over)
        if (_gameStateManager.IsGameActive)
        {
            _player.Update(gameTime, keyboardState);
            
            // Update enemies
            _enemyManager.Update(gameTime, _player.Position, _gameStateManager.IsGameActive);
            _enemyManager.LoadContentForNewEnemies(GraphicsDevice);
            
            // Check for collisions between enemies and player
            if (_enemyManager.CheckCollisions(_player.Bounds))
            {
                _player.TakeDamage(GameConfig.KeziaDamage);
                
                // Add damage effects
                _particleSystem.AddDamageEffect(_player.Position, GameConfig.DamageEffectColor);
                _visualEffects.TriggerScreenShake(8f, 0.2f);
                
                // Check if player is dead
                if (_player.Health <= 0)
                {
                    _gameStateManager.SetGameOver();
                }
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(GameConfig.BackgroundColor);

        // Apply screen shake offset
        Matrix transformMatrix = Matrix.CreateTranslation(_visualEffects.ScreenShakeOffset.X, _visualEffects.ScreenShakeOffset.Y, 0);
        _spriteBatch.Begin(transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend);
        
        // Draw animated background
        _visualEffects.DrawBackground(_spriteBatch, gameTime, GameConfig.ScreenWidth, GameConfig.ScreenHeight);
        
        // Draw background particles
        _particleSystem.Draw(_spriteBatch);
        
        _spriteBatch.End();
        
        // Draw main game elements with additive blending for glow effects
        _spriteBatch.Begin(transformMatrix: transformMatrix, blendState: BlendState.Additive);
        
        // Draw player with enhanced effects
        _player.Draw(_spriteBatch, _visualEffects, _particleSystem, gameTime);
        
        // Draw enemies with enhanced effects
        _enemyManager.Draw(_spriteBatch, _visualEffects, _particleSystem, gameTime);
        
        _spriteBatch.End();
        
        // Draw UI with normal blending
        _spriteBatch.Begin(transformMatrix: transformMatrix);
        
        // Draw UI (health, timer, win message)
        _uiManager.Draw(_spriteBatch, _player, _gameStateManager, _enemyManager, _visualEffects, gameTime);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
