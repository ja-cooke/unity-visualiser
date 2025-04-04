using System;
using System.Collections.Generic;
using UnityEngine;

namespace Visualiser
{
    public class Scatter : Chart
    {
        private GameObject[] plot;
        private PlotType plotType;
        private float pixelScale = 0.005f;
            
        public Scatter(Transform graphBoundaryT, int bufferSize, PlotType plotType){
            plot = new GameObject[bufferSize];
            this.plotType = plotType;
            
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

        public override void update(float[] dataArray, int audioBufferSize)
        {
            base.update(dataArray, audioBufferSize);

            switch(this.plotType){
                case PlotType.TimeLin:
                    timeLin(dataArray, audioBufferSize);
                    break;
                case PlotType.FreqLin:
                    break;
                case PlotType.FreqLog:
                    break;
                case PlotType.FreqLogLog:
                    break;
                default:
                    break;
            }
            
        }

        /* 
        * Plots 2D waveform within a 3D space
        */
        private GameObject[] timeLin(float[] dataArray, int audioBufferSize)
        {
            // x coordinate is depth, y coordinate is amplitude, z coordinate is time / frequency axis
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