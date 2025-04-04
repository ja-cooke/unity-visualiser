using System;
using System.Collections.Generic;
using UnityEngine;

namespace Visualiser
{
    public class Scatter : Chart
    {
        private GameObject[] plot;
        private ScatterType scatterType;
        private const float pixelScale = 0.005f;
            
        public Scatter(Transform graphBoundaryT, int bufferSize, ScatterType scatterType){
            plot = new GameObject[bufferSize];
            this.scatterType = scatterType;
            
            // Instantiate cubes in the game world
            for (int datum = 0; datum<bufferSize; datum++)
            {
                // Generate a cube
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // Set the parent as the GraphBoundary
                cube.transform.SetParent(graphBoundaryT);
                // Set the coordinates to local to the graph boundary
                cube.transform.localPosition = Vector3.zero;
                // Scale the size of each individual cube
                cube.transform.localScale = Vector3.one * pixelScale;
                plot[datum] = cube;
            }
        }

        public override void update(SignalData signalDataPacket)
        {
            Dictionary<ScatterType, GameObject[]> method = new Dictionary<ScatterType, GameObject[]>{
                {ScatterType.TimeLin, timeLin(signalDataPacket)}
            };

            plot = method[scatterType];
        }

        /* 
        * Plots 2D waveform within a 3D space
        */
        private GameObject[] timeLin(SignalData signalDataPacket)
        {
            // x coordinate is depth, y coordinate is amplitude, z coordinate is time / frequency axis
            float[] dataArray = signalDataPacket.TimeAmplitude;
            int audioBufferSize = signalDataPacket.BufferSize;
            
            int n = 0;
            foreach (float datum in dataArray)
            {
                // 2D plot
                float xPos = 0;
                // Divide by 2 for a vertically centred plot of scale -0.5 <-> +0.5
                float yPos = dataArray[n]/2;
                // -0.5f offset for a horizontally centred plot
                float zPos = -0.5f + (n/(float)audioBufferSize);

                plot[n].transform.localPosition =  new Vector3(xPos,yPos,zPos);
                n++;
            }
            return plot;
        }
    }
}