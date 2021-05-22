namespace Deltics.PeResourceInfo
{
    public class ImageResourceData
    {
        public ulong OffsetToData { get; internal set; }
        public ulong Size         { get; internal set; }
        public ulong CodePage     { get; internal set; }
        public ulong Reserved     { get; internal set; }

        public ulong  Id    { get; internal set; }
        public int    Index { get; internal set; }
        public int    Level { get; internal set; }
        public string Name  { get; internal set; }
    }
}