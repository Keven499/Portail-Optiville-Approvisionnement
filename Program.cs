using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data;
using Portail_OptiVille.Data.Models;
using Portail_OptiVille.Data.Utilities;
using static Portail_OptiVille.Data.Utilities.MailManager;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.Cookies;
using Portail_OptiVille.Data.Attributes;
using Portail_OptiVille.Data.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ICookie, CookieService>();

#region Configuration des services
builder.Services.AddScoped<ListeVillesAPI>();
#endregion

#region Add services for DB management
builder.Services.AddScoped<HistoriqueService>();
builder.Services.AddScoped<GestionUserService>();
#endregion

#region Load Config from Setting.json
var configFilePath = Path.Combine(builder.Environment.WebRootPath, "Setting.json");
var config = await Config.LoadFromJsonAsync(configFilePath);
builder.Services.AddSingleton(config);
#endregion

#region Load Modele from Modele.json
var modeleFilePath = Path.Combine(builder.Environment.WebRootPath, "Modele.json");
var modeles = await Modele.LoadFromJsonAsync(modeleFilePath);
builder.Services.AddSingleton(modeles);
#endregion

#region Mail
//Envoie de mail
builder.Services.Configure<DefaultMail>(builder.Configuration.GetSection("DefaultMail"));
builder.Services.AddTransient<MailManager>();
#endregion

#region JWT
//Injection de d�pendance pour la g�n�ration de token JWT
builder.Services.Configure<JwtTokenGeneratorOptions>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<JwtTokenGenerator>();
#endregion

#region Database
//Injection de d�pendance pour la connexion � la base de donn�es
builder.Services.AddDbContext<A2024420517riGr1Eq6Context>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 18)) // Remplacez par une version g�n�rique
    )
);
#endregion

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<DownloadService>();

#region Connexion
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorizationCore();
#endregion

//builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Ajout de l'authentification et de l'autorisation
app.UseAuthentication();  // Middleware d'authentification
app.UseAuthorization();   // Middleware d'autorisation

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
