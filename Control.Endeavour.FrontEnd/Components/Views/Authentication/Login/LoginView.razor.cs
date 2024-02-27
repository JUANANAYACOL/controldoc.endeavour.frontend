using Control.Endeavour.FrontEnd.Components.Components.Captcha;
using Control.Endeavour.FrontEnd.Components.Components.DropDownList;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.Login.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.PasswordRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Views.Authentication.Login
{
    public partial class LoginView
    {
        #region Variables
        #region Inject 
        [Inject]
        private AuthenticationStateContainer? AuthenticationStateContainer { get; set; }
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }
        [Inject]
        public IAuthenticationJWT? AuthenticationJWT { get; set; }
        [Inject]
        private HttpClient? HttpClient { get; set; }

        //TODO: Comentar para pruebas y eliminar para produccion
        [Inject]
        private NavigationManager? NavigationManager { get; set; }
        [Inject]
        private IConfiguration? Configuration { get; set; }
        #endregion

        #region Components
        private InputComponent? NameInput { get; set; }
        private InputComponent? PasswordInput { get; set; }
        private CaptchaComponent? Captcha { get; set; }
        private NotificationsComponentModal? NotificationModal { get; set; }

        #region RecaptchaGoogle

        private ReCaptchaGoogleComponent? reCaptchaGoogle;
        private string siteKey = "";
        private string captchaResponse = string.Empty;
        private bool ValidReCAPTCHA = false;
        private bool DisablePostButton => !ValidReCAPTCHA;
        private void OnSuccess() => ValidReCAPTCHA = true;
        private void OnExpired() => ValidReCAPTCHA = false;

        #endregion

        #endregion

        #region Models
        #endregion

        #region DTOs
        private LoginUserRequest LoginUserRequest { get; set; } = new();
        #endregion

        #region Entorno
        private bool formSubUser = false;
        private bool formSubPassw = false;
        private string userEnteredCaptcha = string.Empty;
        public object FormModel { get; set; } = new();
        #endregion
        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await AuthenticationJWT.LogoutToken();
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            AuthenticationStateContainer.ComponentChange += StateHasChanged;
            siteKey = Configuration["RecaptchaSiteKey"].ToString();
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region Components
        private void CambiarComponente()
        {
            AuthenticationStateContainer.SelectedComponentChanged("PasswordRecovery");
        }
        #endregion

        #region Valid Submit

        private void validateFields()
        {
            NameInput?.ValidateInput();
            PasswordInput?.ValidateInput();

            formSubUser = NameInput!.IsInvalid;
            formSubPassw = PasswordInput!.IsInvalid;
        }

        private async Task HandleValidSubmit()
        {
            captchaResponse = await reCaptchaGoogle.GetResponseAsync();
            if (string.IsNullOrEmpty(captchaResponse))
            {

                NotificationModal?.UpdateModal(ModalType.Warning, "!Por favor realizar la validación del captcha¡", true);
            }
            else
            {
                await HandleLoginSubmit();
                NameInput?.Reset();
                PasswordInput?.Reset();
            }

        }
        #endregion

        #region Handle Login Submit
        private async Task HandleLoginSubmit()
        {
            Guid myuuid = Guid.NewGuid();

            LoginUserRequest.UserName = NameInput?.InputValue ?? string.Empty;
            LoginUserRequest.Password = PasswordInput?.InputValue ?? string.Empty;
            LoginUserRequest.Ip = "1.1.1.1";
            LoginUserRequest.Uuid = myuuid.ToString();
            LoginUserRequest.CompanyId = 17;
            LoginUserRequest.ReCaptchaResponse = captchaResponse;

            /*var tokenClient = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJJZGVudGlmaWVyTyI6ImV5SmhiR2NpT2lKU1UwRXRUMEZGVUNJc0ltVnVZeUk2SWtFeU5UWkRRa010U0ZNMU1USWlMQ0owZVhBaU9pSktWMVFpTENKamRIa2lPaUpLVjFRaWZRLklhYU5iNnJnVzh4V0dEa0hVa3pNMy11ZDNybms3eDlQWlJKcnZsVU9uaFBBWTVPZERSSDFUaF9DNnBybDB4UFN6SHF4MW94T1E4cGVuYmtvTTAyRndBa1RMZi1oSU9MM1lqa1FOREljOHBaMFp5VTFqMzU2bHEzLTl3TXpKaGxrVHYyZnlHR2x3S3F4RktYSjlPTWlhbXhCYl8tYVhxNDF3LVRhcjFfOXB3MEVzejJ1eUlWVmRqT2RjWU1PLUtfcThTdUtCRkFHczBycXE2bTdkdS1CeWs3QUZpUlZRYVU0NXNNb0ZueWwzQ2FIUGpKb3BwU2ZpY2ExcG1vNjZvVGgzN1Itb3V1dVprX0xZbVFaVzZRRVBpQXV4RGszMHZEeUNHd0dGSlpYYUNieno0bGxaOVhrWXVsQ2E2MzJvUTV6OHJSa3FZcDA5QmFtTEJBTWJIeGRuQTlIRDlFeGFvNGs4R3FSRVBqUWFnaHlWbzJQVWUwYWItVC1JYi1aZmwwQlVzdWdaZGx5SktDV1pWY28zX1FGUGRobXVFZ1pfblJTeVpfb0pMdDdkTkpxSWpJUzA1c0dZS1NfR29fQ0lUU3hHR2xDRVpSUjIwS1VRdnRucGRXdXBwaWgzSE9oZTE0UjZtR0FKRWpLcHVnOU52RXJFWjA2VDZDNXI3Wnc5bFFIOE44elNxa1B0clNXZEtJVGZVTHdaYVI5eWRvdWpVX3JkOThFTVFtTnVkdzZUM2VIcE8xbEhzbENlWEktNEFwSnBpeHpiQVFRSVJQX1YtVlUyU3UyaVd5c2VkTzJLWTFrckVydmEtS3BWTlFkcTFYTWdfcm1tTENwamloM0k3X2ZMV2w1b3RjT19yOVRHY0lhaWxFRjlQckxOc245Qko3eDFvZTJEWG9Qd1pvLjFaWmNSb1g2akFYNWVHUkM3MmxmT2cubUFpMWV1eHk4dGlqRTRIemdEWXFxVFpjcUpCOXAxaGwwVkNUNmtsX0F6cDUtOWdrTVU0QTRVNjRzUW5CbTVpWXo2SzhqdFNTU29hU1laRlI4N1F5U3lMX19KeEx6UzNzMEdEWHljT24wZThCR0Y4cENtS01YdnRTTDdwQ1UwR1lZZmxXM2RNaURQWnI1SEkzOXVCYllKMFN0UTgzeWo3VHZmUF9vODFKTy1MSW5IZEJ6QTFGNDFLMUVtM3VfSnpGQmdhcGw3dkJZTFVGQmdjTmtOaHViQXk4Nm56azJOZVVPSG1iMzFOT1BGTHo1Z1F0V0dMRDJlNXhnWFI0MGZ3Ul9aOXFUTW1wbDE1OGJPRzdHdXdSMDV3VGxsVlk1VTd4LVE0dGpoR3BhdE16d3NQcFFWdXRuS3dhMUF0NkZfaDZsd1hyV0Y5d0dhTDBzeERXeWxQN04xdTlZbHRhQzBoMlM3UHQ1aGl6M3A5UkMyTTdvOHdjeFozUkpMTVNWcWJ6aDdxLXBJSGRzbWhaSkhQS2M1R2wwb3pwNnBXLW0xWTNQVFd0ZzlrTk5tYk0xV2VMN1ZUWmQ1VDhhYjhuZzk4dTNPOFB5OThxUmFkQk1NTHYzem8zYV9WQXdsTGdTc0dJTmRzNFdnUkhRa2cuSng0QlhMcTNXLUNueUNOaVVLQ0dyelF3LU1GWFFubmFRcy1qWnZwbi1COCIsIklkZW50aWZpZXJUIjoiZXlKaGJHY2lPaUpTVTBFdFQwRkZVQ0lzSW1WdVl5STZJa0V5TlRaRFFrTXRTRk0xTVRJaUxDSjBlWEFpT2lKS1YxUWlMQ0pqZEhraU9pSktWMVFpZlEuV2RmT2dYdnc1Q0szdDJNX3lBNXpEdk5GaS1aSVFyM0M0ZEJUUmJBeEhxUk1NZjEtMmxtMjVtcUFiaGhOR1NDc1FmRlB6UEQ2SXctR0JMVXRYNXRLVXlkc1pBOVNrTzY2ajA0bDZKa1Zqb0lHMVRNdnR6MDlxOWRzYkRCZzF3N2pRRnRaY2hNMTlTNGNJRTJob0FzUW1ncnF5RHZIYkhHYVlhRUVhRDFxcnhmN2V2Mi1MOVUwZk9YeS14VXg3Smw4MEVFcHJfM1hHc0plYndta19xbVNGVFg2d0JqbzA0b1FOY2xhWmtHTi1idEZpa0N4bngycnh3cHVXQUgwWXU0TkROWi1XRzhjMlE3amVkck5wMDJSb0VBTFdrdHkxalViR0o3WlZwTHdXVEFkY3RJb1F1QVlnSEI1NjZ3NWZoSG5PM3dMYXpacTBtTDh1MlRvTlRPN0taRng2SFF3eXB0SDNlSjFsYVFWWmFaaTQwaWh2cHZYaHhTdnQ0UTh6VUFpaTR6VDUtUGZRUzdIZTdja3kwMU1UdFU3NElHUG5ZYVVreE5WdkswUUF1R1RuaFFlRU13NkhBWGsxSjlfTFliRFZlWVJodVcxLV81ZmlIekwxa0FpdjEyaTVXODZyT3BkVmVaME5oTkFMdkhJWHNlNUpmSE5weWM1U2dIMDlzQ2xSNTFwYnRDc013dE9CYl94SVlTVWlVTnVvaDgtODhGLWV5ZnVHV3BPSHk3UEo4WVhHckZicXVnakVDZ0NyNzJ4VXQyVzdPalFVa1hyS1dKU0lneDhyX3FsbnZ6TFlFMXMyck5qS0pWcDZmX1RWVERnTjAwclBCWFV2YzktRmZ1cHVLbnN6cWdNYW1QZG40LThJY1k0cmFPUklyVUZUbm1ZdkxVVU84ZkpUWGsuQTBBbnlwQVZzUVliT2N0VEpoZURsZy56Z3RxaXdrSVM1bHByVjdSckVGQlh0VXlsNzFGa0ZEWE9VeVFSaUdaazMwNVJqSWhfN2c1SGpKTnFSOEtFNkZTWmJfV2pIWG1BU3dpcE5LdTR1Si1wZ2h6UnhicWFOQ054Ni1IZ2RmSy1sVFo1LVMxRDN5QzU5RnpsUE5FUzBUOWM2WHlySVhuemhpOVdMRzg1MGxsNW1HV0R2Z3ZOdi04dGhnZVZoRnZSc3RRYW45VzdOUWRtV0IyZ2dSQXo1UWV4blQ2N0NrVjgxWUFjUktQNV9aQnVibjVXQUxiTWlEakZVRjBsN3BSc2k1Qnp4WmFsb1pxd2xNQTRJZmxzQVdkQ01PZVFZMHNtLV9sS3AxMmgxYURfc0FtVnNkU0NLVDNtUEd4YnozZU16M3NIYU1IaUMtelkyeDFOdzRSbWJPUlpCMkJ0cVBUVUVSdUN6OGJjTl9ZNlNsT2dOOWg1bzIycnpDZW1IcFNLWlg4TU1OdGtWaFpzNjRsbVVOaktJZGs5M1JZMVg1blNZX2I3ZTVRU2d1UXdRVjlaajgyNnZHdDJFbDNoYzV1U1VkUUVYMnRPQ2F1SFFTUGgtbWVLNndIR19rSHB4Nm1WclRMY2JVM2F6OXgzU1RMWTNEeVFfb3VjTHZNZ3BRVi1qTGVFQkR5ckhSN1FxbU1Ub1VCYmF1V2d1UXFZamZ5YWh3TlRhcXkxWkFFTDJkWmJDVXNiV2FMZHVBLWFxNWZIc0llNl9RaDRmbFV4allFa3ZQMERqTXhobXp2QUVhUy1WbFBYQkxaZTlGS1JzaGQ4Zk1kYTZOcmVGUER5VFJ2U0IxdVdxb05GeGc2ZnRUQk5Xc0RjVFV6OWNSdUU3eHE3WUdqcmd4VlJ5OWpUT3VOSnZmSEFVLWJZczlWZFB2Y09CUWZKVHQ3a2IybWFvbFBNSHNuVVpEbDQwczNVSWRXcHBNYU5FT3U0d3dXNmFNbzdCQnJQNlhnRDJja3VzUVNOd3pYTTBKbjBoUVFlZlZuQ1FNaXJ4Q05QcVJzNHhld1lqTmNDa3MtUEE3Wm1aNVhpcU1CYlVmVnBmb3BMRDQtV0dLU3BqMkk5YlgtZGlxc0lQSVZEaTczYjJFZkwzOHFyS3Z5eGlQaURwNHBPWHhXNml2MU56NEFVOW04dFhTLVIwWU5KRjVVMWVQZnNnZmFlTHNxM20wNnUxdWRwS3otR0pSaWd3Y0FfeG5xM0V0NXhKeG5zRXRFTVoyX04wRGlJQWZodXpObUZPOE13R1NtMU5na1R6em1ESDVvT1U5cTBveFNaMXNvUGN4cmRBcld1QmlWXzZ4RDVXSjFGZ1R0c0FFUEJrS1lWSU16VzBvajR0X1E2SzJ5Tnl4ZWZON3lmbUVBaDFidnpzRE9NY3hqdXYzbGlKNEJ6YncyWjFLSEFGUkQzdTlWS2hFMktJbUVJXzVJVkR6RkhQSl8zVk5fWkNVM3oxRExGZkE3b18zNFZVZmNlZlhNeklyT0xQSGE0dVhrSDhDQ3h1TTlZWGRBbjl4eXdUckVPSURGWmp5YnZZaVBXVjJQOTdyY0JsYXllYkc2Vzh0bWRHZDI1Y0dxVUlsNmkyM3JmRV9ENElpbUowdDVkVlY5OFdvbmt2M19MTmFxQjRfdHhMY3NlUVRZZGFubEpQdlFpUG83ZHE2WDFoTm5rMVZOZXNES0JfTVhodU9sbXlsb1NjeUZ1UTJyU1pBX24wcFMxcXJEYk1MQUl5bG1xMkdKYWExeUxQdjhFZVo5VENjQnE0Mmt4VEU4T1M4QnlrLWtMb2dQZkhVWUJHUkl5NEdSdlpTUXphbThpQzZ1azI4ZmhkM21JNzNJSmF0QkNvd0Z3SDdVNHZjSmtxS2l2QTh6V29mWmpIejBLMWZKTlVBMGFRRWRJUklhZ29veEF0YXZ4YXotaU5tV2EzdGI1cXN5OU1FelJzQl9tNEFoMWlLYWR1NGkyWmpwcTJIZmdRcWJZZl8waGZ0czNTR0h6VHVxSVNmbFRHX2tfUFAxRGszVlpnSEh4VHpITWoxWVI3VFZEMFRxSGdxRmU5Q2xlbTFlaC00RVIxbTZxTGFZbHpnVllHNHBCTWh5WkMtU1J2TFNhSExBNVdFQVo2SDc0MUxWWDNpQnZlWTZFQWNKVFhubUluODg4c0ljNE13a250cURlX3dSUk9RNS03cWlvZkc2eGR1VzY4NTlXdl9hWVpEdDM5d05OME1aSVpIRlpSOXZsdll6cXRhSDJiVE5PVXlkRHp1QVB2ckRkRVBTalJnaWhobzVlXzBTY0FaeXhGcTgyZzF0UGlWRE5xODdLaFlmNXFfSU5FZ1FsQU9JZkx6bGhQRnpkSHhjZmFKUWVTMVpaVHIwVkpuSEVnNWhSUG12WmRzS3A5c1hyclJ2NDJqNW5PVkRQRWVDc0tMWHdsc1kzYVVXUTJ1aFRzcDZRZmZ3eFJQMlhGY2pyZmlaRGhmRDBmNzRKbloxUEtFZ2xNemVPUk8ydklRLVl3QVl3X0xKSW9RS2ZNcUlOeFd3ckdCcTAwMlBxbWlmZ0VKa3RVZWNkQVV4aVd3Q1dYam5EM3JmblhMcnd6MWxtQnlrOGhaVWhJN05aaF9oOGJjd3RubHNFM2RISFNaR0VZMF82SDJScHV2UkgydFpqMm1aWmFKSjB1Nk53MDZnU25zcUdtOFFSelNWdEFVWmdsZV92NlhselFRYm1iVUdpRmVOSkxXa3JDd3B4U2tRUVNjX2RHc19yQ082a0lTVGx4TG9pZG1PcXVDRi1UQ0ZZVDNQcjE1Z0RJOF9HbXdzZDBOVnJjX1JXNUlpdFAzdkJOTThTcmFMQl9LZDRQaUN6T0tCUDc3S2xCazhwYS1UR1RTd052U3NycWc0X2NvRTc1aThkSU5TTlJDU2pMcHhTZFpOU1ZxUndXUUdyeU5fMjhLTnF4UVE1ZmI1eUJlUUVQNWdST2RiUHBtbWxXLWFtbHh4b1JFXzZQYm55LXNfU1dWY0pqUkZfM2xsdDlwd1hPZEdWSmJ5aEc5c1lIektZZE1CNmlEaDFITzZUN3dIbkxwa0JYR2ZmSEEyRDhoaXoydGE3TzRTNGMwbXlWeGdjaC1TNFZBZEZNc1BTNzlYMjRIY05rSkZqSWVDM1hNVnR3MFFRWWIwV0huX05LU1NJTlRxa2RiM29mSHNlRDlXT3M1d3k5VHdvbEFJUVNqUWd5QS04Q0hrenNxUXRoSllaZWJmLXZPR3p6cDVnLWo5ajFZSldyY1VyWWNYSkhWcFhHX2ZEcWhWZFgtXzJUVUZBNmIwaEo1OVViREVDMkNMS2dfMm5OcDZfN2NCdUFqUUZINjNKRlJRVFJSREYzanJab3V3SFNGMWF0dVQtUmxDel9qdFlOZkxHamNIYjdHVzNkbjZVTFFGQ3ZVbHJlWlZOdTkxdFdQSWhMT0tuQjRuNlBwazR6OXZkc1pkV1NvWEdaTC1fQUNQNDBxcHI3UElqTjZwNm9kOGdkdUpCRkpUdmt1U1A2VVNCcGNDbVgwekU2U2o4OVRpTDBTanA1VEh2UUVjcHNjc1lsWWkyb09pdUhtamNicTNoMEdVZTRfRWl4RUVjbEkzaFRGb1N2QzNEWGh5Snh6bWVEbm9UajFDLXZ2Y1FMQzhmT2kxdW9XNnl5a2dtdWp4cWlIMW0wbFJjT0UtVVY5V3otR2tEdlpaMXkzYm91b1lPekxIcmlvMWt4SndkZEZiOWVOWlpab01BaHhrTUJ1NnNsTkYzUHVfcnA3Q2o3b0JNcHRDdXJ4QUh2SWc4eWdYVF9hYmtRSElJNUpDekFuOUt4cUVoc2lxcHZxOXF2R3ouTVFlME9Gb1VwcjhrNXdua3dRbjJlUTdVT2ZNVnFJMUhwcG9BTFNGQUdBWSIsIm5iZiI6MTcwNzM1MzcxMCwiZXhwIjoxNzM4ODg5NzEwLCJpc3MiOiJodHRwOi8vMjUuNTYuMTYwLjIwMzo3MDAwIiwiYXVkIjoiaHR0cDovLzI1LjU2LjE2MC4yMDM6NzAwMCJ9.5KVGqcSYztOXKWzE9U33IY514vmqJtkVgh1MJWGF9SPxQZAwKvLgjyghYPymj-VDiNuCOYnVVoP16l5HLtWgng";
            await AuthenticationJWT.LoginToken(tokenClient);
            NavigationManager?.NavigateTo("Home");*/

            //TODO: Comentar para pruebas y eliminar para produccion
            try
            {

                var answer = await HttpClient.PostAsJsonAsync("security/Session/CreateLoginGoogle", LoginUserRequest);

                var loginResponse = await answer.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                if (loginResponse!.Succeeded)
                {
                    switch (loginResponse.Data)
                    {
                        case "SLS2A1":
                            AuthenticationStateContainer?.SelectedComponentChanged("Multisession");
                            break;

                        case "SLS2A2":
                            AuthenticationStateContainer?.Parametros(LoginUserRequest.UserName, LoginUserRequest.Uuid, LoginUserRequest.Ip);
                            AuthenticationStateContainer?.SelectedComponentChanged("CodeRecovery");
                            Captcha!.Dispose();
                            break;
                        default:

                            NotificationModal?.UpdateModal(ModalType.Error, "¡Problemas con el inicio de sesion!", true, "Aceptar");
                            break;
                    }
                }
                else
                {
                    NotificationModal?.UpdateModal(ModalType.Error, "¡Usuario o Contraseña Incorrectos, Digite correctamente sus credenciales!", true, "Aceptar");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar iniciar sesión: {ex.Message}");
            }
        }
        #endregion

        private static string SetKeyName(string key)
        {
            return DropDownListLanguageComponent.GetText(key);
        }

        private void OnCaptchaEntered(string captcha)
        {
            userEnteredCaptcha = captcha;
        }
    }
}
