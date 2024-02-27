using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Address;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Company;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class CompanyPage
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals
        private CompanyModal modalCompanies = new();
        private AddressModal modalAddress = new();
        private NotificationsComponentModal notificationModal = new();
        #endregion

        #region Parameters


        #endregion

        #region Models
        private AddressDtoRequest addressrequest = new();
        private CompanyCreateDtoRequest companyDtoRequest = new();
        private CompanyDtoResponse companyUpdate = new();
        private CompanyDtoResponse companyToDelete = new();
        private CompanyDtoResponse _selectedRecord = new();
        private MetaModel? meta { get; set; } = new() { PageSize = 10 };
        private PaginationComponent<CompanyDtoResponse, CompanyCreateDtoRequest> PaginationComponent = new();
        #endregion

        #region Environments

        #region Environments(String)
        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool modalStatus = false;
        #endregion

        #region Environments(List & Dictionary)
        private List<CompanyDtoResponse> CompaniesList = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetCompanies();
        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OtherMethods

        #region GetCompany
        private async Task GetCompanies()
        {
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("companies/Company/ByFilter", companyDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<CompanyDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    CompaniesList = deserializeResponse.Data ?? new();
                    meta = deserializeResponse.Meta;
                    PaginationComponent.ResetPagination(meta!);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las personas, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las compañias: {ex.Message}");
            }
        }
        #endregion GetCompany

        #region DeleteCompany

        private void HandleRecordToDelete(CompanyDtoResponse record)
        {
            companyToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el mensaje?", true, "Si", "No", modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin!.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new() { Id = companyToDelete.CompanyId, User = "user" };

                    var responseApi = await HttpClient!.PostAsJsonAsync("companies/Company/DeleteCompany", request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        await HandleRefreshGridData(true);
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true, "Aceptar");
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        #endregion DeleteCompany

        #region RefreshGrid

        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetCompanies();
        }

        #endregion RefreshGrid

        #region ModalCompanies

        private async Task ShowModalEdit(CompanyDtoResponse record)
        {

            modalCompanies.UpdateModalStatus(true);
            await modalCompanies.RecibirRegistro(record);
        }

        #endregion ModalCompanies

        #region ModalAddress
        private void ShowModal()
        {
            modalCompanies.UpdateModalStatus(true);
        }

        private void HandleStatusChanged(bool status)
        {

            modalAddress.UpdateModalStatus(status);
        }

        private async Task HandleId(int id)
        {
            await modalAddress.UpdateModalIdAsync(id);
        }

        private void HandleForm()
        {
            modalAddress.ResetForm();
        }

        private void HandleUserSelectedChanged(MyEventArgs<List<(string, AddressDtoRequest)>> address)
        {
            modalAddress.UpdateModalStatus(address.ModalStatus);
            modalCompanies.updateAddressSelection(address.Data!);
        }
        #endregion

        #region PaginationGrid
        private void HandlePaginationGrid(List<CompanyDtoResponse> newDataList)
        {
            CompaniesList = newDataList;
        }
        #endregion

        #endregion OtherMethods

        #endregion

    }
}
