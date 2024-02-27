using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.MetaData;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VMetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class MetaDataPage
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent nameInput { get; set; } = new();
        private InputModalComponent codeInput { get; set; } = new();
        private NotificationsComponentModal modalNotification { get; set; } = new();
        private NotificationsComponentModal notificationModalSucces { get; set; } = new();

        #endregion Components

        #region Modals

        private MetaModel meta { get; set; } = new() { PageSize = 10 };
        private List<MetaFieldsDtoResponse> MetaFields { get; set; } = new();
        private List<VSystemParamDtoResponse> systemParamList { get; set; } = new();
        private MetaFieldsFilterDtoRequest metaFieldByFilter { get; set; } = new();
        private MetaDataModal modalMetaFields { get; set; } = new();

        #endregion Modals

        #region Models

        private DeleteGeneralDtoRequest deleteRequest { get; set; } = new();

        #endregion Models

        #region Environments

        private int metaTitleId { get; set; } = new();
        private string fieldTypeCode { get; set; } = "";
        private string name { get; set; } = "";
        private string code { get; set; } = "";

        private bool dataChargue { get; set; } = false;

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetFieldType();
            await GetMetaFields();
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

        #region GetFieldType

        private async Task GetFieldType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "FTY");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    systemParamList = deserializeResponse.Data ?? new();
                }
                else
                {
                    systemParamList = new();
                }
            }
            catch
            {
                systemParamList = new();
            }
        }

        #endregion GetFieldType

        #region GetMetaFields

        private async Task GetMetaFields()
        {
            try
            {
                metaFieldByFilter.Code = code;
                metaFieldByFilter.NameMetaField = name;
                metaFieldByFilter.FieldType = fieldTypeCode;

                var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaFields/ByFilter", metaFieldByFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<MetaFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    MetaFields = deserializeResponse.Data ?? new();
                    meta = deserializeResponse.Meta ?? new() { PageSize = 10 };
                    dataChargue = true;
                }
                else
                {
                    MetaFields = new();
                    meta = new() { PageSize = 10 };
                    dataChargue = false;
                }
            }
            catch
            {
                MetaFields = new();
                meta = new() { PageSize = 10 };
            }
        }

        #endregion GetMetaFields

        #region GetFieldTypeCode

        public void GetFieldTypeCode(string code)
        {
            try
            {
                fieldTypeCode = code;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Officinas Productoras: {ex.Message}");
            }
        }

        #endregion GetFieldTypeCode

        #region OnClickReset

        private void OnClickReset()
        {
            name = "";
            code = "";
            fieldTypeCode = "";

            StateHasChanged();
        }

        #endregion OnClickReset

        #region OnClickSearch

        private async Task OnClickSearch()
        {
            await GetMetaFields();
        }

        #endregion OnClickSearch

        #region

        private async Task HandleStatusChanged(bool status)

        {
            /*        PageLoadService.MostrarSpinnerReadLoad(Js);*/
            await GetMetaFields();
            /*            PageLoadService.OcultarSpinnerReadLoad(Js);*/
        }

        #endregion Methods

        #region ShowModal

        private void ShowModal()
        {
            /*     PageLoadService.MostrarSpinnerReadLoad(Js);*/
            modalMetaFields.UpdateModalStatus(true);
            modalMetaFields.ResetForm();
            /*            PageLoadService.OcultarSpinnerReadLoad(Js);*/
        }

        #endregion ShowModal

        #region ShowModalEdit

        private async Task ShowModalEdit(MetaFieldsDtoResponse value)
        {
            /*PageLoadService.MostrarSpinnerReadLoad(Js);*/

            modalMetaFields.UpdateModalStatus(true);
            modalMetaFields.UpdateMetaData(value);

            /*            PageLoadService.OcultarSpinnerReadLoad(Js);*/
        }

        #endregion ShowModalEdit

        #region ShowModalDelete

        private void ShowModalDelete(MetaFieldsDtoResponse value)
        {
            metaTitleId = value.MetaFieldId;
            modalNotification.UpdateModal(ModalType.Warning, $"Esta seguro de eliminar el meta campo: {value.NameMetaField}", true, "Si", "No");
        }

        #endregion ShowModalDelete

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await DeleteMetatitle();
                await GetMetaFields();
            }
        }

        #endregion HandleModalNotiClose

        #region HandleModalClose

        private async Task HandleModalClose(bool newValue)
        {
            modalMetaFields.UpdateModalStatus(newValue);

            await GetFieldType();
            await GetMetaFields();
        }

        #endregion HandleModalClose

        #region DeleteMetatitle

        public async Task DeleteMetatitle()
        {
            /*   PageLoadService.MostrarSpinnerReadLoad(Js);*/

            deleteRequest = new() { Id = metaTitleId, User = "" };

            var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaFields/DeleteMetaFields", deleteRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
            if (deserializeResponse!.Succeeded)
            {
                notificationModalSucces.UpdateModal(ModalType.Success, "Registro Borrado Correctamente", true, "Aceptar");
            }
            else
            {
                notificationModalSucces.UpdateModal(ModalType.Error, "Error al borrar el registro", true, "Aceptar");
            }

            /*PageLoadService.OcultarSpinnerReadLoad(Js);
        */
        }

        #endregion DeleteMetatitle

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<MetaFieldsDtoResponse> newDataList)
        {
            MetaFields = newDataList;
        }

        #endregion HandlePaginationGrid

        #endregion Methods
    }
}