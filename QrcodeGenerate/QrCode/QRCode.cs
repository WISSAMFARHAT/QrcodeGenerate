namespace QrCode;

public class QRCode : AbstractQRCode, IDisposable
{
    /// <summary>
    /// Constructor without params to be used in COM Objects connections
    /// </summary>
    public QRCode() { }

    private bool isFrame(int x, int y, int size, int pixelsPerModule)
    {
        x /= pixelsPerModule;
        y /= pixelsPerModule;
        size /= pixelsPerModule;

        // Top Left Frame
        if (x < 11 && y < 11)
            return true;

        if (size - 11 <= x && y < 11)
            return true;

        if (x < 11 && size - 11 <= y)
            return true;

        return false;
    }

    private bool isLeftStartBar(int x, int y, int pixelsPerModule)
    {
        x /= pixelsPerModule;
        y /= pixelsPerModule;

        if (x <= 4)
            return true;

        if (!_regionsArray[x - 1, y])
            return true;

        return false;
    }

    private bool isRightEndBar(int x, int y, int pixelsPerModule)
    {
        x /= pixelsPerModule;
        y /= pixelsPerModule;

        if (_regionsArray.GetLength(0) - 5 <= x)
            return true;

        if (!_regionsArray[x + 1, y])
            return true;

        return false;
    }

    private bool isAloneBar(int x, int y, int pixelsPerModule)
    {
        if (isLeftStartBar(x, y, pixelsPerModule) && isRightEndBar(x, y, pixelsPerModule))
            return true;

        return false;
    }

    private bool[,] _regionsArray;

    private void FillInRegionsArray(int size, int offset, int pixelsPerModule)
    {
        _regionsArray = new bool[size / pixelsPerModule, size / pixelsPerModule];

        for (int x = 0; x < size + offset; x += pixelsPerModule)
            for (int y = 0; y < size + offset; y += pixelsPerModule)
            {
                bool module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                if (module)
                    _regionsArray[x / pixelsPerModule, y / pixelsPerModule] = true;
            }
    }

}

public static class QRCodeHelper
{
    private static byte[] ToBytes(Stream stream)
    {
        long originalPosition = 0;

        if (stream.CanSeek)
        {
            originalPosition = stream.Position;
            stream.Position = 0;
        }

        try
        {
            byte[] readBuffer = new byte[4096];

            int totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead == readBuffer.Length)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte != -1)
                    {
                        byte[] temp = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                        Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                        readBuffer = temp;
                        totalBytesRead++;
                    }
                }
            }

            byte[] buffer = readBuffer;
            if (readBuffer.Length != totalBytesRead)
            {
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
            }
            return buffer;
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = originalPosition;
            }
        }
    }
}
