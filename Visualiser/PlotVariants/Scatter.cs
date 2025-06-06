/// Author: Jonathan Cooke
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Visualiser
{
    /// <summary>
    /// Class used for 2D scatter plots.
    /// Third axis position will be placed in the midpoint of the graph
    /// boundary.
    /// </summary>
    public class Scatter : Chart
    {
        private readonly SubChartType ScatterType;
        private readonly ScatterPointSeries Series;
        /// <summary>
        /// This is a peak value stored to help with normalisation.
        /// </summary>
        private float PeakMagnitude = 0;
        
            
        public Scatter(Transform graphBoundaryT, int bufferSize, SubChartType scatterType){
            ScatterType = scatterType;
            
            // Instantiate GameObjects in the game world
            Series = new ScatterPointSeries(bufferSize, graphBoundaryT);
        }

        public override void Update(SignalData dataPacket)
        {
            Dictionary<SubChartType, Action<SignalData>> axes = new()
            {
                {SubChartType.ScatterTimeLin, TimeLin},
                {SubChartType.ScatterFreqLin, FreqLin},
                {SubChartType.ScatterFreqLog, FreqLog},
                {SubChartType.ScatterFreqLogLog, FreqLogLog},
            };

            axes[ScatterType](dataPacket);
        }

        /* 
        * Plots 2D waveform within a 3D space
        */
        private void TimeLin(SignalData dataPacket)
        {
            // Initialise arrays of the correct size to hold the plot data
            int N =  dataPacket.TimeAmplitude.Length;
            float[] xData = new float[N];
            float[] yData = new float[N];
            float[] zData = new float[N];

            // x coordinate is depth, y coordinate is amplitude, z coordinate is time / frequency axis
            float[] dataArray = dataPacket.TimeAmplitude;
            
            for (int n = 0; n < N; n++)
            {
                // 2D plot
                xData[n] = 0;
                // Divide by 2 for a vertically centred plot of scale -0.5 <-> +0.5
                yData[n] = dataArray[n]/2;
                // -0.5f offset for a horizontally centred plot
                zData[n] = -0.5f + (n/(float)N);
            }
            Series.Update(new PlotData3(xData, yData, zData));
        }

        /// <summary>
        /// Plots a linear 2D frequency spectrum in a 3D space.
        /// </summary>
        /// <param name="dataPacket"></param>
        private void FreqLin(SignalData dataPacket)
        {
            // Initialise arrays of the correct size to hold the plot data
            int N =  dataPacket.FreqMagnitude.Length;
            float[] xData = new float[N];
            float[] yData = new float[N];
            float[] zData = new float[N];

            // x coordinate is depth, y coordinate is magnitude, z coordinate is frequency axis
            float[] dataArray = dataPacket.FreqMagnitude;

            for (int n = 0; n < N; n++)
            {
                // 2D plot
                xData[n] = 0;
                // Scale the freqency data so that it is normalised to the highest level
                PeakMagnitude = (PeakMagnitude > dataArray[n]) ? PeakMagnitude : dataArray[n];
                // Offset by -0.5 to rest on the bottom face of the boundary
                yData[n] = dataArray[n]/PeakMagnitude - 0.5f;
                // 0.5f offset for a horizontally centred plot
                zData[n] = 0.5f - (n/(float)N);
            }
            Series.Update(new PlotData3(xData, yData, zData));
        }

        /// <summary>
        /// Plots a logarithmic 2D frequency spectrum in a 3D space.
        /// Magnitude axis remains linear.
        /// </summary>
        /// <param name="dataPacket"></param>
        private void FreqLog(SignalData dataPacket)
        {
            // Initialise arrays of the correct size to hold the plot data
            int N =  dataPacket.FreqMagnitude.Length;
            int bufferSize = dataPacket.BufferSize;
            float[] xData = new float[N];
            float[] yData = new float[N];
            float[] zData = new float[N];

            // x coordinate is depth, y coordinate is magnitude, z coordinate is frequency axis
            float[] dataArray = dataPacket.FreqMagnitude;

            // ---- AXIS SCALING ----

            int nyquist = dataPacket.BufferSize/2;
            float freqRes = 1f/(float)dataPacket.BufferSize;
            float[] freqAxis = new float[dataPacket.BufferSize];

            // Determines values and applies log scaling for frequency axis
            for (int i = 0; i < bufferSize; i++){
                freqAxis[i] = (float)Math.Log10(freqRes + freqRes*i)/(float)Math.Log10(bufferSize);
            }

            // ---- PLOT POSITIONS ----

            for (int n = 0; n < N; n++)
            {
                PeakMagnitude = (PeakMagnitude > dataArray[n]) ? PeakMagnitude : dataArray[n];
                // 2D plot
                xData[n] = 0;
                // Offset by -0.5 to rest on the bottom face of the boundary
                yData[n] = dataArray[n]/PeakMagnitude - 0.5f;
                
                zData[n] = - 0.5f - freqAxis[n];
            }
            Series.Update(new PlotData3(xData, yData, zData));
        }

        /// <summary>
        /// Plots a logarithmic 2D frequency spectrum in a 3D space.
        /// 
        /// Magnitude axis is converted to dBFS scale.
        /// </summary>
        /// <param name="dataPacket"></param>
        private void FreqLogLog(SignalData dataPacket)
        {
            // Data Reduction Step
            dataPacket = Utils.ReduceSpectrumData(dataPacket);

            // Initialise arrays of the correct size to hold the plot data
            int N =  dataPacket.FreqMagnitude.Length;
            int bufferSize = dataPacket.BufferSize;
            float[] xData = new float[N];
            float[] yData = new float[N];
            float[] zData = new float[N];

            // SERIES SETTINGS
            const float minDbVal = -120;
            const float alignment = 0.5f;
            const float depth = 0f;

            // x coordinate is depth, y coordinate is magnitude, z coordinate is frequency axis
            float[] dataArray = dataPacket.FreqMagnitude;

            // ---- AXIS SCALING ----

            int nyquist = dataPacket.BufferSize/2;
            float freqRes = 1f/(float)dataPacket.BufferSize;

            float[] freqAxis = new float[bufferSize];

            // Determines values and applies log scaling for frequency axis
            for (int i = 0; i < bufferSize; i++){
                freqAxis[i] = (float)Math.Log10(freqRes + freqRes*i)/(float)Math.Log10(bufferSize);
            }
            // Determines values and applies log scaling for magnitude axis
            for (int n = 0; n < N; n++){
                // Uses voltage decibel scale: y (dB) = 20 * log10(x/ref)
                dataArray[n] = 20f*(float)Math.Log10(dataArray[n]);
            }

            // ---- PLOT POSITIONS ----

            for (int n = 0; n < N; n++)
            {
                // 2D plot
                xData[n] = depth;
                // Offset to rest on the top face of the boundary
                yData[n] = dataArray[n]/(-minDbVal) + alignment;
                
                zData[n] = - alignment - freqAxis[n];
            }
            Series.Update(new PlotData3(xData, yData, zData));
        }
    }
}