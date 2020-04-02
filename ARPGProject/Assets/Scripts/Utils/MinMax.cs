using UnityEngine;

[System.Serializable]
public class MinMax_Int {
    [SerializeField] private int _min;
    [SerializeField] private int _max;
    public int Min => _min;
    public int Max => _max;
    
    public static int GetRandomFromMinMax(MinMax_Int minMax) {
        return Random.Range(minMax.Min, minMax.Max);
    }
}

[System.Serializable]
public class MinMax_Float {
    [SerializeField] private float _min;
    [SerializeField] private float _max;
    public float Min => _min;
    public float Max => _max;

    public static float GetCountFromMinMax(MinMax_Float minMax) {
        return Random.Range(minMax.Min, minMax.Max);
    }
}
