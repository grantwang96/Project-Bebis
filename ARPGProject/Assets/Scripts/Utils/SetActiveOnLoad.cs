using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnLoad : MonoBehaviour {

    public bool _active;

    private void Awake() {
        gameObject.SetActive(_active);
    }
}
