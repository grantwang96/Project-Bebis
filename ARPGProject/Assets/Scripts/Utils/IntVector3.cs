
[System.Serializable]
public struct IntVector3 {

    public IntVector3(int newX, int newY, int newZ = 0) {
        x = newX;
        y = newY;
        z = newZ;
    }

    public static IntVector3 Forward = new IntVector3(0, 0, 1);
    public static IntVector3 Back = new IntVector3(0, 0, -1);
    public static IntVector3 Right = new IntVector3(1, 0, 0);
    public static IntVector3 Left = new IntVector3(-1, 0, 0);
    public static IntVector3 Up = new IntVector3(0, 1, 0);
    public static IntVector3 Down = new IntVector3(0, -1, 0);
    public static IntVector3 Zero = new IntVector3(0, 0, 0);

    public static bool operator ==(IntVector3 v1, IntVector3 v2) {
        return v1.Equals(v2);
    }
    public static bool operator !=(IntVector3 v1, IntVector3 v2) {
        return !v1.Equals(v2);
    }
    public static IntVector3 operator +(IntVector3 v1, IntVector3 v2) {
        return new IntVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }
    public static IntVector3 operator -(IntVector3 v1, IntVector3 v2) {
        return new IntVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public int x;
    public int y;
    public int z;

    public override string ToString() {
        return $"IntVector3({x}, {y}, {z})";
    }
}
