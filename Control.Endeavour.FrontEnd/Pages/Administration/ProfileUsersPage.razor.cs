using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Address;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.ProfileUsers;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffices;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ProfileUsersPage
    {

        #region Variables

        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Modals
        private ProfileUsersModal modalProfileUsers = new();
        private NotificationsComponentModal notificationModal = new();
        #endregion Modals

        #region Models
        private ProfileCreateDtoRequest? FilterDtoRequest { get; set; } = new();
        private List<ProfileDtoResponse> ProfileUsersList = new();
        private ProfileDtoResponse profileToDelete = new();
        private PaginationComponent<ProfileDtoResponse, ProfileCreateDtoRequest> PaginationComponet = new();
        private MetaModel? meta { get; set; } = new() { PageSize = 10 };
        #endregion Models

        #region Environments
        private bool CrearEditar { get; set; } = true;
        #endregion Environments

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            EventAggregator!.LanguageChangedEvent += HandleLanguageChanged;
            await GetProfiles();
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

        #region GetProfiles

        private async Task GetProfiles()
        {
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("permission/Profile/ByFilter", FilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ProfileDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    ProfileUsersList = deserializeResponse.Data ?? new();
                    meta = deserializeResponse!.Meta;
                    PaginationComponet.ResetPagination(meta!);
                }
                else
                {
                    ProfileUsersList = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar los Perfiles de Usuario, por favor intente de nuevo!", true);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles: {ex.Message}");
            }
        }

        #endregion GetProfiles

        #region DeleteProfile

        private void HandleRecordToDelete(ProfileDtoResponse record)
        {
            profileToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el mensaje?", true, "Si", "No", modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin!.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new() { Id = profileToDelete.ProfileId, User = "user" };

                    var responseApi = await HttpClient!.PostAsJsonAsync("permission/Profile/DeleteProfile", request);
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

        #endregion DeleteProfile

        #region Modal

        private void ShowModal()
        {
            modalProfileUsers.UpdateModalStatus(true);
        }

        private void ShowModalEdit(ProfileDtoResponse record)
        {
            modalProfileUsers.UpdateModalStatus(true);
            modalProfileUsers.ReceiveProfileUser(record);
        }

        private void HandleStatusChanged(bool status)
        {
            modalProfileUsers.UpdateModalStatus(status);
        }

        #endregion Modal

        #region RefreshGrid

        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetProfiles();
        }

        #endregion RefreshGrid

        #region HandlePagination

        private void HandlePaginationGrid(List<ProfileDtoResponse> newDataList)
        {
            ProfileUsersList = newDataList;
        }

        #endregion HandlePagination

        #endregion OtherMethods

        #endregion

    }
}
