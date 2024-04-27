using UnityEngine;

namespace DoubleHeat.Physics {

    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodySpeedLimiter : MonoBehaviour {

        Rigidbody _rigidbody;
        public Rigidbody AttachedRigidbody {
            get {
                _rigidbody ??= GetComponent<Rigidbody>();
                return _rigidbody;
            }
        }

        [Header("Rigidbody Speed Limiter")]
        [SerializeField] float _maxSpeed = Mathf.Infinity;
        public float MaxSpeed => _maxSpeed;

        protected virtual void FixedUpdate() {
            if (AttachedRigidbody.velocity.sqrMagnitude > MaxSpeed * MaxSpeed) {
                AttachedRigidbody.velocity = AttachedRigidbody.velocity.normalized * MaxSpeed;
            }
        }

    }

}