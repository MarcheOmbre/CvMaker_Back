class Contact {
    constructor(type, value) {
        this.type = type;
        this.value = value;
    }

}

class Link {
    constructor(name, value) {
        this.name = name;
        this.value = value;
    }
}

class WorkBloc {
    constructor(name, corporation, fromDate, toDate, description) {
        this.name = name;
        this.corporation = corporation;
        this.fromDate = fromDate;
        this.toDate = toDate;
        this.description = description;
    }
}

class EducationBloc {
    constructor(name, date) {
        this.name = name;
        this.date = date;
    }
}

class Project {
    constructor(name, date, description) {
        this.name = name;
        this.date = date;
        this.description = description;
    }
}

class Language {
    constructor(name, level) {
        this.name = name;
        this.level = level;
    }
}

class SetContentDto {
    constructor(id, content) {

        if (!isNumeric(id))
            throw new Error("Id must be numeric");

        if (!isString(content))
            throw new Error("Content must be string");

        this.cvId = id;
        this.content = content;
    }
}


let systemJson;
const systemLanguageSelect = document.getElementById("system-language");
const photoInput = document.getElementById("photo-input");
const photoReader = document.getElementById("photo-reader");
const nameInput = document.getElementById("name_input");
const professionInput = document.getElementById("profession_input");
const aboutMeInput = document.getElementById("about-me_input");
const contactsDiv = document.getElementById("contacts");
const linksDiv = document.getElementById("links");
const skillsDiv = document.getElementById("skills");
const worksDiv = document.getElementById("works");
const educationsDiv = document.getElementById("educations");
const languagesDiv = document.getElementById("languages");
const projectsDiv = document.getElementById("projects");
const hobbiesDiv = document.getElementById("hobbies");
const alertElement = document.getElementById("alert");
const frame = document.getElementById("preview");

const encapsulationArrowTemplate = document.getElementById("encapsulation-arrow_template");
const contactItemTemplate = document.getElementById("contact-item_template");
const linkItemTemplate = document.getElementById("link-item_template");
const skillItemTemplate = document.getElementById("skill-item_template");
const workItemTemplate = document.getElementById("work-item_template");
const educationItemTemplate = document.getElementById("education-item_template");
const languageItemTemplate = document.getElementById("language-item_template");
const projectItemTemplate = document.getElementById("project-item_template");
const hobbyItemTemplate = document.getElementById("hobby-item_template");


async function importFromJson(dataJson) {

    document.body.style.display = "none";

    // Clear the fields
    contactsDiv.innerHTML = "";
    linksDiv.innerHTML = "";
    skillsDiv.innerHTML = "";
    worksDiv.innerHTML = "";
    educationsDiv.innerHTML = "";
    languagesDiv.innerHTML = "";
    projectsDiv.innerHTML = "";
    hobbiesDiv.innerHTML = "";

    // Restore the fields
    const contentJson = JSON.parse(dataJson.content) ?? "";
    
    if (contentJson) {

        fetch(contentJson.SystemLanguage).then((response) => {

            systemJson = response.json().then(json => {

                systemJson = json
                
                if (isString(contentJson.Name))
                    nameInput.value = contentJson.Name;

                if (isString(contentJson.Profession))
                    professionInput.value = contentJson.Profession;

                if (isString(contentJson.AboutMe))
                    aboutMeInput.value = contentJson.AboutMe;

                if (contentJson.Contacts)
                    contentJson.Contacts.forEach(element => {
                        addContact(element.type, element.value)
                    });

                if (contentJson.Links)
                    contentJson.Links.forEach(element => {
                        addLink(element.name, element.value)
                    });

                if (contentJson.WorkBlocs) {
                    contentJson.WorkBlocs.forEach(element => addWork(element.name, element.corporation,
                        element.fromDate, element.toDate, element.description));
                }

                if (contentJson.EducationBlocs) {
                    contentJson.EducationBlocs.forEach(element => addEducation(element.name, element.date));
                }

                if (contentJson.Projects)
                    contentJson.Projects.forEach(element => addProject(element.name, element.date, element.description));

                if (contentJson.Languages)
                    contentJson.Languages.forEach(element => addLanguage(element.level, element.name));

                if (contentJson.Skills)
                    contentJson.Skills.forEach(element => addSkill(element));

                if (contentJson.Hobbies)
                    contentJson.Hobbies.forEach(element => addHobby(element));
                
            })
        });
    } else {
        await fetch(systemLanguageSelect.value).then((response) => {
            systemJson = response.json().then(json => systemJson = json)
        });
    }

    if (dataJson.image) {
        if (isString(dataJson.image) && dataJson.image.length > 0) {
            fetch(dataJson.image).then((response) => response.blob())
                .then((blob) => {
                    const dt = new DataTransfer();
                    dt.items.add(new File([blob], 'image.jpg'));
                    photoInput.files = dt.files;
                }).then(_ => refreshImagePreview());
        }
    }

    document.body.style.display = "block";
}

