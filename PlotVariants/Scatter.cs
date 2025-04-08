using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

namespace Visualiser
{
    public class Scatter : Chart
    {
        private VisualiserFrame plot;
        private GameObject[] meshArray;
        private ScatterType scatterType;
        private const float pixelScale = 0.005f;
        private float peakMagnitude = 0;
            
        public Scatter(Transform graphBoundaryT, int bufferSize, ScatterType scatterType){
            meshArray = new GameObject[bufferSize];
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
                // Set all meshes to invisible by default
                cube.GetComponent<MeshRenderer>().enabled = false;
                // Save the mesh
                meshArray[datum] = cube;
            }
            plot = new VisualiserFrame(meshArray, new SignalData());
        }

        public override void update(SignalData signalDataPacket)
        {
            Dictionary<ScatterType, Func<SignalData, VisualiserFrame>> method = new Dictionary<ScatterType, Func<SignalData, VisualiserFrame>>
            {
                {ScatterType.TimeLin, timeLin},
                {ScatterType.FreqLin, freqLin},
                {ScatterType.FreqLog, freqLog},
                {ScatterType.FreqLogLog, freqLogLog},
                {ScatterType.FreqLogLogE, freqLogLogE},
            };

            plot = method[scatterType](signalDataPacket);
        }

        /* 
        * Plots 2D waveform within a 3D space
        */
        private VisualiserFrame timeLin(SignalData signalDataPacket)
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

