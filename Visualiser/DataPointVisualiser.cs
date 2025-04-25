using UnityEngine;

namespace Visualiser 
{
    public class DataPointVisualiser
    {
        private MonoBehaviour Visualiser;
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
        private Plot Plot;
        private float[] audioDataArray = new float[audioBufferSize];
        private float[] freqDataArray = new float[audioBufferSize];

        // ---------------------------------------------------------- //
        private ChartType ChartType { get; }
        private SubChartType ScatterType { get; }
        
        public DataPointVisualiser(MonoBehaviour visualiser, ChartType chartType, SubChartType subChartType)
        {
            Visualiser = visualiser;
            ChartType = chartType;
            ScatterType = subChartType;
        }

        // ---------------------------------------------------------- //
        // ----------------------- METHODS -------------------------- //
        // ---------------------------------------------------------- //

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void Start()
        {
            audioObject = Visualiser.gameObject;
            audioSource = audioObject.GetComponent<AudioSource>();
            
            // Load the resource for the AudioClip from the Visualiser object
            AudioClip audioClip = Visualiser.GetComponent<AudioSource>().clip;
            // Load the resource into the AudioSource
            audioSource.clip = audioClip;

            // Find the Graph Boundary
            graphBoundaryT = Visualiser.GetComponent<Transform>();

            Plot = new Plot(graphBoundaryT, audioBufferSize, ChartType, ScatterType);
        }

        // Update is called once per frame
        public void Update()
        {
            // Replenishes audio data
            audioSource.GetOutputData(audioDataArray, 1);
            audioSource.GetSpectrumData(freqDataArray, 1, FFTWindow.BlackmanHarris);

            SignalData audioDataPacket = new(audioDataArray, freqDataArray, audioBufferSize, audioSource.clip.frequency);

            // Refreshes the visualiser
            Plot.Update(audioDataPacket);
        }
    }
}


