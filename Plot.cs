using System.Collections.Generic;
using UnityEngine;

namespace Visualiser 
{
    public class Plot
    {
        private Chart plot;
        private ChartType chartType;

        public Plot(Transform graphBoundaryT, int bufferSize, ChartType chartType, ScatterType scatterType)
        {       
            Dictionary<ChartType, Chart> method = new Dictionary<ChartType, Chart>{
                {ChartType.Scatter, new Scatter(graphBoundaryT, bufferSize, scatterType)}
            };
            plot = method[chartType];
        }

        public void update(SignalData signalDataPacket)
        {
            plot.update(signalDataPacket);
        }

        public GameObject[] getPlot()
        {
            return plot.getPlot();
        }
    }
}