                plot.Visualisation[n].transform.localPosition =  new Vector3(xPos,yPos,zPos);
                n++;
            }
            return plot;
        }

        private VisualiserFrame freqLin(SignalData signalDataPacket)
        {
            // x coordinate is depth, y coordinate is magnitude, z coordinate is frequency axis
            float[] dataArray = signalDataPacket.FreqMagnitude;
            int audioBufferSize = signalDataPacket.BufferSize;

            int n = 0;
            foreach (float datum in dataArray)
            {
                peakMagnitude = (peakMagnitude > dataArray[n]) ? peakMagnitude : dataArray[n];
                // 2D plot
                float xPos = 0;
                // Offset by -0.5 to rest on the bottom face of the boundary
                float yPos = dataArray[n]/peakMagnitude - 0.5f;
                // 0.5f offset for a horizontally centred plot
                float zPos = 0.5f - (n/(float)audioBufferSize);

                plot.Visualisation[n].transform.localPosition =  new Vector3(xPos,yPos,zPos);
                n++;
            }
            return plot;
        }

        private VisualiserFrame freqLog(SignalData signalDataPacket)
        {
            // x coordinate is depth, y coordinate is magnitude, z coordinate is frequency axis
            float[] dataArray = signalDataPacket.FreqMagnitude;
            int audioBufferSize = signalDataPacket.BufferSize;

            int nyquist = signalDataPacket.BufferSize/2;
            float freqRes = 1f/(float)signalDataPacket.BufferSize;

            float[] freqAxis = new float[signalDataPacket.BufferSize];

            // Determines values and applies log scaling for frequency axis
            for (int i = 0; i < 1024; i++){
                freqAxis[i] = (float)Math.Log10(freqRes + freqRes*i)/(float)Math.Log10(1024);
            }

            int n = 0;
            foreach (float datum in dataArray)
            {
                peakMagnitude = (peakMagnitude > dataArray[n]) ? peakMagnitude : dataArray[n];
                // 2D plot
                float xPos = 0;
                // Offset by -0.5 to rest on the bottom face of the boundary
                float yPos = dataArray[n]/peakMagnitude - 0.5f;
                
                float zPos = - 0.5f - freqAxis[n];

                plot.Visualisation[n].transform.localPosition =  new Vector3(xPos,yPos,zPos);
                n++;
            }
            return plot;
        }

        private VisualiserFrame freqLogLog(SignalData signalDataPacket)
        {
            plot.SignalData = signalDataPacket;
            const float minDbVal = -120;
            const float alignment = 0.5f;
            const float depth = 0f;

            // x coordinate is depth, y coordinate is magnitude, z coordinate is frequency axis
            float[] dataArray = signalDataPacket.FreqMagnitude;
            int audioBufferSize = signalDataPacket.BufferSize;

            int nyquist = signalDataPacket.BufferSize/2;
            float freqRes = 1f/(float)signalDataPacket.BufferSize;

            float[] freqAxis = new float[signalDataPacket.BufferSize];

            // Determines values and applies log scaling for frequency axis
            for (int i = 0; i < audioBufferSize; i++){
                freqAxis[i] = (float)Math.Log10(freqRes + freqRes*i)/(float)Math.Log10(audioBufferSize);
            }
            // Determines values and applies log scaling for magnitude axis
            for (int i = 0; i < audioBufferSize; i++){
                // Uses voltage decibel scale: y (dB) = 20 * log10(x/ref)
                dataArray[i] = 20f*(float)Math.Log10(dataArray[i]);

                // Out of bounds pixel behaviour
                outOfBounds(i, minDbVal, "hide");
            }

            
            for (int n = 0; n < audioBufferSize; n++)
            {
                // 2D plot
                float xPos = depth;
                // Offset to rest on the top face of the boundary
                float yPos = dataArray[n]/(-minDbVal) + alignment;
                
                float zPos = - alignment - freqAxis[n];

                plot.Visualisation[n].transform.localPosition =  new Vector3(xPos,yPos,zPos);
            }
            return plot;
        }

        private VisualiserFrame freqLogLogE(SignalData signalDataPacket)
        {
            plot.SignalData = signalDataPacket;
            const float minDbVal = -120;
            const float alignment = 0.5f;
            const float depth = 0f;

            // x coordinate is depth, y coordinate is magnitude, z coordinate is frequency axis
            float[] dataArray = signalDataPacket.FreqMagnitude;
            int audioBufferSize = signalDataPacket.BufferSize;

            int nyquist = signalDataPacket.BufferSize/2;
            float freqRes = 1f/(float)signalDataPacket.BufferSize;

            float[] freqAxis = new float[signalDataPacket.BufferSize];

            // Determines values and applies log scaling for frequency axis
            for (int i = 0; i < audioBufferSize; i++){
                freqAxis[i] = (float)Math.Log10(freqRes + freqRes*i)/(float)Math.Log10(audioBufferSize);
            }
            // Determines values and applies log scaling for magnitude axis
            for (int i = 0; i < audioBufferSize; i++){
                // Uses voltage decibel scale: y (dB) = 20 * log10(x/ref)
                dataArray[i] = 20f*(float)Math.Log10(dataArray[i]);

                // Out of bounds pixel behaviour
                outOfBounds(i, minDbVal, "hide");
            }

            // Determine the reduced data set for plotting (reduces high f info)
            int[] indices = Utils.ReducePlotData(plot);

            foreach (int n in indices)
            {
                // 2D plot
                float xPos = depth;
                // Offset to rest on the top face of the boundary
                float yPos = dataArray[n]/(-minDbVal) + alignment;
                
                float zPos = - alignment - freqAxis[n];

                plot.Visualisation[n].GetComponent<MeshRenderer>().enabled = true;
                plot.Visualisation[n].transform.localPosition =  new Vector3(xPos,yPos,zPos);
            }
            return plot;
        }

        private void outOfBounds(int elementIndex, float minDbVal, string behaviour){
            ref float plotValue = ref plot.SignalData.FreqMagnitude[elementIndex];
            
            switch (behaviour)
            {
                case "rest":
                    plotValue = (plotValue > minDbVal) ? plotValue : minDbVal;
                    break;
                case "hide":
                    if (plotValue > minDbVal)
                    {
                        // Set the element to visible
                        plot.Visualisation[elementIndex].GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        plotValue = minDbVal;
                        // Set the element to invisible
                        plot.Visualisation[elementIndex].GetComponent<MeshRenderer>().enabled = false;
                    }
                    break;
                default:
                    break;
            }
            
        }
    }
}