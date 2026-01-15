const passwordInput = document.getElementById("password_input");
const passwordConfirmationInput = document.getElementById("password-confirmation_input");
const resetButton = document.getElementById("reset_button");
const alert = document.getElementById("alert");

class ResetPasswordDto {
    constructor(email, password) {
        
        if(!isMatchingMailPattern(email))
            throw new Error("Invalid email pattern");
        
        if(!isString(password))
            throw new Error("Password must be a string");
        
        if(password === "")
            throw new Error("Password can't be null");
        
        this.password = password;
        this.passwordConfirmation = password;
    }
}

document.addEventListener("DOMContentLoaded", async function () {

    if (checkIsLogged()) {
        location.assign("./Listing/index.html")
        return;
    }

    resetButton.onclick = async _ => {

        if (passwordInput.value === "") {
            alert.textContent = "Password can't be null";
            return;
        }

        if (passwordConfirmationInput.value !== passwordInput.value) {
            alert.textContent = "Passwords doesn't match";
            return;
        }

        let params = new URLSearchParams(document.location.search);
        let token = params.get("token");
        if (!isString(token))
            throw new Error("Token must be a string");

        resetButton.disabled = true;
        
        SendRequest("POST", token, null,
            APILink + "Authentification/ResetPassword",
            new ResetPasswordDto(passwordInput.value, passwordConfirmationInput.value),
            _ => location.assign("../index.html"),
            res => {
                resetButton.disabled = false;

                if (res.status === 401)
                    alert.textContent = "Unauthorized : the token is invalid";
                else
                    alert.textContent = res;
            });
    }

})