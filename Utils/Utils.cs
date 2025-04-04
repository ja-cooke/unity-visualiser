namespace Visualiser{
    public enum ScatterType 
    {
        TimeLin,
        FreqLin,
        FreqLog,
        FreqLogLog,
    };

    public enum ChartType 
    {
        Scatter,
        Bar,
    };

    public struct SignalData{
        public SignalData(float[] signalWaveform, float[] spectrumData, int bufferSize){
            BufferSize = bufferSize;
            TimeAmplitude = signalWaveform;
            FreqMagnitude = spectrumData;
        }
        public int BufferSize { get; }
        public float[] TimeAmplitude { get; }
        public float[] FreqMagnitude { get; }
    }

}