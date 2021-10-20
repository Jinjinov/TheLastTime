// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}

export function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

export function getDimensions() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}

export function setElementProperty(element, property, value) {
    element[property] = value;
}

export async function readDirectoryFiles() {
    const dirHandle = await showDirectoryPicker();
    var directory = {
        kind: dirHandle.kind,
        name: dirHandle.name,
        nodes: []
    };
    await handleDirectoryEntry(dirHandle, directory);
    return directory;
};
export async function handleDirectoryEntry(dirHandle, directory) {
    for await (const handle of dirHandle.values()) {
        if (handle.kind === "file") {
            const file = await handle.getFile();
            const text = await file.text();
            var fileContent = {
                kind: handle.kind,
                name: handle.name,
                text: text
            };
            directory.nodes.push(fileContent);
        }
        if (handle.kind === "directory") {
            var subDirectory = {
                kind: handle.kind,
                name: handle.name,
                nodes: []
            };
            directory.nodes.push(subDirectory);
            await handleDirectoryEntry(handle, subDirectory);
        }
    }
}