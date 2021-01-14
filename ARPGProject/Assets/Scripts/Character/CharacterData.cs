using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public interface ICharacterData
    {
        CharacterStats CharacterStats { get; }
        float BaseWalkSpeed { get; }
        float BaseRunSpeed { get; }
        float Weight { get; }
    }

    /// <summary>
    /// represents general information for a character
    /// </summary>
    public abstract class CharacterData : ScriptableObject, ICharacterData
    {
        [SerializeField] private CharacterStats _characterStats;
        [SerializeField] private float _baseWalkSpeed;
        [SerializeField] private float _baseRunSpeed;
        [SerializeField] private float _weight;

        public CharacterStats CharacterStats => _characterStats;
        public float BaseWalkSpeed => _baseWalkSpeed;
        public float BaseRunSpeed => _baseRunSpeed;
        public float Weight => _weight;
    }
}
