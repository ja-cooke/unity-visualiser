using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace Visualiser
{
    public abstract class Point {
        
        // --- PROPERTIES ----
        public Transform GraphBoundaryT { get; }
        public abstract Vector3 Coordinates { get; set; }
        
        // ---- CONSTRUCTOR ----
        public Point(Transform graphBoundaryT)
        {
            GraphBoundaryT = graphBoundaryT;
            Coordinates = Vector3.zero;
        }
    }
}