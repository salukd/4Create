using MediatR;

namespace Clinica.Application.Common.Caching;

public interface IQuery<out TResponse> : IRequest<TResponse>;

