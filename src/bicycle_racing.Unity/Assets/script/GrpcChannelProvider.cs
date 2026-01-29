using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Net.Http;
using UnityEngine;
using Cysharp.Net.Http;
public static class GrpcChannelProvider
{
    private static GrpcChannel _channel;
    // URLは末尾スラッシュなし
#if DEBUG
    private const string ServerURL = "http://localhost:5244";
# else
    private const string ServerURL = "http://ge202400.japaneast.cloudapp.azure.com";
# endif
    public static GrpcChannel GetChannel()
    {
        if (_channel != null) return _channel;
        Debug.Log("★ YetAnotherHttpHandler (YAHH) でチャンネルを作成します");
        // 1. YetAnotherHttpHandler を生成
        var yahh = new YetAnotherHttpHandler
        {
            // HTTP/2を強制しない（1.1も許可する）
            Http2Only = false,
            // 接続タイムアウト
            ConnectTimeout = System.TimeSpan.FromSeconds(10),
        };
        // 2. gRPC-Web Textモードで包む
        // これで "HTTP/1.1 の POST" として送信されます
        var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, yahh);
        // 3. オプション設定
        var options = new GrpcChannelOptions
        {
            HttpHandler = grpcWebHandler,
            DisposeHttpClient = true,
        };
        _channel = GrpcChannel.ForAddress(ServerURL, options);
        return _channel;
    }
    public static void Dispose()
    {
        if (_channel != null)
        {
            try { _channel.Dispose(); } catch { }
            _channel = null;
        }
    }
}