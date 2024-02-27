using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Country;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration.Country
{
    public partial class CountryPage: ComponentBase
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

        private PaginationComponent<CountryDtoResponse, CountryDtoRequest> paginationComponetPost = new();

        #endregion

        #region Modals

        private CountryModal modalCountry = new();
        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModalSucces = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        private MetaModel meta = new();
        private CountryDtoResponse recordToDelete = new();

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)

        private List<CountryDtoResponse> PaisesList = new();

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
                DeleteGeneralDtoRequest deleteCountryDtoRequest = new();
                deleteCountryDtoRequest.Id = recordToDelete.CountryId;
                deleteCountryDtoRequest.User = "admin";

                var responseApi = await HttpClient.PostAsJsonAsync("location/Country/DeleteCountry", deleteCountryDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModalSucces.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true, "Aceptar");
                    await GetCountry();
                }
                else
                {
                    notificationModalSucces.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
                }
            }

        }
        #endregion

        #region HandlePaginationGrid
        private void HandlePaginationGrid(List<CountryDtoResponse> newDataList)
        {
            PaisesList = newDataList;
        }
        #endregion

        #endregion

        #region OthersMethods

        #region ShowModalAdd
        private async Task ShowModalAdd()
        {
            modalCountry.UpdateModalStatus(true);
        }
        #endregion

        #region ShowModalEdit
        private void ShowModalEdit(CountryDtoResponse record)
        {

            modalCountry.UpdateModalStatus(true);
            modalCountry.UpdateRecord(record);
        }
        #endregion

        #region ShowModalDelete
        private void ShowModalDelete(CountryDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el País?", true, "Si", "No");
        }
        #endregion

        #region GetCountry
        private async Task GetCountry()
        {
            try
            {
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<CountryDtoResponse>>>("location/Country/ByFilterPagination");
                PaisesList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<CountryDtoResponse>();
                meta = deserializeResponse.Meta;
                paginationComponetPost.ResetPagination(meta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el país: {ex.Message}");
            }
        }
        #endregion

        #endregion

        #endregion

    }
}


    
