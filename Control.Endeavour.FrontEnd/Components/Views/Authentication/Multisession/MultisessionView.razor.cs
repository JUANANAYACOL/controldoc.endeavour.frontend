using Control.Endeavour.FrontEnd.Components.Components.Captcha;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Inputs;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.Login.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.Multisession.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;

namespace Control.Endeavour.FrontEnd.Components.Views.Authentication.Multisession
{
    public partial class MultisessionView
    {
        #region Variables
        #region Inject 
        [Inject]
        public IAuthenticationJWT? AuthenticationJWT { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private AuthenticationStateContainer? AuthenticationStateContainer { get; set; }

        #endregion

        #region Components

        private InputComponent? emailInput;
        private CaptchaComponent? captcha;
        private NotificationsComponentModal? NotificationModal { get; set; }


        #endregion

        #region Models
        private MultisessionDtoRequest multisessionDtoRequest = new();
        LoginUserRequest loginUserDtoRequest = new();

        private CodeInputComponent CodeInputComponent { set; get; } = new();

        #endregion

        #region Entorno
        public string ComponenteRenderizar { get; set; } = "CodeRecovery";
        private bool formSubmitted = true;
        private bool formSubEmail = false;
        private string userEnteredCaptcha = string.Empty;

        #endregion
        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            AuthenticationStateContainer.ComponentChange += StateHasChanged;

        }

        #endregion

        #region Components
        private void CambiarComponente()
        {
            AuthenticationStateContainer?.SelectedComponentChanged("PasswordRecovery");
        }
        #endregion

        #region Valid Submit


        private void validateEmail()
        {
            emailInput?.ValidateInput();

            if ((emailInput?.InputValue == "") || (emailInput?.IsInvalid==true))
            {
                formSubEmail = true;
                formSubmitted = true;
            }
            else
            {
                formSubEmail = false;
                formSubmitted = false;
            }
        }

        private async Task HandleValidSubmit()
        {
            if (userEnteredCaptcha != captcha?.CaptchaValue)
            {
                NotificationModal?.UpdateModal(ModalType.Error, "¡Captcha Incorrecto, Vuelva a intentar el Captcha!", true, "Aceptar", "", "", "");
                return;
            }
            else
            {
                await HandleLoginSubmit();
            }

            ResetForm();

        }
        #endregion

        #region Handle Login Submit
        private async Task HandleLoginSubmit()
        {
            try
            {
                multisessionDtoRequest.Email = emailInput.InputValue;
                multisessionDtoRequest.Code = captcha.CaptchaValue; 

                var answer = await HttpClient.PostAsJsonAsync("security/Session/SignOff", multisessionDtoRequest);
                var multisessionResponse = await answer.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();
                
                if (multisessionResponse.Succeeded)
                {
                    AuthenticationStateContainer?.SelectedComponentChanged("Login");
                }
                else
                {
                    NotificationModal?.UpdateModal(ModalType.Error, "¡No tiene sesiones activas!", true, "Aceptar");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar iniciar sesión: {ex.Message}");
            }
        }
        #endregion

        #region Render Component 
        public void ClickCallBack(string ComponenteRenderizarnew)
        {
            ComponenteRenderizar = ComponenteRenderizarnew;
        }
        #endregion

        #region Cleanup
        private void ResetForm()
        {
            emailInput?.Reset();
            formSubmitted = false;
        }
        #endregion

        #region Captcha
        private void OnCaptchaEntered(string captcha)
        {
            userEnteredCaptcha = captcha;
        }
        #endregion

        #region Return
        private async Task ReturnLoginAsync()
        {
            AuthenticationStateContainer?.SelectedComponentChanged("Login");
        }
        #endregion

    }
}