async function generateJson() {
    const jsonObject = {};

    // Photo
    jsonObject.image = "";

    if (photoInput.files.length > 0 && photoInput.files[0])
        jsonObject.image = await convertFileToBase64(photoInput.files[0]);

    // Content
    jsonObject.content = "";

    const contentObject = {}
    contentObject.SystemLanguage = systemLanguageSelect.value;
    contentObject.Name = nameInput.value;
    contentObject.Profession = professionInput.value;

    contentObject.AboutMe = aboutMeInput.value;

    contentObject.Contacts = [];
    [...contactsDiv.children].forEach(element => {
        const children = element.children[1].children;
        const type = children[0].selectedIndex;
        const value = children[1].value;
        contentObject.Contacts.push(new Contact(type, value));
    });

    contentObject.Links = [];
    [...linksDiv.children].forEach(element => {
        const children = element.children[1].children;
        const name = children[0].value;
        const value = children[1].value;
        contentObject.Links.push(new Link(name, value));
    });

    contentObject.WorkBlocs = [];
    [...worksDiv.children].forEach((element) => {
        const children = element.children[1].children;
        const name = children[1].children[0].value;
        const corporation = children[3].value;
        const fromDate = children[5].children[0].value;
        const toDate = children[5].children[1].value;
        const description = children[7].value;
        contentObject.WorkBlocs.push(new WorkBloc(name, corporation, fromDate, toDate, description));
    });

    contentObject.EducationBlocs = [];
    [...educationsDiv.children].forEach(element => {
        const children = element.children[1].children;
        const name = children[0].value;
        const date = children[1].value;
        contentObject.EducationBlocs.push(new EducationBloc(name, date));
    });

    contentObject.Projects = [];
    [...projectsDiv.children].forEach((element, index) => {
        const children = element.children[1].children;
        const name = children[1].children[0].value;
        const date = children[3].value;
        const description = children[4].value;

        contentObject.Projects.push(new Project(name, date, description));
    })

    contentObject.Languages = [];
    [...languagesDiv.children].forEach(element => {
        const children = element.children[1].children;
        const name = children[0].value;
        const level = children[1].selectedIndex;
        contentObject.Languages.push(new Language(name, level));
    });

    contentObject.Skills = [];
    [...skillsDiv.children].forEach(element => {
        contentObject.Skills.push(element.children[1].children[0].value);
    })

    contentObject.Hobbies = [];
    [...hobbiesDiv.children].forEach(element => {
        contentObject.Hobbies.push(element.children[1].children[0].value);
    })

    jsonObject.content = JSON.stringify(contentObject);

    return jsonObject;
}

function refreshImagePreview() {
    const hidden = photoInput.files <= 0 || !photoInput.files[0];

    photoReader.style.display = hidden ? "none" : "block";

    if (!hidden) {

        const fileReader = new FileReader();
        fileReader.onload = function (e) {
            document.getElementById("photo-reader").src = e.target.result;
        };
        fileReader.readAsDataURL(photoInput.files[0]);

    } else {
        photoReader.src = "";
    }
}

