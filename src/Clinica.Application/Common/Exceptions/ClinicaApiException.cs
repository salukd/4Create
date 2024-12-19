using System.Net;

namespace Clinica.Application.Common.Exceptions;

public class ClinicaApiException(string message, string errorCode, HttpStatusCode statusCode)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
    public HttpStatusCode StatusCode { get; } = statusCode;
}