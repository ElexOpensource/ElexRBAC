using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using RbacDashboard.DAL.Models.Domain;
using RbacDashboard.Common;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Authentication;


[ExcludeFromCodeCoverage]
public partial class Login
{
    #region Inject
#pragma warning disable CS8618
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected SignInManager<RbacApplicationUser> SignInManager { get; set; }
#pragma warning restore CS8618
    #endregion

    #region Parameter
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private LoginModel LoginInput { get; set; } = new LoginModel();

    private string? ErrorMessage { get; set; } 
    #endregion

    protected override async Task OnInitializedAsync()
    {
        if (HttpMethods.IsGet(HttpContext.Request.Method))
        {
            Console.WriteLine("\n Sign out calling on OnInitializedAsync...");
            await HttpContext.SignOutAsync(RbacConstants.AuthenticationSchema);
        }
    }

    public async Task OnLogin()
    {
        ErrorMessage = string.Empty;

        if (LoginInput.Token == null || LoginInput.Token == string.Empty)
        {
            ErrorMessage = "Error: Please enter token";
            return;
        }

        var result = SignInResult.Failed;

        try
        {
            result = await SignInManager.PasswordSignInAsync(LoginInput.Token, string.Empty, false, lockoutOnFailure: false);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return;
        }

        if (result.Succeeded)
        {
            NavigationManager.NavigateTo(NavigationManager.BaseUri, true);
        }
        else
        {
            Console.WriteLine("Error on SignInManager.PasswordSignInAsync");
            ErrorMessage = "Error: Invalid login attempt";
        }
    }

    private sealed class LoginModel
    {
        [Required(ErrorMessage = "Please Enter Valid Token")]
        public string Token { get; set; } = "";
    }
}
