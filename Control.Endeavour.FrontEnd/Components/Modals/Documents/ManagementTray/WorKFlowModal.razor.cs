using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Reflection.Metadata;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
public partial class WorKFlowModal : ComponentBase
{
    #region Variables

    #region Inject

    [Inject]
    private EventAggregatorService? EventAggregator { get; set; }

    [Inject]
    private IJSRuntime Js { get; set; }

    [Inject]
    private HttpClient? HttpClient { get; set; }

    #endregion Inject

    #region Parameters
    [Parameter] public string? Title { get; set; } = "Flujo de Gestión Documental (WorKFlow)";
    private bool modalStatus { get; set; } = false;

    #endregion Parameters

    #region Models
    private WorKFlowDtoResponse worKFlowDtoResponse = new();
    private MetaModel meta { get; set; } = new() { PageSize = 10 };
    private NotificationsComponentModal notificationModal { get; set; } = new();
    #endregion Models

    #region Environments
    private int controlId { get; set; } = 0;
    private bool dataChargue { get; set; } = false;
    #endregion Environments

    #endregion Variables

    #region OnInitializedAsync

    protected override async Task OnInitializedAsync()
    {
        EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
    }

    #endregion OnInitializedAsync

    #region Methods

    #region HandleMethods

    private async Task HandleLanguageChanged()
    {
        StateHasChanged();
    }
    private void HandleModalClosed(bool value)
    {
        UpdateModalStatus(value);
    }
    private void HandleModalNotiClose(ModalClosedEventArgs args)
    {
        UpdateModalStatus(args.ModalStatus);
    }
    #endregion HandleMethods

    #region MethodsGeneral
    public void UpdateModalStatus(bool newValue)
    {
        modalStatus = newValue;
        StateHasChanged();
    }

    #endregion MethodsGeneral

    #region MethodsAsync
    async Task GetWorKFlowAsync()
    {
        try
        {           
            if (controlId > 0)
            {
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                HttpClient?.DefaultRequestHeaders.Add("ControlId", $"{controlId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<WorKFlowDtoResponse>>("documentmanagement/DocumentManagement/ByFilterControlId").ConfigureAwait(false);
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                if (deserializeResponse!.Succeeded)
                {
                    worKFlowDtoResponse = deserializeResponse.Data!;                  
                }
            }
           
        }
        catch (Exception ex)
        {
            notificationModal.UpdateModal(ModalType.Warning, $"Error al procesar la solicitud: {ex.Message}", true, "Aceptar", "Cancelar");
            Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
        }
    }
    public async Task WorKFlowAsync(int Idcontrol)
    {
        dataChargue = false;
        controlId = Idcontrol;
        await GetWorKFlowAsync();
        StateHasChanged();
    }
    #endregion MethodsGeneral

    #endregion Methods

}
