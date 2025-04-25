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
        // Bar charts
    }
    
    public enum ChartType 
    {
        Scatter,
        Bar,
    };

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

    public struct ProcessedData
    {
        public ProcessedData(SignalData signalData)
        {
            Raw = signalData;
            Processed = new SignalData();
            SpectralFlux = 0f;
            MMFCs = new float[0];
            CustomFFT = new float[0];
        }
        public SignalData Raw { get; }
        public SignalData Processed { get; }
        public float SpectralFlux { get; }
        public float[] MMFCs { get; }
        public float[] CustomFFT { get; }
    };
    
    public struct RefinedData
    {
        public int NumPoints;
        public SignalData ReducedRaw { get; set; }
        public SignalData ReducedProcessed { get; set; }
        public float SpectralFlux { get; set; }
        public float[] MMFCs { get; set; }
        public float[] CustomFFT { get; set; }
    }

    public struct PlotData3
    {
        public Vector3[] Data { get; set; }
        public int NumPoints { get; }
        public PlotData3(float[] xData, float[] yData, float[] zData)
        {
            bool accept = (xData.Length == yData.Length) && (xData.Length == zData.Length);
            if (accept)
            {
                NumPoints = xData.Length;
                Data = new Vector3[NumPoints];

                for (int i = 0; i < NumPoints; i++)
                {
                    bool xOutOfBoundsFlag, yOutOfBoundsFlag, zOutOfBoundsFlag;
                    xOutOfBoundsFlag = -0.5f > xData[i] | xData[i] > 0.5f;
                    yOutOfBoundsFlag = -0.5f > yData[i] | yData[i] > 0.5f;
                    zOutOfBoundsFlag = -0.5f > zData[i] | zData[i] > 0.5f;

                    if (xOutOfBoundsFlag | yOutOfBoundsFlag | zOutOfBoundsFlag)
                    {
                        Data[i] = new Vector3(float.NaN, float.NaN, float.NaN);
                    }
                    else
                    {
                        Data[i] = new Vector3(xData[i], yData[i], zData[i]);
                    }
                }
            }
            else
            {
                Data = null;
                NumPoints = 0;
            }
        }
    }

    public class Utils
    {
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

            return new SignalData(  dataPacket.TimeAmplitude, 
                                    reducedArray, 
                                    dataPacket.BufferSize, 
                                    dataPacket.SampleRate
                                    );
        }
    }
}