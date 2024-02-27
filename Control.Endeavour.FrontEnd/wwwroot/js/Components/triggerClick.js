window.triggerClick = (elementId) => {
    document.getElementById(elementId).click();
};

window.initializeFilePaste = (dropContainer, inputFile) => {
    function onPaste(event) {
        inputFile.files = event.clipboardData.files;
        const changeEvent = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(changeEvent);
    }

    function onDragOver(event) {
        event.preventDefault();
    }

    function onDrop(event) {
        event.preventDefault();
        inputFile.files = event.dataTransfer.files;
        const changeEvent = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(changeEvent);
    }

    dropContainer.addEventListener('paste', onPaste);
    dropContainer.addEventListener('dragover', onDragOver);
    dropContainer.addEventListener('drop', onDrop);

    return {
        dispose: () => {
            dropContainer.removeEventListener('paste', onPaste);
            dropContainer.removeEventListener('dragover', onDragOver);
            dropContainer.removeEventListener('drop', onDrop);
        }
    };
};
