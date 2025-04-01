using System.Runtime.Serialization;
using UnityEditor.SearchService;
using UnityEngine;

namespace Visualiser {
    public class DataPointVisualiser : MonoBehaviour
    {
        private const int audioBufferSize = 1024;
        private Transform graphBoundaryT;
        private GameObject audioObject;
        private AudioSource audioSource;
        private GameObject[] waveformPlot;
        private Plot.Setup visualiser;
        private float[] audioDataArray = new float[audioBufferSize];
        private float[] freqDataArray = new float[audioBufferSize];

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
                audioObject = new GameObject("audioObject", typeof(AudioSource));
                audioSource = audioObject.GetComponent<AudioSource>();
                
                // Load the resource for the AudioClip
                AudioClip audioClip = Resources.Load<AudioClip>("test2");
                // Load the resource into the AudioSource
                audioSource.clip = audioClip;
                audioSource.Play();
                // Get Visualisation data
                audioSource.GetOutputData(audioDataArray, 1);
                audioSource.GetSpectrumData(freqDataArray, 1, FFTWindow.BlackmanHarris);
                

                // Find the Graph Boundary

                graphBoundaryT = this.transform.Find("GraphBoundary");

                visualiser = new Plot.Setup(graphBoundaryT, audioBufferSize);
                waveformPlot = visualiser.getPlot();
        }

        // Update is called once per frame
        void Update()
        {
            audioSource.GetOutputData(audioDataArray, 1);
            audioSource.GetSpectrumData(freqDataArray, 1, FFTWindow.BlackmanHarris);

            plot(audioDataArray, audioBufferSize);
        }

        /* 
        * Plots 2D waveform within a 3D space
        */
        private void plot(float[] dataArray, int audioBufferSize){
            // x coordinate is depth, y coordinate is amplitude, z coordinate is time / frequency axis
            int n = 0;
            foreach (float datum in dataArray){
                // 2D plot
                float xPos = 0;
                // Divide by 2 for a vertically centred plot of scale -0.5 <-> +0.5
                float yPos = dataArray[n]/2;
                // -0.5f offset for a horizontally centred plot
                float zPos = -0.5f + (n/(float)audioBufferSize);

                waveformPlot[n].transform.localPosition =  new Vector3(xPos,yPos,zPos);
                n++;
            }

        }
    }
}


