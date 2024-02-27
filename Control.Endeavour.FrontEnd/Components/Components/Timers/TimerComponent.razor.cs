

using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.Login.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.PasswordRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Components.Timers
{
    public partial class TimerComponent : ComponentBase
    {



        #region Variables
        #region Inject 
        [Inject]
        private AuthenticationStateContainer? AuthenticationStateContainer { get; set; }

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Parameter
        [Parameter]
        public TimeSpan TimeDuration { get; set; } = TimeSpan.FromSeconds(30); // Tiempo por defecto
        [Parameter]
        public string ResendButtonText { get; set; } = "Reenviar código"; // Texto por defecto del botón
        [Parameter]
        public EventCallback OnTimerEnd { get; set; }
        [Parameter]
        public PasswordCodeRecoveryDtoRequest request { get; set; } = new();

        [Parameter]
        public SendVerificationCodeDtoRequest requestLogin { get; set; } = new();

        [Parameter]
        public int TypeCode { get; set; } = 1;
        #endregion

        #region Entorno
        private CancellationTokenSource cts = new CancellationTokenSource();
        private TimeSpan timeLeft;
        private bool isButtonDisabled = true;
        private NotificationsComponentModal? NotificationModal { get; set; }
        #endregion
        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await InitializeTimer();
        }

        #endregion

        #region Initialize Timer
        private async Task InitializeTimer()
        {
            timeLeft = TimeDuration;
            isButtonDisabled = true;
            await RunTimer();
        }
        #endregion

        #region Run Timer
        private async Task RunTimer()
        {
            while (timeLeft > TimeSpan.Zero)
            {
                try
                {
                    await Task.Delay(1000, cts.Token);
                }
                catch (TaskCanceledException)
                {
                    return; // Cancelar si el token de cancelación se activa
                }

                timeLeft = timeLeft.Add(TimeSpan.FromSeconds(-1));
                StateHasChanged(); // Re-render the component
            }

            isButtonDisabled = false;
            StateHasChanged(); // Re-render the component

            if (OnTimerEnd.HasDelegate)
            {
                await OnTimerEnd.InvokeAsync(null);
            }
        }
        #endregion

        #region Resend Code
        private async Task ResendCode()
        {
            if (!isButtonDisabled)
            {
                try
                {
                    if(TypeCode==1)
                    {
                        var answer = await HttpClient!.PostAsJsonAsync("security/Session/SendVerificationCode", requestLogin);
                        var loginResponse = await answer.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                        if (!loginResponse!.Succeeded)
                        {
                            NotificationModal?.UpdateModal(ModalType.Error, "¡Problemas con el envio del código de seguridad!", true, "Aceptar");
                        }
                        else
                        {
                            // Reiniciar el temporizador
                            cts.Cancel();
                            cts.Dispose();
                            cts = new CancellationTokenSource();
                            InitializeTimer();
                        }
                    }
                    else if(TypeCode==2)
                    {
                        var responseApi = await HttpClient!.PostAsJsonAsync("security/Session/CreateRecoveryPassword", request);
                        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();
                        if (!deserializeResponse!.Succeeded)
                        {
                            NotificationModal?.UpdateModal(ModalType.Error, "¡Problemas con el envio del código de seguridad!", true, "Aceptar");
                        }
                        else
                        {
                            // Reiniciar el temporizador
                            cts.Cancel();
                            cts.Dispose();
                            cts = new CancellationTokenSource();
                            InitializeTimer();
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al intentar enviar el código de seguridad: {ex.Message}");
                }

                
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
        }
        #endregion
    }
}
