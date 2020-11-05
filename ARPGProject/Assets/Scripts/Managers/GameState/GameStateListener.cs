using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Winston;

namespace Bebis
{
    public abstract class GameStateListener : MonoBehaviour
    {
        protected GameState _gameState;

        private void Awake() {
            _gameState = GetComponent<GameState>();
            if(_gameState == null) {
                return;
            }
            _gameState.OnStateEntered += OnGameStateEntered;
            _gameState.OnStateExited += OnGameStateExited;
        }

        private void OnDestroy() {
            if (_gameState == null) {
                return;
            }
            _gameState.OnStateEntered -= OnGameStateEntered;
            _gameState.OnStateExited -= OnGameStateExited;
        }

        protected virtual void OnGameStateEntered(bool success) {

        }

        protected virtual void OnGameStateExited(bool success) {

        }
    }
}
