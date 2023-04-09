using static QrCode.QRCodeGenerator;
using static QrCode.SvgQRCode;
using System.Drawing;
using System.Text;

namespace QrCode;

public class SvgQRCode : AbstractQRCode, IDisposable
{
    /// <summary>
    /// Constructor without params to be used in COM Objects connections
    /// </summary>
    public SvgQRCode() { }
    public SvgQRCode(QRCodeData data) : base(data) { }

    public string Instructions { get; set; }

    public string GetGraphic(int pixelsPerModule)
    {
        Size viewBox = new(pixelsPerModule * this.QrCodeData.ModuleMatrix.Count, pixelsPerModule * this.QrCodeData.ModuleMatrix.Count);
        return this.GetGraphic(viewBox, Color.Black, Color.White);
    }
    public string GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
    {
        Size viewBox = new(pixelsPerModule * this.QrCodeData.ModuleMatrix.Count, pixelsPerModule * this.QrCodeData.ModuleMatrix.Count);
        return this.GetGraphic(viewBox, darkColor, lightColor, drawQuietZones, sizingMode);
    }

    public string GetGraphic(int pixelsPerModule, string darkColorHex, string lightColorHex, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
    {
        Size viewBox = new(pixelsPerModule * this.QrCodeData.ModuleMatrix.Count, pixelsPerModule * this.QrCodeData.ModuleMatrix.Count);

        return this.GetGraphic(viewBox, darkColorHex, lightColorHex, drawQuietZones, sizingMode);
    }

    public string GetGraphic(Size viewBox, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
    {
        return this.GetGraphic(viewBox, Color.Black, Color.White, drawQuietZones, sizingMode);
    }

    public string GetGraphic(Size viewBox, Color darkColor, Color lightColor, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
    {
        return this.GetGraphic(viewBox, ColorTranslator.ToHtml(Color.FromArgb(darkColor.ToArgb())), ColorTranslator.ToHtml(Color.FromArgb(lightColor.ToArgb())), drawQuietZones, sizingMode);
    }

    private string FrameGraphic(double x, double y, string accentColorHex)
    {
        StringBuilder svgFile = new();

        svgFile.AppendLine($"<g transform=\"translate({CleanSvgVal(x)},{CleanSvgVal(y)}) scale(0.56 0.56)\">");
        svgFile.AppendLine($"<g style=\"fill: {accentColorHex};\">");

        svgFile.AppendLine("<path d=\"M161.35,0H83.65A83.68,83.68,0,0,0,3.43,59.22,82,82,0,0,0,0,82.69v79.55C0,207.86,37.53,245,83.65,245h77.7c46.11,0,83.65-37.09,83.65-82.71V82.69C245,37.09,207.47,0,161.35,0Zm46.9,162.24c0,25.34-21,46-46.91,46H83.66c-25.87,0-46.91-20.62-46.91-46V82.69a44.85,44.85,0,0,1,5.67-21.88A46.53,46.53,0,0,1,56.74,45.08a47.28,47.28,0,0,1,26.9-8.33h77.71c25.86,0,46.9,20.61,46.9,45.94Z\"/>");
        svgFile.AppendLine("<path d=\"M146.3,70H98.7a29,29,0,0,0-16.47,5.1,28.37,28.37,0,0,0-8.78,9.64A27.65,27.65,0,0,0,70,98.11v48.72C70,162.35,82.87,175,98.72,175H146.3c15.83,0,28.71-12.64,28.71-28.15V98.11C175,82.6,162.13,70,146.3,70Z\" />");

        svgFile.AppendLine("</g>");
        svgFile.AppendLine("</g>");

        return svgFile.ToString();
    }

    public string GetGraphic(Size viewBox, string accentColorHex, string backgroundColorHex, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
    {
        int offset = drawQuietZones ? 0 : 4;
        int drawableModulesCount = this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : offset * 2);
        double pixelsPerModule = (double)Math.Min(viewBox.Width, viewBox.Height) / (double)drawableModulesCount;
        double qrSize = drawableModulesCount * pixelsPerModule;
        double totalHeight = viewBox.Height + (6 * pixelsPerModule);

        bool hasInlineImage = !string.IsNullOrEmpty(QrCodeData.ImageSource);
        int inlineImageSize = GetInlineImageSize(viewBox.Width);
        double inlineImageStart = GetInlineImageStart(viewBox.Width);

        string svgSizeAttributes = (sizingMode == SizingMode.WidthHeightAttribute) ? $@"width=""{viewBox.Width}"" height=""{totalHeight}""" : $@"viewBox=""0 0 {viewBox.Width} {totalHeight}""";
        StringBuilder svgFile = new($@"<svg version=""1.1"" {svgSizeAttributes} xmlns=""http://www.w3.org/2000/svg"">");

        if (!string.IsNullOrEmpty(backgroundColorHex))
            svgFile.AppendLine($@"<rect x=""0"" y=""0"" width=""{CleanSvgVal(qrSize)}"" height=""{CleanSvgVal(totalHeight)}"" fill=""{backgroundColorHex}"" />");

        svgFile.AppendLine($"<g style=\"position: relative;width: {viewBox.Width}px;height: {viewBox.Width}px\">");

        svgFile.AppendLine(FrameGraphic(4 * pixelsPerModule, 4 * pixelsPerModule, accentColorHex));
        svgFile.AppendLine(FrameGraphic(4 * pixelsPerModule, qrSize - (11 * pixelsPerModule), accentColorHex));
        svgFile.AppendLine(FrameGraphic(qrSize - (11 * pixelsPerModule), 4 * pixelsPerModule, accentColorHex));

        if (hasInlineImage)
            for (int xi = (int)(inlineImageStart / pixelsPerModule); xi <= (int)((inlineImageStart + inlineImageSize) / pixelsPerModule); xi++)
                for (int yi = (int)(inlineImageStart / pixelsPerModule); yi <= (int)((inlineImageStart + inlineImageSize) / pixelsPerModule); yi++)
                    if (QrCodeData.ModuleMatrix[yi][xi])
                        QrCodeData.ModuleMatrix[yi][xi] = false;

        for (int xi = offset; xi < offset + drawableModulesCount; xi++)
            for (int yi = offset; yi < offset + drawableModulesCount; yi++)
                if (QrCodeData.ModuleMatrix[yi][xi])
                {
                    double x = (xi - offset) * pixelsPerModule;
                    double y = (yi - offset) * pixelsPerModule;

                    PixelPosition position = GetPixelPosition(xi, yi);

                    if (position == PixelPosition.Frame)
                        continue;

                    svgFile.AppendLine($"<g transform=\"translate({CleanSvgVal(x)},{CleanSvgVal(y)}) scale(0.04,0.04)\">");
                    svgFile.AppendLine($"<g style=\"fill: {accentColorHex};\">");

                    switch (position)
                    {
                        case PixelPosition.Left:
                            svgFile.AppendLine("<path d=\"M470,38.5h-47c0,0-29.157,0-66.5,0c-5.711,0-11.611,0-17.63,0c-47.461,0-102.168,0-127.37,0  C94.65,38.5,0,133.222,0,250c0,116.765,94.65,211.5,211.5,211.5c31.48,0,83.041,0,127.37,0c6.035,0,11.932,0,17.63,0  c37.686,0,66.5,0,66.5,0h47h30v-423H470z\"/>");
                            break;
                        case PixelPosition.Right:
                            svgFile.AppendLine("<path d=\"M288.5,38.5c-42.907,0-123.188,0-171.55,0c-23.857,0-39.95,0-39.95,0H59.5H30H0v423h30h29.5H77c0,0,16.216,0,39.95,0  c51.332,0,137.84,0,171.55,0C405.301,461.5,500,366.8,500,250C500,133.191,405.301,38.5,288.5,38.5z\"/>");
                            break;
                        case PixelPosition.Center:
                            svgFile.AppendLine("<path d=\"M250,38.5c116.813,0,211.5,94.693,211.5,211.5c0,116.813-94.687,211.5-211.5,211.5c-116.807,0-211.5-94.687-211.5-211.5  C38.5,133.193,133.193,38.5,250,38.5z\"></path>");
                            break;
                        default:
                            svgFile.AppendLine("<polygon points=\"250,38.5 0,38.5 0,461.5 250,461.5 500,461.5 500,38.5 \"/>");
                            break;
                    }

                    svgFile.AppendLine("</g>");
                    svgFile.AppendLine("</g>");
                }

        // Instructions

        double instructionsScale = QrCodeData.ModuleMatrix.Count * .1;

        if (hasInlineImage)
        {
            QrCodeData.ImageSource = QrCodeData.ImageSource.Replace("<svg ", $"<svg height=\"{inlineImageSize}px\" width=\"{inlineImageSize}px\" ");
            svgFile.AppendLine($"<g transform=\"translate({CleanSvgVal(inlineImageStart)},{CleanSvgVal(inlineImageStart)})\">{QrCodeData.ImageSource}</g>");
        }
        //svgFile.AppendLine($"<g style=\"transform: scale(.1) translate({CleanSvgVal(QrCodeData.ModuleMatrix.Count * pixelsPerModule)}px, {CleanSvgVal(QrCodeData.ModuleMatrix.Count * pixelsPerModule)}px)\">{QrCodeData.ImageSource}</g>");

        svgFile.AppendLine("</g>");

        svgFile.AppendLine($"<g transform=\"translate({CleanSvgVal(QrCodeData.ModuleMatrix.Count * pixelsPerModule * .18)}, {CleanSvgVal(QrCodeData.ModuleMatrix.Count * pixelsPerModule)}) scale({instructionsScale}, {instructionsScale})\">");
        svgFile.AppendLine($"<g style=\"fill: {accentColorHex};\">");
        svgFile.AppendLine("</g>");
        svgFile.AppendLine(Instructions);
        svgFile.AppendLine("</g>");

        svgFile.AppendLine(@"</svg>");

        return svgFile.ToString();
    }

    private enum PixelPosition
    {
        Left,
        Right,
        Sided,
        Center,
        Frame
    }

    private int GetInlineImageSize(int qrCodeSize) => Convert.ToInt32(qrCodeSize * .10);
    private double GetInlineImageStart(int qrCodeSize) => (qrCodeSize / 2) - (GetInlineImageSize(qrCodeSize) / 2);

    private PixelPosition GetPixelPosition(int xi, int yi)
    {
        double frameStart = 11;
        double frameEnd = QrCodeData.ModuleMatrix[0].Length - frameStart;

        if (xi <= frameStart && yi <= frameStart
            || xi <= frameStart && yi >= frameEnd
            || xi >= frameEnd && yi <= frameStart)
            return PixelPosition.Frame;

        bool isLeftSide = xi == 0 || !QrCodeData.ModuleMatrix[yi][xi - 1];
        bool isRightSide = xi + 1 == QrCodeData.ModuleMatrix[yi].Length || !QrCodeData.ModuleMatrix[yi][xi + 1];

        if (isLeftSide && isRightSide)
            return PixelPosition.Center;

        if (isLeftSide)
            return PixelPosition.Left;

        if (isRightSide)
            return PixelPosition.Right;


        return PixelPosition.Sided;
    }

    private static string CleanSvgVal(double input)
    {
        //Clean double values for international use/formats
        return input.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public enum SizingMode
    {
        WidthHeightAttribute,
        ViewBoxAttribute
    }
}

public static class SvgQRCodeHelper
{
    public static string GetQRCode(string plainText, int pixelsPerModule, string darkColorHex, string lightColorHex, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
    {
        using QRCodeGenerator qrGenerator = new();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
        using SvgQRCode qrCode = new(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule, darkColorHex, lightColorHex, drawQuietZones, sizingMode);
    }
}