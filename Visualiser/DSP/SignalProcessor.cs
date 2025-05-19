/// Author: Jonathan Cooke
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
    /// <summary>
    /// Class for performing digital signal processing operations.
    /// 
    /// The processed data is held in class and reprocessed in series
    /// each time a new processing method is called.
    /// 
    /// Use the GetProcessedData() getter to return the processed data
    /// when you have applied the desired operations.
    /// </summary>
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

        // Method to replenish the audio data from an audio source for each frame
        public void Update()
        {
            SignalData[] signalData = new SignalData[AudioSource.clip.channels];

            // Multichannel audio supported!
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

        /// <summary>
        /// Returns the processed data with any changes that have been made
        /// </summary>
        /// <returns></returns>
        public ProcessedData GetProcessedData()
        {
            return ProcessedDataPacket;
        }

        /// <summary>
        /// Creates a mono signal from any number of channels
        /// </summary>
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

        /// <summary>
        /// Creates a side image. Supports stereo (2 channel) signals only.
        /// Does not return a true side image for frequency.
        /// </summary>
        public void SideImage()
        {
            if (Channels == 2)
            {
                float[] sideFreq = new float[BufferSize];
                float[] sideTime = new float[BufferSize];
        
                for (int i = 0; i < BufferSize; i++)
                {
                    // This is a true side image
                    sideTime[i] = ProcessedDataPacket.Processed[0].TimeAmplitude[i] - ProcessedDataPacket.Processed[1].TimeAmplitude[i];
                    // Force fourier magnitude differences to a positive value
                    // -- Note that this means it is not a true side image.
                    // Complex Fourier values are required for a true side image,
                    // but the Unity API will only return magnitudes.
                    sideFreq[i] = Math.Abs(ProcessedDataPacket.Processed[0].FreqMagnitude[i] - ProcessedDataPacket.Processed[1].FreqMagnitude[i]);
                }

                SignalData sideData = new(sideTime, sideFreq, BufferSize, SampleRate);
                
                ProcessedDataPacket.Side = sideData;
            }
        }

    }
}