
[System.Serializable]
public class SerializeableKeyValuePair<K, V> {
    public K Key;
    public V Value;
}

[System.Serializable]
public class SerializedStringBool {
    public string Key;
    public bool Value;
}

[System.Serializable]
public class SerializedStringInt {
    public string Key;
    public int Value;
}

[System.Serializable]
public class SerializedStringFloat {
    public string Key;
    public float Value;
}