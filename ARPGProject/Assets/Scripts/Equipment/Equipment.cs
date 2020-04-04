using System.Collections.Generic;

namespace Bebis {
    public interface IEquipment {
        int Attack { get; }
        int Defense { get; }

        EquipmentType EquipmentType { get; }
        CharacterStats Stats { get; }
        CharacterStatModifiers Modifiers { get; }
        IReadOnlyList<AnimationClipOverride> AnimationOverrides { get; }

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
