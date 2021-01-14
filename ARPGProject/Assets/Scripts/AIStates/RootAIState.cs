using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class RootAIState : AIStateV2
    {
        public const string Name = "Root";

        public RootAIState(ICharacterV2 character, AIStateMachineV2 stateMachine, AIStateData stateData) : base(character, stateMachine, stateData) {

        }
    }
}
