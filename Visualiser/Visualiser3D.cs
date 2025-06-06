/// Author: Jonathan Cooke
using UnityEngine;

namespace Visualiser 
{
    /// <summary>
    /// Visualiser Enginer Controller Class
    /// Called by prefabs to access and control the visualiser engine.
    /// 
    /// This one is called by MidSideSpectrogram and features support
    /// for the SignalProcessor class.
    /// </summary>
    public class Visualiser3D
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
        private SignalProcessor SignalProcessor;
        private float[] audioDataLeft = new float[audioBufferSize];
        private float[] audioDataRight = new float[audioBufferSize];
        private float[] freqDataLeft = new float[audioBufferSize];
        private float[] freqDataRight = new float[audioBufferSize];

        // ---------------------------------------------------------- //
        private ChartType ChartType { get; }
        private SubChartType ScatterType { get; }
        
        public Visualiser3D(MonoBehaviour visualiser, ChartType chartType, SubChartType subChartType)
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

            SignalProcessor = new(audioSource, audioBufferSize);
        }

        // Update is called once per frame
        public void Update()
        {
            // ----- Replenishes audio data ----------------------------
            SignalProcessor.Update();

            // ----------- PERFORM SIGNAL PROCESSING -------------------
            SignalProcessor.SumToMono();
            SignalProcessor.SideImage();
            ProcessedData processedData = SignalProcessor.GetProcessedData(); 

            // ----------- UPDATE THE VISUALISER ------------------------
            // Refreshes the visualiser
            if (audioSource.isPlaying & (audioSource.time != 0))
            {
                Plot.Update(processedData);
            }
            
        }
    }
}


