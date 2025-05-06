namespace apbd_tutorial9.Service;

public class ResultIdService
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int? Id { get; set; }
    
    public static ResultIdService Ok(int? id) => new ResultIdService{ Success = true,Id=id};
    public static ResultIdService Fail(string message) => new ResultIdService{ Success = false,Message=message };
}