﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazorise;
using Blazorise.Icons.FontAwesome;
using Blazorise.Icons.Material;
using Blazorise.Material;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plachtovac.Client.Services;
using Tewr.Blazor.FileReader;

namespace Plachtovac.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services
                .AddBlazorContextMenu()
                .AddBlazorise(options => { options.ChangeTextOnKeyPress = true; })
                .AddMaterialProviders()
                .AddMaterialIcons()
                .AddFontAwesomeIcons()
                .AddBlazoredModal()
                .AddBlazoredLocalStorage()
                .AddFileReaderService(options => options.UseWasmSharedBuffer = true);

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.Authority = "https://accounts.google.com/";
                options.ProviderOptions.RedirectUri = builder.HostEnvironment.BaseAddress+"authentication/login-callback";
                options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress+"authentication/logout-callback";
                options.ProviderOptions.ClientId = "259222768075-ke2p7ppur3q34kfed28qnu0puklisa55.apps.googleusercontent.com";
                options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive.file");
                //options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive.appdata");
                options.ProviderOptions.ResponseType = "id_token token";
                options.UserOptions.AuthenticationType = "google";
            });

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton<PlachtaService>();
            builder.Services.AddScoped<GoogleDriveWrapper>();
            builder.Services.AddScoped<GoogleDriveStorage>();

            var host = builder.Build();

            await host.RunAsync();
        }
    }
}
