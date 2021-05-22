
using System.Text;


namespace Deltics.PeResourceInfo.Reader
{
    internal partial class ResourceDirectoryReader
    {
        // Use this to read a string from the unicode string table within the resource
        //  data section.  Strings in this area are formatted with a leading length
        //  indicator.  After reading the string from the specified location, the
        //  reader is restored to the position it was at before the method was called.
        internal string ReadResourceDirectoryString(ulong offset)
        {
            var result = "";
            var org    = _reader.GetPosition();
            try
            {
                _reader.SetPosition(_baseAddress + offset);

                var len   = _reader.ReadUInt16();
                var bytes = _reader.ReadBytes(len * 2);

                result = Encoding.Unicode.GetString(bytes);
            }
            finally
            {
                _reader.SetPosition(org);
            }

            return result;
        }
    }
}