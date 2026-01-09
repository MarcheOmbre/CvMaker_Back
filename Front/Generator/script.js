class Contact {
    constructor(type, value) {
        this.type = type;
        this.value = value;
    }
    
}

class Link{
    constructor(name, value) {
        this.name = name;
        this.value = value;
    }
}

class WorkBloc{
    constructor(name, corporation, fromDate, toDate, description) {
        this.name = name;
        this.corporation = corporation;
        this.fromDate = fromDate;
        this.toDate = toDate;
        this.description = description;
    }
}

class EducationBloc{
    constructor(name, date) {
        this.name = name;
        this.date = date;
    }
}

class Project{
    constructor(name, date, description) {
        this.name = name;
        this.date = date;
        this.description = description;
    }
}

class Language{
    constructor(name, level) {
        this.name = name;
        this.level = level;
    }
}

class EditorHandler {
    
    #systemJson;
    #systemLanguageSelect = document.getElementById("system-language");
    #photoInput = document.getElementById("photo-input");
    #photoReader = document.getElementById("photo-reader");
    #nameInput = document.getElementById("name_input");
    #professionInput = document.getElementById("profession_input");
    #aboutMeInput = document.getElementById("about-me_input");
    #contactsDiv = document.getElementById("contacts");
    #linksDiv = document.getElementById("links");
    #skillsDiv = document.getElementById("skills");
    #worksDiv = document.getElementById("works");
    #educationsDiv = document.getElementById("educations");
    #languagesDiv = document.getElementById("languages");
    #projectsDiv = document.getElementById("projects");
    #hobbiesDiv = document.getElementById("hobbies");
    
    
    constructor(dataJson) 
    {
        document.getElementById("add-contact").onclick = _ => this.#addContact();
        document.getElementById("add-link").onclick = _ => this.#addLink();
        document.getElementById("add-skill").onclick = _ => this.#addSkill();
        document.getElementById("add-work").onclick = _ => this.#addWork();
        document.getElementById("add-education").onclick = _ => this.#addEducation();
        document.getElementById("add-language").onclick = _ => this.#addLanguage();
        document.getElementById("add-project").onclick = _ => this.#addProject();
        document.getElementById("add-hobby").onclick = _ => this.#addHobby();
        document.getElementById("generate_button").onclick = _ => this.#exportToJson();

        this.#systemLanguageSelect.onchange = _ => this.#switchSystemLanguage(this.#systemLanguageSelect.value);
        
        this.#photoInput.onchange = _ => this.#refreshImagePreview();

        this.#importFromJson(dataJson).then(() => {})
    }
    
