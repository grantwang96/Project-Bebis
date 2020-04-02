using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayHelper {
    
    public static bool Contains<T>(T[] array, T item) {
        for(int i = 0; i < array.Length; i++) {
            if(array[i] != null && array[i].Equals(item)) {
                return true;
            }
        }
        return false;
    }

    public static bool Contains<T>(T[] array, T item, out int index) {
        index = -1;
        for (int i = 0; i < array.Length; i++) {
            if (array[i].Equals(item)) {
                index = i;
                return true;
            }
        }
        return false;
    }

    public static bool IsWithinArray<T>(int index, T[] oneDimensionArray) {
        if(oneDimensionArray.Length == 0) { return false; }
        return index >= 0 && index < oneDimensionArray.Length;
    }

    public static bool IsWithinArray<T>(int x, int y, T[][] twoDimensionArray) {
        if(twoDimensionArray.Length == 0 || twoDimensionArray[0].Length == 0) { return false; }
        return x >= 0 && x < twoDimensionArray.Length && y >= 0 && y < twoDimensionArray[0].Length;
    }

    public static bool IsWithinArray<T>(int x, int y, int z, T[][][] threeDimensionArray) {
        if(threeDimensionArray.Length == 0 || threeDimensionArray[0].Length == 0 || threeDimensionArray[0][0].Length == 0) { return false; }
        return x >= 0 && x < threeDimensionArray.Length && y >= 0 && y < threeDimensionArray[0].Length &&
            z >= 0 && z < threeDimensionArray[0][0].Length;
    }

    public static void Shuffle<T>(T[] array) {
        for(int i = 0; i < array.Length; i++) {
            int index = Random.Range(0, array.Length);
            T temp = array[i];
            array[i] = array[index];
            array[index] = temp;
        }
    }
}
