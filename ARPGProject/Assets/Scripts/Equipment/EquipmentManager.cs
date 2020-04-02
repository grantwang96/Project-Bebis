using System;
using UnityEngine;

namespace Bebis {

    public class EquipmentManager : MonoBehaviour{
        // helmet slot
        [SerializeField] private Armor _helmetSlot;
        public Armor HelmetSlot => _helmetSlot;
        // torso slot
        [SerializeField] private Armor _torsoSlot;
        public Armor TorsoSlot => _torsoSlot;
        // legging slot
        [SerializeField] private Armor _leggingsSlot;
        public Armor LeggingsSlot => _leggingsSlot;

        public event Action OnEquipmentUpdated;

        public void SetEquipment(Armor armor) {
            switch (armor.EquipmentType) {
                case EquipmentType.Helmet:
                    _helmetSlot = armor;
                    break;
                case EquipmentType.Armor:
                    _torsoSlot = armor;
                    break;
                case EquipmentType.Leggings:
                    _leggingsSlot = armor;
                    break;
                default:
                    CustomLogger.Error(nameof(EquipmentManager), $"Could not assign type {armor.EquipmentType}!");
                    break;
            }
            OnEquipmentUpdated?.Invoke();
        }

        // EDITOR ONLY
        private void OnValidate() {
            OnEquipmentUpdated?.Invoke();
        }
    }
}
