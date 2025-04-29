using UnityEngine;

namespace Visualiser {
    public class ScatterStretchPoint : Point
    {
        // ---- PROPERTIES ----
        public override Vector3 Coordinates { get; set; }
        private readonly GameObject Primitive;
        private readonly float PixelScale = 0.01f;
        private bool IsVisible { get; set; }
        
        // ---- CONSTRUCTOR ----
        public ScatterStretchPoint(Transform graphBoundaryT, PrimitiveType primitiveType) : base(graphBoundaryT)
        {
            Coordinates = Vector3.zero;
            GameObject primitive = GameObject.CreatePrimitive(primitiveType);
            Primitive = primitive;
            Collider collider = Primitive.GetComponent<Collider>();
            collider.enabled = false;

            // Set the parent as the GraphBoundary
            Primitive.transform.SetParent(GraphBoundaryT);
            // Set the coordinates to local to the graph boundary
            Primitive.transform.localPosition = Vector3.zero;
            // Scale the size of each individual cube
            Primitive.transform.localScale = Vector3.one * PixelScale;
            // For some reason points are initalised out of the expected rotation
            Primitive.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            // Set all meshes to invisible by default
            Visible(false);

            // Next will set the material for the points
            Primitive.GetComponent<MeshRenderer>().material = GraphBoundaryT.parent.Find("SampleMaterial").GetComponent<MeshRenderer>().material;
        }

        // ---- METHODS ----
        public void MoveTo(Vector3 newCoordinates)
        {
            Coordinates = newCoordinates;

            if (!float.IsNaN(Coordinates.y * Coordinates.z)) // Ignore NaN in X
            {
                // z position shifted by PixelScale/2 to prevent primitive mesh from crossing the boundary
                Primitive.transform.localPosition = new Vector3(0, Coordinates.y, Coordinates.z - PixelScale/2);
                // Stretching along the x-axis is acheived using a z-axis scale
                Primitive.transform.localScale = new Vector3(PixelScale, PixelScale, Coordinates.x);
                Visible(true);
            }
            else
            {
                Primitive.transform.localPosition = Vector3.zero;
                Primitive.transform.localScale = new Vector3(PixelScale, PixelScale, PixelScale);
                Visible(false);
            }

        }

        public void Visible(bool isVisible)
        {
            Primitive.GetComponent<MeshRenderer>().enabled = isVisible;
            IsVisible = isVisible;
        }
    }
}