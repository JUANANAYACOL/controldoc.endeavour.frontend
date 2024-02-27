using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

using System.Net.Http.Json;
namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
public partial class ManagementOfProceduresModal
{
    #region Variables

    #region Injects

    [Inject]
    private IJSRuntime Js { get; set; }
    [Inject]
    private HttpClient? HttpClient { get; set; }
    [Inject]
    private EventAggregatorService? EventAggregator { get; set; }

    #endregion

    #region Components

    private NotificationsComponentModal notificationModalSucces { get; set; } = new();
    private NotificationsComponentModal notificationModal { get; set; } = new();
    private InputModalComponent inputRadicado { get; set; } = new();
    private InputModalComponent inputIdDocumento { get; set; } = new();
    private InputModalComponent inputAnio { get; set; } = new();
    private string? ValueTipoAction { get; set; }

    #endregion Components       

    #region Models
    private List<VSystemParamDtoResponse>? lstTypeActions { get; set; } = new();
    private List<VSystemParamDtoResponse>? lstTypeInstructions { get; set; } = new();
    private List<ObjectTransaction>? userSenderTramite { get; set; } = new();
    private List<VUserDtoResponse> userListSenders = new();
    private List<VUserDtoResponse> userListCopies = new();
    private ManagementTrayDtoResponse managementTray = new();
    private GenericSearchModal GenericSearchModal { get; set; } = new();

    #endregion Models

    #region Enviroment

    string panel_1 = "";
    string panel_2 = "d-none";
    string panel_3 = "d-none";
    string pnlAdjunto = "d-none";
    private int positionNumber = 0;
    private string managementInstructions = "INI";
    string documentoId = "";
    string numRadicado = "";
    string anio = "";
    int controlId = 0;
    int processedUserId = 0;
    private string? texAcctionType = "Seleccione una acción ......";

    #endregion Enviroment

    #region Parameters

    [Parameter]
    public string idModalIdentifier { get; set; } = null!;

    [Parameter]
    public bool modalStatus { get; set; } = false;

    [Parameter]
    public EventCallback<bool> OnChangeData { get; set; }

    #endregion Parameters

    #endregion Variables

