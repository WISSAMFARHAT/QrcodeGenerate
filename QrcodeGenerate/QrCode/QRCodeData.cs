namespace QrCode;

using QrCode.Framework4._0Methods;
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;

public class QRCodeData : IDisposable
{
    public List<BitArray> ModuleMatrix { get; set; }

    public QRCodeData(int version)
    {
        this.Version = version;
        int size = ModulesPerSideFromVersion(version);
        this.ModuleMatrix = new List<BitArray>();
        for (int i = 0; i < size; i++)
            this.ModuleMatrix.Add(new BitArray(size));
    }
    public QRCodeData(string pathToRawData, Compression compressMode) : this(File.ReadAllBytes(pathToRawData), compressMode)
    {
    }
    public QRCodeData(byte[] rawData, Compression compressMode)
    {
        List<byte> bytes = new(rawData);

        //Decompress
        if (compressMode == Compression.Deflate)
        {
            using MemoryStream input = new(bytes.ToArray());
            using MemoryStream output = new();
            using (DeflateStream dstream = new(input, CompressionMode.Decompress))
            {
                Stream4Methods.CopyTo(dstream, output);
            }
            bytes = new List<byte>(output.ToArray());
        }
        else if (compressMode == Compression.GZip)
        {
            using (MemoryStream input = new(bytes.ToArray()))
            {
                using (MemoryStream output = new())
                {
                    using (GZipStream dstream = new(input, CompressionMode.Decompress))
                    {
                        Stream4Methods.CopyTo(dstream, output);
                    }
                    bytes = new List<byte>(output.ToArray());
                }
            }
        }

        if (bytes[0] != 0x51 || bytes[1] != 0x52 || bytes[2] != 0x52)
            throw new Exception("Invalid raw data file. Filetype doesn't match \"QRR\".");

        //Set QR code version
        int sideLen = (int)bytes[4];
        bytes.RemoveRange(0, 5);
        this.Version = (sideLen - 21 - 8) / 4 + 1;

        //Unpack
        Queue<bool> modules = new(8 * bytes.Count);
        foreach (byte b in bytes)
        {
            BitArray bArr = new(new byte[] { b });
            for (int i = 7; i >= 0; i--)
            {
                modules.Enqueue((b & (1 << i)) != 0);
            }
        }

        //Build module matrix
        this.ModuleMatrix = new List<BitArray>(sideLen);
        for (int y = 0; y < sideLen; y++)
        {
            this.ModuleMatrix.Add(new BitArray(sideLen));
            for (int x = 0; x < sideLen; x++)
            {
                this.ModuleMatrix[y][x] = modules.Dequeue();
            }
        }

    }
    public string ImageSource { get; set; }

    public byte[] GetRawData(Compression compressMode)
    {
        List<byte> bytes = new();

        //Add header - signature ("QRR")
        bytes.AddRange(new byte[] { 0x51, 0x52, 0x52, 0x00 });

        //Add header - rowsize
        bytes.Add((byte)ModuleMatrix.Count);

        //Build data queue
        Queue<int> dataQueue = new();
        foreach (BitArray row in ModuleMatrix)
        {
            foreach (object module in row)
            {
                dataQueue.Enqueue((bool)module ? 1 : 0);
            }
        }
        for (int i = 0; i < 8 - (ModuleMatrix.Count * ModuleMatrix.Count) % 8; i++)
        {
            dataQueue.Enqueue(0);
        }

        //Process queue
        while (dataQueue.Count > 0)
        {
            byte b = 0;
            for (int i = 7; i >= 0; i--)
            {
                b += (byte)(dataQueue.Dequeue() << i);
            }
            bytes.Add(b);
        }
        byte[] rawData = bytes.ToArray();

        //Compress stream (optional)
        if (compressMode == Compression.Deflate)
        {
            using (MemoryStream output = new())
            {
                using (DeflateStream dstream = new(output, CompressionMode.Compress))
                {
                    dstream.Write(rawData, 0, rawData.Length);
                }
                rawData = output.ToArray();
            }
        }
        else if (compressMode == Compression.GZip)
        {
            using (MemoryStream output = new())
            {
                using (GZipStream gzipStream = new(output, CompressionMode.Compress, true))
                {
                    gzipStream.Write(rawData, 0, rawData.Length);
                }
                rawData = output.ToArray();
            }
        }
        return rawData;
    }

    public void SaveRawData(string filePath, Compression compressMode)
    {
        File.WriteAllBytes(filePath, GetRawData(compressMode));
    }

    public int Version { get; private set; }

    private static int ModulesPerSideFromVersion(int version)
    {
        return 21 + (version - 1) * 4;
    }

    public void Dispose()
    {
        this.ModuleMatrix = null;
        this.Version = 0;

    }

    public enum Compression
    {
        Uncompressed,
        Deflate,
        GZip
    }
}
