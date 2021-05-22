namespace Deltics.PeResourceInfo
{
    public struct ImageResourceDirectoryEntry
    {
        private string _name;
        private ulong  _offsetToData;

        public ulong Id    { get; internal set; }
        public int   Level { get; internal set; }

        public bool IsData      => (_offsetToData & 0x80000000) == 0;
        public bool IsDirectory => (_offsetToData & 0x80000000) != 0;
        public bool IdIsName    => (Id & 0x80000000) != 0;

        public ulong OffsetToData
        {
            get => _offsetToData & 0x7fffffff;
            internal set { _offsetToData = value; }
        }
        public ulong OffsetToName => Id & 0x7fffffff;

        public string Name
        {
            get => GetName();
            internal set { _name = value; }
        }


        private string GetName()
        {
            return Level switch
            {
                0 => "<root>",
                1 => GetResourceType(),
                2 => GetResourceId(),
                3 => GetLanguages(),
                _ => "ERR!"
            };
        }

        private string GetResourceType()
        {
            return (ResourceType) Id switch
            {
                ResourceType.CURSOR       => "CURSOR",
                ResourceType.BITMAP       => "BITMAP",
                ResourceType.ICON         => "ICON",
                ResourceType.MENU         => "MENU",
                ResourceType.DIALOG       => "DIALOG",
                ResourceType.STRING       => "STRING",
                ResourceType.FONTDIR      => "FONTDIR",
                ResourceType.FONT         => "FONT",
                ResourceType.ACCELERATOR  => "ACCELERATOR",
                ResourceType.RCDATA       => "RCDATA",
                ResourceType.MESSAGETABLE => "MESSAGETABLE",
                ResourceType.GROUP_CURSOR => "GROUP_CURSOR",
                ResourceType.GROUP_ICON   => "GROUP_ICON",
                ResourceType.VERSION      => "VERSION",
                ResourceType.DLGINCLUDE   => "DLGINCLUDE",
                ResourceType.PLUGPLAY     => "PLUGPLAY",
                ResourceType.VXD          => "VXD",
                ResourceType.ANICURSOR    => "ANICURSOR",
                ResourceType.ANIICON      => "ANIICON",
                ResourceType.HTML         => "HTML",
                ResourceType.MANIFEST     => "MANIFEST",
                _                         => "UNKNOWN(" + Id + ")"
            };
        }
        
        private string GetResourceId()
        {
            return IdIsName ? _name : $"ID(${Id})";
        }
        
        private string GetLanguages()
        {
            var lang    = (Id & 0x000003ff);
            var subLang = (Id & 0x0000fc00) >> 10;
            return $"LANG({Id})";
        }
    }
}