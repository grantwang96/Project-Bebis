using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public static class AIStateTreeBuilder
    {
        public static void Build(ICharacterV2 character, INPCUnitController npcUnitController, AIStateMachineV2 stateMachine, AIStateData rootAIStateData) {
            IAIState rootState = ParseFromStateData(character, npcUnitController, stateMachine, rootAIStateData);
            List<IAIState> allStates = new List<IAIState>();
            allStates.Add(rootState);
            CreateTree(character, npcUnitController, stateMachine, rootState, rootAIStateData, allStates);
            InitializeAIStates(allStates);
        }

        // recursive tree building function
        private static void CreateTree(ICharacterV2 character, INPCUnitController npcUnitController, AIStateMachineV2 stateMachine, IAIState parentState, AIStateData stateData, List<IAIState> allStates) {
            // build out all children data for the state tree
            for(int i = 0; i < stateData.ChildrenData.Count; i++) {
                // attempt to create the ai state from the data
                AIStateData childData = stateData.ChildrenData[i];
                IAIState childState = ParseFromStateData(character, npcUnitController, stateMachine, childData, parentState);
                if(childState == null) {
                    Debug.LogWarning($"[{nameof(AIStateTreeBuilder)}]: Could not build AI state for child data: {childData.AIStateName}!");
                    continue;
                }
                allStates.Add(childState);
                // create the ai state children for this child ai state
                CreateTree(character, npcUnitController, stateMachine, childState, childData, allStates);
            }
        }

        // initialize all states after building the tree
        private static void InitializeAIStates(List<IAIState> allStates) {
            for(int i = 0; i < allStates.Count; i++) {
                allStates[i].Initialize();
            }
        }

        // mega parser to generate an AI state
        private static IAIState ParseFromStateData(ICharacterV2 character, INPCUnitController npcUnitController, AIStateMachineV2 stateMachine, AIStateData stateData, IAIState parentState = null) {
            switch (stateData.AIStateName) {
                case RootAIState.Name:
                    return new RootAIState(character, stateMachine, stateData);
                case ListenForHitAIState.Name:
                    return new ListenForHitAIState(character, stateMachine, stateData, parentState);
                case WaitForDurationAIState.Name:
                    return new WaitForDurationAIState(character, stateMachine, stateData, parentState);
                case PlayAnimationAIState.Name:
                    return new PlayAnimationAIState(character, stateMachine, stateData, parentState);
                default:
                    return null;
            }
        }
    }
}
