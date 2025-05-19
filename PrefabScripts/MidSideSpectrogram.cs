/// Author: Jonathan Cooke
using UnityEngine;
using Visualiser;

/// <summary>
/// Class <c>MidSideSpectrogram<\c>. Script for the MidSideSpectrogram visualiser prefab.
/// </summary>
public class MidSideSpectrogram : MonoBehaviour
{
    Visualiser3D Visualiser;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Visualiser = new Visualiser3D(this, ChartType.ScatterStretch, SubChartType.ScatterStretchFreqLogLog);
        Visualiser.Start();
    }

    void Update()
    {
        Visualiser.Update();
    }
}
