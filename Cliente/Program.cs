using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class SocketClient
{
    static void Main()
    {
        string serverIP = "127.0.0.1";
        int port = 9000;
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);

        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            clientSocket.Connect(endPoint);
            Console.WriteLine("Conectado ao Agregador com Protocolo TCP na porta 9000");

            //ENVIO DE IP
            string message = "IP WAVY: 127.0.0.1:9000";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(messageBytes);

            //TROCA DE IP
            string soma = "Dados ultimo";
            byte[] somaBytes = Encoding.UTF8.GetBytes(soma);
            clientSocket.Send(somaBytes);

            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string receivedText = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

            Console.WriteLine($"{receivedText}");


            //Enviar a wavyID
            string wavyID = "WAVY_ID:pré_processamento:volume_dados_enviar:servidor_associado";
            byte[] wavyIDBytes = Encoding.UTF8.GetBytes(wavyID);
            clientSocket.Send(wavyIDBytes);

            byte[] wavyIDbuffer = new byte[1024];
            int wavyIDBytesResponse = clientSocket.Receive(wavyIDbuffer);
            string wavyIDTEXT = Encoding.UTF8.GetString(wavyIDbuffer, 0, wavyIDBytesResponse);

            Console.WriteLine($"{wavyIDTEXT}");


            //Enviar a wavyStatus
            string wavyStatus = "WAVY_ID:status:[data_types]:last_sync";
            byte[] wavyStatusBytes = Encoding.UTF8.GetBytes(wavyStatus);
            clientSocket.Send(wavyStatusBytes);

            byte[] wavyStatusbuffer = new byte[1024];
            int wavyStatusBytesResponse = clientSocket.Receive(wavyStatusbuffer);
            string wavyStatusTEXT = Encoding.UTF8.GetString(wavyStatusbuffer, 0, wavyStatusBytesResponse);

            Console.WriteLine($"{wavyStatusTEXT}");





            clientSocket.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}