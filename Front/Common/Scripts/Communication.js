class KeyPairValue{
    constructor(key, value) {
        
        if(!isString(key))
            throw new Error("Key must be a string");
        
        this.key = key;
        this.value = value;
    }
}

function SendRequest(type, tokenToInject, parameters, link, data, onSucceed, onFailed){
    
    if(type !== "GET" && type !== "POST" && type !== "PUT" && type !== "DELETE")
        throw new Error("Invalid request type");
    
    if(!isString(link))
        throw new Error("Link must be a string")
    
    // Inject parameters if there are any
    if(parameters)
    {
        let parametersString = "";
        for(let i = 0; i < parameters.length; i++)
        {
            if(!(parameters[i] instanceof KeyPairValue))
                throw new Error("Parameters must be KeyPairValue");

            parametersString += parameters[i].key + "=" + parameters[i].value;
            if(i !== parameters.length - 1)
                parametersString += "&";
        }

        link += "?" + parametersString;
    }

    // Create the request
    const options = {
        method: type,
        headers: {"Content-Type": data ? "application/json" : "text/plain; charset=utf-8"}
    };

    if (tokenToInject) {
        options.headers["Authorization"] = `Bearer ${tokenToInject}`;
    }

    if (data) {
        options.body = JSON.stringify(data);
    }

    fetch(link, options)
        .then(response => 
        {
            if (!response.ok) 
                throw response;
            return response;
        })
        .then(response => response.text().then(text => onSucceed(text)))
        .catch(error => error.text().then(text => { onFailed(text) }));
}