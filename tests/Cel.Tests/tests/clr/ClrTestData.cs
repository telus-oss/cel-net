namespace Cel.Tests.tests;

public class ClrTestData
{
    public bool BoolValue { get; set; }
    public string StringValue { get; set; }
    public double DoubleValue { get; set; }
    public int Int32Value { get; set; }
    public long Int64Value { get; set; }
    public uint UInt32Value { get; set; }
    public ulong UInt64Value { get; set; }
    public decimal DecimalValue { get; set; }
    
    // Additional integer types for comprehensive testing
    public short Int16Value { get; set; }
    public ushort UInt16Value { get; set; }
    public byte ByteValue { get; set; }
    public sbyte SByteValue { get; set; }
}