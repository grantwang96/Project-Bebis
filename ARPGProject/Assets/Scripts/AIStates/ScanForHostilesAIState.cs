using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class ScanForHostilesAIState : AIStateV2
    {
        private INPCUnitController _npcUnitController;

        public ScanForHostilesAIState(ICharacterV2 character, INPCUnitController npcUnitController, AIStateMachineV2 stateMachine, AIStateData aiStateData, IAIState parent) :
            base(character, stateMachine, aiStateData, parent) {
            _npcUnitController = npcUnitController;
        }

        public override void Initialize() {
            base.Initialize();
        }
    }
}