    #region Initialization
    protected override async Task OnInitializedAsync()
    {
        try
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            modalStatus = false;
            await GetActionType();
            await GetInstructionsType();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
        }
    }

    #endregion

    #region HandleMethods

    async Task HandleLanguageChanged()
    {
        StateHasChanged();
    }
    private void HandleUserSelectedChanged(MyEventArgs<List<object>> usersData)
    {
        userSenderTramite.Clear();
        var usersSearchResultList = usersData.Data.ToList();
        var senders = usersSearchResultList[0];
        var copies = usersSearchResultList[1];

        if (senders != null)
        {
            try
            {
                userListSenders = (List<VUserDtoResponse>)senders;
            }
            catch (InvalidCastException)
            {

                userListSenders = new List<VUserDtoResponse>();
            }
        }
        if (copies != null)
        {
            try
            {
                userListCopies = (List<VUserDtoResponse>)copies;
            }
            catch (InvalidCastException)
            {

                userListCopies = new List<VUserDtoResponse>();
            }
        }

        foreach (var sender in userListSenders)
        {
            var tramite = new ObjectTransaction
            {
                UserInfo = sender,
                Action = null,
                Days = null,
                Hours = null,
                Subject = null,
                Position = positionNumber
            };

            userSenderTramite.Add(tramite);
            positionNumber++;
        }

        if (userSenderTramite.Count > 0)
        {
            panel_3 = panel_1;
        }

        GenericSearchModal.UpdateModalStatus(usersData.ModalStatus);

    }
    private void HandleModalClosed(bool status)
    {
        modalStatus = status;
        StateHasChanged();
    }
    private void HandleTramite()
    {
        if (!ValidateData(out string errormsg))
        {
            errormsg = errormsg[..^2];
            notificationModalSucces.UpdateModal(ModalType.Warning, $"Por favor validar la siguiente información, debe ingresar {errormsg}", true, "Aceptar", "Cancelar");
        }
        else
        {
            notificationModal.UpdateModal(ModalType.Information, $"¿Desea tramitar el documento?", true, "Aceptar", "Cancelar");
        }

    }
    async Task HandleModalTramiteClosed(ModalClosedEventArgs args)
    {
        if (args.IsAccepted)
        {
            await AddManagementOfProcedure();
        }
    }
    async Task HandleModalSuccesClosed(ModalClosedEventArgs args)
    {
        if (notificationModalSucces.Type == ModalType.Success)
        {
            ResetFormAsync();
            UpdateModalStatus(args.ModalStatus);
        }
    }
    async Task AddManagementOfProcedure()
    {
        try
        {
            //se crea la lista de assignedUserIds
            var assignedUsers = new List<AssignedUserIdDtoRequest>();
            userSenderTramite.ForEach(a =>
            {
                assignedUsers.Add(new AssignedUserIdDtoRequest
                {
                    AssignUserId = a.UserInfo.UserId,
                    Commentary = a.Subject ?? "",
                    InstructionCode = a.Action ?? "",
                    ItsCopy = false
                });
            });
            // los usuarios que se asignarón como copias
            userListCopies?.ForEach(c =>
            {
                assignedUsers.Add(new AssignedUserIdDtoRequest
                {
                    AssignUserId = c.UserId,
                    Commentary = "Copia",
                    InstructionCode = "",
                    ItsCopy = true
                });
            });
            // se genera el objeto de tramite
            var model = new ManagementTrayDtoRequest
            {
                ControlId = controlId,
                ProcessedUserId = processedUserId,
                ActionCode = ValueTipoAction,
                AssignedUserIds = assignedUsers
            };
            var json = JsonConvert.SerializeObject(model);
            var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/DocumentManagement/CreateProcess", model);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();
            if (deserializeResponse.Succeeded)
            {
                notificationModalSucces.UpdateModal(ModalType.Success, "¡Se tramitó el documento satisfactoriamente!", true);
                await OnChangeData.InvokeAsync(true);

            }
            else
            {
                notificationModalSucces.UpdateModal(ModalType.Error, "¡Se presentó un error al tramitar, por favor intente de nuevo!", true, "Aceptar");

            }

        }
        catch (Exception ex)
        {
            notificationModalSucces.UpdateModal(ModalType.Error, $"Error al enviar la solicitud: {ex.Message}", true, "Aceptar");
            Console.WriteLine($"Error al enviar la solicitud: {ex.Message}");
        }

    }

    #endregion HandleMethods

    #region  Methods

    #region MethodsGenerales
    public void UpdateModalStatus(bool newValue)
    {
        modalStatus = newValue;
        StateHasChanged();
    }
    private void EnablePanel(string value)
    {
        if (value == null)
        {
            panel_2 = "none";
            panel_3 = "none";
            ValueTipoAction = value;
        }
        else
        {
            ValueTipoAction = value;
            int panel = value.Equals("ACO,ETR") ? 1 : 2;
            switch (panel)
            {
                case 1:
                    panel_2 = panel_1;
                    panel_3 = "d-none";
                    break;
                case 2:
                    panel_2 = "d-none";
                    panel_3 = panel_1;
                    break;

            }
        }


    }
    private void ChangeValueAction(string fieldName, string value, ObjectTransaction tramite, int aux)
    {
        if (aux == 0)
        {
            foreach (var userTramite in userSenderTramite)
            {
                switch (fieldName.ToLower())
                {
                    case "action":
                        userTramite.Action = value;
                        break;
                    case "days":
                        userTramite.Days = value;
                        break;
                    case "hours":
                        userTramite.Hours = value;
                        break;
                    case "asunto":
                        userTramite.Subject = value;
                        userTramite.CountCharacters = tramite.CountCharacters;
                        break;
                }
            }
        }
        else
        {
            switch (fieldName.ToLower())
            {
                case "action":
                    tramite.Action = value;
                    break;
                case "days":
                    tramite.Days = value;
                    break;
                case "hours":
                    tramite.Hours = value;
                    break;
                case "asunto":
                    tramite.Subject = value;
                    break;
                default:
                    throw new ArgumentException("Campo no válido", nameof(fieldName));
            }
        }
    }
    private void CountCharacters(ChangeEventArgs e, ObjectTransaction tramite)
    {
        String value = e.Value.ToString() ?? String.Empty;
        int countCarac = tramite.CountCharacters;

        if (!string.IsNullOrEmpty(value))
        {
            countCarac = value.Length;
            tramite.CountCharacters = countCarac;
        }
        else
        {
            countCarac = 0;
            tramite.CountCharacters = countCarac;

        }
        ChangeValueAction("asunto", value, tramite, tramite.Position);

    }
    private bool ValidateData(out string errormsg)
    {
        var msgAsunto = String.Empty;
        var msgAction = String.Empty;
        userSenderTramite.ForEach(c =>
        {

            msgAsunto = string.IsNullOrWhiteSpace(c.Subject) ? "Asunto(s), " : "";
            msgAction = string.IsNullOrWhiteSpace(c.Action) ? "Instrucción  " : "";
        });
        errormsg = $"{msgAsunto}{msgAction}";
        return string.IsNullOrEmpty(errormsg);
    }
    public void ResetFormAsync()
    {
        panel_2 = "d-none";
        panel_3 = "d-none";
        userSenderTramite = new();
        userListCopies = new();
        ValueTipoAction = "Seleccione una acción ......";
         
    }

    #endregion MethodsGenerales

    #region  MethodsAsync
    async Task GetActionType()
    {
        try
        {
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", "ACO");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                lstTypeActions = deserializeResponse.Data;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la acción: {ex.Message}");
        }

    }
    async Task GetInstructionsType()
    {
        try
        {
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", managementInstructions);
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                lstTypeInstructions = deserializeResponse.Data;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener los tipos de instruciones: {ex.Message}");
        }



    }
    public async Task ManagementOfProcedures(ManagementTrayDtoResponse model)
    {
        controlId = model.controlId;
        processedUserId = model.assignUserId;
        managementTray = model;
        documentoId = $"{model.controlId}";
        numRadicado = model.filingCode;
        anio = $"{model.docDate:yyyy}";
    }
    private async Task showModalSearchUser()
    {

        GenericSearchModal.UpdateModalStatus(true);
        StateHasChanged();
    }
    #endregion MethodsAsync

    #endregion Methods
}
