public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new ApiResponse<T> { Success = true, Message = message ?? "OK", Data = data };

    public static ApiResponse<T> Fail(string message)
        => new ApiResponse<T> { Success = false, Message = message, Data = default };
}