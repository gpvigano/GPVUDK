using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPVUDK
{
    public static class MathUtil
    {
        /// <summary>
        /// Find a value in a SORTED array and get the upper and lower nearest indices
        /// </summary>
        /// <param name="referenceArray">array where to search</param>
        /// <param name="valueToFound">value to be found</param>
        /// <param name="lowerIndex">lower index (0 if before the beginning or last index if after the end)</param>
        /// <param name="upperIndex">upper index (last index if after the end or 0 if before the beginning)</param>
        /// <param name="trim">trimming factor between the values at the found indices (0 if not in range)</param>
        /// <returns>Return true if the given value is in range, else false.</returns>
        public static bool FindTrimmedIndices(float[] referenceArray, float valueToFound,
            out int lowerIndex, out int upperIndex, out float trim, bool reversed)
        {
            float absVal = Math.Abs(valueToFound);

            lowerIndex = -1;
            upperIndex = -1;

            bool greaterThanAll = reversed ? absVal > referenceArray[0] : absVal > referenceArray[referenceArray.Length - 1];
            bool lesserThanAll = reversed ? absVal < referenceArray[referenceArray.Length - 1] : absVal < referenceArray[0];
            if (lesserThanAll)
            {
                lowerIndex = upperIndex = 0;
                trim = 0;
                return false;
            }
            if (greaterThanAll)
            {
                lowerIndex = upperIndex = referenceArray.Length - 1;
                trim = 0;
                return false;
            }
            for (int i = 0; i < referenceArray.Length; i++)
            {
                if (absVal >= referenceArray[i])
                {
                    if (reversed)
                    {
                        upperIndex = i;
                    }
                    else
                    {
                        lowerIndex = i;
                    }
                }
                if (absVal < referenceArray[i])
                {
                    if (reversed)
                    {
                        lowerIndex = i;
                    }
                    else
                    {
                        upperIndex = i;
                    }
                }
            }
            if (reversed)
            {
                trim = (absVal - referenceArray[upperIndex]) / (referenceArray[lowerIndex] - referenceArray[upperIndex]);
            }
            else
            {
                trim = (absVal - referenceArray[lowerIndex]) / (referenceArray[upperIndex] - referenceArray[lowerIndex]);
            }
            return true;
        }

        public static float BilinearInterpolate(float[,] referenceMatrix, float[] referenceValues1, float value1, float[] referenceValues2, float value2)
        {
            int w1;
            int w2;
            float wt;
            // wheelDegree indices are sorted (ascending)
            if (!FindTrimmedIndices(referenceValues1, value1, out w1, out w2, out wt, false))
            {
                // TODO: what to do if out of range?
                return float.NaN;
            }
            int s1 = 0;
            int s2 = 0;
            float st = 0;
            // knotsSpeed indices are sorted (ascending)
            if (!FindTrimmedIndices(referenceValues2, value2, out s1, out s2, out st, false))
            {
                // TODO: what to do if out of range?
                return float.NaN;
            }
            float d1 = Mathf.Lerp(referenceMatrix[s1, w1], referenceMatrix[s1, w2], wt);
            float d2 = Mathf.Lerp(referenceMatrix[s2, w1], referenceMatrix[s2, w2], wt);
            return Mathf.Lerp(d1, d2, st);
        }

        public static float MetersToNauticMiles(float meters)
        {
            return meters/ 1852f;
        }
        public static float NauticMilesToMeters(float nauticMiles)
        {
            return nauticMiles* 1852f;
        }
        public static float MpsToKnots(float speedMS)
        {
            return 1.94f * speedMS;
        }

    }
}