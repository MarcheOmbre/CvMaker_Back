class ViewerHandler {

    #systemJson
    #dataJson;

    constructor(dataJson) {
        this.#initialize(dataJson).then(() => this.#generateCv())
    }

    async #initialize(dataJson) {

        this.#dataJson = dataJson;
        this.#systemJson = await fetch(this.#dataJson.content.SystemLanguage).then(response => response.json());

    }

    #fillSection(sectionTitle, title, fillSection) {

        const section = document.getElementById(sectionTitle);

        section.getElementsByClassName("title-parent")[0].getElementsByClassName("title")[0].textContent = title;
        fillSection(section.getElementsByClassName("content")[0]);
    }

    #generateSettings() {

        document.documentElement.lang = "en";
        document.title = "My CV";

    }

    #generateCvHeader() {

        document.getElementById("header_name").textContent = this.#dataJson.content.Name;
        document.getElementById("header_profession").textContent = this.#dataJson.content.Profession;

        if (isString(this.#dataJson.image) && this.#dataJson.image.length > 0) {
            document.getElementById("header_photo").src = this.#dataJson.image;
        }


        // Generate contacts
        const contactsDivision = document.getElementById("header_contacts_list");
        this.#dataJson.content.Contacts.forEach(element => {
            const li = document.createElement("li");
            li.classList.add("header_contacts_item");
            li.textContent = `${element.value}`;
            contactsDivision.appendChild(li);
        });

        // Generate links
        const linksDivision = document.getElementById("header_links_list");
        this.#dataJson.content.Links.forEach((element) => {
            const li = document.createElement("li");
            li.classList.add("header_links_item");
            li.innerHTML = element.name + ": " + element.value;
            linksDivision.appendChild(li);
        });

    }

    #generateCvAboutMe() {
        this.#fillSection("about-me", this.#systemJson.AboutMeTitle, content => content.innerHTML = `<p>${this.#dataJson.content.AboutMe}</p>`)
    }

    #generateCvSkills() {

        this.#fillSection("skills", this.#systemJson.SkillsTitle, content =>
            this.#dataJson.content.Skills.forEach(element => {
                const li = document.createElement("li");
                li.classList.add("skills_item");
                li.innerHTML = element;
                content.appendChild(li);
            }));

    }

    #generateCvWork() {

        this.#fillSection("work", this.#systemJson.WorkTitle, content => {
            this.#dataJson.content.WorkBlocs.forEach(element => {

                // Format date
                const fromDate = new Date(element.fromDate);
                const toDate = new Date(element.toDate);
                
                // Format month to force xx/yyyy format
                let month = (fromDate.getMonth() + 1).toString();
                if(month.length === 1)
                    month = "0" + month;
                
                let stringDate = isDate(fromDate) ? `${month}/${fromDate.getFullYear()}` : "";
                if(isDate(toDate)){
                    if(stringDate !== "")
                        stringDate += " - ";

                    // Format month to force xx/yyyy format
                    month = (toDate.getMonth() + 1).toString();
                    if(month.length === 1)
                        month = "0" + month;
                    
                    stringDate += `${month}/${toDate.getFullYear()}`;
                }
                
                const div = document.createElement("div");
                div.classList.add("work_item");
                div.innerHTML = `
            <div class ="work_bloc_header">
                <h4 class="work_bloc_title">${element.name}</h4>
                <p class="work_bloc_corporation">${element.corporation}</p>
                <p class="work_bloc_date">${stringDate}</p>
            </div>
            <p class="work_bloc_description">${element.description}</p>
        `;
                content.appendChild(div);
            })
        });

    }

    #generateCvEducation() {

        this.#fillSection("education", this.#systemJson.EducationTitle, content => {
            this.#dataJson.content.EducationBlocs.forEach(element => {
                const div = document.createElement("div");
                div.classList.add("education_item");
                div.innerHTML = `
            <h4 class="education_bloc_title">${element.name}</h4>
            <p class="education_bloc_date">${isString(element.date) ? new Date(element.date).getFullYear() : ""}</p>
        `;
                content.appendChild(div);
            })
        });

    }

    #generateCvLanguage() {

        this.#fillSection("languages", this.#systemJson.LanguagesTitle, content => {
            this.#dataJson.content.Languages.forEach(element => {
                const li = document.createElement("li");
                li.classList.add("languages_item");
                li.textContent = `${element.name} (${this.#systemJson.LanguageLevels[element.level]})`;
                content.appendChild(li);
            })
        });

    }

    #generateCvProjects() {

        this.#fillSection("projects", this.#systemJson.ProjectsTitle, content => {
            this.#dataJson.content.Projects.forEach(element => {
                const div = document.createElement("div");
                div.classList.add("project_item");
                div.innerHTML = `
            <div class="project_header">
            <h4 class="project_bloc_title">${element.name}</h4>
            <p class="project_bloc_date">${isString(element.date) ? new Date(element.date).getFullYear() : ""}</p>
            </div>
            <p class="project_bloc_description">${element.description}</p>
        `;
                content.appendChild(div);
            })
        });

    }

    #generateCvHobbies() {

        this.#fillSection("hobbies", this.#systemJson.HobbiesTitle, content => {
            this.#dataJson.content.Hobbies.forEach(element => {
                const li = document.createElement("li");
                li.textContent = element;
                content.appendChild(li);
            })
        });

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

document.addEventListener("DOMContentLoaded", async function () 
{
    new ViewerHandler(JSON.parse(sessionStorage.getItem(CvDataItemKey)));
})