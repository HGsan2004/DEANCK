﻿namespace QLCHNT.DATA;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public ApiResponse(bool success, string message, T? data = default, List<string>? errors = null)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors;
    }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Request successful")
    {
        return new ApiResponse<T>(true, message, data);
    }

    public static ApiResponse<T> FailResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>(false, message, default, errors);
    }
}