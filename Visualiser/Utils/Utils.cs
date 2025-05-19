/// This class contains data structures and methods used throughout the
/// visualiser engine.
/// Author: Jonathan Cooke
using System;
using UnityEngine;

namespace Visualiser
{
    public enum SubChartType
    {
        // Scatter charts
        ScatterTimeLin,
        ScatterFreqLin,
        ScatterFreqLog,
        ScatterFreqLogLog,
        // ScatterStretch charts
        ScatterStretchFreqLogLog,
        // Bar charts
    }
    
    public enum ChartType 
    {
        Scatter,
        ScatterStretch,
        Bar,
    };

    /// <summary>
    /// Struct for holding time and frequency domain data for a single signal.
    /// Supports multichannel audio.
    /// </summary>
    public struct SignalData
    {
        public int BufferSize { get; }
        public int SampleRate { get; }
        public float[] TimeAmplitude { get; }
        public float[] FreqMagnitude { get; }
        public SignalData(float[] signalWaveform, float[] spectrumData, int bufferSize, int sampleRate)
        {
            BufferSize = bufferSize;
            TimeAmplitude = signalWaveform;
            FreqMagnitude = spectrumData;
            SampleRate = sampleRate;
        }
    };

    /// <summary>
    /// Struct for holding collections of SignalData after signal processing
    /// has taken place. Makes sure the raw and processed signals are stored
    /// separately so that the raw data is preserved and readonly.
    /// </summary>
    public struct ProcessedData
    {
        public ProcessedData(SignalData[] rawSignalData, SignalData[] processedData)
        {
            Raw = rawSignalData;
            Processed = processedData;
            SpectralFlux = 0f;
            MMFCs = null;
            Mono = new SignalData();
            Side = new SignalData();
        }
        public SignalData[] Raw { get; }
        public SignalData[] Processed { get; set; }
        public SignalData Mono { get; set; }
        public SignalData Side { get; set; }
        public float SpectralFlux { get; set; }
        public float[] MMFCs { get; set; }
    };
    
    /// <summary>
    /// Holds signal data after the number of points have been reduced for use
    /// in visualiser plots.
    /// </summary>
    public struct RefinedData
    {
        public int NumPoints;
        public SignalData[] ReducedRaw { get; set; }
        public SignalData[] ReducedProcessed { get; set; }
        public SignalData ReducedMono { get; set; }
        public SignalData ReducedSide { get; set; }
        public float SpectralFlux { get; set; }
        public float[] ReducedMMFCs { get; set; }
    }

    /// <summary>
    /// Holds Cartesian co-ordinate data for visual plotting in the game scene
    /// </summary>
    public struct PlotData3
    {
        public Vector3[] Data { get; set; }
        public int NumPoints { get; }
        /// <summary>
        /// Constructor enforces rules that all input arrays must be the
        /// same length, and that all points lie between -0.5f and 0.5f.
        /// Out of bounds points are stored as NaN and will not be rendered
        /// in the game scene.
        /// </summary>
        /// <param name="xData"></param>
        /// <param name="yData"></param>
        /// <param name="zData"></param>
        public PlotData3(float[] xData, float[] yData, float[] zData)
        {
            // Checks that arrays are the same length
            // i.e. all data points have the same number of dimensions
            // will reject the data if they do not match as something
            // will almost certainly be wrong.
            bool accept = (xData.Length == yData.Length) && (xData.Length == zData.Length);
            if (accept)
            {
                NumPoints = xData.Length;
                Data = new Vector3[NumPoints];

                for (int i = 0; i < NumPoints; i++)
                {
                    // Checks if the point lies within the boundary of the
                    // GraphBoundaryT component. Out of bounds points are
                    // flagged and stored as NaN.
                    bool xOutOfBoundsFlag, yOutOfBoundsFlag, zOutOfBoundsFlag;
                    xOutOfBoundsFlag = -0.5f > xData[i] | xData[i] > 0.5f;
                    yOutOfBoundsFlag = -0.5f > yData[i] | yData[i] > 0.5f;
                    zOutOfBoundsFlag = -0.5f > zData[i] | zData[i] > 0.5f;

                    if (xOutOfBoundsFlag | yOutOfBoundsFlag | zOutOfBoundsFlag)
                    {
                        // Out of bounds point, will be stored as NaN
                        Data[i] = new Vector3(float.NaN, float.NaN, float.NaN);
                    }
                    else
                    {
                        // In bounds point, co-ordinate data will be preserved
                        Data[i] = new Vector3(xData[i], yData[i], zData[i]);
                    }
                }
            }
            else
            {
                // The lengths of the coordinate arrays do not match.
                // No data will be stored.
                Data = null;
                NumPoints = 0;
            }
        }
    }

