function isNumeric(num) 
{
    return (typeof(num) === 'number' || typeof(num) === "string" && num.trim() !== '') && !isNaN(num);
}

function isString(str) 
{
    return typeof(str) === 'string' && str.trim() !== '';
}