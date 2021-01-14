using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    /// <summary>
    /// Controller for NPC characters
    /// </summary>
    public class NPCUnitController : IUnitController
    {
        public ITargetManagerV2 TargetManager => _targetManager;
        public ICharacterData CharacterData => _characterData;

        public Vector3 MovementInput { get; private set; }
        public Vector3 RotationInput { get; private set; }

        public event Action<ICharacterActionDataV2, CharacterActionContext> OnActionAttempted;
        public event Action OnRegisteredCharacterActionsUpdated;

        private ICharacterV2 _character;
        private NPCCharacterData _characterData;
        // ai state machine here
        private AIStateMachineV2 _aiStateMachine;
        // target manager here
        private NPCTargetManagerV2 _targetManager;
        // detection system here
        private NPCDetectableManager _detectionManager;

        public NPCUnitController() {
            _aiStateMachine = new AIStateMachineV2();
            _targetManager = new NPCTargetManagerV2();
            _detectionManager = new NPCDetectableManager();
        }

        /// <summary>
        /// set the individual data for this character
        /// </summary>
        public void SetData(NPCCharacterData characterData) {
            _characterData = characterData;
            // initialize each parts of the character with unit data
        }

        public void Initialize(ICharacterV2 character) {
            // set the character that this class will be controlling
            _character = character;

            _aiStateMachine.Initialize(_character);
            _aiStateMachine.GenerateAIStateTree(_characterData.AIStateTreeData, _characterData.StartingAIStateId);
            _aiStateMachine.StartStateMachine();

            _targetManager.Initialize(_character);
            _detectionManager.Initialize(_character);
        }
    }
}
