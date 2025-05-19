/// Author: Jonathan Cooke
using UnityEngine;

namespace Visualiser
{
    /// <summary>
    /// Holds an array of ScatterPoints.
    /// </summary>
    public class ScatterPointSeries
    {
        private readonly ScatterPoint[] Series;

        public ScatterPointSeries(int numPoints, Transform graphBoundaryT)
        {
            Series = new ScatterPoint[numPoints];
            // Instantiate cubes in the game world
            for (int point = 0; point<numPoints; point++)
            {
                // Save the mesh
                Series[point] = new ScatterPoint(graphBoundaryT, PrimitiveType.Cube);
            }
        }

        /// <summary>
        /// Moves datapoints to the locations specified in the plot data
        /// </summary>
        /// <param name="plotData"></param>
        public void Update(PlotData3 plotData)
        {
            for (int i = 0; i < plotData.NumPoints-1; i++)
            {
                Series[i].MoveTo(plotData.Data[i]);
            }
            // The maximum number of meshes that might be required by the
            // visualisation are instantiated at runtime, but fewer points
            // can be plotted. All excess points instantiated but not called
            // this frame are set to invisible.
            for (int i = plotData.NumPoints; i < Series.Length-1; i++)
            {
                Series[i].Visible(false);
            }
        }
    }
}