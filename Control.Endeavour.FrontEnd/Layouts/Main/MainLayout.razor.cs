using Control.Endeavour.Frontend.Client.Models.ComponentViews.Menu.Request;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Views.Menu;
using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Components.Components.User;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Control.Endeavour.FrontEnd.Services.Services.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.SvgIcons;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Captcha;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Control.Endeavour.FrontEnd.Models.Models.Menu.Request;
using System;

namespace Control.Endeavour.FrontEnd.Layouts.Main
{
    public partial class MainLayout
    {

        #region Variables

        #region Inject 
        [Inject] private EventAggregatorService? EventAggregator { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IJSRuntime Js { get; set; }
        [Inject] private RenewTokenService RenewToken { get; set; }
        [Inject] private ISessionStorage SessionStorage { get; set; }
        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private IAuthenticationJWT? AuthenticationJWT { get; set; }
        [Inject] private AuthenticationStateContainer authenticationStateContainer { get; set; }
        [Inject] private FilingStateContainer? FilingSC { get; set; }
        [Inject] private IConfiguration? Configuration { get; set; }


        #endregion

        #region Parameters
        [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }

        #endregion

        #region Models

        private SubMenu subMenuInstance = new();
        private NotificationsComponentModal notificationModal = new();
        //public SpinnerCargandoComponent spinerLoader = new();

        #endregion

        #region Environments

        #region Environments(String)

        string navbarCollapseCss => isNavbarCollapsed ? "collapse navbar-collapse" : "navbar-collapse";
        string DropdownMenuCss => isDropdownOpen ? "dropdown-menu show" : "dropdown-menu";

        private string subDefault = "";
        private string subNavFiling = "d-none";
        private string subNavManagement = "d-none";
        private string subNavDocumentaryTasks = "d-none";
        private string subNavRecord = "d-none";
        private string subNavSearchers = "d-none";
        private string version { get; set; } = string.Empty;

        #region QuickAccessMenu

        private string HomePage = "Home";
        private string FilingPage = "Filing";
        private string MassiveFiling = "MassiveFiling";
        private string CreateDocumentaryTaskPage = "CreateDocumentaryTask";
        private string DocumentaryTaskTrayPage = "DocumentaryTaskTray";
        private string ManagementTrayPage = "ManagementTray";

        // <--[Iconos del Menu]-->

        private string _default = "";
        private string body_container = "bc";
        private string IconMenu = "../img/menu/iconMenu.svg";
        private string HomeImage = "../img/menu/inicio.svg";

        #region Acciones Radicacion
        string FilingImage = "../img/menu/radicacion.svg";
        string FilingReceived = "../img/menu/ventanillaRecibida.svg";
        string FilingInternal = "../img/menu/ventanillaInterna.svg";
        string Filingsent = "../img/menu/ventanillaEnviada.svg";
        string FilingUnofficial = "../img/menu/ventanillaNoOficial.svg";
        string FilingFaster = "../img/menu/ventanillaRapida.svg";
        string MassiveRadiation = "../img/menu/ventanillaRapida.svg";
        #endregion

        #region Gestion
        string ManagementImage = "../img/menu/gestion.svg";
        string ManagementTray = "../img/menu/gestionsubItem.svg";
        string ManagementBoard = "../img/menu/tableroControl.svg";
        #endregion

        #region Tareas Documentales
        string DocumentaryTasksImage = "../img/menu/tareasDocumentales.svg";
        string DocumentaryTasksEditor = "../img/menu/editorTexto.svg";
        string DocumentaryTasksSpreadsheet = "../img/menu/hojaCalculo.svg";
        string DocumentaryTasksTray = "../img/menu/bandejaTareasSubItem.svg";
        #endregion

        #region Expedientes
        string RecordImage = "../img/menu/expedientes.svg";
        string RecordConsult = "../img/menu/consultaExpediente.svg";
        string RecordAdministration = "../img/menu/administraacionExp.svg";
        string RecordRequest = "../img/menu/solicitudPrestamo.svg";
        #endregion

        #region Buscadores
        string SearchersImage = "../img/menu/buscadores.svg";
        string SearchersConsult = "../img/menu/consultaDocumentos.svg";
        string SearchersSearch = "../img/menu/busquedaRapida.svg";
        #endregion

        string BpmImage = "../img/menu/bpm.svg";
        string EnvironmentalImpactImage = "../img/menu/impactoAmbiental.svg";
        string LogoutImage = "../img/menu/cerrarSesion.svg";

        #endregion

        #endregion

        #region Environments(Numeric)

        private int activeIndex = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        public bool radicacipnActiva { get; set; } = false;
        private bool isNavbarCollapsed = true;
        private bool isDropdownOpen = false;
        private bool darkMode = false;
        private bool showNewMenu = false;

        #endregion

        #region Environments(List & Dictionary)

        // Lista de Menus de la BD
        private List<MenuModels> MenusModels { get; set; } = new List<MenuModels>();

        // Diccionario que agrupa los diccionarios anteriores por nivel.
        private Dictionary<int, Dictionary<int, bool>> expandedItemsByLevel = new Dictionary<int, Dictionary<int, bool>>
        {
            { 1, new Dictionary<int, bool>() },
            { 2, new Dictionary<int, bool>() },
            { 3, new Dictionary<int, bool>() },
            { 4, new Dictionary<int, bool>() },
        };
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            version = Configuration["version"].ToString();



            var authState = await AuthenticationState;
            if (!authState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("");
            }
            else
            {
                EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetMenus();
                var timeExpiration = await SessionStorage.GetValue<string>(ValuesKeysEnum.TimeExpiration);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timeExpiration));
                var tiempoExpiracionString = dateTimeOffset.LocalDateTime.ToString();
                var timeExpirationMinute = (DateTime.Parse(tiempoExpiracionString) - DateTime.Now).Minutes;
                RenewToken.Start(timeExpirationMinute);
                await Js.InvokeVoidAsync("checkTheme"); // Verificar Theme almacenado en el localStorage               
                
            }

		}


		#endregion

		#region Methods

		#region HandleMethods
		private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.ModalOrigin == "CerrarSesion")
            {
                if (args.IsAccepted)
                {
                    var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<bool>>("security/Session/UpdateLogout");

                    if (response.Succeeded)
                    {
                        await AuthenticationJWT.LogoutToken();
                        NavigationManager?.NavigateTo("");
                        authenticationStateContainer.SelectedComponentChanged("Login");
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error al cerrar la sesión!", true, "Aceptar", "", "", "");
                    }
                }
            }else if (args.ModalOrigin.Contains("~"))
            {
                if (args.IsAccepted)
                {
                    var valores = args.ModalOrigin.Split('~');

                    NavigationManager.NavigateTo("/" + valores[0]);
                    ModificarSubnavItem(ref valores[1]);
                }
            }
        }

        void ModificarSubnavItem(ref string subnavItem)
        {
            subnavItem = "d-none";
        }

        /// <summary>
        /// Verifica si un menu tiene submenus/opciones de nivel 1 o si esta activo.
        /// </summary>
        /// <param name="menu">Objeto de tipo Menu</param>
        /// <returns>True si el MenuItems1s del menu no esta vacio</returns>
        private bool HasSubMenuItems(MenuModels menu)
        {
            return menu.MenuItems1s != null;
        }

        /// <summary>
        /// Verifica si un submenú de nivel 1 tiene una vista, si no es asi significa que tiene opciones.
        /// </summary>
        /// <param name="menuItem1">Objeto de tipo menuItem1</param>
        /// <returns>True si el menuItem1 tiene una vista null</returns>
        private bool HasSubMenuItems(MenuItems1 menuItem1)
        {
            return menuItem1.View == null;
        }

        /// <summary>
        /// Verifica si un submenu de nivel 2 tiene una vista, si no es asi significa que tiene opciones.
        /// </summary>
        /// <param name="menuItem2">Objeto de tipo menuItem2</param>
        /// <returns>True si el menuItem2 tiene una vista null</returns>
        private bool HasSubMenuItems(MenuItems2 menuItem2)
        {
            return menuItem2.View == null;
        }

        #endregion

        #region GetMethods
        private async Task GetMenus()
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<MenuModels>>>("access/Access/ByFilter");

                if (response.Succeeded)
                {
                    MenusModels = response.Data;
                }
                else
                {
                    Console.WriteLine("No hay menus disponibles");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles de usuario: {ex.Message}");
            }
        }

        #endregion

        #region PostMethods

        private async Task SignOff()
        {
            try
            {
                notificationModal.UpdateModal(ModalType.Information, "¿Está seguro que quiere cerrar sesión?", true, "Si", "No", modalOrigin: "CerrarSesion");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }

        #endregion

        #region ToggleMethods
        void ToggleNavbar()
        {
            isNavbarCollapsed = !isNavbarCollapsed;
        }

        void ToggleDropdown()
        {
            isDropdownOpen = !isDropdownOpen;
        }

        private void ToggleNewMenu()
        {
            showNewMenu = !showNewMenu;
            CloseAllMenus();
        }

        /// <summary>
        /// Verifica si esta expandido y cierra otros niveles
        /// </summary>
        /// <param name="id">El id del menu o submenu</param>
        /// <param name="level">El nivel en el que se desea verificar la expansion</param>
        private void ToggleSubMenu(int id, int level, string NameView, List<ViewParameters> parameters = null)
        {
            if (!string.IsNullOrEmpty(NameView))
            {
                if (parameters != null && parameters.Count > 0)
                {
                    string _parameters = parameters.First().Value;
                    NavigationManager.NavigateTo("/" + NameView+"/{"+_parameters+"}");
                    showNewMenu = false;
                }
                else
                {
                    NavigationManager.NavigateTo("/" + NameView);
                    showNewMenu = false;
                }
            }
            // Antes de abrir un nuevo menu, cierra los menus previamente abiertos.
            CloseMenusInSameOrLowerLevel(id, level);
            // Cambia el estado de expansion del menú seleccionado.
            ToggleExpansion(id, level);
        }

        /// <summary>
        /// Cambia el estado de expansion del menu en el nivel especificado.
        /// </summary>
        /// <param name="id">El id del menu o submenu</param>
        /// <param name="level">El nivel en el que se desea verificar la expansion</param>
        private void ToggleExpansion(int id, int level)
        {
            for (int i = 1; i <= level; i++)
            {
                if (!expandedItemsByLevel.ContainsKey(i))
                {
                    expandedItemsByLevel[i] = new Dictionary<int, bool>();
                }
            }
            if (expandedItemsByLevel.TryGetValue(level, out var expandedItems))
            {
                if (expandedItems.ContainsKey(id))
                {
                    expandedItems[id] = !expandedItems[id];
                }
                else
                {
                    expandedItems[id] = true;
                }
            }
        }

        async Task ToggleTheme()
        {
            darkMode = await Js.InvokeAsync<bool>("toggleTheme");
            changeImage(darkMode);
        }

        #endregion

        #region OthersMethods
        void SetActive(int index)
        {
            activeIndex = index;
        }

        /// <summary>
        /// Verifica si un menu esta expandido en un nivel determinado (menu principal o subniveles de este)
        /// </summary>
        /// <param name="id">El id del menu o submenu</param>
        /// <param name="level">El nivel en el que se desea verificar la expansion</param>
        /// <returns>Si el menu o submenu está expandido en el nivel especificado sera True; de lo contrario, false.</returns>
        private bool IsExpanded(int id, int level)
        {
            if (expandedItemsByLevel.TryGetValue(level, out var expandedItems))
            {
                return expandedItems.ContainsKey(id) && expandedItems[id];
            }

            return false;
        }


        /// <summary>
        /// Cierra los menús y niveles anteriormente abiertos.
        /// </summary>
        /// <param name="id">El id del menu o submenu</param>
        /// <param name="level">El nivel en el que se desea verificar la expansion</param>
        public void CloseMenusInSameOrLowerLevel(int id, int level)
        {

            // Cerrar todos los menús en niveles iguales o inferiores.
            foreach (var kvp in expandedItemsByLevel)
            {
                var menuLevel = kvp.Key;
                var expandedItems = kvp.Value;

                if (menuLevel >= level)
                {
                    foreach (var key in expandedItems.Keys.ToList())
                    {
                        if (key != id && expandedItems[key])
                        {
                            expandedItems[key] = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cierra todos los menus.
        /// </summary>
        public void CloseAllMenus()
        {
            foreach (var expandedItems in expandedItemsByLevel.Values)
            {
                foreach (var key in expandedItems.Keys.ToList())
                {
                    if (expandedItems[key])
                    {
                        expandedItems[key] = false;
                    }
                }
            }
        }

        private void OnNavitigionMenu(ref string subnavItem, string Page, string Parameters = "")
        {
            //PageLoadService.MostrarSpinnerReadLoad(JSRuntime);

            if (FilingSC.ActiveFiling == true)
            {
                notificationModal.UpdateModal(ModalType.Warning, "Actualmente, se está realizando una radicación. ¿Está seguro(a) que desea terminar el proceso?", true, "Si", "No",modalOrigin: Page + "~"+ subnavItem + "~ActiveFiling");
                subnavItem = "d-none";
            }
            else
            {
                if (!string.IsNullOrEmpty(Parameters))
                {
                    NavigationManager.NavigateTo("/" + Page + "/{" + Parameters + "}");
                    subnavItem = "d-none";
                }
                else
                {
                    NavigationManager.NavigateTo("/" + Page);
                    subnavItem = "d-none";
                }
            }
        }

        private void OnCloseImageBox(ref string image, ref string subnavItem)
        {
            image = image.Contains("Hover.svg") ? image.Replace("Hover.svg", ".svg") : image;
            subnavItem = "d-none";
        }

        private void changeImage(bool darkMode)
        {
            if (darkMode)
            {
                #region Iconos SubMenu

                IconMenu = "../img/menu/iconMenuWhite.svg";
                HomeImage = "../img/menu/inicioWhite.svg";

                #region Acciones Radicacion
                FilingImage = "../img/menu/radicacionWhite.svg";
                FilingReceived = "../img/menu/ventanillaRecibidaWhite.svg";
                FilingInternal = "../img/menu/ventanillaInternaWhite.svg";
                Filingsent = "../img/menu/ventanillaEnviadaWhite.svg";
                FilingUnofficial = "../img/menu/ventanillaNoOficialWhite.svg";
                FilingFaster = "../img/menu/ventanillaRapidaWhite.svg";
                MassiveRadiation = "../img/menu/ventanillaRapidaWhite.svg";

                #endregion

                #region Gestion
                ManagementImage = "../img/menu/gestionWhite.svg";
                ManagementTray = "../img/menu/gestionsubItemWhite.svg";
                ManagementBoard = "../img/menu/tableroControlWhite.svg";
                #endregion

                #region Tareas Documentales
                DocumentaryTasksImage = "../img/menu/tareasDocumentalesWhite.svg";
                DocumentaryTasksEditor = "../img/menu/editorTextoWhite.svg";
                DocumentaryTasksSpreadsheet = "../img/menu/hojaCalculoWhite.svg";
                DocumentaryTasksTray = "../img/menu/bandejaTareasSubItemWhite.svg";
                #endregion

                #region Expedientes
                RecordImage = "../img/menu/expedientesWhite.svg";
                RecordConsult = "../img/menu/consultaExpedienteWhite.svg";
                RecordAdministration = "../img/menu/administraacionExpWhite.svg";
                RecordRequest = "../img/menu/solicitudPrestamoWhite.svg";
                #endregion

                #region Buscadores
                SearchersImage = "../img/menu/buscadoresWhite.svg";
                SearchersConsult = "../img/menu/consultaDocumentosWhite.svg";
                SearchersSearch = "../img/menu/busquedaRapidaWhite.svg";
                #endregion

                BpmImage = "../img/menu/bpmWhite.svg";
                EnvironmentalImpactImage = "../img/menu/impactoAmbientalWhite.svg";
                LogoutImage = "../img/menu/cerrarSesionWhite.svg";

                #endregion
            }
            else
            {
                #region Iconos SubMenu

                IconMenu = "../img/menu/iconMenu.svg";
                HomeImage = "../img/menu/inicio.svg";

                #region Acciones Radicacion
                FilingImage = "../img/menu/radicacion.svg";
                FilingReceived = "../img/menu/ventanillaRecibida.svg";
                FilingInternal = "../img/menu/ventanillaInterna.svg";
                Filingsent = "../img/menu/ventanillaEnviada.svg";
                FilingUnofficial = "../img/menu/ventanillaNoOficial.svg";
                FilingFaster = "../img/menu/ventanillaRapida.svg";
                MassiveRadiation = "../img/menu/ventanillaRapida.svg";

                #endregion

                #region Gestion
                ManagementImage = "../img/menu/gestion.svg";
                ManagementTray = "../img/menu/gestionsubItem.svg";
                ManagementBoard = "../img/menu/tableroControl.svg";
                #endregion

                #region Tareas Documentales
                DocumentaryTasksImage = "../img/menu/tareasDocumentales.svg";
                DocumentaryTasksEditor = "../img/menu/editorTexto.svg";
                DocumentaryTasksSpreadsheet = "../img/menu/hojaCalculo.svg";
                DocumentaryTasksTray = "../img/menu/bandejaTareasSubItem.svg";
                #endregion

                #region Expedientes
                RecordImage = "../img/menu/expedientes.svg";
                RecordConsult = "../img/menu/consultaExpediente.svg";
                RecordAdministration = "../img/menu/administraacionExp.svg";
                RecordRequest = "../img/menu/solicitudPrestamo.svg";
                #endregion

                #region Buscadores
                SearchersImage = "../img/menu/buscadores.svg";
                SearchersConsult = "../img/menu/consultaDocumentos.svg";
                SearchersSearch = "../img/menu/busquedaRapida.svg";
                #endregion

                BpmImage = "../img/menu/bpm.svg";
                EnvironmentalImpactImage = "../img/menu/impactoAmbiental.svg";
                LogoutImage = "../img/menu/cerrarSesion.svg";

                #endregion
            }
        }

        #endregion

        #region FXMethods

        private void OnMouseOverWithSubItem(ref string image, ref string subnavItem, int activeIndex)
        {
            if (!string.IsNullOrEmpty(image))
            {
                if (subnavItem == "bc")
                {
                    showNewMenu = showNewMenu ? !showNewMenu : showNewMenu;
                }
                else
                {
                    image = !image.Contains("White.svg") ? !image.Contains("Hover.svg") ? image.Replace(".svg", "Hover.svg") : image : image.Replace("White.svg", "Hover.svg");
                    SetActive(activeIndex);
                }
            }

            switch (activeIndex)
            {
                case 0:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;
                case 1:

                    subNavFiling = "";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;
                case 2:

                    subNavFiling = "d-none";
                    subNavManagement = "";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;
                case 3:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;
                case 4:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "";
                    subNavSearchers = "d-none";

                    break;
                case 5:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "";

                    break;
                case 6:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;
                case 7:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;
                case 8:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;
            }
        }
        private void OnMouseOutWithSubItem(ref string image, ref string subnavItem)
        {
            if (darkMode)
            {
                image = image.Replace("Hover.svg", "White.svg");
            }
            else
            {
                image = image.Replace("Hover.svg", ".svg");
            }
        }
        private void OnMouseOver(ref string image, int activeIndex)
        {
            if (darkMode)
            {
                image = image.Contains("White.svg") ? image.Replace("White.svg", "Hover.svg") : image.Replace(".svg", "Hover.svg");
            }
            else
            {
                image = image.Replace(".svg", "Hover.svg");
            }
            //SetActive(activeIndex);
        }
        private void OnMouseOut(ref string image)
        {
            if (darkMode)
            {
                image = image.Replace("Hover.svg", "White.svg");
            }
            else
            {
                image = image.Replace("Hover.svg", ".svg");
            }
        }

        #endregion

        #endregion


    }
}
