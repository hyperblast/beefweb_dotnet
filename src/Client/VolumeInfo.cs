namespace Beefweb.Client
{
    public sealed class VolumeInfo
    {
        public VolumeType Type { get; set; }

        public decimal Min { get; set; }

        public decimal Max { get; set; }
        
        public decimal Value { get; set; }
        
        public bool IsMuted { get; set; }
    }
}