using System;
using System.Collections.Generic;
using Oculus.Interaction.Editor;
using Unity.XR.CoreUtils.Datums;
using UnityEngine;

namespace Visualiser 
{
    public class Plot
    {
        private Chart plot;
        private PlotType plotType;

        public Plot(Transform graphBoundaryT, int bufferSize, PlotType plotType)
        {       
            Dictionary<PlotType, Chart> method = new Dictionary<PlotType, Chart>{
                {PlotType.TimeLin, new Scatter(graphBoundaryT, bufferSize, plotType)}
            };
            plot = method[plotType];
        }

        public void update(float[] dataArray, int audioBufferSize)
        {
            plot.update(dataArray, audioBufferSize);

        }

        public GameObject[] getPlot()
        {
            return plot.getPlot();
        }
    }
}