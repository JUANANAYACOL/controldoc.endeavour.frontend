using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.UsersAdministration
{
    public partial class UserProfilesModal
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

        PaginationComponent<ProfilesDtoResponse, ProfileByFilterDtoRequest> paginationComponetPost = new();

        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters

        [Parameter] public EventCallback<IEnumerable<ProfilesDtoResponse>> OnProfilesSelected { get; set; }
        [Parameter] public int CompanyID { get; set; }

        #endregion

        #region Models
        private MetaModel ProfilesMeta = new();
        private ProfileByFilterDtoRequest profileByFilterDto = new();
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
        private List<ProfilesDtoResponse>? lstProfilesByCompanyID { get; set; } = new List<ProfilesDtoResponse>();
        private IEnumerable<ProfilesDtoResponse> SelectedProfiles { get; set; } = Enumerable.Empty<ProfilesDtoResponse>();

        private List<int> SelectdProfilesId { get; set; } = new List<int>();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			
            try
            {
                EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetProfileByCompany(CompanyID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
            //PageLoadService.OcultarSpinnerReadLoad(Js);

        }


        #endregion

        #region Methods

        #region HandleMethods
        private void HandlePaginationGrid(List<ProfilesDtoResponse> newDataList)
        {
            lstProfilesByCompanyID = newDataList;
        }
        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }

        private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region OthersMethods

        #region Get Data Methods
        private async Task GetProfileByCompany(int companyID)
        {
            try
            {
                profileByFilterDto.CompanyId = companyID;
                //var response = await CallService.Get<List<ProfilesDtoResponse>>("permission/Profile/ByFilter", HeadersProfilesByCompanyId);
                //lstProfilesByCompanyID = response.Data;
                //ProfilesMeta = response.Meta;
                var responseApi = await HttpClient.PostAsJsonAsync("permission/Profile/ByFilter", profileByFilterDto);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ProfilesDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstProfilesByCompanyID = deserializeResponse.Data;
                    ProfilesMeta = deserializeResponse.Meta;
                    paginationComponetPost.ResetPagination(ProfilesMeta);
                }
                else
                {
                    lstProfilesByCompanyID = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar los perfiles, por favor intente de nuevo!", true);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener los perfiles del usuario: {ex.Message}");
            }
        }
        #endregion

        #region ModalMethods

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        
        protected void OnSelect(IEnumerable<ProfilesDtoResponse> profiles)
        {
            SelectedProfiles = profiles;
        }
        private async Task SendProfilesId()
        {

            await OnProfilesSelected.InvokeAsync(SelectedProfiles);
            UpdateModalStatus(false);

        }

        

        
        #endregion

        #endregion

        #endregion

    }
}
