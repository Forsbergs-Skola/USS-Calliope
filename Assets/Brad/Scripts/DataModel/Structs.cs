using UnityEngine;

public class Structs
{
    [System.Serializable]
    public struct ObjectiveCriterion
    {
        public EnumProgressionField progressionField;
        //public EnumComparisonType comparisonType;
        public EnumValueComparison comparisonValue;
        public bool negate;

        [Header("Target Value")]
        public bool BoolTarget;
        public int IntTarget;
        public float FloatTarget;
        public string StringTarget;

        

    }
}
