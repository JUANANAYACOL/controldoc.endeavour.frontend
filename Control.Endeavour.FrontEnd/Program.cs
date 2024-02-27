using Control.Endeavour.FrontEnd;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Control.Endeavour.FrontEnd.Services.Services.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.Services.Services.Storage;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using DevExpress.Blazor;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Control.Endeavour.FrontEnd.StateContainer.ManagementTray;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<AuthenticationStateContainer>();
builder.Services.AddSingleton<DocumentsStateContainer>();
builder.Services.AddSingleton<FilingStateContainer>();
builder.Services.AddScoped<EventAggregatorService>();
builder.Services.AddSingleton<ManagementTrayStateContainer>();

//Session storage y Local storage
builder.Services.AddScoped<ISessionStorage, SessionStorageService>();
builder.Services.AddScoped<ILocalStorage, LocalStorageService>();


//Manejo del token de seguridad
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetValue<string>("ServiceConfiguration:UrlApiGateway")) });
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationJWTService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationJWTService>(provider => provider.GetRequiredService<AuthenticationJWTService>());
builder.Services.AddScoped<IAuthenticationJWT, AuthenticationJWTService>(provider => provider.GetRequiredService<AuthenticationJWTService>());
builder.Services.AddScoped<RenewTokenService>();

builder.Services.AddSpeechSynthesis();

//Para el funcionamiento del DevExpress
builder.Services.AddDevExpressBlazor(configure => configure.BootstrapVersion = BootstrapVersion.v5);



await builder.Build().RunAsync();
