namespace RecipeBookService.DTOs;

public class BaseDTO<T>
{
    public BaseDTO(int statusCode, string message, T data)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public int StatusCode { get; set; }

    public string Message { get; set; }

    public T Data { get; set; }
}