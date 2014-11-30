using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System;

namespace TiS.Core.TisCommon.Storage
{
    [ComVisible(false)]
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class MemoryDataChunk
    {
        public MemoryDataChunk() {}
        
        public MemoryDataChunk(
            MemoryStream ms,
            int collectionChunkSize)
        {
            int bufferSize = (int)Math.Min(collectionChunkSize, ms.Length - ms.Position);

            Index = (int)(ms.Position / collectionChunkSize);

            Data = new byte[bufferSize];
            Size = ms.Read(Data, 0, bufferSize);

            IsLastChunk = ms.Length - ms.Position == 0;
        }

        [DataMember]
        public int Size { get; set; }

        [DataMember]
        public byte[] Data { get; set; }

        [DataMember]
        public bool IsLastChunk { get; set; }
    
        [DataMember]
        public int Index { get; set; }
    }
}
