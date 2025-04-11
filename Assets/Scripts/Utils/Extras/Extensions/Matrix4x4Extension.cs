using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    // http://answers.unity.com/answers/402281/view.html
    public static class Matrix4x4Extension
    {
        public static Vector3 DecomposeTranslation(this Matrix4x4 m) => m.GetColumn(3);


        public static Quaternion DecomposeRotation(this Matrix4x4 m) => Quaternion.LookRotation(
            m.GetColumn(2),
            m.GetColumn(1)
        );


        public static Vector3 DecomposeScale(this Matrix4x4 m) => new Vector3(
            m.GetColumn(0).magnitude,
            m.GetColumn(1).magnitude,
            m.GetColumn(2).magnitude
        );
    }
}
