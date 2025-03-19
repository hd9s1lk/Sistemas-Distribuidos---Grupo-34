using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Middleman
{
    static void Main()
    {
        int listenPort = 9000; 
        string serverIP = "127.0.0.1";
        int serverPort = 800;

        IPEndPoint middlemanEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
        Socket middlemanSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        middlemanSocket.Bind(middlemanEndPoint);
        middlemanSocket.Listen(5);

        Console.WriteLine($"Middleman rodando na porta {listenPort}...");

        while (true)
        {
            Socket clientSocket = middlemanSocket.Accept();
            Console.WriteLine("Cliente conectado ao middleman!");

            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string receivedText = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            Console.WriteLine($"Mensagem recebida do cliente: {receivedText}");

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                serverSocket.Connect(new IPEndPoint(IPAddress.Parse(serverIP), serverPort));
                serverSocket.Send(buffer, receivedBytes, SocketFlags.None);

                // Receber resposta do servidor
                byte[] responseBuffer = new byte[1024];
                int responseBytes = serverSocket.Receive(responseBuffer);
                string serverResponse = Encoding.UTF8.GetString(responseBuffer, 0, responseBytes);
                Console.WriteLine($"Resposta do servidor: {serverResponse}");

                // Enviar resposta de volta ao cliente
                clientSocket.Send(responseBuffer, responseBytes, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar ao servidor: {ex.Message}");
            }
            finally
            {
                serverSocket.Close();
                clientSocket.Close();
            }
        }
    }
}