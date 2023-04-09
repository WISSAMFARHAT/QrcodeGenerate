using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using QrCode;
using System.IO.Pipelines;
using System.Text.RegularExpressions;

namespace QrcodeGenerate.Pages;
public partial class Index
{
    public int Type { get; set; } = 0;
    public string Svg { get; set; }
    public string Url { get; set; }
    public string email { get; set; }
    public string Name { get; set; } = "";
    public string SvgCodeIcon { get; set; } = "";
    public string Firtname
    {
        get
        {
            return Name.Split(" ")[0];
        }
    }
    public string Lastname
    {
        get
        {
            if (Name.Split(" ").Count() > 1)
                return Name.Split(" ")[1].ToString();

            return "";
        }
    }
    public string Work { get; set; }
    public string Phone { get; set; }
    public string NameDownload { get; set; } = "QrCode";
    public string Logo { get; set; } = "";
    public string Color { get; set; } = "#000000";
    public bool CheckLogo { get; set; } = false;

    MarkupString Ms
    {
        get
        {
            if (string.IsNullOrEmpty(Svg))
                return (MarkupString)"";

            return (MarkupString)Svg.Replace("#000000", Color);
        }
    }
    public string VsCard { get; set; }

    public void Generate()
    {

        if (Type == 0)
        {
            VsCard = $"BEGIN:VCARD\n" +
            $"VERSION:3.0\n" +
            $"N:{Lastname};{Firtname}\n" +
            $"FN:{Name}\n" +
            $"ORG:{Work}\n" +
            $"TITLE:\n" +
            $"TEL;TYPE=WORK:{Phone}\n" +
            $"EMAIL;INTERNET:{email}\n" +
            $"END:VCARD";

            GenerateQrCode(VsCard, CheckLogo);

        }
        else if (Type == 1)
        {
            GenerateQrCode(Url, CheckLogo);
        }

    }

    public async Task Download() => await JS.InvokeAsync<object>("download", $"{NameDownload}.svg", Svg.Replace("#000000", Color), "text/plain");

    protected void GenerateQrCode(string Value, bool displaylogo)
    {
        Svg = RenderQrCode(Value, displaylogo);
    }


