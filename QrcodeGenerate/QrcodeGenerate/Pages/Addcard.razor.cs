using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace QrcodeGenerate.Pages;
public partial class Addcard
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Mobile { get; set; }

    public string PhoneWork { get; set; }

    public string Email { get; set; }

    public string Company { get; set; }

    public string JobTitle { get; set; }

    public string Address { get; set; }

    public string FileInput { get; set; }

    public string Base64data { get; set; }

    private async Task OnFileSelected(InputFileChangeEventArgs e)
    {
        FileInput = e.File.Name;
        byte[] buf = new byte[e.File.Size];
        using (var stream = e.File.OpenReadStream())
        {
            await stream.ReadAsync(buf); // copy the stream to the buffer
        }

        Base64data = Convert.ToBase64String(buf);
    }

    async Task Save()
    {
        string VSCard = $"BEGIN:VCARD\n" + $"VERSION:3.0\n" + $"N:{FirstName};{LastName}\n" + $"FN:{FirstName} {LastName}\n" + $"ORG:{Company}\n" + $"TITLE:{JobTitle}\n" + $"ADR:{Address}\n" + $"TEL;type=pref:{Mobile}\n" + $"TEL;type=WORK:{PhoneWork}\n" + $"EMAIL;INTERNET:{Email}\n" + $"PHOTO;ENCODING=b;TYPE=PNG:{Base64data}\n" + $"END:VCARD";
        //string SavePath = System.IO.Path.Combine(AppContext.BaseDirectory, "output.vcf");
        //System.IO.File.WriteAllText(SavePath, VSCard);
        //Console.WriteLine("File saved at " + SavePath.Trim());
        await JS.InvokeAsync<object>("download", $"{FirstName}-{LastName}.vcf", VSCard, "text/vcard");
    }
}
