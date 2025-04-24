using UnityEngine;

namespace Visualiser
{
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

        public void Update(PlotData3 plotData)
        {
            for (int i = 0; i < plotData.NumPoints-1; i++)
            {
                Series[i].MoveTo(plotData.Data[i]);
            }
            for (int i = plotData.NumPoints; i < Series.Length-1; i++)
            {
                Series[i].Visible(false);
            }
        }
    }
}