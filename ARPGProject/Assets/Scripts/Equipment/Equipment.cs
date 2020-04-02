
namespace Bebis {
    public interface IEquipment {

        EquipmentType EquipmentType { get; }
        CharacterStats Stats { get; }
        CharacterStatModifiers Modifiers { get; }

        CharacterStats ApplyStats(CharacterStats stats);
        CharacterStatModifiers ApplyModifiers(CharacterStatModifiers modifiers);
    }

    public enum EquipmentType {
        Weapon,
        Helmet,
        Armor,
        Leggings
    }
}
