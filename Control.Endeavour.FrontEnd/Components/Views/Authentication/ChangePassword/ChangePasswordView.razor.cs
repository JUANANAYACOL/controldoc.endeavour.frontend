using Control.Endeavour.FrontEnd.Components.Components.DropDownList;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.PasswordRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Views.Authentication.ChangePassword
{
    public partial class ChangePasswordView
    {

        #region Variables
        #region Inject 
        [Inject]
        private AuthenticationStateContainer? AuthenticationStateContainer { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private IAuthenticationJWT? AuthenticationJWT { get; set; }

        [Inject]
		private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
		private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components
        private CodeInputComponent codeInputComponent { set; get; } = new();
        private ChangePasswordDtoRequest formModel = new();
        private InputComponent? passwordInput;
        private InputComponent? passwordConfirmationInput;
        #endregion

        #region Modals
        private NotificationsComponentModal? NotificationModal { get; set; }

        #endregion

        #region Parameters
        
        #endregion

        #region Models
        private ChangePasswordDtoRequest recoveryPasswordRequest { get; set; } = new();
        private PasswordCodeRecoveryDtoRequest passwordRecoveryCodeRequestResend { get; set; }=new();
        #endregion

        #region Environments
        private bool formSubmitted = true;
        private bool formSubmPass = false;
        private bool formSubmPassConfirmation = false;
        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            passwordRecoveryCodeRequestResend.Email = AuthenticationStateContainer!.User;
            passwordRecoveryCodeRequestResend.UUID = AuthenticationStateContainer.Uuid;
        }

        #endregion

        #region Methods
        #region HandleLanguageChanged
        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
		}
        #endregion

        private void validatePassword()
        {
            passwordInput!.ValidateInput();
            passwordConfirmationInput!.ValidateInput();

            // Verifica si alguno de los campos de contraseña es inválido
            bool passwordInvalid = passwordInput.IsInvalid;
            bool confirmationInvalid = passwordConfirmationInput.IsInvalid;

            // Verifica si las contraseñas no coinciden
            bool passwordsMismatch = !passwordInvalid && !confirmationInvalid && passwordInput.InputValue != passwordConfirmationInput.InputValue && passwordConfirmationInput.InputValue!="";

            formSubmPass = passwordInvalid;
            formSubmPassConfirmation = confirmationInvalid || passwordsMismatch;
            if(!formSubmPass && !formSubmPassConfirmation)
            {
                formSubmitted = false;
            }
        }


        private async Task HandleValidSubmit()
        {
            await HandleChangePasswordSubmit();
            ResetForm();
        }
        private void ResetForm()
        {
            codeInputComponent.Reset();
            passwordInput?.Reset();
            passwordConfirmationInput?.Reset();
            formSubmitted = true;
        }

        private async Task HandleChangePasswordSubmit()
        {
            if (!codeInputComponent.IsInvalid)
            {

                recoveryPasswordRequest.Email = AuthenticationStateContainer?.User;
                recoveryPasswordRequest.Code = codeInputComponent?.InputValue.ToUpper() ?? string.Empty;
                recoveryPasswordRequest.NewPassword = passwordInput?.InputValue ?? string.Empty;
                recoveryPasswordRequest.UUID = AuthenticationStateContainer?.Uuid;
                recoveryPasswordRequest.Ip = "1.1.1.1";

                try
                {
                    var answer = await HttpClient!.PostAsJsonAsync("security/Session/UpdatePassword", recoveryPasswordRequest);
                    var CodeRecoveryResponse = await answer.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                    if (CodeRecoveryResponse!.Succeeded)
                    {
                        NotificationModal?.UpdateModal(ModalType.Success, "¡Se cambio la contraseña de forma exitosa!", true, "Aceptar");
                        await Task.Delay(4000);
                        AuthenticationStateContainer?.SelectedComponentChanged("Login");

                    }
                    else
                    {
                        NotificationModal?.UpdateModal(ModalType.Error, "¡Problemas con el cambio de contraseña!", true, "Aceptar");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al intentar cambiar la contraseña: {ex.Message}");
                }
            }

        }

        private string setKeyName(string key)
        {
            return DropDownListLanguageComponent.GetText(key);
        }

        #endregion

    }
}
