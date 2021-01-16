using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public interface INPCUnitController : IUnitController
    {
        IDetectableManager DetectableManager { get; }
        IDetectableScanner NPCVision { get; }

        void SetMovementInput(Vector3 input);
        void SetRotationInput(Vector3 input);
    }

    /// <summary>
    /// Controller for NPC characters
    /// </summary>
    public class NPCUnitController : INPCUnitController
    {
        public ITargetManagerV2 TargetManager => _targetManager;
        public ICharacterData CharacterData => _characterData;
        public IDetectableManager DetectableManager => _detectableManager;
        public IDetectableScanner NPCVision => _npcVision;

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
        private NPCDetectableManager _detectableManager;
        // vision component here
        private NPCVisionV2 _npcVision;

        public NPCUnitController() {
            _aiStateMachine = new AIStateMachineV2();
            _targetManager = new NPCTargetManagerV2();
            _detectableManager = new NPCDetectableManager();
            _npcVision = new NPCVisionV2();
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
            _aiStateMachine.GenerateAIStateTree(this, _characterData.AIStateTreeData, _characterData.StartingAIStateId);
            _aiStateMachine.StartStateMachine();

            _targetManager.Initialize(_character);
            _detectableManager.Initialize(_character);
            _npcVision.Initialize(_character);
        }

        public void SetMovementInput(Vector3 input) {
            MovementInput = input;
        }

        public void SetRotationInput(Vector3 input) {
            RotationInput = input;
        }
    }
}
