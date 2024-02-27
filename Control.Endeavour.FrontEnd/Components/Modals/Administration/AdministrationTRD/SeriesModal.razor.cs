using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class SeriesModal
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

        [Parameter] public int ProOfficeID { get; set; }
        [Parameter] public string? ProOfficeName { get; set; }
        [Parameter] public EventCallback<bool> OnStatusUpdate { get; set; }

        #endregion

        #region Models
        private SeriesDtoRequest serieDtoRequest = new();
        private SeriesDtoResponse _selectedRecord = new();
        private MetaModel meta = new();
        #endregion

        #region Environments

        #region Environments(String)

        private string responseMessage = string.Empty;
        private string Text = "Seleccione una opción";
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
        private bool modalStatus = false;
        private bool IsDisabledCode = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<ProductionOfficesDtoResponse> proOfficeList = new();

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
                    serieDtoRequest.ProductionOfficeId = ProOfficeID;
                    serieDtoRequest.ActiveState = activeState;
                    serieDtoRequest.CreateUser = "Front"; //Cambiar por varibale de usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/Series/CreateSeries", serieDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<SeriesDtoResponse>>();

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
                _selectedRecord.CreateUser = "Front"; //Cambiar por variable de usuario
                SeriesUpdateDtoRequest seriesUpdateDto = new();
                seriesUpdateDto.SerieId = _selectedRecord.SeriesId;
                seriesUpdateDto.ProductionOfficeId = _selectedRecord.ProductionOfficeId;
                seriesUpdateDto.Name = serieDtoRequest.Name;
                seriesUpdateDto.Code = serieDtoRequest.Code;
                seriesUpdateDto.Description = serieDtoRequest.Description;
                seriesUpdateDto.ActiveState = activeState;
                seriesUpdateDto.User = serieDtoRequest.CreateUser;
                var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/Series/UpdateSeries", seriesUpdateDto);

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
        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                serieDtoRequest = new();
            }
            else
            {
                serieDtoRequest.ProductionOfficeId = _selectedRecord.ProductionOfficeId;
                serieDtoRequest.Name = _selectedRecord.Name;
                serieDtoRequest.Code = _selectedRecord.Code;
                serieDtoRequest.Description = _selectedRecord.Description;
                serieDtoRequest.ActiveState = _selectedRecord.ActiveState;
                serieDtoRequest.CreateUser = _selectedRecord.CreateUser;
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
        
        public void UpdateSelectedRecord(SeriesDtoResponse response)
        {
            _selectedRecord = response;
            activeState = _selectedRecord.ActiveState;
            UpdateForm = false;
            IsEditForm = true;
            Text = _selectedRecord.ProductionOfficeName;
            IsDisabledCode = true;
            serieDtoRequest.Code = _selectedRecord.Code;
            serieDtoRequest.Name = _selectedRecord.Name;
            serieDtoRequest.Description = _selectedRecord.Description;
            serieDtoRequest.ActiveState =_selectedRecord.ActiveState;
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            ResetFormAsync();
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion



        #endregion

        #endregion

    }
}
