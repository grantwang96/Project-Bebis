using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class ScanVisionAIState : AIState {

        [SerializeField] private AIState _onNewHostileRegistered;
        [SerializeField] private AIState _onHostileTargetFound;
        [SerializeField] private AIState _onAllHostilesLost;
        [SerializeField] private NPCTargetManager _npcTargetManager;

        public override void Enter() {
            base.Enter();
            _npcTargetManager.OnNewHostileRegistered += OnNewHostileRegistered;
            _npcTargetManager.OnRegisteredHostilesUpdated += OnRegisteredHostilesUpdated;
            _npcTargetManager.OnCurrentTargetSet += OnCurrentTargetSet;
        }

        public override void Execute() {
            base.Execute();
            ScanForHostiles();
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _npcTargetManager.OnNewHostileRegistered -= OnNewHostileRegistered;
            _npcTargetManager.OnRegisteredHostilesUpdated -= OnRegisteredHostilesUpdated;
            _npcTargetManager.OnCurrentTargetSet -= OnCurrentTargetSet;
        }

        // searches for any hostile targets
        private void ScanForHostiles() {
            _npcTargetManager.ScanForHostiles();
        }

        private void OnNewHostileRegistered(HostileEntry entry) {
            Debug.Log($"New Hostile Registered: {entry.Hostile.MoveController.Body.name}");
            if(_onNewHostileRegistered != null) {
                FireReadyToChangeState(_onNewHostileRegistered);
            }
        }

        private void OnCurrentTargetSet() {
            if (_onHostileTargetFound != null && _npcTargetManager.CurrentTarget != null) {
                Debug.Log($"Target Found: {_npcTargetManager.CurrentTarget.MoveController.Body.name}");
                FireReadyToChangeState(_onHostileTargetFound);
            }
        }

        private void OnRegisteredHostilesUpdated(int count) {
            if (count == 0 && _onAllHostilesLost != null) {
                Debug.Log("All Hostiles lost!");
                FireReadyToChangeState(_onAllHostilesLost);
            }
        }
    }
}
