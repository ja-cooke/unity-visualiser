using System.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.PlayerLoop;

namespace Visualiser
{
    public class SignalProcessor
    {
        private ProcessedData ProcessedDataPacket;
        private readonly AudioSource AudioSource;
        private SignalData[] SignalData;
        private readonly int BufferSize;
        private readonly int Channels;
        private readonly int SampleRate;

        public SignalProcessor(AudioSource audioSource, int bufferSize)
        {
            AudioSource = audioSource;
            BufferSize = bufferSize;
            Channels = AudioSource.clip.channels;
            SampleRate = AudioSource.clip.frequency;

        }

        public void Update()
        {
            SignalData = new SignalData[AudioSource.clip.channels];

            for (int n = 0; n < Channels; n++)
            {
                float [] channelTimeData = new float[BufferSize];
                float [] channelFreqData = new float[BufferSize];
                AudioSource.GetOutputData(channelTimeData, n+1);
                AudioSource.GetSpectrumData(channelFreqData, n+1, FFTWindow.BlackmanHarris);
                
                SignalData[n] = new SignalData( channelTimeData, 
                                                channelFreqData, 
                                                BufferSize, 
                                                SampleRate
                                                );
            }

        }

        public ProcessedData GetProcessedData()
        {
            return ProcessedDataPacket;
        }

        public void SumToMono()
        {
            float[] monoTime = new float[BufferSize];
            float[] monoFreq = new float[BufferSize];
            
            for (int n = 0; n < Channels; n++)
            {
                for (int i = 0; i < BufferSize; i++)
                {
                    monoTime[i] = monoTime[i] + SignalData[n].TimeAmplitude[i];
                    monoFreq[i] = monoFreq[i] + SignalData[n].FreqMagnitude[i];
                }
            }

            SignalData monoSignal = new SignalData(monoTime, monoFreq, BufferSize, SampleRate);
            
            ProcessedDataPacket = new ProcessedData(SignalData[0], monoSignal);
        }

    }
}