    private string RenderQrCode(string Value, bool displaylogo)
    {
        if (string.IsNullOrEmpty(Value))
            return null;

        string level = "L";

        QRCodeGenerator.ECCLevel eccLevel = (QRCodeGenerator.ECCLevel)(level == "L" ? 0 : level == "M" ? 1 : level == "Q" ? 2 : 3);

        using QRCodeGenerator qrGenerator = new();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(Value, eccLevel);
        using SvgQRCode qrCode = new(qrCodeData);


        if (displaylogo)
        {
            if (!string.IsNullOrEmpty(Logo))
            {
                switch (Logo)
                {
                    case "coverbox":
                        qrCodeData.ImageSource = $$"""
            <svg viewBox="0 0 91.3 91.3">
            <style type="text/css">
            	.st0{fill:none;stroke:{{Color}};stroke-width:6;stroke-miterlimit:10;}
            </style>
            <circle class="st0" cx="45.7" cy="45.7" r="42.7"/>
            <g>
            	<path fill="{{Color}}" d="M64.2,30.1c-0.8-1-1.8-1.9-2.9-2.8c-1.4-1.1-2.9-2-4.5-2.7c-1.7-0.7-3.4-1.3-5.2-1.8c-1.8-0.4-3.6-0.6-5.4-0.6
            		c-3.3,0-6.5,0.6-9.4,1.8c-2.9,1.2-5.4,2.8-7.6,4.8s-3.8,4.4-5,7.2c-0.8,1.8-1.3,3.7-1.6,5.6c0,0.1,0.1,0.1,0.1,0.1
            		c2.5-2.4,5.4-4.5,8.5-6.1c3.2-1.7,6.5-2.8,10-3.3c3.6-0.5,7.5-0.3,10.8,1.3c1.8,0.9,3.4,2.2,4.8,3.7c0.4,0.4,1,0.4,1.5,0l5.8-5.7
            		C64.6,31.2,64.6,30.6,64.2,30.1z"/>
            	<path fill="{{Color}}" d="M63.6,58.4l-5.4-5.3c-0.4-0.4-1.1-0.4-1.5,0c-1.3,1.4-2.7,2.5-4.2,3.3c-1.8,0.9-3.8,1.4-6,1.4c-1.8,0-3.6-0.3-5.1-1
            		c-1.6-0.7-3-1.6-4.1-2.7c-1.2-1.2-2.1-2.5-2.7-4.1c-0.6-1.5-1-3.2-1-5c0-1.2,0.2-2.4,0.5-3.6c0.1-0.3-0.2-0.6-0.5-0.5
            		c-2.4,1-6.5,3.2-10.7,7.7c-0.1,0.1-0.1,0.2-0.1,0.4c0.3,1.7,0.8,3.4,1.5,5c1.2,2.8,2.9,5.2,5,7.2c2.1,2,4.6,3.6,7.5,4.8
            		c2.9,1.2,6,1.8,9.4,1.8c3.5,0,7-0.8,10.3-2.3c2.9-1.4,5.3-3.1,7.2-5.2C64.2,59.8,64.2,58.9,63.6,58.4z"/>
            </g>
            </svg>
            """;
                        break;
                    case "facebook":
                        qrCodeData.ImageSource = $$"""
            <svg fill="{{Color}}" viewBox="-143 145 512 512">
                <g>
                    <path fill="{{Color}}" d="M113,145c-141.4,0-256,114.6-256,256s114.6,256,256,256s256-114.6,256-256S254.4,145,113,145z M272.8,560.7
            		c-20.8,20.8-44.9,37.1-71.8,48.4c-27.8,11.8-57.4,17.7-88,17.7c-30.5,0-60.1-6-88-17.7c-26.9-11.4-51.1-27.7-71.8-48.4
            		c-20.8-20.8-37.1-44.9-48.4-71.8C-107,461.1-113,431.5-113,401s6-60.1,17.7-88c11.4-26.9,27.7-51.1,48.4-71.8
            		c20.9-20.8,45-37.1,71.9-48.5C52.9,181,82.5,175,113,175s60.1,6,88,17.7c26.9,11.4,51.1,27.7,71.8,48.4
            		c20.8,20.8,37.1,44.9,48.4,71.8c11.8,27.8,17.7,57.4,17.7,88c0,30.5-6,60.1-17.7,88C309.8,515.8,293.5,540,272.8,560.7z" />
                    <path d="M146.8,313.7c10.3,0,21.3,3.2,21.3,3.2l6.6-39.2c0,0-14-4.8-47.4-4.8c-20.5,0-32.4,7.8-41.1,19.3
            		c-8.2,10.9-8.5,28.4-8.5,39.7v25.7H51.2v38.3h26.5v133h49.6v-133h39.3l2.9-38.3h-42.2v-29.9C127.3,317.4,136.5,313.7,146.8,313.7z" />
                </g>
            </svg>
            """;
                        break;
                    case "instagram":
                        qrCodeData.ImageSource = $$"""
            <svg fill="{{Color}}" viewBox="0 0 32 32">
                <path fill="{{Color}}" d="M20.445 5h-8.891A6.559 6.559 0 0 0 5 11.554v8.891A6.559 6.559 0 0 0 11.554 27h8.891a6.56 6.56 0 0 0 6.554-6.555v-8.891A6.557 6.557 0 0 0 20.445 5zm4.342 15.445a4.343 4.343 0 0 1-4.342 4.342h-8.891a4.341 4.341 0 0 1-4.341-4.342v-8.891a4.34 4.34 0 0 1 4.341-4.341h8.891a4.342 4.342 0 0 1 4.341 4.341l.001 8.891z" />
                <path fill="{{Color}}" d="M16 10.312c-3.138 0-5.688 2.551-5.688 5.688s2.551 5.688 5.688 5.688 5.688-2.551 5.688-5.688-2.55-5.688-5.688-5.688zm0 9.163a3.475 3.475 0 1 1-.001-6.95 3.475 3.475 0 0 1 .001 6.95zM21.7 8.991a1.363 1.363 0 1 1-1.364 1.364c0-.752.51-1.364 1.364-1.364z" />
            </svg>
            """;
                        break;
                    case "youtube":
                        qrCodeData.ImageSource = $$"""
            <svg fill="{{Color}}" viewBox="-143 145 512 512">
                <g>
                    <path fill="{{Color}}" d="M113,145c-141.4,0-256,114.6-256,256s114.6,256,256,256s256-114.6,256-256S254.4,145,113,145z M272.8,560.7
            		c-20.8,20.8-44.9,37.1-71.8,48.4c-27.8,11.8-57.4,17.7-88,17.7c-30.5,0-60.1-6-88-17.7c-26.9-11.4-51.1-27.7-71.8-48.4
            		c-20.8-20.8-37.1-44.9-48.4-71.8C-107,461.1-113,431.5-113,401s6-60.1,17.7-88c11.4-26.9,27.7-51.1,48.4-71.8
            		c20.9-20.8,45-37.1,71.9-48.5C52.9,181,82.5,175,113,175s60.1,6,88,17.7c26.9,11.4,51.1,27.7,71.8,48.4
            		c20.8,20.8,37.1,44.9,48.4,71.8c11.8,27.8,17.7,57.4,17.7,88c0,30.5-6,60.1-17.7,88C309.8,515.8,293.5,540,272.8,560.7z" />
                    <path fill="{{Color}}" d="M196.9,311.2H29.1c0,0-44.1,0-44.1,44.1v91.5c0,0,0,44.1,44.1,44.1h167.8c0,0,44.1,0,44.1-44.1v-91.5
            		C241,355.3,241,311.2,196.9,311.2z M78.9,450.3v-98.5l83.8,49.3L78.9,450.3z" />
                </g>
            </svg>
            """;
                        break;
                    case "linkedin":
                        qrCodeData.ImageSource = $$"""
            <svg  fill="{{Color}}" viewBox="-143 145 512 512">
                <g>
                    <path  fill="{{Color}}" d="M113,145c-141.4,0-256,114.6-256,256s114.6,256,256,256s256-114.6,256-256S254.4,145,113,145z M272.8,560.7
            		c-20.8,20.8-44.9,37.1-71.8,48.4c-27.8,11.8-57.4,17.7-88,17.7c-30.5,0-60.1-6-88-17.7c-26.9-11.4-51.1-27.7-71.8-48.4
            		c-20.8-20.8-37.1-44.9-48.4-71.8C-107,461.1-113,431.5-113,401s6-60.1,17.7-88c11.4-26.9,27.7-51.1,48.4-71.8
            		c20.9-20.8,45-37.1,71.9-48.5C52.9,181,82.5,175,113,175s60.1,6,88,17.7c26.9,11.4,51.1,27.7,71.8,48.4
            		c20.8,20.8,37.1,44.9,48.4,71.8c11.8,27.8,17.7,57.4,17.7,88c0,30.5-6,60.1-17.7,88C309.8,515.8,293.5,540,272.8,560.7z" />
                    <rect x="-8.5" y="348.4" width="49.9" height="159.7" />
                    <path  fill="{{Color}}" d="M15.4,273c-18.4,0-30.5,11.9-30.5,27.7c0,15.5,11.7,27.7,29.8,27.7h0.4c18.8,0,30.5-12.3,30.4-27.7
            		C45.1,284.9,33.8,273,15.4,273z" />
                    <path d="M177.7,346.9c-28.6,0-46.5,15.6-49.8,26.6v-25.1H71.8c0.7,13.3,0,159.7,0,159.7h56.1v-86.3c0-4.9-0.2-9.7,1.2-13.1
            		c3.8-9.6,12.1-19.6,27-19.6c19.5,0,28.3,14.8,28.3,36.4v82.6H241v-88.8C241,369.9,213.2,346.9,177.7,346.9z" />
                </g>
            </svg>
            """;
                        break;

                }
            }
            if (!string.IsNullOrEmpty(SvgCodeIcon))
                qrCodeData.ImageSource = SvgCodeIcon;

        }
        string svg = qrCode.GetGraphic(20, "#000000", "#ffffff", sizingMode: SvgQRCode.SizingMode.ViewBoxAttribute);

        return svg;
    }

