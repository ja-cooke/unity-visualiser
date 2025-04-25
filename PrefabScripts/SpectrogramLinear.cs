using UnityEngine;
using Visualiser;

public class SpectrogramLinear : MonoBehaviour
{
    DataPointVisualiser Visualiser;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Visualiser = new DataPointVisualiser(this, ChartType.Scatter, SubChartType.ScatterFreqLin);
        Visualiser.Start();
    }

    void Update()
    {
        Visualiser.Update();
    }
}