function refreshElementsArrows(movableElementsParent) {
    for (let i = 0; i < movableElementsParent.children.length; i++) {
        const arrowsDiv = movableElementsParent.children[i].children[0];
        arrowsDiv.children[0].style.display = i === 0 ? "none" : "block";
        arrowsDiv.children[1].style.display = i === movableElementsParent.children.length - 1 ? "none" : "block";
    }
}

function encapsulateInMovable(htmlElement) {

    // Generate from the template
    const template = document.importNode(encapsulationArrowTemplate.content, true).children[0];
    template.append(htmlElement);
    template.children[0].children[0].addEventListener('click', _ => {
        const divParent = template.parentNode;
        divParent.insertBefore(template, template.previousElementSibling);
        refreshElementsArrows(template.parentElement);
    });
    template.children[0].children[1].addEventListener('click', _ => {
        const divParent = template.parentNode;
        divParent.insertBefore(template, template.nextElementSibling.nextElementSibling);
        refreshElementsArrows(template.parentElement);
    });

    return template;
}

function addContact(contactType = 0, contactValue = "") {

    const template = document.importNode(contactItemTemplate.content, true).children[0];
    const children = template.children;

    systemJson.ContactTypes.forEach(element => {
        const option = document.createElement("option");
        option.textContent = element;
        children[0].append(option);
    });
    
    if (isNumeric(contactType) && contactType <= children[0].options.length)
        children[0].selectedIndex = contactType;

    children[1].value = contactValue;
   
    const encapsulated = encapsulateInMovable(template);
    children[2].onclick = _ => encapsulated.remove();

    contactsDiv.append(encapsulated);
    refreshElementsArrows(contactsDiv);
}

function addLink(linkName = "", linkValue = "") {

    const template = document.importNode(linkItemTemplate.content, true).children[0];
    const children = template.children;
    children[0].value = linkName;
    children[1].value = linkValue;

    const encapsulated = encapsulateInMovable(template);
    children[2].onclick = _ => encapsulated.remove();

    linksDiv.append(encapsulated);
    refreshElementsArrows(linksDiv);
}

function addSkill(skillName = "") {

    const template = document.importNode(skillItemTemplate.content, true).children[0];
    const children = template.children;
    children[0].value = skillName;
   
    const encapsulated = encapsulateInMovable(template);
    children[1].onclick = _ => encapsulated.remove();

    skillsDiv.append(encapsulated);
    refreshElementsArrows(skillsDiv);
}

function addWork(title = "", corporation = "", fromDate = Date.now(), toDate = Date.now(), description = "") {
    
    //Generate from the template
    const templateClone = document.importNode(workItemTemplate.content, true).children[0];
    const children = templateClone.children;
    children[1].children[0].value = title;
    children[3].value = corporation;
    children[5].children[0].value = fromDate.toString();
    children[5].children[1].value = toDate.toString();
    children[7].value = description;
    worksDiv.append(templateClone);
    
    const encapsulated = encapsulateInMovable(templateClone);
    children[1].onclick = _ => encapsulated.remove();

    worksDiv.append(encapsulated);
    refreshElementsArrows(worksDiv);
}

function addEducation(title = "", date = Date.now()) {

    const templateClone = document.importNode(educationItemTemplate.content, true).children[0];
    const children = templateClone.children;
    children[0].value = title;
    children[1].value = date.toString();

    const encapsulated = encapsulateInMovable(templateClone);
    children[2].onclick = _ => encapsulated.remove();

    educationsDiv.append(encapsulated);
    refreshElementsArrows(educationsDiv);
}

