using CvBuilderBack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CvBuilderBack.Services;

public class AspNetUserService(ControllerBase controller) : IUserService
{
    private const string UserKey = "UserId";

    public int GetId()
    {
        var user = controller.User.FindFirst(UserKey);
        if(user is null)
            return -1;
        
        return int.TryParse(user.Value, out var id) ? id : -1;
    }
}