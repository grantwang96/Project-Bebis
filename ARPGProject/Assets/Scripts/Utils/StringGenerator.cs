using UnityEngine;

public static class StringGenerator {

    private const string AlphaNumeric =
        "abcdefghikjlmnopqrstuvwxyz" +
        "0123456789";

    public static string RandomString(int length) {
        string randString = "";
        for(int i = 0; i < length; i++) {
            char c = AlphaNumeric[Random.Range(0, AlphaNumeric.Length)];
            randString = $"{randString}{c}";
        }
        return randString;
    }
}
