using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Winston;

namespace Bebis
{
    public interface ICharactersManager
    {
        void SpawnCharacter(string characterPrefabId, CharacterInitializationData characterInitData);
    }

    public class CharactersManager : ICharactersManager
    {
        private readonly Dictionary<string, ICharacter> _activeCharacters = new Dictionary<string, ICharacter>();

        public void SpawnCharacter(string characterPrefabId, CharacterInitializationData characterInitData) {
            IPooledObject pooledObject = ManagerMaster.PooledObjectManager.UsePooledObject(characterPrefabId);
            if(pooledObject == null) {
                Debug.LogError($"[{nameof(CharactersManager)}]: Could not retrieve pooled object from id \"{characterPrefabId}\"!");
                return;
            }
            ICharacter character = pooledObject as ICharacter;
            if(character == null) {
                Debug.LogError($"[{nameof(CharactersManager)}]: Pooled object from id \"{characterPrefabId}\" was not of type {nameof(ICharacter)}!");
                return;
            }
            character.Initialize(characterInitData);
            character.SetActive(true);
        }
    }
}
