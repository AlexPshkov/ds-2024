using System.Text;
using Newtonsoft.Json;

namespace SocketCommunication.Packet;

public class CustomSocketPacket
{
    public string Name { get; set; }

    public byte[] ToBytes()
    {
        Name = GetType().Name;
        
        string serializeObject = JsonConvert.SerializeObject( this );

        if ( serializeObject.Length >= UInt16.MaxValue )
        {
            throw new AggregateException( $"Content length too bit. Must be less then {UInt16.MaxValue} chars" );
        }
        
        ushort contentLength = (ushort) serializeObject.Length; 

        MemoryStream stream = new MemoryStream();

        using (BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode, true)) {
            writer.Write( BitConverter.GetBytes( contentLength ) );
            writer.Write( Encoding.UTF8.GetBytes( serializeObject ) );
        }

        stream.Capacity = (int) stream.Length;
        
        return stream.GetBuffer();
    }
}