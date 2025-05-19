/// Author: Jonathan Cooke
using UnityEngine;
using Visualiser;

/// <summary>
/// Class <c>AudioWaveform<\c>. Script for the SpectrogramLogarithmic visualiser prefab.
/// </summary>
public class SpectrogramLogarithmic : MonoBehaviour
{
    DataPointVisualiser Visualiser;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Visualiser = new DataPointVisualiser(this, ChartType.Scatter, SubChartType.ScatterFreqLogLog);
        Visualiser.Start();
    }

    void Update()
    {
        Visualiser.Update();
    }
}
