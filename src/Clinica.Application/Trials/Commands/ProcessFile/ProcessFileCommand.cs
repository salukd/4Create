using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Clinica.Application.Trials.Commands.ProcessFile;

public record ProcessFileCommand(IFormFile File) : IRequest<ErrorOr<string>>;