using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.State;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffices;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components.Scheduler.Rendering;

namespace Control.Endeavour.FrontEnd.Pages.Administration.State
{
    public partial class StatePage: ComponentBase
    {


        #region Variables

        #region Inject 
        [Inject]
        private IJSRuntime Js { get; set; }
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components

        private PaginationComponent<StateDtoResponse, StateDtoRequest> paginationComponetPost = new();

        #endregion

        #region Modals

        private StateModal modalState = new();
        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModalSucces = new();

        #endregion

        #region Parameters

        private int IdPaises { get; set; }

        #endregion

        #region Models

        private MetaModel meta = new();
        private StateDtoResponse recordToDelete = new();

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool isEnabled = true;

        #endregion

        #region Environments(List & Dictionary)

        private List<CountryDtoResponse> PaisesList = new();
        private List<StateDtoResponse> DepartamentosList = new();
        private Dictionary<string, dynamic> CountryHeader { get; set; } = new Dictionary<string, dynamic>();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetCountry();
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region HandleRefreshGridDataAsync
        private async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetState();
        }
        #endregion

        #region HandleModalNotiClose
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                DeleteGeneralDtoRequest deleteStateDtoRequest = new();
                deleteStateDtoRequest.Id = recordToDelete.StateId;
                deleteStateDtoRequest.User = "Admin";
                var responseApi = await HttpClient.PostAsJsonAsync("location/State/DeleteState", deleteStateDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModalSucces.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true, "Aceptar");
                    await GetState();
                }
                else
                {
                    notificationModalSucces.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
                }
            }
        }
        #endregion

        #region HandlePaginationGrid
        private void HandlePaginationGrid(List<StateDtoResponse> newDataList)
        {
            DepartamentosList = newDataList;
        }
        #endregion

        #endregion

        #region OthersMethods

        #region ShowModalAdd
        private void ShowModalAdd()
        {
            modalState.PreparedModal();
            modalState.UpdateModalStatus(true);
        }
        #endregion

        #region ShowModalEdit
        private void ShowModalEdit(StateDtoResponse record)
        {
            record.CountryId = IdPaises;
            modalState.UpdateRecord(record);
            modalState.UpdateModalStatus(true);
        }
        #endregion

        #region ShowModalDelete
        private void ShowModalDelete(StateDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el departamento?", true, "Si", "No", modalOrigin: "DeleteModal");
        }
        #endregion

        #region GetCountry
        private async Task GetCountry()
        {
            try
            {
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<CountryDtoResponse>>>("location/Country/ByFilter");
                PaisesList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<CountryDtoResponse>();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener el país: {ex.Message}");
            }
        }
        #endregion

        #region GetState
        private async Task GetState()
        {
            try
            {
                if (IdPaises > 0)
                {
                    CountryHeader = new()
                    {
                        {"countryId", IdPaises }
                    };

                    HttpClient?.DefaultRequestHeaders.Remove($"{CountryHeader.Keys.FirstOrDefault()}");
                    HttpClient?.DefaultRequestHeaders.Add($"{CountryHeader.Keys.FirstOrDefault()}", $"{CountryHeader.Values.FirstOrDefault()}");
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<StateDtoResponse>>>("location/State/ByFilterPagination");
                    HttpClient?.DefaultRequestHeaders.Remove($"{CountryHeader.Keys.FirstOrDefault()}");
                    DepartamentosList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<StateDtoResponse>();
                    meta = deserializeResponse.Meta;
                    paginationComponetPost.ResetPagination(meta);
                    isEnabled = false;
                }
                else { DepartamentosList = new(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el departamento: {ex.Message}");
            }
        }
        #endregion

        #endregion

        #endregion

    }
}