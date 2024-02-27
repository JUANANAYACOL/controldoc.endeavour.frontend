using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
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
    public partial class ProductionOfficePage
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Modals

        private DeleteGeneralDtoRequest deleteRequest { get; set; } = new();
        private ProductionOfficeFilterDtoRequest productionByFilter { get; set; } = new();
        private ProductionOfficeModal productionOfficeModal { get; set; } = new();

        private NotificationsComponentModal modalNotification { get; set; } = new();
        private NotificationsComponentModal notificationModalSucces { get; set; } = new();

        private GenericSearchModal genericSearchModal { get; set; } = new();

        #endregion Modals

        #region Models

        private MetaModel meta { get; set; } = new() { PageSize = 10 };

        private List<AdministrativeUnitsDtoResponse> administrativeUnitsList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> productionOfficesList { get; set; } = new();

        #endregion Models

        #region Enviroment

        private int userIdToDelete { get; set; } = new();
        private int idAdUnit { get; set; }
        private bool isEnable { get; set; } = true;
        private bool dataChargue { get; set; } = false;

        #endregion Enviroment

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await FillAdministrativeUnitDdl();
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region ShowModal

        private void ShowModal()
        {
            var auSelected = administrativeUnitsList.Find(x => x.AdministrativeUnitId == idAdUnit);
            productionOfficeModal.UpdateModalStatus(true);
            productionOfficeModal.UpdateAdministrativeUnitSelected(idAdUnit, auSelected?.Name!);
        }

        #endregion ShowModal

        #region HandleStatusChanged

        private async Task HandleStatusChanged(bool status)
        {
            genericSearchModal.UpdateModalStatus(status);
            await SearchByAdministrativeUnit();
        }

        #endregion HandleStatusChanged

        #region HandleStatusChangedUpdated

        private async Task HandleStatusChangedUpdated(bool status)
        {
            productionOfficeModal.UpdateModalStatus(status);
            await SearchByAdministrativeUnit();
        }

        #endregion HandleStatusChangedUpdated

        #region HandleUserSelectedChanged

        private void HandleUserSelectedChanged(MyEventArgs<VUserDtoResponse> user)
        {
            productionOfficeModal.updateBossSelection(user.Data);
            genericSearchModal.UpdateModalStatus(user.ModalStatus);
        }

        #endregion HandleUserSelectedChanged

        #region OnDropDownValueChanged

        private async Task OnDropDownValueChanged(int newValue)
        {
            idAdUnit = newValue;

            isEnable = ( newValue <= 0 );
            productionByFilter.AdministrativeUnitId = newValue;

            try
            {
                await SearchByAdministrativeUnit();
            }
            catch (Exception ex)
            {
                // Manejar excepciones, logearlas o tomar medidas apropiadas según tu aplicación
                notificationModalSucces.UpdateModal(ModalType.Error, $"Error en OnDropDownValueChanged: {ex.Message}", true, "Aceptar");
            }
        }

        #endregion OnDropDownValueChanged

        #region SearchByAdministrativeUnit

        private async Task SearchByAdministrativeUnit()
        {
            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/ProductionOffice/ByFilter", productionByFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>();
            if (deserializeResponse!.Succeeded && ( deserializeResponse.Data != null ))
            {
                productionOfficesList = deserializeResponse.Data ?? new();
                dataChargue = true;
                meta = deserializeResponse.Meta ?? new() { PageSize = 10 };
            }
            else
            {
                productionOfficesList = new();
                meta = new() { PageSize = 10 };
                dataChargue = false;
            }
        }

        #endregion SearchByAdministrativeUnit

        #region FillAdministrativeUnitDdl

        private async Task FillAdministrativeUnitDdl()
        {
            HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
            HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", "0");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
            HttpClient?.DefaultRequestHeaders.Remove("key");

            if (deserializeResponse!.Succeeded)
            {
                administrativeUnitsList = deserializeResponse.Data ?? new();
            }
        }

        #endregion FillAdministrativeUnitDdl

        #region ShowModalEdit

        private async Task ShowModalEdit(ProductionOfficesDtoResponse value)
        {
            productionOfficeModal.UpdateModalStatus(true);
            await productionOfficeModal.UpdateSelectedRecord(value);
        }

        #endregion ShowModalEdit

        #region ShowModalDelete

        private void ShowModalDelete(ProductionOfficesDtoResponse value)
        {
            deleteRequest.Id = value.ProductionOfficeId;
            modalNotification.UpdateModal(ModalType.Warning, $"¿Está seguro de eliminar la siguiente oficina productora: {value.Name} ?", true, "Si", "No");
        }

        #endregion ShowModalDelete

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await DeleteUser();
                await SearchByAdministrativeUnit();
            }
        }

        #endregion HandleModalNotiClose

        #region DeleteUser

        public async Task DeleteUser()
        {
            var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/ProductionOffice/DeleteProductionOffice", deleteRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
            if (deserializeResponse.Succeeded)
            {
                notificationModalSucces.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true, "Aceptar");
            }
            else
            {
                notificationModalSucces.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
            }
        }

        #endregion DeleteUser

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<ProductionOfficesDtoResponse> newDataList)
        {
            productionOfficesList = newDataList;
        }

        #endregion HandlePaginationGrid

        #endregion Methods
    }
}