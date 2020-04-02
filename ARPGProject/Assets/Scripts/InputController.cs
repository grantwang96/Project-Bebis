using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class InputController : MonoBehaviour {

        public static InputController Instance { get; private set; }

        [SerializeField] private Vector3 _moveInput = new Vector3(0, 1);
        public Vector3 MoveInput => _moveInput;

        public event Action OnBtn1Pressed;
        public event Action OnBtn1Held;
        public event Action OnBtn1Released;

        public event Action OnBtn2Pressed;
        public event Action OnBtn2Held;
        public event Action OnBtn2Released;

        private void Awake() {
            Instance = this;
        }

        private void Update() {
            AxisInputs();
            ButtonInputs();
        }

        private void AxisInputs() {
            _moveInput.x = Input.GetAxis("Horizontal");
            _moveInput.y = Input.GetAxis("Vertical");
        }

        private void ButtonInputs() {
            AButtonInputs();
        }

        private void AButtonInputs() {
            if (Input.GetButtonDown("Fire1")) {
                OnBtn1Pressed?.Invoke();
            } else if (Input.GetButton("Fire1")) {
                OnBtn1Held?.Invoke();
            } else if (Input.GetButtonUp("Fire1")) {
                OnBtn1Released?.Invoke();
            }
        }
    }
}
