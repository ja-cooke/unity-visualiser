using System;
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
            SignalData[] signalData = new SignalData[AudioSource.clip.channels];

            for (int n = 0; n < Channels; n++)
            {
                float [] channelTimeData = new float[BufferSize];
                float [] channelFreqData = new float[BufferSize];

                AudioSource.GetOutputData(channelTimeData, n);
                AudioSource.GetSpectrumData(channelFreqData, n, FFTWindow.BlackmanHarris);
                
                signalData[n] = new SignalData( channelTimeData, 
                                                channelFreqData, 
                                                BufferSize, 
                                                SampleRate
                                                );
            }

            ProcessedDataPacket = new ProcessedData(signalData, signalData);

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
                    monoTime[i] = monoTime[i] + ProcessedDataPacket.Processed[n].TimeAmplitude[i];
                    monoFreq[i] = monoFreq[i] + ProcessedDataPacket.Processed[n].FreqMagnitude[i];
                }
            }

            SignalData monoSignal = new(monoTime, monoFreq, BufferSize, SampleRate);
            
            ProcessedDataPacket.Mono = monoSignal;
        }

        public void SideImage()
        {
            if (Channels == 2)
            {
                float[] sideFreq = new float[BufferSize];
                float[] sideTime = new float[BufferSize];
        
                for (int i = 0; i < BufferSize; i++)
                {
                    sideTime[i] = ProcessedDataPacket.Processed[0].TimeAmplitude[i] - ProcessedDataPacket.Processed[1].TimeAmplitude[i];
                    // Force fourier magnitude differences to a positive value
                    sideFreq[i] = Math.Abs(ProcessedDataPacket.Processed[0].FreqMagnitude[i] - ProcessedDataPacket.Processed[1].FreqMagnitude[i]);
                }

                SignalData sideData = new(sideTime, sideFreq, BufferSize, SampleRate);
                
                ProcessedDataPacket.Side = sideData;
            }
        }

    }
}