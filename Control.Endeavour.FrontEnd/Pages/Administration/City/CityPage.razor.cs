using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.City;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration.City
{
    public partial class CityPage
    {

        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        #endregion

        #region Components

        private PaginationComponent<CityDtoResponse, CityDtoRequest> paginationComponetPost = new();

        #endregion

        #region Modals

        private CityModal modalCity = new();
        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModalSucces = new();

        #endregion

        #region Parameters

        private int IdDepartamento { get; set; }
        private int IdPaises { get; set; }
        private bool EnabledDepartamento { get; set; } = false;

        #endregion

        #region Models

        private CityDtoResponse recordToDelete = new();
        private MetaModel meta = new();

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

        private List<CountryDtoResponse>? PaisesList = new();
        private List<StateDtoResponse>? DepartamentosList = new();
        private List<CityDtoResponse>? CiudadList = new();
        private Dictionary<string, dynamic> StateHeader { get; set; } = new Dictionary<string, dynamic>();

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
            await GetCountry();
        }

        #endregion

        #region HandleModalNotiClose
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                DeleteGeneralDtoRequest deleteCityDtoRequest = new DeleteGeneralDtoRequest();
                deleteCityDtoRequest.Id = recordToDelete.CityId;
                deleteCityDtoRequest.User = "admin";

                var responseApi = await HttpClient.PostAsJsonAsync("location/City/DeleteCity", deleteCityDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    notificationModalSucces.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true, "Aceptar");
                    await GetCity();
                }
                else
                {
                    //Logica no Exitosa
                    notificationModalSucces.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
                }
            }
        }
        #endregion

        #region HandlePaginationGrid
        private void HandlePaginationGrid(List<CityDtoResponse> newDataList)
        {
            CiudadList = newDataList;
        }
        #endregion

        #endregion

        #region OthersMethods

        #region ShowModalAdd
        private async Task ShowModalAdd()
        {
            await modalCity.PreparedModal();
            modalCity.UpdateModalStatus(true);
        }
        #endregion

        #region ShowModalEdit
        private async Task ShowModalEdit(CityDtoResponse record)
        {
            modalCity.Country = IdPaises;
            record.StateId = IdDepartamento;
            await modalCity.UpdateRecord(record);
            modalCity.UpdateModalStatus(true);
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
                    EnabledDepartamento = true;

                    HttpClient?.DefaultRequestHeaders.Remove("countryId");
                    HttpClient?.DefaultRequestHeaders.Add("countryId", IdPaises.ToString());
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<StateDtoResponse>>>("location/State/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("countryId");
                    DepartamentosList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<StateDtoResponse>();
                    CiudadList = new();

                    if (DepartamentosList.Count > 0)
                    {
                        IdDepartamento = 0;
                    }
                    else
                    {
                        DepartamentosList = new();
                        EnabledDepartamento = false;
                    }

                }
                else
                {
                    IdDepartamento = 0;
                    EnabledDepartamento = false;
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener el departamento: {ex.Message}");
            }
        }
        #endregion

        #region GetCity
        private async Task GetCity()
        {
            try
            {
                if (IdDepartamento > 0)
                {
                    StateHeader = new()
                    {
                        {"stateId", IdDepartamento }
                    };

                    HttpClient?.DefaultRequestHeaders.Remove($"{StateHeader.Keys.FirstOrDefault()}");
                    HttpClient?.DefaultRequestHeaders.Add($"{StateHeader.Keys.FirstOrDefault()}", $"{StateHeader.Values.FirstOrDefault()}");
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<CityDtoResponse>>>("location/City/ByFilterPagination");
                    HttpClient?.DefaultRequestHeaders.Remove($"{StateHeader.Keys.FirstOrDefault()}");

                    if (deserializeResponse.Succeeded)
                    {
                        CiudadList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<CityDtoResponse>();
                        meta = deserializeResponse.Meta;
                        paginationComponetPost.ResetPagination(meta);
                        isEnabled = false;
                    }
                    else
                    {
                        CiudadList = new();
                        meta = new();
                        IdDepartamento = 0;
                        isEnabled = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el municipio: {ex.Message}");
            }
        }
        #endregion

        #region ShowModalDelete
        private void ShowModalDelete(CityDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar la ciudad?", true, "Si", "No");
        }
        #endregion

        #endregion

        #endregion

    }
}
