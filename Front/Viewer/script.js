const printButton = document.getElementById("print_button");

const nameEntry = document.getElementById("header_name");
const professionEntry = document.getElementById("header_profession");
const imageEntry = document.getElementById("header_photo");
const aboutMeSection = document.getElementById("about-me");
const contactsSection = document.getElementById("header_contacts_list");
const linksSection = document.getElementById("header_links_list");
const skillsSection = document.getElementById("skills");
const worksSection = document.getElementById("work");
const educationSection = document.getElementById("education");
const languagesSection = document.getElementById("languages");
const projectsSection = document.getElementById("projects");
const hobbiesSection = document.getElementById("hobbies");

const contactItemTemplate = document.getElementById("contact-item_template");
const linkItemTemplate = document.getElementById("link-item_template");
const skillItemTemplate = document.getElementById("skill-item_template");
const workItemTemplate = document.getElementById("work-item_template");
const educationItemTemplate = document.getElementById("education-item_template");
const languageItemTemplate = document.getElementById("language-item_template");
const projectItemTemplate = document.getElementById("project-item_template");
const hobbyItemTemplate = document.getElementById("hobby-item_template");


function fillSection(section, title, fillSection) {
    if (!section)
        return;

    section.getElementsByClassName("title-parent")[0].getElementsByClassName("title")[0].textContent = title;
    fillSection(section.getElementsByClassName("content")[0]);
}

function refreshLanguage(language) {
    if (isString(language))
        document.documentElement.lang = language;
}

function refreshTitle(title) {
    if (isString(title))
        document.title = title;
}

function refreshName(name) {
    if (isString(name))
        nameEntry.textContent = name;
}

function refreshProfession(profession) {
    if (isString(profession))
        professionEntry.textContent = profession;
}

function refreshImage(image) {
    if (isString(image))
        imageEntry.src = image;
}

function refreshContacts(contacts) {

    if (!contacts || !Array.isArray(contacts))
        return;

    contactsSection.innerHTML = "";

    contacts.forEach(element => {

        if (!isString(element.value))
            return;

        const templateClone = document.importNode(contactItemTemplate.content, true).children[0];
        templateClone.textContent = element.value;
        contactsSection.appendChild(templateClone);
    });
}

function refreshLinks(links) {

    if (!links || !Array.isArray(links))
        return;

    contactsSection.innerHTML = "";

    links.forEach(element => {

        if (!isString(element.name) || !isString(element.value))
            return;

        const templateClone = document.importNode(linkItemTemplate.content, true).children[0];
        templateClone.textContent = element.name + ": " + element.value;
        linksSection.appendChild(templateClone);
    });
}

function refreshAboutMe(title, text) {
    
    if (!isString(title))
        return;

    fillSection(aboutMeSection, title, content => {

        const textBloc = document.createElement("p");
        textBloc.textContent = text;
        content.appendChild(textBloc);
    });
}

function refreshSkills(title, skills) {
    if (!isString(title) || !skills || !Array.isArray(skills))
        return;

    skillsSection.children[1].innerHTML = "";

    fillSection(skillsSection, title, content =>
        skills.forEach(element => {

            if (!isString(element))
                return;

            const templateClone = document.importNode(skillItemTemplate.content, true).children[0];
            templateClone.textContent = element;
            content.appendChild(templateClone);
        }));

}

function refreshWorks(title, works) {
    if (!isString(title) || !works || !Array.isArray(works))
        return;

    worksSection.children[1].innerHTML = "";

    this.fillSection(worksSection, title, content => {
        works.forEach(element => {

            if (!isString(element.name) || !isString(element.corporation) || !isString(element.description))
                return;

            // Format date
            const fromDate = new Date(element.fromDate);
            const toDate = new Date(element.toDate);

            // Format month to force xx/yyyy format
            let month = (fromDate.getMonth() + 1).toString();
            if (month.length === 1)
                month = "0" + month;

            let stringDate = isDate(fromDate) ? `${month}/${fromDate.getFullYear()}` : "";
            if (isDate(toDate)) {
                if (stringDate !== "")
                    stringDate += " - ";

                // Format month to force xx/yyyy format
                month = (toDate.getMonth() + 1).toString();
                if (month.length === 1)
                    month = "0" + month;

                stringDate += `${month}/${toDate.getFullYear()}`;
            }

            const templateClone = document.importNode(workItemTemplate.content, true).children[0];
            const children = templateClone.children;
            children[0].children[0].textContent = element.name;
            children[0].children[1].textContent = element.corporation;
            children[0].children[2].textContent = stringDate;
            children[1].textContent = element.description;

            content.appendChild(templateClone);
        })
    });

}

