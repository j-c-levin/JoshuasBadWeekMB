using System;
using Microsoft.Xna.Framework;
using joshuas_bad_week.Config;

namespace joshuas_bad_week.Managers
{
    /// <summary>
    /// Manages game state including timer, win condition, and game flow
    /// </summary>
    public class GameStateManager
    {
        public enum GameState
        {
            Playing,
            Won,
            GameOver,
            Paused
        }
        
        private float _timeRemaining;
        private GameState _currentState;
        
        public GameState CurrentState => _currentState;
        public float TimeRemaining => _timeRemaining;
        public int TimeRemainingSeconds => (int)Math.Ceiling(_timeRemaining);
        public bool IsGameWon => _currentState == GameState.Won;
        public bool IsGameOver => _currentState == GameState.GameOver;
        public bool IsGameActive => _currentState == GameState.Playing;
        
        public GameStateManager()
        {
            _timeRemaining = GameConfig.GameDurationSeconds;
            _currentState = GameState.Playing;
        }
        
        public void Update(GameTime gameTime)
        {
            if (_currentState == GameState.Playing)
            {
                _timeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if (_timeRemaining <= 0)
                {
                    _timeRemaining = 0;
                    _currentState = GameState.Won;
                }
            }
        }
        
        public void SetGameOver()
        {
            if (_currentState == GameState.Playing)
            {
                _currentState = GameState.GameOver;
            }
        }
        
        public void Reset()
        {
            _timeRemaining = GameConfig.GameDurationSeconds;
            _currentState = GameState.Playing;
        }
        
        public void Pause()
        {
            if (_currentState == GameState.Playing)
                _currentState = GameState.Paused;
        }
        
        public void Resume()
        {
            if (_currentState == GameState.Paused)
                _currentState = GameState.Playing;
        }
    }
}
