using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QrcodeGenerate;
using AngryMonkey.Cloud.Components;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddCloudWeb(new CloudWebOptions()
{
    DefaultTitle = "QrcodeGenerate",
    TitleSuffix = " - QrcodeGenerate",
    SiteBundles = new List<CloudBundle>()
    {
		new CloudBundle(){ Source = "css/site.css"},
        new CloudBundle(){ Source = "js/site.js"},
          new CloudBundle(){ Source = "QrcodeGenerate.styles.css", MinOnRelease = false},

    }
});


builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
