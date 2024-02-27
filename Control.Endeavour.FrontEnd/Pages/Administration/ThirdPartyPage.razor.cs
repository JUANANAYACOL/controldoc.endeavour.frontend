using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Address;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.ThirdParty;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Data.Filtering.Helpers;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ThirdPartyPage
    {

        #region Variables
        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Modals
        private ThirdPartyModal modalThirdParty = new();
        private AddressModal modalAddress = new();
        private NotificationsComponentModal notificationModal = new();
        #endregion

        #region Models
        private ThirdPartyFilterDtoRequest FilterDtoRequest { get; set; } = new();
        private List<ThirdPartyDtoResponse> ThirdPartyList = new();
        private ThirdPartyDtoResponse thirdPartyToDelete = new();
        private List<TabDtoResponse> tabs = new List<TabDtoResponse>();
        private MetaModel meta { get; set; } = new() { PageSize = 10 };
        #endregion

        #region Environments
        //inputs
        private string names = "";
        private string email = "";
        private string identification = "";

        //Tabs
        private int currentTab { get; set; } = 0;
        private MetaModel currentMeta { get; set; } = new();
        private PaginationComponent<ThirdPartyDtoResponse, ThirdPartyFilterDtoRequest> paginationComponent = new();
        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator!.LanguageChangedEvent += HandleLanguageChanged;
            await GetThirdPartyAll();

            var pnResponse = await GetThirdPartyPNandPJ("PN");
            tabs.Add(new TabDtoResponse { Title = "Persona Natural", FilteredData = FilterData("PN"), Meta = pnResponse.Meta! });

            var pjResponse = await GetThirdPartyPNandPJ("PJ");
            tabs.Add(new TabDtoResponse { Title = "Persona Jurídica", FilteredData = FilterData("PJ"), Meta = pjResponse.Meta! });

            currentMeta = tabs[currentTab].Meta;
            paginationComponent.ResetPagination(currentMeta);
        }
        #endregion

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OtherMethods

        #region Tabs

        // Función para filtrar los datos según el tipo de persona
        List<ThirdPartyDtoResponse> FilterData(string personType)
        {
            return ThirdPartyList
                .Where(item => item.PersonType == personType)
                .ToList();
        }

        //Aqui se debe ver la parte de como mantener los registros y concordar con la pagina
        private void HandlePaginationGridAsync(List<ThirdPartyDtoResponse> newDataList)
        {
            ThirdPartyList = newDataList;
            tabs[currentTab].FilteredData = FilterData(currentTab == 0 ? "PN" : "PJ");
        }

        #endregion Tabs

        #region GetThirdParty

        private async Task GetThirdParty(string personType)
        {
            try
            {
                ThirdPartyFilterDtoRequest request = new() { PersonType = personType };
                var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ThirdPartyDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    ThirdPartyList.AddRange(deserializeResponse.Data!);
                    StateHasChanged();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las personas, por favor intente de nuevo!", true);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener personas naturales y juridicas: {ex.Message}");
            }
        }

        private async Task GetThirdPartyAll()
        {
            await GetThirdParty("PN");
            await GetThirdParty("PJ");
        }

        #endregion GetThirdParty

        #region GetPNAndPJ

        public async Task<HttpResponseWrapperModel<List<ThirdPartyDtoResponse>>> GetThirdPartyPNandPJ(string personType)
        {
            try
            {
                ThirdPartyFilterDtoRequest request = new() { PersonType = personType };
                var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/ByFilter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ThirdPartyDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    return deserializeResponse;
                }
                else
                {
                    return new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener personas naturales y juridicas: {ex.Message}");
                return new();
            }
        }

        #endregion GetPNAndPJ

        #region RefreshGrid

        private async Task HandleRefreshGridData(bool refresh)
        {
            var thirdPList = await GetThirdPartyPNandPJ(currentTab == 0 ? "PN" : "PJ");
            ThirdPartyList = thirdPList.Data!;

            // Llama al método FilterData después de la actualización de GetThirdParty
            tabs[currentTab].FilteredData = FilterData(currentTab == 0 ? "PN" : "PJ");

            var pnResponse = await GetThirdPartyPNandPJ("PN");
            tabs[0].Meta = pnResponse.Meta!;
            var pjResponse = await GetThirdPartyPNandPJ("PJ");
            tabs[1].Meta = pjResponse.Meta!;
        }

        #endregion RefreshGrid

        #region Filters

        #region ApplyFilters

        private async Task ApplyFiltersAsync()
        {
            if (string.IsNullOrEmpty(names) && string.IsNullOrEmpty(email) && string.IsNullOrEmpty(identification))
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Debe ingresar al menos un criterio de búsqueda!", true, "Aceptar");
            }
            else
            {
                try
                {
                    ThirdPartyFilterDtoRequest request = new() { Names = names, Email = email, IdentificationNumber = identification };
                    var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/ByFilter", request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ThirdPartyDtoResponse>>>();

                    if (deserializeResponse!.Data != null)
                    {
                        ThirdPartyList = deserializeResponse.Data;
                        meta = deserializeResponse.Meta!;

                        // Actualiza los datos filtrados en cada pestaña
                        tabs[0].FilteredData = FilterData("PN");
                        tabs[1].FilteredData = FilterData("PJ");
                    }
                    else
                    {
                        names = "";
                        email = "";
                        identification = "";
                        notificationModal.UpdateModal(ModalType.Error, "¡No hay coincidencias!", true, "Aceptar");

                        await GetThirdPartyAll();

                        // Actualiza los datos filtrados en cada pestaña
                        tabs[0].FilteredData = FilterData("PN");
                        tabs[1].FilteredData = FilterData("PJ");
                        await HandleRefreshGridData(true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener personas naturales y juridicas: {ex.Message}");
                }

            }
            StateHasChanged();
        }
        
        #endregion ApplyFilters

        #region ResetFilter

        public async Task ResetFiltersAsync()
        {
            if (!String.IsNullOrEmpty(names) || !String.IsNullOrEmpty(email) || !String.IsNullOrEmpty(identification))
            {
                names = "";
                email = "";
                identification = "";
                await GetThirdPartyAll();
                tabs[0].FilteredData = FilterData("PN");
                tabs[1].FilteredData = FilterData("PJ");
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, "¡Para limpiar debe ingresar al menos un criterio de búsqueda!", true, "Aceptar");
            }
            
            StateHasChanged();
        }

        #endregion ResetFilter

        #endregion Filters

        #region ModalThirdParty

        private async Task ShowModal()
        {
            await modalThirdParty.selectPersonType(currentTab);
            modalThirdParty.UpdateModalStatus(true);
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

        private async Task TabChangedHandler(int newIndex)
        {
            await GetThirdPartyAll();

            currentTab = newIndex;
            currentMeta = tabs[newIndex].Meta;

            await modalThirdParty.selectPersonType(currentTab);
            paginationComponent.ResetPagination(currentMeta);

            StateHasChanged();
        }

        private async Task ShowModalEdit(ThirdPartyDtoResponse record)
        {
            await modalThirdParty.selectPersonType(currentTab);
            modalThirdParty.UpdateModalStatus(true);
            modalThirdParty.ReceiveThirdParty(record);
        }

        private void HandleUserSelectedChanged(MyEventArgs<List<(string, AddressDtoRequest)>> address)
        {
            modalAddress.UpdateModalStatus(address.ModalStatus);
            modalThirdParty.updateAddressSelection(address.Data!);

        }

        #endregion ModalThirdParty

        #region DeleteRecord

        private void HandleRecordToDelete(ThirdPartyDtoResponse record)
        {
            thirdPartyToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el mensaje?", true, "Si", "No", modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin!.Equals("DeleteModal"))
                {

                    DeleteGeneralDtoRequest request = new() { Id = thirdPartyToDelete.ThirdPartyId, User = "user" };
                    var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/DeleteThirdParty", request);
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

        #endregion DeleteRecord

        #endregion OtherMethods

        #endregion

    }
}
