// Title: UnityMiscUtils
// Author: Harry Donovan
// Collaborators:
// License: GNU General Public License v3.0
// Date Last Edited: 25/07/20
// Last Edited By: Harry Donovan
// References: 
// Description: A small collection of miscellaneous code snippets I've found useful. 
// Currently included are;
// casts to/ from float[] from/ to Vector3,
// and vector rotation using quaternions.
// Use 'using static UnityMiscUtils.classname' to use functions without having to reference the classes directly.

using UnityEngine;

namespace UnityMiscUtils
{
    public static class ExtraCasts
    {
        public static float[] CastVectorToFloatArray(Vector3 input)
        {
            return new float[3] {input.x, input.y, input.z};
        }

        public static Vector3 CastFloatArrayToVector(float[] input)
        {
            return new Vector3(input[0], input[1], input[1]);
        }
    }

    /// <summary>
    /// Returns a vector, rotated around an axis.
    /// </summary>

    public static class VectorRotation
    {
        public static Vector3 RotateVector(float angleOfRotation, float[] axisOfRotation, float[] pointToRotate)
        {
            return RotateVector(angleOfRotation, ExtraCasts.CastFloatArrayToVector(axisOfRotation), ExtraCasts.CastFloatArrayToVector(pointToRotate));
        }

        public static Vector3 RotateVector(float angleOfRotation, Vector3 axisOfRotation, Vector3 pointToRotate)
        {
            float theta = angleOfRotation / 2;
            axisOfRotation = axisOfRotation.normalized;
            pointToRotate = pointToRotate.normalized;

            float q0 = Mathf.Cos(theta);
            Vector3 q = Mathf.Sin(theta) * axisOfRotation;

            Vector3 newPoint = (Mathf.Pow(q0, 2) - Mathf.Pow(q.magnitude, 2)) * pointToRotate;
            newPoint += 2 * (Vector3.Dot(q, pointToRotate)) * q;
            newPoint += 2 * q0 * Vector3.Cross(q, pointToRotate);

            return newPoint;
        }
    }
}