    #refreshImagePreview()
    {
        const hidden = this.#photoInput.files <= 0 || !this.#photoInput.files[0];

        this.#photoReader.style.display = hidden ? "none" : "block";

        if(!hidden)
        {

            const fileReader = new FileReader();
            fileReader.onload = function(e)
            {document.getElementById("photo-reader").src = e.target.result;};
            fileReader.readAsDataURL(this.#photoInput.files[0]);

        }
        else{
            this.#photoReader.src = "";
        }
    }
    
    async #importFromJson(dataJson){
        
        // Clear the fields
        [...this.#contactsDiv.children].forEach(element => element.remove());
        [...this.#linksDiv.children].forEach(element => element.remove());
        [...this.#skillsDiv.children].forEach(element => element.remove());
        [...this.#worksDiv.children].forEach(element => element.remove());
        [...this.#educationsDiv.children].forEach(element => element.remove());
        [...this.#languagesDiv.children].forEach(element => element.remove());
        [...this.#projectsDiv.children].forEach(element => element.remove());
        [...this.#hobbiesDiv.children].forEach(element => element.remove());
        
        // Restore the fields
        if(dataJson)
        {
            await this.#switchSystemLanguage(dataJson.SystemLanguage);
            
            if(isString(dataJson.Name))
                this.#nameInput.value = dataJson.Name;

            if(isString(dataJson.Profession))
                this.#professionInput.value = dataJson.Profession;
            
            if(isString(dataJson.Photo) && dataJson.Photo.length > 0)
            {
                fetch(dataJson.Photo).then((response) => response.blob())
                    .then((blob) => {
                        const dt = new DataTransfer();
                        dt.items.add(new File([blob], 'image.jpg'));
                        this.#photoInput.files = dt.files;
                    }).then(_ => this.#refreshImagePreview());
            }
            
            if(isString(dataJson.AboutMe))
                this.#aboutMeInput.value = dataJson.AboutMe;
            
            dataJson.Contacts.forEach(element => {this.#addContact(element.type, element.value)});

            dataJson.Links.forEach(element => {this.#addLink(element.name, element.value)});
            
            dataJson.WorkBlocs.forEach(element => this.#addWork(element.name, element.corporation, element.fromDate,
                element.toDate, element.description));

            dataJson.EducationBlocs.forEach(element => this.#addEducation(element.name, element.date));

            dataJson.Projects.forEach(element => this.#addProject(element.name, element.date, element.description));

            dataJson.Languages.forEach(element => this.#addLanguage(element.level, element.name));

            dataJson.Skills.forEach(element => this.#addSkill(element));

            dataJson.Hobbies.forEach(element => this.#addHobby(element));
        }
        else
        {
            await this.#switchSystemLanguage(0);
        }
        
    }
    
    async #exportToJson()
    {
        const jsonObject = {};
        
        jsonObject.SystemLanguage = this.#systemLanguageSelect.value;
        jsonObject.Name = this.#nameInput.value;
        jsonObject.Profession = this.#professionInput.value;

        jsonObject.Photo = "";
        
        if(this.#photoInput.files.length > 0 && this.#photoInput.files[0])
        {
            jsonObject.Photo = await convertFileToBase64(this.#photoInput.files[0]);
        }
        
        jsonObject.AboutMe = this.#aboutMeInput.value;
        
        jsonObject.Contacts = [];
        [...this.#contactsDiv.children].forEach(element => {
            const children = element.children;
            const type = children[1].children[0].selectedIndex;
            const value = children[1].children[1].value;
            jsonObject.Contacts.push(new Contact(type, value));
        });
        
        jsonObject.Links = [];
        [...this.#linksDiv.children].forEach(element => {
            const children = element.children;
            const name = children[1].children[0].value;
            const value = children[1].children[1].value;
            jsonObject.Links.push(new Link(name, value));
        });
        
        jsonObject.WorkBlocs = [];
        [...this.#worksDiv.children].forEach((element, index) => {
            const children = element.children;
            const name = children[1].children[0].children[1].value;
            const corporation = children[1].children[1].children[1].value;
            const fromDate = children[1].children[2].children[1].value;
            const toDate = children[1].children[2].children[2].value;
            const description = children[1].children[3].value;
            jsonObject.WorkBlocs.push(new WorkBloc(name, corporation, fromDate, toDate, description));
        });
        
        jsonObject.EducationBlocs = [];
        [...this.#educationsDiv.children].forEach(element => {
           const children = element.children;
           const name = children[1].children[1].value;
           const date = children[1].children[4].value;
           jsonObject.EducationBlocs.push(new EducationBloc(name, date));
        });
        
        jsonObject.Projects = [];
        [...this.#projectsDiv.children].forEach((element, index) => {
            const children = element.children;
            const name = children[1].children[0].children[1].value;
            const date = children[1].children[1].children[1].value;
            const description =  children[1].children[2].value;
            jsonObject.Projects.push(new Project(name, date, description));
        })

        jsonObject.Languages = [];
        [...this.#languagesDiv.children].forEach(element => {
            const children = element.children;
            const name = children[1].children[0].value;
            const level = children[1].children[1].selectedIndex;
            jsonObject.Languages.push(new Language(name, level));
        });
        
        jsonObject.Skills = [];
        [...this.#skillsDiv.children].forEach(element => {
            jsonObject.Skills.push(element.children[1].children[0].value);
        })

        jsonObject.Hobbies = [];
        [...this.#hobbiesDiv.children].forEach(element => {
            jsonObject.Hobbies.push(element.children[1].children[0].value);
        })
        
        const button = document.createElement("a");
        button.href = `data:text/plain;charset=utf-8,${encodeURIComponent(JSON.stringify(jsonObject))}`;
        button.download = "CV_data.json";
        button.click();
        button.remove();
    }
    
    async #switchSystemLanguage(languageType) 
    {
        let index = 0;
        for(let i = 0; i < this.#systemLanguageSelect.options.length; i++)
        {
            if(this.#systemLanguageSelect.options[i].value !== languageType)
                continue;
            
            index = i;
            break;
        }
        this.#systemLanguageSelect.selectedIndex = index;
        
        await fetch(this.#systemLanguageSelect.value).then(response => this.#systemJson = response.json()).then(data => this.#systemJson = data);
    }
    
    #encapsulateInDraggable(htmlElement) {

        // Create drag button
        const div = document.createElement("div");
        div.className = "draggableDiv";
        div.draggable = true;
        div.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor"
                            className="bi bi-arrows-move" viewBox="0 0 16 16">
            <path fill-rule="evenodd"
                  d="M7.646.146a.5.5 0 0 1 .708 0l2 2a.5.5 0 0 1-.708.708L8.5 1.707V5.5a.5.5 0 0 1-1 0V1.707L6.354 2.854a.5.5 0 1 1-.708-.708zM8 10a.5.5 0 0 1 .5.5v3.793l1.146-1.147a.5.5 0 0 1 .708.708l-2 2a.5.5 0 0 1-.708 0l-2-2a.5.5 0 0 1 .708-.708L7.5 14.293V10.5A.5.5 0 0 1 8 10M.146 8.354a.5.5 0 0 1 0-.708l2-2a.5.5 0 1 1 .708.708L1.707 7.5H5.5a.5.5 0 0 1 0 1H1.707l1.147 1.146a.5.5 0 0 1-.708.708zM10 8a.5.5 0 0 1 .5-.5h3.793l-1.147-1.146a.5.5 0 0 1 .708-.708l2 2a.5.5 0 0 1 0 .708l-2 2a.5.5 0 0 1-.708-.708L14.293 8.5H10.5A.5.5 0 0 1 10 8"/>
        </svg>`;
        div.appendChild(htmlElement);
        
        // Set events
        div.addEventListener('drag',dragEvent =>
        {
            // Avoid last event
            if (dragEvent.screenX === 0) {
                return;
            }
            
            const parentNode = dragEvent.target.parentNode;
            if(parentNode === null)
                return;
            
            // Get children and y position
            const childElements = parentNode.children;
            const childrenPositionTupleArray = [];
            
            for(let i = 0; i < childElements.length; i++) {
                childrenPositionTupleArray.push({
                    element: childElements[i],
                    yPosition: childElements[i] === dragEvent.target ? dragEvent.y: childElements[i].getBoundingClientRect().top
                });
            }
            
            // Reorder
            childrenPositionTupleArray.sort((a,b) => a.yPosition - b.yPosition);
            for(let i = 0; i < childrenPositionTupleArray.length; i++){
                console.log(i + " " + childrenPositionTupleArray[i].element.children[1].children[0].value + " " + childrenPositionTupleArray[i].yPosition);
                parentNode.removeChild(childrenPositionTupleArray[i].element);
                parentNode.insertBefore(childrenPositionTupleArray[i].element, childElements[i]);
            }
        })
        
        return div;
    }

    #addContact(contactType = 0, contactValue = "") {

        // Generate div
        const div = document.createElement("div");
        div.classList.add("contact_item");

        // Generate select
        const select = document.createElement("select");
        this.#systemJson.ContactTypes.forEach(element => {
            const option = document.createElement("option");
            option.value = element;
            option.textContent = element;
            select.append(option);
        });

        if (isNumeric(contactType) && select.options.length > contactType)
            select.value = select.options[contactType].value;

        // Generate input and button
        const input = document.createElement("input");
        input.type = "text";
        input.value = isString(contactValue) ? contactValue : "";
        const button = document.createElement("button");
        button.textContent = "-";
        button.onclick = _ => div.remove();

        // Append elements
        div.append(select, input, button);
        this.#contactsDiv.append(this.#encapsulateInDraggable(div));
    }

    #addLink(linkName = "", linkValue = "") {

        // Generate div
        const div = document.createElement("div");
        div.classList.add("link_item");

        // Generate input and button
        const nameInput = document.createElement("input");
        nameInput.type = "text";
        nameInput.value = isString(linkName) ? linkName : "";
        const linkInput = document.createElement("input");
        linkInput.type = "text";
        linkInput.value = isString(linkValue) ? linkValue : "";
        const button = document.createElement("button");
        button.textContent = "-";
        button.onclick = _ => div.remove();

        // Append elements
        div.append(nameInput, linkInput, button);
        this.#linksDiv.append(this.#encapsulateInDraggable(div));

    }

    #addSkill(skillName = "") {

        // Generate div
        const div = document.createElement("div");
        div.classList.add("skill_item");

        // Generate input and button
        const input = document.createElement("input");
        input.type = "text";
        input.value = isString(skillName) ? skillName : "";
        const button = document.createElement("button");
        button.textContent = "-";
        button.onclick = _ => div.remove();

        // Append elements
        div.append(input, button);
        this.#skillsDiv.append(this.#encapsulateInDraggable(div));

    }

    #addWork(title = "", corporation = "", fromDate = Date.now(), toDate = Date.now(), description = "") {

        // Generate div
        const div = document.createElement("div");
        div.classList.add("work_item");

        // Title
        const titleDiv = document.createElement("div");
        titleDiv.classList.add("title-div");
        const titleLabel = document.createElement("label");
        titleLabel.textContent = "Title : ";
        titleLabel.htmlFor = "work-title-value";
        const titleInput = document.createElement("input");
        titleInput.type = "text";
        titleInput.name = "work-title-value";
        titleInput.value = isString(title) ? title : "";
        const button = document.createElement("button");
        button.textContent = "-";
        button.onclick = _ => div.remove();
        titleDiv.append(titleLabel, titleInput, button);

        // Corporation
        const corporationDiv = document.createElement("div");
        corporationDiv.classList.add("corporation-div");
        const corporationLabel = document.createElement("label");
        corporationLabel.textContent = "Corporation : ";
        corporationLabel.htmlFor = "work-corporation-value";
        const corporationInput = document.createElement("input");
        corporationInput.type = "text";
        corporationInput.name = "work-corporation-value";
        corporationInput.value = isString(corporation) ? corporation : "";
        corporationDiv.append(corporationLabel, corporationInput);

        // Date
        const dateDiv = document.createElement("div");
        dateDiv.classList.add("date-div");
        const dateLabel = document.createElement("label");
        dateLabel.textContent = "Date : ";
        dateLabel.htmlFor = "work-from-date-value";
        const fromDateInput = document.createElement("input");
        fromDateInput.type = "date";
        fromDateInput.name = "work-from-date-value";
        fromDateInput.value = fromDate.toString();
        const toDateInput = document.createElement("input");
        toDateInput.type = "date";
        toDateInput.value = toDate.toString();
        dateDiv.append(dateLabel, fromDateInput, toDateInput);

        // Description
        const index = this.#worksDiv.children.length;
        const descriptionInput = document.createElement("textarea");
        descriptionInput.value = isString(description) ? description : "";
        descriptionInput.id = "work-description-value-" + index;

        // Append elements
        div.append(titleDiv);
        div.append(corporationDiv);
        div.append(dateDiv);
        div.append(descriptionInput);
        this.#worksDiv.append(this.#encapsulateInDraggable(div));
    }

    #addEducation(title = "", date = Date.now()) {
        
        // Generate div
        const div = document.createElement("div");
        div.classList.add("education_item");

        // Title
        const titleLabel = document.createElement("label");
        titleLabel.textContent = "Title : ";
        titleLabel.htmlFor = "titleValue";
        const titleInput = document.createElement("input");
        titleInput.type = "text";
        titleInput.value = isString(title) ? title : "";
        const button = document.createElement("button");
        button.textContent = "-";
        button.onclick = _ => div.remove();

        // Date
        const dateLabel = document.createElement("label");
        dateLabel.textContent = "Date : ";
        dateLabel.htmlFor = "dateInput";
        const dateInput = document.createElement("input");
        dateInput.type = "date";
        dateInput.value = date.toString();

        // Append elements
        div.append(titleLabel, titleInput, button, dateLabel, dateInput);
        this.#educationsDiv.append(this.#encapsulateInDraggable(div));

    }

    #addLanguage(level = 0, name = "") {

        // Generate div
        const div = document.createElement("div");
        div.classList.add("language_item");

        // Generate input
        const input = document.createElement("input");
        input.type = "text";
        input.value = name;

        // Generate select
        const select = document.createElement("select");
        this.#systemJson.LanguageLevels.forEach(element => {

            const option = document.createElement("option");
            option.value = element;
            option.textContent = element;
            select.append(option);
        });
        
        if(isNumeric(level) && select.options.length > level)
            select.selectedIndex = level;

        // Generate button
        const button = document.createElement("button");
        button.textContent = "-";
        button.onclick = _ => div.remove();

        // Append elements
        div.append(input, select, button);
        this.#languagesDiv.append(this.#encapsulateInDraggable(div));

    }

    #addProject(title = "", date = Date.now(), description = "") {

        // Generate div
        const div = document.createElement("div");
        div.classList.add("work_item");

        // Title
        const titleDiv = document.createElement("div");
        titleDiv.classList.add("title-div");
        const titleLabel = document.createElement("label");
        titleLabel.textContent = "Title : ";
        titleLabel.htmlFor = "project-title-Value";
        const titleInput = document.createElement("input");
        titleInput.type = "text";
        titleInput.name = "project-title-Value";
        titleInput.value = title;
        const button = document.createElement("button");
        button.textContent = "-";
        button.onclick = _ => div.remove();
        titleDiv.append(titleLabel, titleInput, button);

        // Date
        const dateDiv = document.createElement("div");
        dateDiv.classList.add("date-div");
        const dateLabel = document.createElement("label");
        dateLabel.textContent = "Date : ";
        dateLabel.htmlFor = "project-date-value";
        const dateInput = document.createElement("input");
        dateInput.type = "date";
        dateInput.name = "project-date-value";
        dateInput.value = date.toString();
        dateDiv.append(dateLabel, dateInput);

        // Description
        const index = this.#projectsDiv.children.length;
        const descriptionInput = document.createElement("textarea");
        descriptionInput.value = description;
        descriptionInput.id = "project-description-value-" + index;

        // Append elements
        div.append(titleDiv);
        div.append(dateDiv);
        div.append(descriptionInput);
        this.#projectsDiv.append(this.#encapsulateInDraggable(div));
    }

    #addHobby(name = "") {

        // Generate div
        const div = document.createElement("div");
        div.classList.add("hobby_item");

        // Generate input and button
        const nameInput = document.createElement("input");
        nameInput.type = "text";
        nameInput.value = name;
        const button = document.createElement("button");
        button.textContent = "-";
        button.onclick = _ => div.remove();

        // Append elements
        div.append(nameInput, button);
        this.#hobbiesDiv.append(this.#encapsulateInDraggable(div));

    }
}

document.addEventListener("DOMContentLoaded", async function () {

    // Load file
    let file = JSON.parse(localStorage.getItem('json'));
    new EditorHandler(file);
})