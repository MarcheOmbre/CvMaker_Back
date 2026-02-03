namespace CvBuilderBack.Models;

public class UserCvJoin
{
    public int Id { get; init; }
    
    public int UserId { get; set; }
    
    public int CvId { get; set; }
}