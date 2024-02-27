using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VMetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using Telerik.Blazor.Components;
using Telerik.DataSource.Extensions;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.MetaData
{
    public partial class MetaDataModal : ComponentBase
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

        #region Modals

        public NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals

        #region Components

        private string showPanel = "d-none";

        private InputModalComponent codeInput { get; set; } = new();
        private InputModalComponent nameInput { get; set; } = new();

        private InputModalComponent orderInput { get; set; } = new();
        private InputModalComponent valueInput { get; set; } = new();

        #endregion Components

        #region Models

        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }
        private MetaFieldCreateDtoRequest metaFieldRequest { get; set; } = new();
        private List<VSystemParamDtoResponse> systemParamListModal { get; set; } = new();

        private List<MetaValueCreateDtoRequest> metaValueCreateList { get; set; } = new();

        #endregion Models

        #region Environments

        [Parameter] public bool modalStatus { get; set; }

        private string fieldTypeCodeModal { get; set; } = "";
        private bool isEditForm { get; set; } = false;
        private bool mandatory { get; set; } = false;
        private bool topograhpy { get; set; } = false;
        private bool isAnonymous { get; set; } = false;

        private bool active { get; set; } = false;

        private string orderString { get; set; } = "";
        private string valueString { get; set; } = "";

        private int metaFiledIdToUpdate { get; set; } = new();

        private int metaTitleIdToUpdate { get; set; } = new();

        private List<MetaValueCreateDtoRequest> listOfMetaValuesToDelete { get; set; } = new();
        private List<MetaValueCreateDtoRequest> listOfMetaValuesToUpdate { get; set; } = new();

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetFieldType();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            if (isEditForm)
            {
                await HandleFormUpdate();
                isEditForm = false;
            }
            else
            {
                await HandleFormCreate();
            }
            ResetForm();
        }

        #endregion HandleValidSubmit

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            try
            {
                if (codeInput.IsInputValid && nameInput.IsInputValid && metaFieldRequest.FieldType.HasValue())
                {
                    metaFieldRequest.Anonymization = isAnonymous;

                    metaFieldRequest.Mandatory = mandatory;

                    metaFieldRequest.TopographicLocation = topograhpy;

                    metaFieldRequest.Anonymization = isAnonymous;
                    metaFieldRequest.TopographicLocation = topograhpy;
                    metaFieldRequest.Mandatory = mandatory;
                    metaFieldRequest.ActiveState = active;

                    if (metaValueCreateList.Count != 0 && metaFieldRequest.FieldType.Equals("FTY,19"))
                    {
                        metaFieldRequest.MetaValues = metaValueCreateList;
                    }

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaFields/CreateMetaFields", metaFieldRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VMetaFieldDtoResponse>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, deserializeResponse.Message!, true, "Aceptar");
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, deserializeResponse.Message!, true, "Aceptar");
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "Uno o mas campos son necesarios", true, "Aceptar");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "Error at craeting data", true, "Aceptar");
            }
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            try
            {
                if (codeInput.IsInputValid && nameInput.IsInputValid && metaFieldRequest.FieldType.HasValue())
                {
                    metaFieldRequest.Anonymization = isAnonymous;

                    metaFieldRequest.Mandatory = mandatory;

                    metaFieldRequest.TopographicLocation = topograhpy;

                    metaFieldRequest.Anonymization = isAnonymous;
                    metaFieldRequest.TopographicLocation = topograhpy;
                    metaFieldRequest.Mandatory = mandatory;
                    metaFieldRequest.ActiveState = active;

                    var updateMetaFiled = new MetaFieldUpdateDtoRequest()
                    {
                        ActiveState = active,
                        Anonymization = metaFieldRequest.Anonymization,
                        Code = metaFieldRequest.Code,
                        FieldType = metaFieldRequest.FieldType,
                        Mandatory = mandatory,
                        MetaFieldId = metaFiledIdToUpdate,

                        NameMetaField = metaFieldRequest.NameMetaField,
                        TopographicLocation = topograhpy,
                    };

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaFields/UpdateMetaFields", updateMetaFiled);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VMetaFieldDtoResponse>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        if (listOfMetaValuesToDelete.Count != 0)
                        {
                            foreach (var metaValue in listOfMetaValuesToDelete)
                            {
                                var responseDelete = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaValues/DeleteMetaValues", new DeleteGeneralDtoRequest() { Id = metaValue.MetaValueId });
                                var deserializeResponseDelete = await responseDelete.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                                if (!deserializeResponseDelete!.Succeeded)
                                {
                                    notificationModal.UpdateModal(ModalType.Error, "Error al eliminar los meta valores asociados", true, "Aceptar");
                                }
                            }
                        }

                        if (listOfMetaValuesToUpdate.Count != 0)
                        {
                            foreach (var item in listOfMetaValuesToUpdate)
                            {
                                var newMetaValue = new MetaValueCreateDtoRequest()
                                {
                                    ActiveState = item.ActiveState,
                                    MetaFieldId = metaFiledIdToUpdate,
                                    ValueOrder = item.ValueOrder,
                                    ValueText = item.ValueText
                                };

                                var responseUpdate = await HttpClient!.PostAsJsonAsync(" paramsdocumentary/MetaValues/CreateMetaValues", newMetaValue);
                                var deserializeResponseUpdate = await responseUpdate.Content.ReadFromJsonAsync<HttpResponseWrapperModel<MetaValuesDtoResponse>>();
                                if (!deserializeResponseUpdate!.Succeeded)
                                {
                                    notificationModal.UpdateModal(ModalType.Error, "Error al eliminar los meta valores asociados", true, "Aceptar");
                                }
                            }
                        }

                        notificationModal.UpdateModal(ModalType.Success, deserializeResponse.Message!, true, "Aceptar");
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, deserializeResponse.Message!, true, "Aceptar");
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, "Uno o mas campos son necesarios", true, "Aceptar");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "Error at craeting data", true, "Aceptar");
            }
        }

        #endregion HandleFormUpdate

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region GetFieldTypeCode

        public void GetFieldTypeCode(string code)
        {
            try
            {
                metaFieldRequest.FieldType = code;

                if (code.Equals("FTY,19"))
                {
                    showPanel = "";
                }
                else
                {
                    showPanel = "d-none";
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "Error al seleeccionar el tipo", true, "Aceptar");
            }
        }

        #endregion GetFieldTypeCode

        private void OnChangeSwitch()
        {
            active = !active;
        }

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)

        {
            isEditForm = false;
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region ResetFrom

        public void ResetForm()
        {
            fieldTypeCodeModal = "";

            isAnonymous = false;

            active = false;
            metaFieldRequest.FieldType = "";
            metaFieldRequest.Anonymization = false;
            metaFieldRequest.Mandatory = false;
            metaFieldRequest.TopographicLocation = false;
            metaFieldRequest = new();
            metaValueCreateList = new();
            orderString = "";
            valueString = "";
        }

        #endregion ResetFrom

        #region GetFieldType

        private async Task GetFieldType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "FTY");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data != null || deserializeResponse.Data?.Count != 0 ))
                {
                    systemParamListModal = deserializeResponse.Data ?? new();
                }
                else
                {
                    systemParamListModal = new();
                }
            }
            catch
            {
                systemParamListModal = new();
            }
        }

        #endregion GetFieldType

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await OnStatusChanged.InvokeAsync(false);
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #region AddToList

        private void AddToList()
        {
            try
            {
                if (orderInput.IsInputValid && valueInput.IsInputValid)
                {
                    var itemToAdd = new MetaValueCreateDtoRequest()
                    {
                        ActiveState = true,
                        ValueOrder = int.Parse(orderString),
                        ValueText = valueString
                    };

                    if (isEditForm)
                    {
                        listOfMetaValuesToUpdate.Add(itemToAdd);
                    }

                    var copy = metaValueCreateList;
                    copy.Add(itemToAdd);
                    metaValueCreateList = copy;
                    metaValueCreateList = metaValueCreateList.OrderBy(x => x.ValueOrder).ToList();
                }

                StateHasChanged();
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, "Error al añadir el meta valor", true, "Aceptar");
            }
        }

        #endregion AddToList

        #region DeleteToList

        private void DeleteToList(MetaValueCreateDtoRequest request)
        {
            try
            {
                if (isEditForm)
                {
                    listOfMetaValuesToDelete.Add(request);
                }

                metaValueCreateList.Remove(request);
                metaValueCreateList = metaValueCreateList.OrderBy(x => x.ValueOrder).ToList();

                StateHasChanged();
            }
            catch { notificationModal.UpdateModal(ModalType.Error, "Error al remover el meta valor", true, "Aceptar"); }
        }

        #endregion DeleteToList

        #region UpdateMetaData

        public void UpdateMetaData(MetaFieldsDtoResponse data)
        {
            isAnonymous = ( data.Anonymization ?? false );
            topograhpy = ( data.TopographicLocation ?? false );
            mandatory = ( data.Mandatory ?? false );
            active = ( data.ActiveState );
            metaFieldRequest = new()
            {
                ActiveState = data.ActiveState,
                Anonymization = data.Anonymization,
                Code = data.Code,

                FieldType = data.FieldType,
                Mandatory = data.Mandatory,
                NameMetaField = data.NameMetaField,

                TopographicLocation = data.TopographicLocation
            };

            List<MetaValueCreateDtoRequest> existingMetaValues = new();

            foreach (var item in data.MetaValues ?? new())
            {
                existingMetaValues.Add(
                    new MetaValueCreateDtoRequest()
                    {
                        ActiveState = item.ActiveState,
                        MetaFieldId = item.MetaFieldId,
                        MetaValueId = item.MetaValueId,
                        ValueOrder = item.ValueOrder,
                        ValueText = item.ValueText
                    });
            }
            existingMetaValues.OrderBy(x => x.ValueOrder);

            metaFieldRequest.MetaValues = existingMetaValues;
            metaValueCreateList = existingMetaValues;

            metaFiledIdToUpdate = data.MetaFieldId;
            isEditForm = true;

            GetFieldTypeCode(data.FieldType);
        }

        #endregion UpdateMetaData

        #region HandleCheckBoxes

        private void HandleCheckBoxes(bool newValue, int checkBoxCase)
        {
            switch (checkBoxCase)
            {
                case 1:
                    metaFieldRequest.Anonymization = newValue;
                    isAnonymous = newValue;
                    break;

                case 2:
                    metaFieldRequest.Mandatory = newValue;
                    mandatory = newValue;
                    break;

                case 3:
                    metaFieldRequest.TopographicLocation = newValue;
                    topograhpy = newValue;
                    break;
            }
        }

        #endregion HandleCheckBoxes

        #endregion Methods
    }
}