function addLanguage(level = 0, name = "") {

    const templateClone = document.importNode(languageItemTemplate.content, true).children[0];
    const children = templateClone.children;
    children[0].value = name;

    systemJson.LanguageLevels.forEach(element => {

        const option = document.createElement("option");
        option.textContent = element;
        children[1].append(option);
    });

    if (isNumeric(level) && level <= children[1].options.length)
        children[1].selectedIndex = level;

    const encapsulated = encapsulateInMovable(templateClone);
    children[2].onclick = _ => encapsulated.remove();

    languagesDiv.append(encapsulated);
    refreshElementsArrows(languagesDiv);
}

function addProject(title = "", date = Date.now(), description = "") {

    const templateClone = document.importNode(projectItemTemplate.content, true).children[0];
    const children = templateClone.children;
    children[1].children[0].value = title;
    children[3].value = date.toString();
    children[4].value = description;

    const encapsulated = encapsulateInMovable(templateClone);
    children[1].children[1].onclick = _ => encapsulated.remove();

    projectsDiv.append(encapsulated);
    refreshElementsArrows(projectsDiv);
}

function addHobby(name = "") {

    const templateClone = document.importNode(hobbyItemTemplate.content, true).children[0];
    const children = templateClone.children;
    children[0].value = name;

    const encapsulated = encapsulateInMovable(templateClone);
    children[1].onclick = _ => encapsulated.remove();

    hobbiesDiv.append(encapsulated);
    refreshElementsArrows(hobbiesDiv);
}


document.addEventListener("DOMContentLoaded", async function () {

        if (!checkIsLogged()) {
            location.assign("../index.html")
            return;
        }

        document.getElementById("add-contact").onclick = _ => addContact();
        document.getElementById("add-link").onclick = _ => addLink();
        document.getElementById("add-skill").onclick = _ => addSkill();
        document.getElementById("add-work").onclick = _ => addWork();
        document.getElementById("add-education").onclick = _ => addEducation();
        document.getElementById("add-language").onclick = _ => addLanguage();
        document.getElementById("add-project").onclick = _ => addProject();
        document.getElementById("add-hobby").onclick = _ => addHobby();
        document.getElementById("download_template_button").onclick = _ => {
            const templateLink = document.createElement("a");
            templateLink.download = "CvTemplate.css";
            templateLink.href = "../Common/viewStyle.css";
            templateLink.click();
        }

        const saveButton = document.getElementById("save_button");
        saveButton.onclick = _ => {
            saveButton.disabled = true;

            function succeed() {
                alert("Success");
                saveButton.disabled = false;
            }

            function failed(error) {
                saveButton.disabled = false;
                alertElement.textContent = error.responseText;
            }

            generateJson().then(json => {
                const cvId = sessionStorage.getItem(CvIdItemKey);
                SendRequest("POST", document.cookie, null, APILink + "Cv/SetContent/",
                    new SetContentDto(cvId, json.content), () => {
                        if (isString(json.image) && json.image.length > 0) {
                            SendRequest("POST", document.cookie, null, APILink + "Cv/SetImage/",
                                new SetContentDto(cvId, json.image), succeed, err => failed(err))
                        } else succeed();
                    }, err => failed(err));
            })
        }

        systemLanguageSelect.onchange = event => {
            let index = 0;

            for (let i = 0; i < systemLanguageSelect.options.length; i++) {
                if (systemLanguageSelect.options[i].value !== event.target.value) continue;
                index = i;
                break;
            }

            systemLanguageSelect.selectedIndex = index;
            generateJson().then(json => importFromJson(json));
        }

        photoInput.onchange = _ => refreshImagePreview();

        // Download cv data
        const cvId = sessionStorage.getItem(CvIdItemKey);
        const parameters = [];
        parameters.push(new KeyPairValue("id", cvId));

        // Hide the page during loading
        document.body.style.display = "none";

        SendRequest("GET", document.cookie, parameters, APILink + `Cv/Get`, null, res => {
            const file = JSON.parse(res.responseText);
            importFromJson(file).then(() => 
                generateJson().then(json => 
                    frame.contentWindow.generateFromJson(json)))
            
        }, res => alertElement.textContent = res.responseText);
    }
)