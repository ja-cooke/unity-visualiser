using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Visualiser{
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
        public SignalData(float[] signalWaveform, float[] spectrumData, int bufferSize){
            BufferSize = bufferSize;
            TimeAmplitude = signalWaveform;
            FreqMagnitude = spectrumData;
        }
        public int BufferSize { get; }
        public float[] TimeAmplitude { get; }
        public float[] FreqMagnitude { get; }
    };

    public struct VisualiserFrame
    {
        public VisualiserFrame(GameObject[] visualisation, SignalData signalData)
        {
            Visualisation = visualisation;
            SignalData = signalData;
        }
        public SignalData SignalData { get; set; }
        public GameObject[] Visualisation { get; set; }
    };

    public class Utils
    {
        public static int[] ReducePlotData(VisualiserFrame visualiserFrame)
        {
            float reductionFactor = 1.2f;
            int[] indices = new int[0];
            int frameLength = visualiserFrame.SignalData.FreqMagnitude.Length;

            float[] x = new float[frameLength];

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