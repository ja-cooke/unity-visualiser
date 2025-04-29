using UnityEngine;

namespace Visualiser {
    public class ScatterPoint : Point
    {
        // ---- PROPERTIES ----
        public override Vector3 Coordinates { get; set; }
        private readonly GameObject Primitive;
        private readonly float PixelScale = 0.005f;
        private bool IsVisible { get; set; }
        
        // ---- CONSTRUCTOR ----
        public ScatterPoint(Transform graphBoundaryT, PrimitiveType primitiveType) : base(graphBoundaryT)
        {
            Coordinates = Vector3.zero;
            GameObject primitive = GameObject.CreatePrimitive(primitiveType);
            Primitive = primitive;

            // Remove the physics properties of the primitive
            Collider collider = Primitive.GetComponent<Collider>();
            collider.enabled = false;

            // Set the parent as the GraphBoundary
            Primitive.transform.SetParent(GraphBoundaryT);
            // Set the coordinates to local to the graph boundary
            Primitive.transform.localPosition = Vector3.zero;
            // Scale the size of each individual cube
            Primitive.transform.localScale = Vector3.one * PixelScale;
            // Set all meshes to invisible by default
            Visible(false);

            // Next will set the material for the points
            Primitive.GetComponent<MeshRenderer>().material = GraphBoundaryT.parent.Find("SampleMaterial").GetComponent<MeshRenderer>().material;
        }

        // ---- METHODS ----
        public void MoveTo(Vector3 newCoordinates)
        {
            Coordinates = newCoordinates;

            if (!float.IsNaN(Coordinates.sqrMagnitude))
            {
                Primitive.transform.localPosition = Coordinates;
                Visible(true);
            }
            else
            {
                Primitive.transform.localPosition = Vector3.zero;
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