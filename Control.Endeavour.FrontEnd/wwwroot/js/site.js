//Documentation https://www.daterangepicker.com/
window.initializeDateRangePicker = function (dotNetHelper) {
    $('input[name="daterange"]').daterangepicker({
        opens: 'left'
    }, function (start, end, label) {
        dotNetHelper.invokeMethodAsync('HandleDateRangeSelection', start.format('YYYY/MM/DD'), end.format('YYYY/MM/DD'), label);
    });
};

window.resetInactivityTimer = function (dotNetReference, miliseconds) {
    let inactivityTimer;

    resetTimer(dotNetReference);

    document.addEventListener("mousemove", () => handleUserInteraction(dotNetReference));
    document.addEventListener("keydown", () => handleUserInteraction(dotNetReference));

    function resetTimer(dotNetReference) {
        clearTimeout(inactivityTimer);

        //console.log("Tiempo de inactividad en segundos " + (miliseconds / 1000))
        inactivityTimer = setTimeout(() => logOutDueToInactivity(dotNetReference), miliseconds);
    }

    function handleUserInteraction(dotNetReference) {
        resetTimer(dotNetReference);
    }

    function logOutDueToInactivity(dotNetReference) {
        dotNetReference.invokeMethodAsync("LogOutDueToInactivity");
    }
};

//#region Dark-Mode
window.toggleTheme = () => {
    const body = document.body;
    const isDarkMode = body.classList.toggle('dark');
    const btnSwitch = document.querySelector('#switch');

    if (btnSwitch) {
        btnSwitch.classList.toggle('active', isDarkMode);
    }

    //Almacenar el modo en Localstorage
    localStorage.setItem('dark-theme', isDarkMode ? 'true' : 'false');

    return isDarkMode;
};

window.checkTheme = () => {
    if (localStorage.getItem('dark-theme') === 'true') {
        document.body.classList.add('dark');
    } else {
        document.body.classList.remove('dark');
    }
};

//#endregion

//Agregar Valores al Footer
window.changeFooter = (politicas, terminosyCondiciones) => {
    document.getElementById('Politicas').innerText = politicas;
    document.getElementById('TerminosyCondiciones').innerText = terminosyCondiciones;
};
window.DescargarArchivoBase64 = function (nombreArchivo, contenidoBase64) {
    try {
        var byteCharacters = atob(contenidoBase64);
        var byteNumbers = new Array(byteCharacters.length);
        for (var i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        var byteArray = new Uint8Array(byteNumbers);

        var blob = new Blob([byteArray], { type: 'application/octet-stream' });

        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = nombreArchivo;

        document.body.appendChild(link);

        link.click();

        document.body.removeChild(link);
        return true;
    } catch (error) {
        console.error('Error al descargar el archivo:', error.message);
        return false;
    }
}

window.printDiv = function (divId) {
    debugger;
    var content = document.getElementById(divId).innerHTML;
    var printWindow = window.open('', '', 'height=400,width=800');
    printWindow.document.write('<html><head><title>Imprimir</title></head><body>');
    printWindow.document.write(content);
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
}