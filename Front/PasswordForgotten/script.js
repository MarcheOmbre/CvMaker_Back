const emailInput = document.getElementById("email_input");
const retrieveButton = document.getElementById("retrieve_button");
const message = document.getElementById("message");

class RetrievePasswordDto {
    constructor(email, pagePathInput) 
    {
        if(!isMatchingMailPattern(email))
            throw new Error("Invalid email pattern");
        
        if(!isString(pagePathInput))
            throw new Error("Page path must be a string");
        
        if(pagePathInput.trim() === "")
            throw new Error("Page path can't be empty");
        
        this.email = email;
        this.pagePath = pagePathInput;
    }
}

document.addEventListener("DOMContentLoaded", async function () {

    // Link buttons
    retrieveButton.onclick = async _ => {

        if (!isMatchingMailPattern(emailInput.value)) {
            message.color = "red";
            message.textContent = "Invalid email pattern";
            return;
        }

        retrieveButton.disabled = true;

        SendRequest("POST", null, null,
            APILink + "Authentification/ForgotPassword",
            new RetrievePasswordDto(emailInput.value, "http://localhost:63342/CvBuilderBack/Front/ResetPassword/index.html"),
            _ => {
                message.color = "black";
                message.textContent = "An email has been sent to you";
            }, res => {
                retrieveButton.disabled = false;
                message.color = "red";
                message.textContent = res;
            }
        )
    }
})