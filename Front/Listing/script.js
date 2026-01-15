class SetNameDto {
    constructor(cvId, name) {
        if (!isNumeric(cvId))
            throw new Error("Id must be a number");

        if (!isString(name))
            throw new Error("Name must be a string");

        if (name.trim() === "")
            throw new Error("Name can't be empty");

        this.cvId = cvId;
        this.name = name;
    }
}

const cvsParent = document.getElementById("cvs");
const alert = document.getElementById("alert");
const createButton = document.getElementById("create_button");
const cvItemTemplate = document.getElementById("cv-item_template");


function addCv(cv) {
    if (!isString(cv.name) || cv.name.trim() === "")
        throw new Error("Name can't be empty")

    const templateClone = document.importNode(cvItemTemplate.content, true).children[0];
    const children = templateClone.children;
    children[0].value = cv.name;
    children[0].onchange = () => {
        if (!isString(children[0].value) || children[0].value.trim() === "") {
            children[0].value = cv.name;
            return;
        }

        SendRequest("POST", document.cookie, null,
            APILink + "Cv/SetName", new SetNameDto(cv.id, children[0].value),
            () => {
            },
            res => {
                children[0].value = cv.name
                alert.textContent = res;
            });
    }
    children[1].onclick = () => {
        sessionStorage.setItem(CvIdItemKey, cv.id);
        location.assign("../Generator/index.html");
    }
    children[2].onclick = () => {
        if (confirm("Are you sure you want to delete this CV?")) {
            const parameters = [];
            parameters.push(new KeyPairValue("id", cv.id));

            SendRequest("DELETE", document.cookie, parameters,
                APILink + `Cv/Delete/`, null,
                _ => templateClone.remove(),
                res => alert.textContent = res);
        }
    }

    cvsParent.appendChild(templateClone);
}

function reloadCvs() {
    SendRequest("GET", document.cookie, null, APILink + "Cv/GetAll", null,
        res => 
        {
            cvsParent.innerHTML = "";
  
            const cvs = JSON.parse(res);

            for (const cv of cvs)
                addCv(cv);

        }, res => alert.textContent = res);
}

document.addEventListener("DOMContentLoaded", async function () {

    if (!checkIsLogged()) {
        location.assign("../index.html")
        return;
    }

    // Load all the cvs
    reloadCvs();

    const parameters = [];
    parameters.push(new KeyPairValue("name", "CV"));
    createButton.onclick = _ => {
        SendRequest("PUT", document.cookie, parameters,
            APILink + "Cv/Create", null,
            _ => reloadCvs(),
            res => alert.textContent = res)
    };
})