using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMotionController : MonoBehaviour
{
    public enum UnitDefinition { mps, kmh, kn }
    [Tooltip("Speed in the given units")]
    public float currentSpeed = 0;
    [Tooltip("Speed unit")]
    public UnitDefinition unitDefinition = UnitDefinition.mps;

    /// <summary>
    /// Get the multiplying factor for the current unit definition
    /// </summary>
    /// <returns>The conversion factor of the current unit to meters per seconds.</returns>
    public float GetUnitFactor()
    {
        return GetUnitFactor(unitDefinition);
    }

    /// <summary>
    /// Get the multiplying factor for the given unit definition
    /// </summary>
    /// <returns>The conversion factor of the current unit to meters per seconds.</returns>
    static public float GetUnitFactor(UnitDefinition unitDef)
    {
        switch (unitDef)
        {
            case UnitDefinition.mps:
                return 1f;
            case UnitDefinition.kmh:
                return 0.2777778f;
            case UnitDefinition.kn:
                return 0.514444f;
        }
        return 1f;
    }

    public float GetSpeed(UnitDefinition unitDef)
    {
        if(unitDef==unitDefinition)
        {
            return currentSpeed;
        }
        return currentSpeed * GetUnitFactor() / GetUnitFactor(unitDef);
    }

    private void Update()
    {
        Vector3 dir = transform.forward;
        Vector3 currPos = transform.position;
        float t = Time.smoothDeltaTime;
        currPos += dir * currentSpeed * GetUnitFactor() * t;
        transform.position = currPos;
        // TODO: add also currentAccelerationand currentDump
    }
}
