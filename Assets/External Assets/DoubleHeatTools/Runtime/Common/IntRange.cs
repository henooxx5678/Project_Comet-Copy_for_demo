using System;
using UnityEngine;

namespace DoubleHeat.Common {
    
    [Serializable]
    public struct IntRange {
        [SerializeField] int _min;
        public int Min => _min;
        [SerializeField] int _max;
        public int Max => _max;

        public IntRange (int min, int max) {
            _min = min;
            _max = max;
        }

        public bool IsInRange (int value) {
            return (value >= _min && value <= _max);
        }

    }
}