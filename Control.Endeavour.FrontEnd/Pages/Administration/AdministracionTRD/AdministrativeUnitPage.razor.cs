using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration.AdministracionTRD
{
    public partial class AdministrativeUnitPage
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components
        private PaginationComponent<AdministrativeUnitsDtoResponse, AdministrativeUnitFilterDtoRequest> paginationComponetPost = new();

        #endregion

        #region Modals
        private GenericSearchModal userSearchModal = new();
        private AdministrativeUnitModal modalAdministrativeUnit = new();
        private NotificationsComponentModal notificationModal = new();

        #endregion


        #region Models
        private MetaModel meta = new();
        private MetaModel metaAdministrativeUnits = new();
        private AdministrativeUnitsDtoResponse recordToDelete = new();
        DocumentalVersionFilterDtoRequest documentalVersionsFilterDtoResponse = new();
        AdministrativeUnitFilterDtoRequest administrativeUnitFilter = new();
        #endregion

        #region Environments

        #region Environments(Numeric)
        private int IdDocumental { get; set; }
        #endregion

        #region Environments(Bool)

        private bool isEnabled = true;
        private bool dataChargue = false;

        #endregion

        #region Environments(List & Dictionary)
        private List<AdministrativeUnitsDtoResponse> administrativeUnitList = new();
        private List<DocumentalVersionDtoResponse> documentalVersionsList = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            try
            {
                EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetDocumentalVersions();
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }


        }


        #endregion

        #region Methods

        #region HandleMethods
        private async Task HandleChangedData(bool changed)
        {
            await OnDropDownValueChanged(IdDocumental);
        }
        private void HandleStatusChanged(bool status)
        {
            // Recibe el valor de estado (true o false) aquí y haz lo que necesites con él
            userSearchModal.UpdateModalStatus(status);
        }
        #region Language
        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        private void HandlePaginationGrid(List<AdministrativeUnitsDtoResponse> newDataList)
        {
            administrativeUnitList = newDataList;
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.AdministrativeUnitId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/AdministrativeUnit/DeleteAdministrativeUnit", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await GetDocumentalVersions();
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true);
                    }

                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }
        private void HandleUserSelectedChanged(MyEventArgs<VUserDtoResponse> user)
        {

            userSearchModal.UpdateModalStatus(user.ModalStatus);

            modalAdministrativeUnit.updateBossSelection(user.Data);

        }
        #endregion

        #region OthersMethods

        #region GetDataMethods
        private async Task GetDocumentalVersions()
        {
            try
            {
                var responseApi = await HttpClient.GetAsync("paramstrd/DocumentalVersions/ByDocumentalVersions");
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    documentalVersionsList = deserializeResponse.Data;

                    //meta = deserializeResponse.Meta;
                }
                else
                {
                    documentalVersionsList = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las versiones documentales, por favor intente de nuevo!", true);
                }


            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }
        private async Task OnDropDownValueChanged(int newValue)
        {
            try
            {
                //PageLoadService.MostrarSpinnerReadLoad(Js);
                IdDocumental = newValue;
                isEnabled = (IdDocumental <= 0) ? true : false;

                administrativeUnitFilter.DocumentalVersionId = newValue;
                var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/AdministrativeUnit/ByFilter", administrativeUnitFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Any())
                {
                    administrativeUnitList = deserializeResponse.Data;
                    metaAdministrativeUnits = deserializeResponse.Meta;
                    paginationComponetPost.ResetPagination(metaAdministrativeUnits);

                    dataChargue = true;
                }
                else
                {
                    administrativeUnitList = new();
                    metaAdministrativeUnits = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar los registros, por favor intente de nuevo!", true);
                }
                //PageLoadService.OcultarSpinnerReadLoad(Js);
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }



        }

        #endregion

        #region Modals
        private void ShowModalDelete(AdministrativeUnitsDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar esa unidad administrativa?", true, "Si", "No", modalOrigin: "DeleteModal");
        }
        private void ShowModalEdit(AdministrativeUnitsDtoResponse record)
        {
            modalAdministrativeUnit.UpdateModalStatus(true);
            modalAdministrativeUnit.UpdateSelectedRecord(record);
        }
        private async Task ShowModalCreate()
        {
            await modalAdministrativeUnit.PreparedModal();
            modalAdministrativeUnit.UpdateModalStatus(true);
        }

        #endregion

        #endregion

        #endregion



    }
}
