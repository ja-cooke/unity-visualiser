using Unity.XR.CoreUtils.Datums;
using UnityEngine;

namespace Visualiser.Plot {
    
    public class Setup{
        private GameObject[] plot;

        public Setup(Transform graphBoundaryT, int bufferSize){

            plot = new GameObject[bufferSize];
                
            // Instantiate cubes in the game world
            for (int datum = 0; datum<bufferSize; datum++){

                // Generate a cube
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // Set the parent as the GraphBoundary
                cube.transform.SetParent(graphBoundaryT);
                // Set the coordinates to local to the graph boundary
                cube.transform.localPosition = Vector3.zero;
                // Scale the size of each individual cube
                cube.transform.localScale = new Vector3(0.005f,0.005f,0.005f);
                plot[datum] = cube;
                }

        }

        public GameObject[] getPlot(){
            return plot;
        }

    }
}