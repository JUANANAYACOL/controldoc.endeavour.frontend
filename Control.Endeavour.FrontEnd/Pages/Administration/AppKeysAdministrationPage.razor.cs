﻿using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class AppKeysAdministrationPage
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


        #endregion

        #region Modals

        private AppKeysModal modalAppKeys = new();
        private NotificationsComponentModal notificationModal = new();
        #endregion

        #region Parameters


        #endregion

        #region Models
        private AppKeysDtoResponse recordToDelete = new();
        #endregion

        #region Environments

        #region Environments(String)
        private string Panel1Class = "col-md-12";
        private string Panel2Class = "d-none";
        private string FunctionName = string.Empty;
        #endregion

        #region Environments(Numeric)
        private int pageSizeAppFunctions { get; set; }
        private int pageSizeAppKeys { get; set; }
        private int appFunctionId { get; set; }
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)
        private List<AppFunctionDtoResponse> LstAppFunctions = new ();
        private List<AppKeysDtoResponse> LstAppKeys = new ();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			
            try
            {
                EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetAppFunctions();

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
        private async Task HandleRefreshGridDatasystemFieldsAsync(bool refresh)
        {
            await GetAppKeys(FunctionName);
        }
        
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.AppKeyId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/DeleteAppKey", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await GetAppKeys(FunctionName);
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true);
                    }

                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }

        private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region OthersMethods

        #region Get Data Methods

        private async Task GetAppFunctions()
        {
            try
            {

                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AppFunctionDtoResponse>>>("params/AppFunction/ByFilter");
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Any())
                {
                    LstAppFunctions = deserializeResponse.Data;
                    pageSizeAppFunctions = LstAppFunctions.Count();
                    
                }
                else
                {
                    pageSizeAppKeys = new();
                    notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de cargar las versiones documentales, por favor intente de nuevo!", true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        private async Task GetAppKeys(string FunctionName)
        {
            try
            {
                AppKeysFilterDtoRequest appKeysFilter = new();
                appKeysFilter.FunctionName = FunctionName;
                var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Any())
                {
                    LstAppKeys = deserializeResponse.Data;
                    pageSizeAppKeys = LstAppKeys.Count();
                }
                else
                {
                    LstAppKeys = new();
                    notificationModal.UpdateModal(ModalType.Error, "La llave " + FunctionName + "no tiene valores", true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
            
        }

        #endregion

        #region Action Methods

        private async Task ShowAppKeys(AppFunctionDtoResponse record)
        {
            FunctionName = record.FunctionName;
            appFunctionId = record.AppFunctionId;

            await GetAppKeys(FunctionName);

            Panel1Class = "col-md-6";
            Panel2Class = "";

        }
        private async Task ShowModalAppkeys(AppKeysDtoResponse record)
        {
            modalAppKeys.ReceiveRecord(record);
            modalAppKeys.UpdateModalStatusAsync(true);

        }
        private async Task ShowModal()
        {
            modalAppKeys.UpdateModalStatusAsync(true);
            modalAppKeys.AppFunctionId(appFunctionId);

        }
        private void ShowModalDeleteAppkeys(AppKeysDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar el registro?", true, "Si", "No", modalOrigin: "DeleteModal");
        }

        #endregion

        #endregion

        #endregion

    }
}
