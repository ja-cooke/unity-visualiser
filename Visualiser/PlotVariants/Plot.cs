/// Author: Jonathan Cooke
using System.Collections.Generic;
using UnityEngine;

namespace Visualiser 
{
    /// <summary>
    /// Interface between the Engine Controller and the plot functions
    /// calls the chosen Chart class and subclass using the dictionary.
    /// </summary>
    public class Plot
    {
        private Chart plot;
        private ChartType chartType;

        public Plot(Transform graphBoundaryT, int bufferSize, ChartType chartType, SubChartType subChartType)
        {       
            Dictionary<ChartType, Chart> method = new Dictionary<ChartType, Chart>{
                {ChartType.Scatter, new Scatter(graphBoundaryT, bufferSize, subChartType)},
                {ChartType.ScatterStretch, new ScatterStretch(graphBoundaryT, bufferSize, subChartType)},
            };
            plot = method[chartType];

            // Turn off rendering for the sample material and text.
            graphBoundaryT.parent.Find("SampleMaterial").GetComponent<MeshRenderer>().enabled = false;
            graphBoundaryT.parent.Find("Orientation").GetComponent<MeshRenderer>().enabled = false;
        }

        public void Update(SignalData dataPacket)
        {
            plot.Update(dataPacket);
        }

        public void Update(ProcessedData dataPacket)
        {
            plot.Update(dataPacket);
        }

        public ScatterPointSeries GetPlot()
        {
            return plot.GetSeries();
        }
    }
}