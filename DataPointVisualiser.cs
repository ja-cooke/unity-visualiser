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
        private Plot visualiser;
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

                visualiser = new Plot(graphBoundaryT, audioBufferSize);
        }

        // Update is called once per frame
        void Update()
        {
            audioSource.GetOutputData(audioDataArray, 1);
            audioSource.GetSpectrumData(freqDataArray, 1, FFTWindow.BlackmanHarris);

            visualiser.update(audioDataArray, audioBufferSize);
        }
    }
}


