using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.Exceptions;
using Portail_OptiVille.Data.Models;
using static Portail_OptiVille.Pages.Fournisseur.Connexion.ConnexionFournisseur;

namespace Portail_OptiVille.Data.Utilities
{
    public class ProtectedNavigationManager
    {
        private readonly NavigationManager _navigationManager;
        private readonly ICookie _cookieService;
        private readonly ProtectedSessionStorage _protectedSessionStorage;
        private readonly A2024420517riGr1Eq6Context _context;
        public ProtectedNavigationManager(NavigationManager navigationManager, ICookie cookieService, ProtectedSessionStorage protectedSessionStorage, A2024420517riGr1Eq6Context context)
        {
            _navigationManager = navigationManager;
            _cookieService = cookieService;
            _protectedSessionStorage = protectedSessionStorage;
            _context = context;
        }

        public async void NavigateTo(string uri)
        {
            var result = await _protectedSessionStorage.GetAsync<string>("Email");
            if (result.Success || !string.IsNullOrEmpty(result.Value))
            {
                Identification? fournisseur = _context.Identifications.FirstOrDefault(f => f.AdresseCourriel == result.Value);
                if(fournisseur != null)
                {
                        _navigationManager.NavigateTo(uri);
                }
                else
                {
                    var token = await _cookieService.GetValue("SToken");
                    if (!string.IsNullOrEmpty(token))
                    {
                        
                    }

                    _navigationManager.NavigateTo("/connexion");
                }

            }

            //var token = await _cookieService.GetValue("token");
            //if (string.IsNullOrEmpty(token))
            //{
            //    _navigationManager.NavigateTo("/login");
            //}
            //else
            //{
            //    _navigationManager.NavigateTo(uri);
            //}
        }
    }
}
