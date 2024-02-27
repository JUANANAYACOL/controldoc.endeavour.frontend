using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Documents.OverrideTrayPage
{
    public partial class OverrideTrayAdminPage
    {

        #region Variables
        #region Inject 
        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components
        private NotificationsComponentModal notificationModalSucces;
        private NotificationsComponentModal notificationModal;

        #endregion

        #region Modals

        private OverrideTrayModal _ModalOverrideTray;
        private OverrideTrayValidationModal _ModalOverrideTrayValidation;
        #endregion

        #region Parameters

        #endregion

        #region Models

        #endregion

        #region Environments
        private List<OverrideTrayRequestDtoResponse> RequestList;
        private DonnaDtoResponse Dona = new();
        private OverrideTrayRequestDtoResponse recordToDelete;
        private List<DataCardDtoRequest> Data = new();

        private string Target = "TEA,PE";
        private int RequestId = 4055;

        private string CardPendiente;
        private string CardAnulados;
        private string CardDesanulados;
        private string CardRechazados;
        private string CardP;
        private string CardA;
        private string CardD;
        private string CardR;
        public int controlId { get; set; }
        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetRequest();
            await GetDonna();
            EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

        }


        #endregion

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion


        #region ShowModalEdit
        private async Task ShowModalEdit(OverrideTrayRequestDtoResponse args)
        {
            _ModalOverrideTray.UpdateModalStatus(true);
            _ModalOverrideTray.UpdateSelectedRecord(args);

        }
        #endregion

        #region ShowModal
        private async Task ShowModal()
        {
            _ModalOverrideTray.UpdateModalStatus(true);
        }
        #endregion

        #region ShowModalValidation
        private async Task ShowModalValidation()
        {
            _ModalOverrideTrayValidation.controlId = controlId;

            _ModalOverrideTrayValidation.UpdateModalStatus(true);
        }

        private void SendValidation(int data)
        {
            _ModalOverrideTrayValidation.UpdateModalStatus(true);
            controlId = data;
        }

        #endregion




        #region ShowModalDelete
        private void ShowModalDelete(OverrideTrayRequestDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, "¿Está seguro de eliminar la petición?", true, "Si", "No", modalOrigin: "DeleteModal");
        }
        #endregion

        #region HandleRefreshGridDataAsync
        public async Task HandleRefreshGridData(bool refresh)
        {
            await GetRequest();
        }
        #endregion

        #region HandleModalNotiClose
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
            {
                if (recordToDelete != null)
                {
                    DeleteGeneralDtoRequest DeleteRequest = new();
                    DeleteRequest.Id = recordToDelete.CancelationRequestId;
                    DeleteRequest.User = "Admin";

                    var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/DeleteCancelationRequest", DeleteRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true);
                        }
                    }
                    else
                    {
                        //Logica no Exitosa
                        notificationModalSucces.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el administrador, por favor intente de nuevo!", true, "Aceptar");
                    }
                    await HandleRefreshGridData(true);

                }
            }
            else
            {
                Console.WriteLine("Registro No eliminado");
            }


        }
        #endregion

        #region GetRequest
        private async Task GetRequest()
        {
            try
            {
                OverrideTrayRequestDtoResponse Request = new();
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/ByFilterCancelation", Request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayRequestDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    RequestList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<OverrideTrayRequestDtoResponse>();


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las peticiones: {ex.Message}");
            }
        }
        #endregion

        #region GetDonna
        private async Task GetDonna()
        {
            try
            {
                OverrideTrayRequestUserDtoRequest user = new();
                user.UserRequestId = RequestId;
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/ByfilterStatusUserId", user);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DonnaDtoResponse>>();
                if (deserializeResponse.Succeeded)
                {
                    Dona = deserializeResponse.Data;
                    CardAnulados = Dona.NombreAnulado;
                    CardA = Dona.Anulado.ToString();
                    CardDesanulados = Dona.NombreDesanulado;
                    CardD = Dona.Desanulado.ToString();
                    CardRechazados = Dona.NombreRechazado;
                    CardR = Dona.Rechazado.ToString();
                    CardPendiente = Dona.NombrePendiente;
                    CardP = Dona.Pendiente.ToString();
                    DataCardDtoRequest Anulado = new()
                    {
                        Category = Dona.NombreAnulado,
                        Value = Dona.Anulado,
                        color = "#82A738"
                    };
                    DataCardDtoRequest Desanulado = new()
                    {
                        Category = Dona.NombreDesanulado,
                        Value = Dona.Desanulado,
                        color = "#41BAEA"
                    };
                    DataCardDtoRequest Rechazado = new()
                    {
                        Category = Dona.NombreRechazado,
                        Value = Dona.Rechazado,
                        color = "#EAD519"
                    };
                    DataCardDtoRequest Pendiente = new()
                    {
                        Category = Dona.NombrePendiente,
                        Value = Dona.Pendiente,
                        color = "#AB2222"
                    };
                    Data.Add(Anulado);
                    Data.Add(Desanulado);
                    Data.Add(Rechazado);
                    Data.Add(Pendiente);
                }
                else
                {
                    Console.WriteLine($"Error al obtener los datos de la donna");


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos de la donna: {ex.Message}");
            }
        }


        #endregion

        #region MetodosCard
        private async Task HandleClickCardPendiente(bool newvalue)
        {

            Target = "TEA,PE";

        }
        private async Task HandleClickCardAnulado(bool newvalue)
        {

            Target = "TEA,AN";

        }
        private async Task HandleClickCardDesanulado(bool newvalue)
        {

            Target = "TEA,DESAN";

        }
        private async Task HandleClickCardRechazado(bool newvalue)
        {

            Target = "TEA,RE";

        }
        #endregion


        #endregion

    }
}
