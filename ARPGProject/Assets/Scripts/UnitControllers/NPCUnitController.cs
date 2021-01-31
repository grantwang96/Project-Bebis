using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public interface INPCUnitController : IUnitController
    {
        IDetectableManager DetectableManager { get; }

        void SetMovementInput(Vector3 input);
        void SetRotationInput(Vector3 input);
    }

    /// <summary>
    /// Controller for NPC characters
    /// </summary>
    public class NPCUnitController : MonoBehaviour, INPCUnitController
    {
        public ITargetManagerV2 TargetManager => _targetManager;
        public ICharacterData CharacterData => _characterData;
        public IDetectableManager DetectableManager => _detectableManager;

        public Vector3 MovementInput { get; private set; }
        public Vector3 RotationInput { get; private set; }

        public event Action<ICharacterActionDataV2, CharacterActionContext> OnActionAttempted;
        public event Action<IReadOnlyList<ICharacterActionDataV2>> OnRegisteredCharacterActionsUpdated;

        // character data
        [SerializeField] private NPCCharacterData _characterData;
        // ai state machine here
        [SerializeField] private AIStateMachineV2 _aiStateMachine;
        // vision component here
        [SerializeField] private NPCVisionV2 _npcVision;

        private ICharacterV2 _character;
        // target manager here
        private NPCTargetManagerV2 _targetManager;
        // detection system here
        private NPCDetectableManager _detectableManager;

        public NPCUnitController() {
            _targetManager = new NPCTargetManagerV2();
            _detectableManager = new NPCDetectableManager();
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
