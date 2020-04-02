using UnityEngine;

namespace Bebis {
    public interface ILocation {
        int LocationX { get; }
        int LocationY { get; }
    }

    public class Location : MonoBehaviour, ILocation {
        public int LocationX => Mathf.RoundToInt(transform.position.x);
        public int LocationY => Mathf.RoundToInt(transform.position.y);
    }
}