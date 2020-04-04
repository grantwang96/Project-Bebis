using System;
using UnityEngine;

namespace Bebis {

    public class EquipmentManager : MonoBehaviour{
        [SerializeField] private Weapon _weaponSlot;
        public Weapon WeaponSlot => _weaponSlot;
        // helmet slot
        [SerializeField] private Armor _helmetSlot;
        public Armor HelmetSlot => _helmetSlot;
        // torso slot
        [SerializeField] private Armor _torsoSlot;
        public Armor TorsoSlot => _torsoSlot;
        // legging slot
        [SerializeField] private Armor _leggingsSlot;
        public Armor LeggingsSlot => _leggingsSlot;

        public event Action<IEquipment> OnEquipmentUpdated;

        public void SetArmor(Armor armor) {
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
            OnEquipmentUpdated?.Invoke(armor);
        }

        public void SetWeapon(Weapon weapon) {
            _weaponSlot = weapon;
            OnEquipmentUpdated?.Invoke(weapon);
        }
    }
}
