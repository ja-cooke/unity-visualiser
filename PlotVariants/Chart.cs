using UnityEngine;

namespace Visualiser
{
    public abstract class Chart
    {
        private GameObject[] plot;
        private PlotType plotType;

        public virtual void update(float[] dataArray, int audioBufferSize){}

        public GameObject[] getPlot()
        {
            return plot;
        }
    }
}

