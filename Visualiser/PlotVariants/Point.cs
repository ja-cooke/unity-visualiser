/// Author: Jonathan Cooke
using UnityEngine;

namespace Visualiser
{
    /// <summary>
    /// General template for Point classes.
    /// Represent an individual data point in a visualisation.
    /// </summary>
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