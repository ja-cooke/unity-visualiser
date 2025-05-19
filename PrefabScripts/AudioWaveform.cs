/// Author: Jonathan Cooke
using UnityEngine;
using Visualiser;

/// <summary>
/// Class <c>AudioWaveform<\c>. Script for the AudioWaveform visualiser prefab.
/// </summary>
public class AudioWaveform : MonoBehaviour
{
    DataPointVisualiser Visualiser;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Visualiser = new DataPointVisualiser(this, ChartType.Scatter, SubChartType.ScatterTimeLin);
        Visualiser.Start();
    }

    void Update()
    {
        Visualiser.Update();
    }
}
