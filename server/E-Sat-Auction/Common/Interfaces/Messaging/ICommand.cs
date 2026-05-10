using MediatR;

namespace e_Sat_Auction.Common.Interfaces.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest
{
}

public interface IAuditableCommandMarker
{
}

public interface IAuditableCommand<out TResponse> : ICommand<TResponse>, IAuditableCommandMarker
{
}

public interface IAuditableCommand : ICommand, IAuditableCommandMarker
{
}