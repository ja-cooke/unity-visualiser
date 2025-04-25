using UnityEngine;
using Visualiser;

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
