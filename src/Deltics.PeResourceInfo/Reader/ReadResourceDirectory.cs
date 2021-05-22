
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Deltics.PeImageInfo.Reader;


namespace Deltics.PeResourceInfo.Reader
{
    internal partial class ResourceDirectoryReader
    {
        private readonly PeReader _reader;
        private readonly ulong    _baseAddress;


        internal ResourceDirectoryReader(PeReader reader, ulong baseAddress)
        {
            _reader = reader;
            _baseAddress = baseAddress;
        }
        
        
        internal ImageResourceDirectory ReadResourceDirectory(ulong virtualAddress, int index, ImageResourceDirectoryEntry? entry)
        {
            _reader.SetPosition(_baseAddress, entry?.OffsetToData ?? 0);

            var result = new ImageResourceDirectory()
            {
                Index   = index,
                Id      = entry?.Id ?? 0,
                Level   = entry?.Level ?? 0,
                HasName = entry?.IdIsName ?? true,
                Name    = entry?.Name ?? "<root>",

                Characteristics      = _reader.ReadUInt32(),
                TimeDateStamp        = _reader.ReadUInt32(),
                MajorVersion         = _reader.ReadUInt16(),
                MinorVersion         = _reader.ReadUInt16(),
                NumberOfNamedEntries = _reader.ReadUInt16(),
                NumberOfIdEntries    = _reader.ReadUInt16()
            };
            var totalEntries = result.NumberOfIdEntries + result.NumberOfNamedEntries;

            var entries = new ImageResourceDirectoryEntry[totalEntries];
            for (var i = 0; i < entries.Length; i++)
                entries[i] = ReadResourceDirectoryEntry(result.Level + 1);

            var dirEntries  = new List<ImageResourceDirectory>();
            var dataEntries = new List<ImageResourceData>();

            var idx = 0;
            foreach (var dir in entries.Where(e => e.IsDirectory))
                dirEntries.Add(ReadResourceDirectory(virtualAddress, idx++, dir));

            foreach (var data in entries.Where(e => e.IsData))
                dataEntries.Add(ReadResourceData(virtualAddress, data));

            result.Directories = dirEntries.ToImmutableArray();
            result.Data        = dataEntries.ToImmutableArray();

            return result;
        }


        private ImageResourceData ReadResourceData(ulong virtualAddress, ImageResourceDirectoryEntry entry)
        {
            _reader.SetPosition(_baseAddress, entry.OffsetToData);

            var delta = (int) virtualAddress - (int) _baseAddress;
            
            return new()
            {
                Id    = entry.Id,
                Level = entry.Level,
                Name  = entry.Name,

                OffsetToData = (ulong) ((int) _reader.ReadUInt32() - delta),
                Size         = _reader.ReadUInt32(),
                CodePage     = _reader.ReadUInt32(),
                Reserved     = _reader.ReadUInt32()
            };
        }

        
        private ImageResourceDirectoryEntry ReadResourceDirectoryEntry(int level)
        {
            var result = new ImageResourceDirectoryEntry()
            {
                Level = level,

                Id           = _reader.ReadUInt32(),
                OffsetToData = _reader.ReadUInt32()
            };

            if (result.IdIsName)
                result.Name = ReadResourceDirectoryString(result.OffsetToName);
        
            return result;
        }
    }
}