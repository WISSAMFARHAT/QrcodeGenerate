﻿using System.Text;
using static QrCode.QRCodeGenerator;

namespace QrCode;

public class AsciiQRCode : AbstractQRCode, IDisposable
{
    /// <summary>
    /// Constructor without params to be used in COM Objects connections
    /// </summary>
    public AsciiQRCode() { }

    public AsciiQRCode(QRCodeData data) : base(data) { }

    /// <summary>
    /// Returns a strings that contains the resulting QR code as ASCII chars.
    /// </summary>
    /// <param name="repeatPerModule">Number of repeated darkColorString/whiteSpaceString per module.</param>
    /// <returns></returns>
    public string GetGraphic(int repeatPerModule)
    {
        return string.Join("\n", GetLineByLineGraphic(repeatPerModule));
    }


    /// <summary>
    /// Returns a strings that contains the resulting QR code as ASCII chars.
    /// </summary>
    /// <param name="repeatPerModule">Number of repeated darkColorString/whiteSpaceString per module.</param>
    /// <param name="darkColorString">String for use as dark color modules. In case of string make sure whiteSpaceString has the same length.</param>
    /// <param name="whiteSpaceString">String for use as white modules (whitespace). In case of string make sure darkColorString has the same length.</param>
    /// <param name="endOfLine">End of line separator. (Default: \n)</param>
    /// <returns></returns>
    public string GetGraphic(int repeatPerModule, string darkColorString, string whiteSpaceString, string endOfLine = "\n")
    {
        return string.Join(endOfLine, GetLineByLineGraphic(repeatPerModule, darkColorString, whiteSpaceString));
    }


    /// <summary>
    /// Returns an array of strings that contains each line of the resulting QR code as ASCII chars.
    /// </summary>
    /// <param name="repeatPerModule">Number of repeated darkColorString/whiteSpaceString per module.</param>
    /// <returns></returns>
    public string[] GetLineByLineGraphic(int repeatPerModule)
    {
        return GetLineByLineGraphic(repeatPerModule, "██", "  ");
    }


    /// <summary>
    /// Returns an array of strings that contains each line of the resulting QR code as ASCII chars.
    /// </summary>
    /// <param name="repeatPerModule">Number of repeated darkColorString/whiteSpaceString per module.</param>
    /// <param name="darkColorString">String for use as dark color modules. In case of string make sure whiteSpaceString has the same length.</param>
    /// <param name="whiteSpaceString">String for use as white modules (whitespace). In case of string make sure darkColorString has the same length.</param>
    /// <returns></returns>
    public string[] GetLineByLineGraphic(int repeatPerModule, string darkColorString, string whiteSpaceString)
    {
        List<string> qrCode = new();
        //We need to adjust the repeatPerModule based on number of characters in darkColorString
        //(we assume whiteSpaceString has the same number of characters)
        //to keep the QR code as square as possible.
        int adjustmentValueForNumberOfCharacters = darkColorString.Length / 2 != 1 ? darkColorString.Length / 2 : 0;
        int verticalNumberOfRepeats = repeatPerModule + adjustmentValueForNumberOfCharacters;
        int sideLength = QrCodeData.ModuleMatrix.Count * verticalNumberOfRepeats;
        for (int y = 0; y < sideLength; y++)
        {
            bool emptyLine = true;
            StringBuilder lineBuilder = new();

            for (int x = 0; x < QrCodeData.ModuleMatrix.Count; x++)
            {
                bool module = QrCodeData.ModuleMatrix[x][(y + verticalNumberOfRepeats) / verticalNumberOfRepeats - 1];

                for (int i = 0; i < repeatPerModule; i++)
                {
                    lineBuilder.Append(module ? darkColorString : whiteSpaceString);
                }
                if (module)
                {
                    emptyLine = false;
                }
            }
            if (!emptyLine)
            {
                qrCode.Add(lineBuilder.ToString());
            }

        }
        return qrCode.ToArray();
    }
}

public static class AsciiQRCodeHelper
{
    public static string GetQRCode(string plainText, int pixelsPerModule, string darkColorString, string whiteSpaceString, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, string endOfLine = "\n")
    {
        using (QRCodeGenerator qrGenerator = new())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
        using (AsciiQRCode qrCode = new(qrCodeData))
            return qrCode.GetGraphic(pixelsPerModule, darkColorString, whiteSpaceString, endOfLine);
    }
}

