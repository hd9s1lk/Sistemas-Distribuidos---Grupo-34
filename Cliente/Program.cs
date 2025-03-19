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
            Console.WriteLine("Conectado ao servidor!");

            string message = "Olá, servidor!";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(messageBytes);

            string soma = "SOMA 4 2";
            byte[] somaBytes = Encoding.UTF8.GetBytes(soma);
            clientSocket.Send(somaBytes);

            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string receivedText = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

            Console.WriteLine($"Resposta do servidor: {receivedText}");

            clientSocket.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}