    public void HandleColorChange(ChangeEventArgs e)
    {
        Color = e.Value.ToString();

        StateHasChanged();

        GenerateQrCode(Url, CheckLogo);
    }

    public void ChangeIcon(string Icon)
    {
        SvgCodeIcon = "";
        switch (Icon)
        {
            case "none":
                CheckLogo = false;
                StateHasChanged();
                break;
            case "facebook":
                CheckLogo = true;
                Logo = "facebook";
                StateHasChanged();
                break;
            case "instagram":
                CheckLogo = true;
                Logo = "instagram";
                StateHasChanged();
                break;
            case "coverbox":
                CheckLogo = true;
                Logo = "coverbox";
                StateHasChanged();
                break;
            case "youtube":
                CheckLogo = true;
                Logo = "youtube";
                StateHasChanged();
                break;
            case "linkedin":
                CheckLogo = true;
                Logo = "linkedin";
                StateHasChanged();
                break;

        }

        GenerateQrCode(Url, CheckLogo);

    }



    public async Task HandleFileSelect(InputFileChangeEventArgs e)
    {


        var file = e.File;


        if (file != null)
        {
            using var streamReader = new StreamReader(file.OpenReadStream());

            SvgCodeIcon = await streamReader.ReadToEndAsync();
            const string xmlProlog = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            SvgCodeIcon = SvgCodeIcon.StartsWith(xmlProlog)
                ? SvgCodeIcon.Substring(xmlProlog.Length)
                : SvgCodeIcon;

            SvgCodeIcon.Replace("width", "oldwidth").Replace("height", "oldheight");
            //SvgCodeIcon = Regex.Replace(SvgCodeIcon, @"(width|height)=""\d+""", "");
        }
        GenerateQrCode(Url, true);
    }

}

