namespace IrisUsage
{
    public class IrisResult
    {
        public float SepalLength { get; set; }
        public float SepalWidth { get; set; }
        public float PetalLength { get; set; }
        public float PetalWidth { get; set; }

        public string? PredictedSpecies { get; set; }
        public string? PositiveProbability { get; set; }
        public string? NegativeProbability { get; set; }
        public string? ImageBase64 { get; set; }
    }
}
