class ViewerHandler {

    #systemJson
    #dataJson;

    constructor(dataJson) {
        this.#initialize(dataJson).then(() => this.#generateCv())
    }

    async #initialize(dataJson) {

        this.#dataJson = dataJson;
        this.#systemJson = await fetch(this.#dataJson.SystemLanguage).then(response => response.json());

    }

    #generateSettings() {

        document.documentElement.lang = "en";
        document.title = "My CV";

    }

    #generateCvHeader() {

        document.getElementById("header_name").textContent = this.#dataJson.Name;
        document.getElementById("header_profession").textContent = this.#dataJson.Profession;
        
        if(isString(this.#dataJson.Photo) && this.#dataJson.Photo.length > 0)
        {
            document.getElementById("header_photo").src = this.#dataJson.Photo;
        }


        // Generate contacts
        const contactsDivision = document.getElementById("header_contacts_list");
        this.#dataJson.Contacts.forEach(element => {
            const li = document.createElement("li");
            li.classList.add("header_contacts_item");
            li.textContent = `${this.#systemJson.ContactTypes[element.type]} : ${element.value}`;
            contactsDivision.appendChild(li);
        });

        // Generate links
        const linksDivision = document.getElementById("header_links_list");
        this.#dataJson.Links.forEach((element) => {
            const li = document.createElement("li");
            li.classList.add("header_links_item");
            li.innerHTML = `<a href={element.link}>${element.name}</a>`;
            linksDivision.appendChild(li);
        });

    }

    #generateCvAboutMe() {

        document.getElementById("about-me_title").textContent = this.#systemJson.AboutMeTitle;
        document.getElementById("about-me_content").innerHTML = this.#dataJson.AboutMe;

    }

    #generateCvSkills() {

        document.getElementById("skills_title").textContent = this.#systemJson.SkillsTitle;

        // Generate skills
        const skillsDivision = document.getElementById("skills_list");
        this.#dataJson.Skills.forEach(element => {
            const li = document.createElement("li");
            li.classList.add("skills_item");
            li.textContent = element;
            skillsDivision.appendChild(li);
        });

    }

    #generateCvWork() {

        document.getElementById("work_title").textContent = this.#systemJson.WorkTitle;

        // Generate work blocs
        const workExperienceDivision = document.getElementById("work");
        this.#dataJson.WorkBlocs.forEach(element => {
            const div = document.createElement("div");
            div.classList.add("work_item");
            div.innerHTML = `
            <h4 class="work_bloc_title">${element.name}</h4>
            <p class="work_bloc_corporation">${element.corporation}</p>
            <div class="work_bloc_dates">
                <p class="work_bloc_date">${element.fromDate}</p>
                <p class="work_bloc_date">${element.toDate}</p>
            </div>
            <p class="work_bloc_description">${element.description}</p>
        `;
            workExperienceDivision.appendChild(div);
        });

    }

    #generateCvEducation() {

        document.getElementById("education_title").textContent = this.#systemJson.EducationTitle;

        // Generate education blocs
        const educationDivision = document.getElementById("education");
        this.#dataJson.EducationBlocs.forEach(element => {
            const div = document.createElement("div");
            div.classList.add("education_item");
            div.innerHTML = `
            <h4 class="education_bloc_title">${element.name}</h4>
            <p class="education_bloc_date">${element.date}</p>
        `;
            educationDivision.appendChild(div);
        });

    }

    #generateCvLanguage() {

        document.getElementById("languages_title").textContent = this.#systemJson.LanguagesTitle;

        // Map languages data
        const languagesDivision = document.getElementById("languages_list");
        this.#dataJson.Languages.forEach(element => {
            const li = document.createElement("li");
            li.classList.add("languages_item");
            li.textContent = `${element.name} (${this.#systemJson.LanguageLevels[element.level]})`;
            languagesDivision.appendChild(li);
        })

    }

    #generateCvProjects() {

        document.getElementById("projects_title").textContent = this.#systemJson.ProjectsTitle;

        // Generate projects
        const projectsDivision = document.getElementById("projects");
        this.#dataJson.Projects.forEach(element => {
            const div = document.createElement("div");
            div.classList.add("project_item");
            div.innerHTML = `
            <h4 class="project_bloc_title">${element.name}</h4>
            <p class="project_bloc_date">${element.date}</p>
            <p class="project_bloc_description">${element.description}</p>
        `;
            projectsDivision.appendChild(div);
        })

    }

    #generateCvHobbies() {

        document.getElementById("hobbies_title").textContent = this.#systemJson.HobbiesTitle;

        // Generate hobbies
        const hobbiesDivision = document.getElementById("hobbies_list");
        this.#dataJson.Hobbies.forEach(element => {
            const li = document.createElement("li");
            li.textContent = element;
            hobbiesDivision.appendChild(li);
        })

    }

    #generateCv() {
        this.#generateSettings();
        this.#generateCvHeader();
        this.#generateCvAboutMe();
        this.#generateCvSkills();
        this.#generateCvWork();
        this.#generateCvEducation();
        this.#generateCvLanguage();
        this.#generateCvProjects();
        this.#generateCvHobbies();
    }
}

document.addEventListener("DOMContentLoaded", async function () {
    console.log(JSON.parse(localStorage.getItem('json')));
    new ViewerHandler(JSON.parse(localStorage.getItem('json')));
})