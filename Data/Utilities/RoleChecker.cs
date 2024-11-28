using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Portail_OptiVille.Data.Utilities;

public class RoleChecker
{
    private ProtectedSessionStorage _protectedSessionStorage { get; set; }
    private ICookie _cookieService { get; set; }
    private JwtTokenGenerator _jwtTokenGenerator { get; set; }
    
    // Définition de la hiérarchie des rôles (ordre important : index plus élevé = plus autorisé)
    private readonly string[] roles = { "Commis", "Responsable", "Administrateur" };

    public RoleChecker(ProtectedSessionStorage protectedSessionStorage, ICookie cookieService, JwtTokenGenerator jwtTokenGenerator)
    {
        _protectedSessionStorage = protectedSessionStorage;
        _cookieService = cookieService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // Vérifie si l'utilisateur a le rôle ou un rôle supérieur
    public async Task<bool> CheckRole(string requiredRole)
    {
        // Récupère le rôle actuel depuis la session ou le cookie
        var result = await _protectedSessionStorage.GetAsync<string>("Role");
        string currentRole = result.Value;
        
        if (string.IsNullOrEmpty(currentRole))
        {
            var cookie = await _cookieService.GetValue("Email");
            if (string.IsNullOrEmpty(cookie))
            {
                return false; // Pas de rôle valide
            }

            currentRole = _jwtTokenGenerator.GetValueFromToken(cookie).Split(":")[1];
            await _protectedSessionStorage.SetAsync("Role", currentRole);
        }

        // Valide le rôle
        return IsRoleAuthorized(currentRole, requiredRole);
    }

    // Compare les rôles en fonction de leur hiérarchie
    private bool IsRoleAuthorized(string currentRole, string requiredRole)
    {
        // Trouve l'index des rôles
        int currentRoleIndex = Array.IndexOf(roles, currentRole);
        int requiredRoleIndex = Array.IndexOf(roles, requiredRole);

        // Vérifie si le rôle actuel est au moins égal ou supérieur au rôle requis
        return currentRoleIndex >= requiredRoleIndex;
    }
}