    public class Utils
    {
        /// <summary>
        /// Will return a SignalData packet with fewer data points than
        /// the one given.
        /// 
        /// Data points are removed algorithmically to make this less
        /// noticeable for logarithmically plotted data.
        /// </summary>
        /// <param name="dataPacket"></param>
        /// <returns></returns>
        public static SignalData ReduceSpectrumData(SignalData dataPacket)
        {
            float reductionFactor = 3.0f;
            int frameLength = dataPacket.FreqMagnitude.Length;
            float[] reducedData = new float[frameLength];

            float[] x = dataPacket.FreqMagnitude;

            float lastElement = 0;
            int reducedLength = 0;
            int j = 0;
            for (int i = 0; i < frameLength; i++){
                // If this element hasn't already been added:
                if (x[i] != lastElement){
                    reducedData[i] = x[i];
                    lastElement = x[i];
                    reducedLength++;
                }
                /* 
                * i = j + reductionFactor^(j/4) - 1 :: rounded down to integer
                * Makes a :: y = x + e^kx :: shaped curve from which to
                * choose new indexes to include. 
                * Low indices (1, 2, 3...) will almost always be included
                * but higher indices are skipped with increasing frequency.
                */
                i = j + (int)Math.Floor(Math.Pow(reductionFactor,(float)++j/150f)) - 1;
            }

            float[] reducedArray = new float[reducedLength];

            for (int i = 0; i < reducedLength; i++){
                reducedArray[i] = reducedData[i];
            }

            // New data packaged for output
            return new SignalData(  dataPacket.TimeAmplitude, 
                                    reducedArray, 
                                    dataPacket.BufferSize, 
                                    dataPacket.SampleRate
                                    );
        }

        /// <summary>
        /// Will return a RefinedData packet with fewer data points than
        /// the ProcessedData packet given.
        /// 
        /// Data points are removed algorithmically to make this less
        /// noticeable for logarithmically plotted data.
        /// </summary>
        /// <param name="dataPacket"></param>
        /// <returns></returns>
        public static RefinedData ReduceSpectrumData(ProcessedData dataPacket)
        {
            float reductionFactor = 3.0f;
            int frameLength = dataPacket.Mono.FreqMagnitude.Length;
            float[] reducedDataMono = new float[frameLength];
            float[] reducedDataSide = new float[frameLength];

            float[] x = dataPacket.Mono.FreqMagnitude;
            float[] y = dataPacket.Side.FreqMagnitude;

            float lastElement = 0;
            int reducedLength = 0;
            int j = 0;
            for (int i = 0; i < frameLength; i++){
                // If this element hasn't already been added:
                if (x[i] != lastElement){
                    reducedDataMono[i] = x[i];
                    reducedDataSide[i] = y[i];
                    lastElement = x[i];
                    reducedLength++;
                }
                /* 
                * i = j + reductionFactor^(j/4) - 1 :: rounded down to integer
                * Makes a :: y = x + e^kx :: shaped curve from which to
                * choose new indexes to include. 
                * Low indices (1, 2, 3...) will almost always be included
                * but higher indices are skipped with increasing frequency.
                */
                i = j + (int)Math.Floor(Math.Pow(reductionFactor,(float)++j/150f)) - 1;
            }

            float[] reducedArrayMono = new float[reducedLength];
            float[] reducedArraySide = new float[reducedLength];

            for (int i = 0; i < reducedLength; i++){
                reducedArrayMono[i] = reducedDataMono[i];
                reducedArraySide[i] = reducedDataSide[i];
            }

            // New data packaged for output
            RefinedData refinedData = new();
            
            refinedData.ReducedMono = new SignalData(   dataPacket.Mono.TimeAmplitude, 
                                                        reducedArrayMono, 
                                                        dataPacket.Mono.BufferSize, 
                                                        dataPacket.Mono.SampleRate
                                                        );

            refinedData.ReducedSide = new SignalData(   dataPacket.Side.TimeAmplitude, 
                                                        reducedArraySide, 
                                                        dataPacket.Side.BufferSize, 
                                                        dataPacket.Side.SampleRate
                                                        );
            return refinedData;
        }
    }
}