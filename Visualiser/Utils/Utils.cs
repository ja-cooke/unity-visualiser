using System;
using System.Linq;
using Unity.VisualScripting;
using Unity.XR.CoreUtils.Datums;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Visualiser
{
    public enum ScatterType 
    {
        TimeLin,
        FreqLin,
        FreqLog,
        FreqLogLog,
        FreqLogLogE,
    };

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
                    xOutOfBoundsFlag = ((-0.5f >= xData[i]) & (xData[i] >= 0.5f)) ? false : true;
                    yOutOfBoundsFlag = ((-0.5f >= yData[i]) & (yData[i] >= 0.5f)) ? false : true;
                    zOutOfBoundsFlag = ((-0.5f >= zData[i]) & (zData[i] >= 0.5f)) ? false : true;

                    if (!xOutOfBoundsFlag & !yOutOfBoundsFlag & !zOutOfBoundsFlag)
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
        public static int[] ReduceSpectrumData(SignalData dataPacket)
        {
            float reductionFactor = 1.2f;
            int[] indices = new int[0];
            int frameLength = dataPacket.FreqMagnitude.Length;

            float[] x = dataPacket.FreqMagnitude;

            float lastElement = 0;

            int j = 0;
            for (int i = 0; i < frameLength; i++){
                // If this element hasn't already been added:
                if (x[i] != lastElement){
                    indices.Concat(new int[] {i}).ToArray();
                    lastElement = x[i];
                }
                /* 
                * i = 1 + reductionFactor^(j+1) :: rounded down to integer
                * Makes a :: y = e^x + 1 :: shaped curve from which to
                * choose new indexes to include. 
                * Low indices (1, 2, 3...) will almost always be included
                * but higher indices are skipped with increasing frequency.
                */
                i = 1 + (int)Math.Floor(Math.Pow(reductionFactor,++j));;
            }
            return indices;
        }
    }
}