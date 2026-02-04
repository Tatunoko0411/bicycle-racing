using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using MagicOnion.Client;
using static Grpc.Core.Interceptors.Interceptor;

public class GameIdInterceptor : Interceptor {
    private readonly string _gameId;
    public GameIdInterceptor(string gameId) => _gameId = gameId;

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation) {
        var headers = context.Options.Headers ?? new Metadata();
        headers.Add("X-Game-Id", _gameId);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            new CallOptions(headers, context.Options.Deadline, context.Options.CancellationToken,
                            context.Options.WriteOptions, context.Options.PropagationToken, context.Options.Credentials));
        return base.AsyncUnaryCall(request, newContext, continuation);
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation) {
        var headers = context.Options.Headers ?? new Metadata();
        headers.Add("X-Game-Id", _gameId);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            new CallOptions(headers, context.Options.Deadline, context.Options.CancellationToken,
                            context.Options.WriteOptions, context.Options.PropagationToken, context.Options.Credentials));
        return base.AsyncDuplexStreamingCall(newContext, continuation);
    }
}