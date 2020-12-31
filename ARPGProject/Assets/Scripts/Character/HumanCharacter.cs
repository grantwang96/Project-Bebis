using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class HumanCharacter : MonoBehaviour, ICharacterV2
    {
        // unit controller reference
        public IUnitController UnitController { get; private set; }
        // damageable
        public ICharacterDamageable Damageable { get; private set; }
        // move controller
        public IMoveControllerV2 MoveController => _moveController;
        // action controller
        public IActionControllerV2 ActionController { get; private set; }
        // animation controller
        public IAnimationController AnimationController => _animationController;
        // hitbox controller
        public HitboxControllerV2 HitboxController => _hitboxController;
        // hurtbox controller
        public HurtboxControllerV2 HurtboxController => _hurtboxController;
        // character stat manager
        public ICharacterStatManager CharacterStatManager => _characterStatManager;
        // Character GameObject
        public GameObject GameObject => gameObject;
        public Transform Center => _center;

        [SerializeField] private Transform _center;
        [SerializeField] private HumanMoveController _moveController;
        [SerializeField] private CharacterAnimationController _animationController;
        [SerializeField] private HitboxControllerV2 _hitboxController;
        [SerializeField] private HurtboxControllerV2 _hurtboxController;
        [SerializeField] private NPCCharacterStatManager _characterStatManager;

        public void Spawn(IUnitController unitController) {
            UnitController = unitController;
            if (Damageable == null) {
                Damageable = new HumanDamageable();
            }
            if (ActionController == null) {
                ActionController = new HumanActionController();
            }
            UnitController.Initialize(this);
            Damageable.Initialize(this);
            ActionController.Initialize(this);
            _animationController.Initialize(this);
            _moveController.Initialize(this);
            _hitboxController.Initialize(this);
            _hurtboxController.Initialize(this);
        }
    }
}
