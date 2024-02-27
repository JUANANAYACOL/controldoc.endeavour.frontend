using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;


namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class SubSeriesModal
    {

		#region Variables

		#region Inject 
		[Inject]
		private EventAggregatorService? EventAggregator { get; set; }

		[Inject]
		private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components
        private InputModalComponent codeInput = new();
        private InputModalComponent nameInput = new();
        private InputModalComponent descriptionInput = new();

        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        #endregion

        #region Parameters
        [Parameter] public int SerieID { get; set; }
        [Parameter] public EventCallback<bool> OnStatusUpdate { get; set; }

        [Parameter] public string NameSerie { get; set; } = string.Empty;



        #endregion

        #region Models
        private SubSerieDtoRequest subSerieDtoRequest = new();
        private SubSeriesDtoResponse _selectedRecord = new();

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)
        private decimal CharacterCounter = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool IsEditForm = false;
        private bool activeState = true;
        private bool UpdateForm = true;
        private bool IsDisabledCode = false;
        private bool modalStatus = false;
        #endregion

        #region Environments(List & Dictionary)

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

		}


        #endregion

        #region Methods

        #region HandleMethods

        #region FormMethods
        private async Task HandleValidSubmit()
        {
            if (IsEditForm)
            {
                await HandleFormUpdate();

                IsEditForm = false;
            }
            else
            {
                await HandleFormCreate();
            }
            StateHasChanged();


        }
        private async Task HandleFormCreate()
        {
            try
            {
                if (codeInput.IsInputValid && nameInput.IsInputValid)
                {
                    subSerieDtoRequest.SeriesId = SerieID;        
                    subSerieDtoRequest.ActiveState = activeState;
                    subSerieDtoRequest.User = "Front"; 
                    var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/SubSeries/CreateSubSerie", subSerieDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<SubSeriesDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, "¡Se creó el registro de forma exitosa!", true, "Aceptar");
                        await OnStatusUpdate.InvokeAsync(true);
                    }
                }
                else { notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de crear el registro, por favor intente de nuevo!", true, "Aceptar"); }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true, "Aceptar");
            }
        }

        private async Task HandleFormUpdate()
        {
            try
            {
                _selectedRecord.ActiveState = activeState;
                _selectedRecord.CreateUser = "Front";
                SubSeriesUpdateDtoRequest SubSeriesUpdateDto = new();
                SubSeriesUpdateDto.SubSeriesId = _selectedRecord.SubSeriesId;
                SubSeriesUpdateDto.SeriesId = _selectedRecord.SeriesId;
                SubSeriesUpdateDto.Name = subSerieDtoRequest.Name;
                SubSeriesUpdateDto.Code = subSerieDtoRequest.Code;
                SubSeriesUpdateDto.Description = subSerieDtoRequest.Description;
                SubSeriesUpdateDto.ActiveState = activeState;
                SubSeriesUpdateDto.User = "Front";
                var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/SubSeries/UpdateSubSerie", SubSeriesUpdateDto);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<AdministrativeUnitsDtoResponse>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    notificationModal.UpdateModal(ModalType.Success, "¡Se actualizó el registro de forma exitosa!", true, "Aceptar");
                    await OnStatusUpdate.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de actualizar el registro, por favor intente de nuevo!", true, "Aceptar");
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true, "Aceptar");
            }
        }
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
                ResetFormAsync();
            }
        }

        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                subSerieDtoRequest = new();
            }
            else
            {
                subSerieDtoRequest.SeriesId = _selectedRecord.SeriesId;
                subSerieDtoRequest.Name = _selectedRecord.Name;
                subSerieDtoRequest.Code = _selectedRecord.Code;
                subSerieDtoRequest.Description = _selectedRecord.Description;
                subSerieDtoRequest.ActiveState = _selectedRecord.ActiveState;
                subSerieDtoRequest.User = _selectedRecord.CreateUser;
            }
            CharacterCounter = 0;

        }
        #endregion

        private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region OthersMethods
        #region ValidationMethods
        private void CountCharacters(ChangeEventArgs e)
        {
            string value = e.Value.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                CharacterCounter = value.Length;

            }
            else
            {
                CharacterCounter = 0;

            }
        }

        #endregion

        #region ModalMethods
        public void UpdateSelectedRecord(SubSeriesDtoResponse response)
        {
            _selectedRecord = response;
            activeState = _selectedRecord.ActiveState;
            UpdateForm = false;
            IsEditForm = true;
            IsDisabledCode = true;
            subSerieDtoRequest.Code = _selectedRecord.Code;
            subSerieDtoRequest.Name = _selectedRecord.Name;
            subSerieDtoRequest.Description = _selectedRecord.Description;
        }
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();

        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;

        }
        #endregion
        #endregion

        #endregion

    }
}
