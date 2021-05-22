
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Deltics.PeImageInfo;
using Deltics.PeImageInfo.Reader;
using Deltics.PeResourceInfo.Reader;


namespace Deltics.PeResourceInfo
{
    public enum ResourceType
    {
        CURSOR = 1,
        BITMAP = 2,          
        ICON = 3,            
        MENU = 4,            
        DIALOG = 5,          
        STRING = 6,          
        FONTDIR = 7,         
        FONT = 8,            
        ACCELERATOR = 9,     
        RCDATA = 10,         
        MESSAGETABLE = 11,   
        GROUP_CURSOR = 12,   
        GROUP_ICON = 14,     
        VERSION = 16,        
        DLGINCLUDE = 17,     
        PLUGPLAY = 19,       
        VXD = 20,            
        ANICURSOR = 21,      
        ANIICON = 22,        
        HTML = 23,           
        MANIFEST = 24       
    }
    
    
    public class ResourceInfo
    {
        private Section _section;
        
        public  PeImage                                Header    { get; internal set; }
        public  ImmutableArray<ImageResourceDirectory> Directory { get; internal set; }

        public bool     IsValid { get; private set; }
        public bool     IsEmpty => Directory.Length == 0;
        public PeReader Reader  => Header?.Reader;
            
        
        public ResourceInfo(Stream stream)
        {
            Read(stream);
        }


        public ImageResourceData GetResource(ResourceType type)
        {
            var typeDir = Directory.FirstOrDefault(dir => dir.Id == (ulong) type);
            if ((typeDir == null) || (typeDir.Directories.Length == 0) || (typeDir.Directories[0].Data.Length != 1))
                return null;

            return typeDir.Directories[0].Data[0];
        }

        
        public ImageResourceData GetResource(ResourceType type, uint id)
        {
            var typeDir = Directory.FirstOrDefault(dir => dir.Id == (ulong) type);
            if ((typeDir == null) || (typeDir.Directories.Length == 0) || (typeDir.Directories[0].Data.Length == 0))
                return null;

            var resource = typeDir.Directories.FirstOrDefault(dir => dir.Id == id);
            if (resource.Data.Length != 1)
                return null;

            return resource.Data[0];
        }
        
        
        private void Read(Stream stream)
        {
            Header = new PeImage(stream);
            
            _section = Header.Sections.Where(section => section.Name.Equals(".rsrc", StringComparison.Ordinal)).FirstOrDefault();
            if (_section == null)
                return;

            var reader = new ResourceDirectoryReader(Header.Reader, _section.PointerToRawData);

            var root = reader.ReadResourceDirectory(_section.VirtualAddress, 0, null);
            Directory = root.Directories.ToImmutableArray();

            IsValid = true;
        }
        
        
        public override string ToString()
        {
            var builder = new StringBuilder(1024);

            foreach (var dir in Directory)
                BuildResourceDirTree(builder, 1, dir);

            return builder.ToString();
        }
        
        
        private static void BuildResourceDirTree(StringBuilder builder, int indent, ImageResourceDirectory dir)
        {
            if (dir == null)
                return;
            
            var indentStr = new String(' ', indent * 3);
            builder.AppendLine($"{indentStr}Dir #{dir.Index}: {dir.Name}" + (dir.Level == 1 ? $" ({dir.Id})" : ""));

            if (dir.Directories == null)
                return;
            
            foreach (var subDir in dir.Directories)
                BuildResourceDirTree(builder, indent + 1, subDir);

            foreach (var data in dir.Data)
                builder.AppendLine($"{indentStr}Data #{data.Index}: {data.Name} => Size({data.Size}) @ Address({data.OffsetToData}) [CodePage({data.CodePage})]");
        }
    }
}