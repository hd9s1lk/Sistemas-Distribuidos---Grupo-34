using Grpc.Core;
using RPC_AGREGADOR_SERVIDOR;

namespace RPC_AGREGADOR_SERVIDOR.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }

    public override Task<wavyIDReply> MandarID(wavyID request, ServerCallContext context)
    {
        return Task.FromResult(new wavyIDReply
        {
            IDrecebida = request.ID,
            PreProcessamentoRecebido = request.PreProcessamento,
            VolumeDadosRecebido = request.VolumeDadosEnviar,
            ServidorAssociado = request.ServidorAssociado
        });
    }
}


