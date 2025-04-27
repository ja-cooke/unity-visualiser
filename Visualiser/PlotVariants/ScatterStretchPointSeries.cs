using UnityEngine;

namespace Visualiser
{
    public class ScatterStretchPointSeries
    {
        private readonly ScatterStretchPoint[] Series;

        public ScatterStretchPointSeries(int numPoints, Transform graphBoundaryT)
        {
            Series = new ScatterStretchPoint[numPoints];
            // Instantiate cubes in the game world
            for (int point = 0; point<numPoints; point++)
            {
                // Save the mesh
                Series[point] = new ScatterStretchPoint(graphBoundaryT, PrimitiveType.Cube);
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