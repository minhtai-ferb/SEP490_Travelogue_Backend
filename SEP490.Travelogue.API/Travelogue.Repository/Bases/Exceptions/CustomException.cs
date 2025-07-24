﻿namespace Travelogue.Repository.Bases.Exceptions;

public class CustomException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }
    public object? ErrorMessage { get; }
    public object? DetailMessage { get; }

    public CustomException(int statusCode, string errorCode, string? message = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorMessage = message ?? "An error occurred";
    }

    public CustomException(int statusCode, string errorCode, object errorMessage)
        : base(errorMessage.ToString())
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public CustomException(int statusCode, string errorCode, string? message = null, string? detailMessage = null)
        : base(detailMessage)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorMessage = message ?? "An error occurred";
        DetailMessage = detailMessage;
    }

    public CustomException(int statusCode, string errorCode, object errorMessage, string? detailMessage = null)
        : base(errorMessage.ToString())
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        DetailMessage = detailMessage;
    }
}
