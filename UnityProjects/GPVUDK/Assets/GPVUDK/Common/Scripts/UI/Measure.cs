using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    /// <summary>
    /// Measure display between two transforms, with configurable units.
    /// </summary>
    public class Measure : MonoBehaviour
    {
        /// <summary>
        /// Measure units used to display the measure.
        /// Auto means that the most suitable metric units are selected according to measure value.
        /// </summary>
        public enum UnitDefinition { auto, km, m, cm, mm, NM }

        /// <summary>
        /// Number of decimal digits displayed.
        /// </summary>
        [Tooltip("Number of decimal digits displayed")]
        [Range(0,5)]
        public int decimalDigits = 1;
		
        /// <summary>
        /// Measure unit (auto=select automatically).
        /// </summary>
        [Tooltip("Measure unit (auto=select automatically)")]
        public UnitDefinition unitDefinition = UnitDefinition.auto;
		
        /// <summary>
        /// First reference transform (=this one if left empty).
        /// </summary>
        [Tooltip("First reference transform (=this one if left empty)")]
        public Transform ref1;
		
        /// <summary>
        /// Second reference transform (=this one if left empty).
        /// </summary>
        [Tooltip("Second reference transform (=this one if left empty)")]
        public Transform ref2;
		
        /// <summary>
        /// Text used to display the measure.
        /// </summary>
        [Tooltip("Text used to display the measure.")]
        public Text text;

        private float unitFactor = 1f;
        private string format;

		
        private float GetUnitFactor(UnitDefinition unitDefinition)
        {
            switch (unitDefinition)
            {
                case UnitDefinition.NM:
                    return 1852f;
                case UnitDefinition.km:
                    return 1000f;
                case UnitDefinition.m:
                    return 1f;
                case UnitDefinition.cm:
                    return 0.01f;
                case UnitDefinition.mm:
                    return 0.001f;
            }
            return unitFactor;
        }

		
        private UnitDefinition GetAutoUnit(float unitFactor)
        {
            if (unitFactor >= 1000f)
            {
                return UnitDefinition.km;
            }
            else if (unitFactor >= 1f)
            {
                return UnitDefinition.m;
            }
            else if (unitFactor >= 0.01f)
            {
                return UnitDefinition.cm;
            }
            else
            {
                return UnitDefinition.mm;
            }
        }

		
        private void Awake()
        {
            if(ref1==null)
            {
                ref1 = transform;
            }
        }

		
        private void Start()
        {
        }

		
        private void Update()
        {
            unitFactor = GetUnitFactor(unitDefinition);
            format = "F" + decimalDigits.ToString();
            float distance = Vector3.Distance(ref1.position, ref2.position);
            string unitText = unitDefinition.ToString();
            if (unitDefinition == UnitDefinition.auto)
            {
                UnitDefinition unitDef = GetAutoUnit(distance);
                unitText = unitDef.ToString().ToLowerInvariant();
                unitFactor = GetUnitFactor(unitDef);
            }
            text.text = (distance / unitFactor).ToString(format) + " " + unitText;
        }
    }
}
