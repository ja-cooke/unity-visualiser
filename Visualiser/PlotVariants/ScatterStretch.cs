using System;
using System.Collections.Generic;
using UnityEngine;

namespace Visualiser
{
    public class ScatterStretch : Chart
    {
        private readonly SubChartType ScatterStretchType;
        private readonly ScatterStretchPointSeries Series;
        private float PeakMagnitude = 0;
        RefinedData RefinedDataPacket; 
        
            
        public ScatterStretch(Transform graphBoundaryT, int bufferSize, SubChartType scatterStretchType){
            ScatterStretchType = scatterStretchType;
            
            // Instantiate GameObjects in the game world
            Series = new ScatterStretchPointSeries(bufferSize, graphBoundaryT);
        }

        public override void Update(ProcessedData dataPacket)
        {
            Dictionary<SubChartType, Action<ProcessedData>> axes = new()
            {
                {SubChartType.ScatterStretchFreqLogLog, FreqLogLog},
            };

            axes[ScatterStretchType](dataPacket);
        }

        private void FreqLogLog(ProcessedData dataPacket)
        {
            // Data Reduction Step
            RefinedDataPacket = Utils.ReduceSpectrumData(dataPacket);

            // Initialise arrays of the correct size to hold the plot data
            int N =  RefinedDataPacket.ReducedMono.FreqMagnitude.Length;
            int bufferSize = dataPacket.Raw[0].BufferSize;
            float[] xData = new float[N];
            float[] yData = new float[N];
            float[] zData = new float[N];

            // SERIES SETTINGS
            const float minDbVal = -120;
            const float alignment = 0.5f;

            // x coordinate is depth (side image), y coordinate is magnitude, z coordinate is frequency axis
            float[] dataArray = RefinedDataPacket.ReducedMono.FreqMagnitude;
            float[] depth = RefinedDataPacket.ReducedSide.FreqMagnitude;

            // ---- AXIS SCALING ----

            int nyquist = bufferSize/2;
            float freqRes = 1f/(float)bufferSize;

            float[] monoFreqAxis = new float[bufferSize];
            float[] sideFreqAxis = new float[bufferSize];

            // Determines values and applies log scaling for frequency axis
            for (int i = 0; i < bufferSize; i++){
                monoFreqAxis[i] = (float)Math.Log10(freqRes + freqRes*i)/(float)Math.Log10(bufferSize);
                //sideFreqAxis[i] = (float)Math.Log10(freqRes + freqRes*i)/(float)Math.Log10(bufferSize);
            }
            // Determines values and applies log scaling for magnitude axis
            for (int n = 0; n < N; n++){
                // Uses voltage decibel scale: y (dB) = 20 * log10(x/ref)
                dataArray[n] = 20f*(float)Math.Log10(dataArray[n]);
                depth[n] = 20f*(float)Math.Log10(depth[n]);
            }

            // ---- PLOT POSITIONS ----

            for (int n = 0; n < N; n++)
            {
                // 3D stretch plot - x values represent scale in the z-axis
                //
                // The correction factor of -0.17f assumes that the greatest
                // magnitude of side image frequency is ~0.17f * mindBVal * 2 
                // below 0dB. This isn't quite correct and so the absolute value
                // needs to be taken to prevent negative values being passed onto
                // the plot data.
                xData[n] = Math.Abs(depth[n]/(minDbVal)/2 - 0.17f); // should all be +ve values
                // Offset to rest on the top face of the boundary
                yData[n] = dataArray[n]/(-minDbVal) + alignment;
                
                zData[n] = - alignment - monoFreqAxis[n];
            }
            Series.Update(new PlotData3(xData, yData, zData));
        }
    }
}