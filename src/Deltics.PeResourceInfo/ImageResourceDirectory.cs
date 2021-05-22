using System.Collections.Immutable;


namespace Deltics.PeResourceInfo
{
    public class ImageResourceDirectory
    {
        public ulong                                  Characteristics      { get; internal set; }
        public ulong                                  TimeDateStamp        { get; internal set; }
        public ushort                                 MajorVersion         { get; internal set; }
        public ushort                                 MinorVersion         { get; internal set; }
        public ushort                                 NumberOfNamedEntries { get; internal set; }
        public ushort                                 NumberOfIdEntries    { get; internal set; }
        public ImmutableArray<ImageResourceDirectory> Directories          { get; internal set; }
        public ImmutableArray<ImageResourceData>      Data                 { get; internal set; }

        public ulong  Id      { get; internal set; }
        public int    Index   { get; internal set; }
        public int    Level   { get; internal set; }
        public string Name    { get; internal set; }
        public bool   HasName { get; internal set; } 
    }
}