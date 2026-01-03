const generateButton = document.getElementById("generate-button");
const editButton = document.getElementById("edit-button");
const createButton = document.getElementById("create-button");
const dataFileInput = document.getElementById("data-file-input");

document.addEventListener("DOMContentLoaded", async function () {
    
    generateButton.disabled = true;
    editButton.disabled = true;

    // Link file input with the open button
    dataFileInput.oninput = _ => {
        const hasData = dataFileInput.files !== null && dataFileInput.files.length > 0;
        generateButton.disabled = !hasData;
        editButton.disabled = !hasData;
    }

    // Link buttons
    generateButton.onclick = async _ => {
        
        localStorage.setItem('json', await new Response(dataFileInput.files[0]).text());
        window.open("./Viewer/index.html");
        
    }

    editButton.onclick = async function(_) {
        
        localStorage.setItem('json', await new Response(dataFileInput.files[0]).text());
        window.open("./Generator/index.html");
        
    }

    createButton.onclick = _ => {
        
        localStorage.setItem('json', "");
        dataFileInput.fileInput = null;

        window.open("./Generator/index.html");
        
    };
})