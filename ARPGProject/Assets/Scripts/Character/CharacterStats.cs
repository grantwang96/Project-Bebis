using UnityEngine;

namespace Bebis {
    [System.Serializable]
    public struct CharacterStats {
        public int Strength;
        public int Agility;
        public int Fortitude;
        public int Intelligence;
        public int Wisdom;
        public int Charm;

        public static CharacterStats operator +(CharacterStats a, CharacterStats b) {
            CharacterStats c = new CharacterStats();
            c.Strength = a.Strength + b.Strength;
            c.Agility = a.Agility + b.Agility;
            c.Fortitude = a.Fortitude + b.Fortitude;
            c.Intelligence = a.Intelligence + b.Intelligence;
            c.Wisdom = a.Wisdom + b.Wisdom;
            c.Charm = a.Charm + b.Charm;
            return c;
        }

        public static CharacterStats operator -(CharacterStats a, CharacterStats b) {
            CharacterStats c = new CharacterStats();
            c.Strength = a.Strength - b.Strength;
            c.Agility = a.Agility - b.Agility;
            c.Fortitude = a.Fortitude - b.Fortitude;
            c.Intelligence = a.Intelligence - b.Intelligence;
            c.Wisdom = a.Wisdom - b.Wisdom;
            c.Charm = a.Charm - b.Charm;
            return c;
        }

        public static CharacterStats ApplyModifiers(CharacterStats stats, CharacterStatModifiers modifiers) {
            CharacterStats results = new CharacterStats();
            results.Strength = Mathf.RoundToInt(stats.Strength * modifiers.Strength);
            results.Agility = Mathf.RoundToInt(stats.Agility * modifiers.Agility);
            results.Fortitude = Mathf.RoundToInt(stats.Fortitude * modifiers.Fortitude);
            results.Intelligence = Mathf.RoundToInt(stats.Intelligence * modifiers.Intelligence);
            results.Wisdom = Mathf.RoundToInt(stats.Wisdom * modifiers.Wisdom);
            results.Charm = Mathf.RoundToInt(stats.Charm * modifiers.Charm);
            return results;
        }
    }

    [System.Serializable]
    public struct CharacterStatModifiers {
        public float Strength;
        public float Agility;
        public float Fortitude;
        public float Intelligence;
        public float Wisdom;
        public float Charm;

        public static CharacterStatModifiers Standard = new CharacterStatModifiers() {
            Strength = 1,
            Agility = 1,
            Fortitude = 1,
            Intelligence = 1,
            Wisdom = 1,
            Charm = 1
        };

        public static CharacterStatModifiers operator +(CharacterStatModifiers a, CharacterStatModifiers b) {
            CharacterStatModifiers c = new CharacterStatModifiers();
            c.Strength = a.Strength + b.Strength;
            c.Strength = a.Strength + b.Strength;
            c.Agility = a.Agility + b.Agility;
            c.Fortitude = a.Fortitude + b.Fortitude;
            c.Intelligence = a.Intelligence + b.Intelligence;
            c.Wisdom = a.Wisdom + b.Wisdom;
            c.Charm = a.Charm + b.Charm;
            return c;
        }

        public static CharacterStatModifiers operator -(CharacterStatModifiers a, CharacterStatModifiers b) {
            CharacterStatModifiers c = new CharacterStatModifiers();
            c.Strength = a.Strength - b.Strength;
            c.Agility = a.Agility - b.Agility;
            c.Fortitude = a.Fortitude - b.Fortitude;
            c.Intelligence = a.Intelligence - b.Intelligence;
            c.Wisdom = a.Wisdom - b.Wisdom;
            c.Charm = a.Charm - b.Charm;
            return c;
        }
    }

    public class CharacterStatBuilder {

        public static CharacterStats BuildStats() {
            return new CharacterStats();
        }
    }
}
