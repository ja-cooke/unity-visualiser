using UnityEngine;

namespace Visualiser
{
    public abstract class Chart
    {
        private GameObject[] plot;

        public virtual void update(SignalData signalDataPacket){}

        public GameObject[] getPlot()
        {
            return plot;
        }
    }
}

