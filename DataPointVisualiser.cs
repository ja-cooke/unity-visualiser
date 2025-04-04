using UnityEngine;

namespace Visualiser {
    public class DataPointVisualiser : MonoBehaviour
    {
        // ---------------------------------------------------------- //
        // ----------------------- CONFIGURATION -------------------- //
        // ---------------------------------------------------------- //

        /* 
         * AUDIO BUFFER SIZE SETTING
         * Note that choosing a different buffer size will affect the 
         * resolution used by the visualiser.
         * Must be a value between 64 and 8192 as per the possible values
         * of AudioSources in Unity.
         */
        private const int audioBufferSize = 1024;
        /* 
         * Ensure that the string value for VisualiserBoundary matches the
         * name of a bounding cube GameObject in the Unity Scene.
         */
        private const string visualiserBoundary = "GraphBoundary";

        // ---------------------------------------------------------- //
        // ----------------------- Declarations --------------------- //
        // ---------------------------------------------------------- //
        private Transform graphBoundaryT;
        private GameObject audioObject;
        private AudioSource audioSource;
        private Plot visualiser;
        private float[] audioDataArray = new float[audioBufferSize];
        private float[] freqDataArray = new float[audioBufferSize];

        // ---------------------------------------------------------- //
        // ----------------------- METHODS -------------------------- //
        // ---------------------------------------------------------- //

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

            // Find the Graph Boundary
            graphBoundaryT = this.transform.Find(visualiserBoundary);
            visualiser = new Plot(graphBoundaryT, audioBufferSize, ChartType.Scatter, ScatterType.TimeLin);
        }

        // Update is called once per frame
        void Update()
        {
            // Replenishes audio data
            audioSource.GetOutputData(audioDataArray, 1);
            audioSource.GetSpectrumData(freqDataArray, 1, FFTWindow.BlackmanHarris);

            // Refreshes the visualiser
            visualiser.update(audioDataArray, audioBufferSize);
        }
    }
}


