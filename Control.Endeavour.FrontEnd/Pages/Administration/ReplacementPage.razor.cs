using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Replacement;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ReplacementPage
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

        #region Components

        private NotificationsComponentModal notificationModal { get; set; } = new();
        private NotificationsComponentModal notificationModalSucces { get; set; } = new();

        #endregion Components

        #region Modals

        private GenericSearchModal genericSearchModal { get; set; } = new();
        private ReplacementModal repalcementModal { get; set; } = new();

        #endregion Modals

        #region Models

        private List<VReplacementDtoResponse> ReplacementList { get; set; } = new();
        private VReplacementDtoResponse profileToUpdate { get; set; } = new();
        private DeleteGeneralDtoRequest deleteRequest { get; set; } = new();

        private MetaModel meta { get; set; } = new() { PageSize = 10 };

        private ReplacementFilterDtoRequest replacementFilterDtoRequest { get; set; } = new();

        #endregion Models

        #region Environments

        private bool dataChargue { get; set; } = false;

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetReplacements();
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

        private void ShowModal()
        {
            repalcementModal.ResetForm();
            repalcementModal.UpdateModalStatus(true);
        }

        private async Task HandleReplacementModalStatusChangedAsync(bool status)
        {
            genericSearchModal.UpdateModalStatus(status);
            repalcementModal.UpdateModalStatus(status);

            if (!status)
            {
                await Task.FromResult(GetReplacements);
            }

            StateHasChanged();
        }

        private void HandleGenericSearchStatusChanged(MyEventArgs<VUserDtoResponse> user)
        {
            /*           PageLoadService.MostrarSpinnerReadLoad(Js);*/
            repalcementModal.updateUserSelection(user!.Data!);
            genericSearchModal.UpdateModalStatus(user!.ModalStatus);
            /*            PageLoadService.OcultarSpinnerReadLoad(Js);*/
        }

        private void ShowModalEdit(VReplacementDtoResponse replacement)
        {
            repalcementModal.UpdateModalStatus(true);
            repalcementModal.UpdateSelectedRemplacement(replacement);
        }

        private async Task GetReplacements()
        {
            try
            {
                replacementFilterDtoRequest = new();

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/Replacement/ByFilter", replacementFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VReplacementDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    ReplacementList = deserializeResponse.Data ?? new();
                    meta = deserializeResponse.Meta ?? new() { PageSize = 10 };
                    dataChargue = true;
                }
                else
                {
                    ReplacementList = new();
                    meta = new() { PageSize = 10 };
                    dataChargue = false;
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles de usuario: {ex.Message}");
            }
        }

        private void HandlePaginationGrid(List<VReplacementDtoResponse> newDataList)
        {
            ReplacementList = newDataList;
        }

        private void HandleRecordToDelete(VReplacementDtoResponse args)
        {
            profileToUpdate = args;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el Reemplazo?", true, "Si", "No");
        }

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                deleteRequest.Id = profileToUpdate.ReplacementId;

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/Replacement/DeleteReplacement", deleteRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                if (deserializeResponse!.Succeeded)
                {
                    notificationModalSucces.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true, "Aceptar");
                    await GetReplacements();
                }
                else
                {
                    notificationModalSucces.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, "Aceptar");
                }
            }
            else
            {
                Console.WriteLine("Registro no eliminado");
            }
        }

        #endregion Methods
    }
}