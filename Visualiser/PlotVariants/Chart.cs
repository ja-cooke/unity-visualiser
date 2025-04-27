namespace Visualiser
{
    public abstract class Chart
    {
        private ScatterPointSeries Series;

        public virtual void Update(SignalData dataPacket){}

        public virtual void Update(ProcessedData dataPacket){}

        public ScatterPointSeries GetSeries()
        {
            return Series;
        }
    }
}

