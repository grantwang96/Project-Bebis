using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    [CreateAssetMenu(menuName = "Character Data / NPC Character Data")]
    public class NPCCharacterData : CharacterData
    {
        [SerializeField] private AIStateData _aiStateData;
        [SerializeField] private string _startingAIStateId;

        public AIStateData AIStateTreeData => _aiStateData;
        public string StartingAIStateId => _startingAIStateId;
    }
}
