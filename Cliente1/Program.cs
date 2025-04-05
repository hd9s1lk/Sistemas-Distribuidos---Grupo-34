using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


class DADOS
{
    public string WAVY_ID { get; set; }
    public string PRE_PROCESSAMENTO { get; set; }
    public string VOLUME_DADOS_ENVIAR { get; set; }
    public string SERVIDOR_ASSOCIADO { get; set; }

    public string STATUS { get; set; }

    public string data_type { get; set; }

    public string Last_Sync { get; set; }
}

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

            Console.WriteLine("Que dados deseja enviar? Strings(1) - Ficheiro TXT(2) - Ficheiro CSV(3) - Ficheiro JSON(4)");
            var ficheiro = Console.ReadLine();
            if(ficheiro == "1")
            {
                //Enviar a wavyID
                string wavyID = "WAVY_ID[1]:Manipulação_Strings:200MB:127.0.0.1:800";
                //servidor associado = IP SV
                //pré_processamento = data type (formato dados)
                //volume_dados_enviar = qtd
                byte[] wavyIDBytes = Encoding.UTF8.GetBytes(wavyID);
                clientSocket.Send(wavyIDBytes);

                byte[] wavyIDbuffer = new byte[1024];
                int wavyIDBytesResponse = clientSocket.Receive(wavyIDbuffer);
                string wavyIDTEXT = Encoding.UTF8.GetString(wavyIDbuffer, 0, wavyIDBytesResponse);

                Console.WriteLine($"{wavyIDTEXT}");


                //Enviar a wavyStatus
                DateTime now = DateTime.Now;
                string wavyStatus = "WAVY_ID[1]:status:Strings:" + now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                //data types = tipo de dados enviados
                //last_sync = data
                byte[] wavyStatusBytes = Encoding.UTF8.GetBytes(wavyStatus);
                clientSocket.Send(wavyStatusBytes);

                byte[] wavyStatusbuffer = new byte[1024];
                int wavyStatusBytesResponse = clientSocket.Receive(wavyStatusbuffer);
                string wavyStatusTEXT = Encoding.UTF8.GetString(wavyStatusbuffer, 0, wavyStatusBytesResponse);

                Console.WriteLine($"{wavyStatusTEXT}");


                clientSocket.Close();
            } else if (ficheiro == "2")
            {
                //Enviar a wavyID
                string filePath = "dados.txt";
                string[] lines = File.ReadAllLines("dados.txt");
                string wavyId = lines[0];
                byte[] wavyIDBytes = Encoding.UTF8.GetBytes(wavyId);
                clientSocket.Send(wavyIDBytes);

                byte[] wavyIDbuffer = new byte[1024];
                int wavyIDBytesResponse = clientSocket.Receive(wavyIDbuffer);
                string wavyIDTEXT = Encoding.UTF8.GetString(wavyIDbuffer, 0, wavyIDBytesResponse);

                Console.WriteLine($"{wavyIDTEXT}");


                //Enviar a wavyStatus
                string wavyStatus = lines[1];
                Console.WriteLine($"{wavyStatus}");
                byte[] wavyStatusBytes = Encoding.UTF8.GetBytes(wavyStatus);
                clientSocket.Send(wavyStatusBytes);

                byte[] wavyStatusbuffer = new byte[1024];
                int wavyStatusBytesResponse = clientSocket.Receive(wavyStatusbuffer);
                string wavyStatusTEXT = Encoding.UTF8.GetString(wavyStatusbuffer, 0, wavyStatusBytesResponse);

                Console.WriteLine($"{wavyStatusTEXT}");


                clientSocket.Close();
            } else if(ficheiro == "3")
            {
                //Enviar a wavyID
                string filePath = "dados.csv";
                string[] lines = File.ReadAllLines("dados.csv");
                string wavyId = lines[0];
                byte[] wavyIDBytes = Encoding.UTF8.GetBytes(wavyId);
                clientSocket.Send(wavyIDBytes);

                byte[] wavyIDbuffer = new byte[1024];
                int wavyIDBytesResponse = clientSocket.Receive(wavyIDbuffer);
                string wavyIDTEXT = Encoding.UTF8.GetString(wavyIDbuffer, 0, wavyIDBytesResponse);

                Console.WriteLine($"{wavyIDTEXT}");


                //Enviar a wavyStatus
                string wavyStatus = lines[1];
                Console.WriteLine($"{wavyStatus}");
                byte[] wavyStatusBytes = Encoding.UTF8.GetBytes(wavyStatus);
                clientSocket.Send(wavyStatusBytes);

                byte[] wavyStatusbuffer = new byte[1024];
                int wavyStatusBytesResponse = clientSocket.Receive(wavyStatusbuffer);
                string wavyStatusTEXT = Encoding.UTF8.GetString(wavyStatusbuffer, 0, wavyStatusBytesResponse);

                Console.WriteLine($"{wavyStatusTEXT}");


                clientSocket.Close();
            }else if (ficheiro == "4")
            {
                //Enviar a wavyID
                string filePath = "dados.json";
                string lines = File.ReadAllText("dados.json");
                DADOS dados = JsonSerializer.Deserialize<DADOS>(lines);
                string JSONdados = ($"{dados.WAVY_ID}:{dados.PRE_PROCESSAMENTO}:{dados.VOLUME_DADOS_ENVIAR}:{dados.SERVIDOR_ASSOCIADO}");
                byte[] wavyIDBytes = Encoding.UTF8.GetBytes(JSONdados);
                clientSocket.Send(wavyIDBytes);

                byte[] wavyIDbuffer = new byte[1024];
                int wavyIDBytesResponse = clientSocket.Receive(wavyIDbuffer);
                string wavyIDTEXT = Encoding.UTF8.GetString(wavyIDbuffer, 0, wavyIDBytesResponse);

                Console.WriteLine($"{wavyIDTEXT}");


                //Enviar a wavyStatus

                string JSONstatus = ($"{dados.WAVY_ID}:{dados.STATUS}:{dados.data_type}:{dados.Last_Sync}");
                byte[] wavyStatusBytes = Encoding.UTF8.GetBytes(JSONstatus);
                clientSocket.Send(wavyStatusBytes);

                byte[] wavyStatusbuffer = new byte[1024];
                int wavyStatusBytesResponse = clientSocket.Receive(wavyStatusbuffer);
                string wavyStatusTEXT = Encoding.UTF8.GetString(wavyStatusbuffer, 0, wavyStatusBytesResponse);

                Console.WriteLine($"{wavyStatusTEXT}");


                clientSocket.Close();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}