const emailInput = document.getElementById("email_input");
const passwordInput = document.getElementById("password_input");
const loginButton = document.getElementById("login_button");
const alert = document.getElementById("alert");

class LoginDto {
    constructor(email, password) {
        
        if(!isMatchingMailPattern(email))
            throw new Error("Invalid email pattern");
        
        if(!isString(password))
            throw new Error("Password must be a string");
        
        if(password === "")
            throw new Error("Password can't be null");
        
        this.email = email;
        this.password = password;
    }
}

document.addEventListener("DOMContentLoaded", async function () 
{
    if(checkIsLogged())
    {
        location.assign("./Listing/index.html")
        return;
    }
    
    loginButton.onclick = async _ => {
        
        if(!isMatchingMailPattern(emailInput.value))
        {
            alert.textContent = "Invalid email pattern";
            return;
        }

        if (passwordInput.value === "") {
            alert.textContent = "Password can't be null";
            return;
        }

        loginButton.disabled = true;

        SendRequest("POST", null, null,
            APILink + "Authentification/Log",
            new LoginDto(emailInput.value, passwordInput.value),
            res => {
                document.cookie = res;
                location.assign("./Listing/index.html")
            },
            response => {
                alert.textContent = response;
                loginButton.disabled = false;
            });
    }

})