function refreshEducations(title, educations) {
    if (!isString(title) || !educations || !Array.isArray(educations))
        return;

    educationSection.children[1].innerHTML = "";
    
    fillSection(educationSection, title, content => {
        educations.forEach(element => {

            if (!isString(element.name))
                return;

            const templateClone = document.importNode(educationItemTemplate.content, true).children[0];
            const children = templateClone.children;
            children[0].textContent = element.name;
            children[1].textContent = isString(element.date) ? new Date(element.date).getFullYear() : "";

            content.appendChild(templateClone);
        })
    });

}

function refreshLanguages(title, languages, languageLevels) {
    if (!isString(title) || !languages || !Array.isArray(languages) || !languageLevels || !Array.isArray(languageLevels))
        return;

    languagesSection.children[1].innerHTML = "";
    
    fillSection(languagesSection, title, content => {
        languages.forEach(element => {

            if (!isString(element.name) || !isNumeric(element.level) || element >= languageLevels.length)
                return;

            const templateClone = document.importNode(languageItemTemplate.content, true).children[0];
            templateClone.textContent = `${element.name} (${languageLevels[element.level]})`;

            content.appendChild(templateClone);
        })
    });

}

function refreshProjects(title, projects) {

    if (!isString(title) || !projects || !Array.isArray(projects))
        return;

    projectsSection.children[1].innerHTML = "";
    
    fillSection(projectsSection, title, content => {
        projects.forEach(element => {

            if (!isString(element.name) || !isString(element.description))
                return;

            const templateClone = document.importNode(projectItemTemplate.content, true).children[0];
            const children = templateClone.children;
            children[0].children[0].textContent = element.name;
            children[0].children[1].textContent = isString(element.date) ? new Date(element.date).getFullYear() : "";
            children[1].textContent = element.description;
            
            content.appendChild(templateClone);
        })
    });

}

function refreshHobbies(title, hobbies) {

    if (!isString(title) || !hobbies || !Array.isArray(hobbies))
        return;

    hobbiesSection.children[1].innerHTML = "";
    
    fillSection(hobbiesSection, title, content => {
        hobbies.forEach(element => {

            if (!isString(element))
                return;

            const templateClone = document.importNode(hobbyItemTemplate.content, true).children[0];
            templateClone.textContent = element;
            content.appendChild(templateClone);
        })
    });

}

function generateFromJson(dataJson) {

    if (!dataJson || !isString(dataJson.content))
        return

    const dataObject = {};
    dataObject.image = dataJson.image;
    dataObject.content = JSON.parse(dataJson.content);

    fetch(dataObject.content.SystemLanguage).then(response => {
        response.json().then(systemJson => {
            refreshLanguage(systemJson.key);
            refreshTitle(dataObject.Name);
            refreshName(dataObject.content.Name);
            refreshAboutMe(systemJson.AboutMeTitle, dataObject.content.AboutMe);
            refreshProfession(dataObject.content.Profession);
            refreshImage(dataObject.image);
            refreshContacts(dataObject.content.Contacts);
            refreshLinks(dataObject.content.Links);
            refreshSkills(systemJson.SkillsTitle, dataObject.content.Skills);
            refreshWorks(systemJson.WorkTitle, dataObject.content.WorkBlocs);
            refreshEducations(systemJson.EducationTitle, dataObject.content.EducationBlocs);
            refreshLanguages(systemJson.LanguagesTitle, dataObject.content.Languages, systemJson.LanguageLevels);
            refreshProjects(systemJson.ProjectsTitle, dataObject.content.Projects);
            refreshHobbies(systemJson.HobbiesTitle, dataObject.content.Hobbies);
        })
    });
}

document.addEventListener("DOMContentLoaded", () => {
    printButton.onclick = _ => {
        printButton.style.display = "none";
        this.print();
        printButton.style.display = "block";
    }
});
