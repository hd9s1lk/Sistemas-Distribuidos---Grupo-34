﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using AgregadorClient;
using System.Net.Sockets;
using System.Net;
using Google.Protobuf;
using System.Security.AccessControl;
using System.Reflection;


var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();


//PROTOCOLO TCP
int listenPort = 9000;
string serverIP = "127.0.0.1";
int serverPort = 800;

IPEndPoint middlemanEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
Socket middlemanSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

middlemanSocket.Bind(middlemanEndPoint);
middlemanSocket.Listen(5);

//conexão com sv

Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
serverSocket.Connect("127.0.0.1", 800);
Console.WriteLine("Conectado ao servidor");





//RPC
using var channelRPC = GrpcChannel.ForAddress("http://localhost:5062");
var client = new Greeter.GreeterClient(channelRPC);
var reply = await client.SayHelloAsync(new HelloRequest { Name = "AgregadorClient" });
var reply2 = await client.MandarIDAsync(new wavyID { ID = "WAVY_ID[1]", PreProcessamento = "Manipulação_Strings", VolumeDadosEnviar = "200MB", ServidorAssociado = "127.0.0.1:800" });
var reply3 = await client.MandarIDAsync(new wavyID { ID = "WAVY_ID[2]", PreProcessamento = "Leitura_Ficheiro_TXT", VolumeDadosEnviar = "200MB", ServidorAssociado = "127.0.0.1:800" });
var reply4 = await client.MandarIDAsync(new wavyID { ID = "WAVY_ID[1234]", PreProcessamento = "Ficheiro_CSV", VolumeDadosEnviar = "356MB", ServidorAssociado = "127.0.0.1:800" });
var reply5 = await client.MandarIDAsync(new wavyID { ID = "3456", PreProcessamento = "JSON FILE", VolumeDadosEnviar = "450MB", ServidorAssociado = "127.0.0.1:800" });

Console.WriteLine("Boas: (MENSAGEM RPC)" + reply.Message);

//DADOS RPC STRINGS
string replyIDStrings = reply2.IDrecebida;
string replyIDProcessamento = reply2.PreProcessamentoRecebido;
string replyIDDados = reply2.VolumeDadosRecebido;
string replyIDServidor = reply2.ServidorAssociado;

//DADOS RPC TXT
string replyIDTXT = reply3.IDrecebida;
string replyProcessamentoTXT = reply3.PreProcessamentoRecebido;
string replyDadosTXT = reply3.VolumeDadosRecebido;
string replyServidorTXT = reply3.ServidorAssociado;
 

//DADOS RPC CSV
string replyIDCSV = reply4.IDrecebida;
string replyProcessamentoCSV = reply4.PreProcessamentoRecebido;
string replyDadosCSV = reply4.VolumeDadosRecebido;
string replyServidorCSV = reply4.ServidorAssociado;


//DADOS RPC JSON
string replyIDJson = reply5.IDrecebida;
string replyProcessamentoJSON = reply5.PreProcessamentoRecebido;
string replyDadosJSON = reply5.VolumeDadosRecebido;
string replyServidorJSON = reply5.ServidorAssociado;

await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
    return Task.CompletedTask;
};


await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer);


//receber IP

await channel.QueueDeclareAsync(queue: "IP", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

var consumer2 = new AsyncEventingBasicConsumer(channel);
consumer2.ReceivedAsync += (model, ea) =>
{
    var body2 = ea.Body.ToArray();
    var message2 = Encoding.UTF8.GetString(body2);
    byte[] resposta = Encoding.UTF8.GetBytes(message2);
    Console.WriteLine($" [x] Received IP:{message2}");
    serverSocket.Send(resposta);
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync("IP", autoAck: true, consumer: consumer2);

//receber ID

await channel.QueueDeclareAsync(queue: "ID", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

var consumer3 = new AsyncEventingBasicConsumer(channel);
consumer3.ReceivedAsync += (model, ea) =>
{
    var body3 = ea.Body.ToArray();
    var message3 = Encoding.UTF8.GetString(body3);
    byte[] resposta = Encoding.UTF8.GetBytes(message3);
    string[] IDcompleto = message3.Split(":");
    var temp = Convert.ToInt64(IDcompleto[4]);
    var VelOndas = Convert.ToInt64(IDcompleto[5]);
    var alturaOndas = Convert.ToInt64(IDcompleto[6]);
    var profundidade = Convert.ToInt64(IDcompleto[7]);
    var date = Convert.ToString(IDcompleto[8]);
    string estado = IDcompleto[9];

    if (IDcompleto[1] == "Manipulação_Strings")
    {
        Console.WriteLine($" [x] Received ID:{message3}");
        Console.WriteLine("Resposta RPC: [ID] -> " + replyIDStrings +
            " [Pre_Processamento] -> " + replyIDProcessamento +
            " [Volume_Dados] -> " + replyIDDados +
            " [Servidor] -> " + replyIDServidor +
            " [Temperatura] -> " + temp +
            " [Velocidade de Ondas] -> " + VelOndas +
            " [Altura de Ondas] -> " + alturaOndas +
            " [Profundidade] -> " + profundidade +
            " [Data] -> " + date +
            " [Estado] -> " + estado);

        Console.WriteLine("Deseja mudar o estado?");
        var leitura = Console.ReadLine();
        if (leitura == "Sim")
        {
            Console.WriteLine("Escolhe o estado: Associada(1) - Operação(2) - Manutenção(3) - Desativada(4)");
            var resp = Console.ReadLine();
            string respostaFinal = null;
            byte[] answer = null;
            switch (resp)
            {
                case "1":
                    IDcompleto[9] = "Associada";
                    string answer1 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer1);
                    Console.WriteLine("Estado alterado: " + answer1);
                    serverSocket.Send(answer);
                    break;
                case "2":
                    IDcompleto[9] = "Operação";
                    string answer2 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer2);
                    Console.WriteLine("Estado alterado: " + answer2);
                    serverSocket.Send(answer);
                    break;
                case "3":
                    IDcompleto[9] = "Manutenção";
                    string answer3 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer3);
                    Console.WriteLine("Estado alterado: " + answer3);
                    serverSocket.Send(answer);
                    break;
                case "4":
                    IDcompleto[9] = "Associada";
                    string answer4 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer4);
                    Console.WriteLine("Estado alterado: " + answer4);
                    serverSocket.Send(answer);
                    break;
            }
        }
    }
    else if (IDcompleto[1] == "Leitura_Ficheiro_TXT")
    {
        Console.WriteLine($" [x] Received ID:{message3}");
        Console.WriteLine("Resposta RPC: [ID] -> " + replyIDTXT +
            " [Pre_Processamento] -> " + replyProcessamentoTXT +
            " [Volume_Dados] -> " + replyDadosTXT +
            " [Servidor] -> " + replyServidorTXT +
            " [Temperatura] -> " + temp +
            " [Velocidade de Ondas] -> " + VelOndas +
            " [Altura de Ondas] -> " + alturaOndas +
            " [Profundidade] -> " + profundidade +
            " [Data] -> " + date + 
            " [Estado] -> " + estado);
        Console.WriteLine("Deseja mudar o estado?");
        var leitura = Console.ReadLine();
        if (leitura == "Sim")
        {
            Console.WriteLine("Escolhe o estado: Associada(1) - Operação(2) - Manutenção(3) - Desativada(4)");
            var resp = Console.ReadLine();
            string respostaFinal = null;
            byte[] answer = null;
            switch (resp)
            {
                case "1":
                    IDcompleto[9] = "Associada";
                    string answer1 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer1);
                    Console.WriteLine("Estado alterado: " + answer1);
                    serverSocket.Send(answer);
                    break;
                case "2":
                    IDcompleto[9] = "Operação";
                    string answer2 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer2);
                    Console.WriteLine("Estado alterado: " + answer2);
                    serverSocket.Send(answer);
                    break;
                case "3":
                    IDcompleto[9] = "Manutenção";
                    string answer3 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer3);
                    Console.WriteLine("Estado alterado: " + answer3);
                    serverSocket.Send(answer);
                    break;
                case "4":
                    IDcompleto[9] = "Associada";
                    string answer4 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer4);
                    Console.WriteLine("Estado alterado: " + answer4);
                    serverSocket.Send(answer);
                    break;
            }
        }
    }
    else if (IDcompleto[1] == "Ficheiro_CSV")
    {
        Console.WriteLine($" [x] Received ID:{message3}");
        Console.WriteLine("Resposta RPC: [ID] -> " + replyIDCSV +
            " [Pre_Processamento] -> " + replyProcessamentoCSV +
            " [Volume_Dados] -> " + replyDadosCSV +
            " [Servidor] -> " + replyServidorCSV +
            " [Temperatura] -> " + temp +
            " [Velocidade de Ondas] -> " + VelOndas +
            " [Altura de Ondas] -> " + alturaOndas +
            " [Profundidade] -> " + profundidade +
            " [Data] -> " + date +
            " [Estado] -> " + estado);
        Console.WriteLine("Deseja mudar o estado?");
        var leitura = Console.ReadLine();
        if (leitura == "Sim")
        {
            Console.WriteLine("Escolhe o estado: Associada(1) - Operação(2) - Manutenção(3) - Desativada(4)");
            var resp = Console.ReadLine();
            string respostaFinal = null;
            byte[] answer = null;
            switch (resp)
            {
                case "1":
                    IDcompleto[9] = "Associada";
                    string answer1 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer1);
                    Console.WriteLine("Estado alterado: " + answer1);
                    serverSocket.Send(answer);
                    break;
                case "2":
                    IDcompleto[9] = "Operação";
                    string answer2 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer2);
                    Console.WriteLine("Estado alterado: " + answer2);
                    serverSocket.Send(answer);
                    break;
                case "3":
                    IDcompleto[9] = "Manutenção";
                    string answer3 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer3);
                    Console.WriteLine("Estado alterado: " + answer3);
                    serverSocket.Send(answer);
                    break;
                case "4":
                    IDcompleto[9] = "Associada";
                    string answer4 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer4);
                    Console.WriteLine("Estado alterado: " + answer4);
                    serverSocket.Send(answer);
                    break;
            }
        }
    }
    else if (IDcompleto[1] == "JSON FILE")
    {
        Console.WriteLine($" [x] Received ID:{message3}");
        Console.WriteLine("Resposta RPC: [ID] -> " + replyIDJson +
            " [Pre_Processamento] -> " + replyProcessamentoJSON +
            " [Volume_Dados] -> " + replyDadosJSON +
            " [Servidor] -> " + replyServidorJSON +
            " [Temperatura] -> " + temp +
            " [Velocidade de Ondas] -> " + VelOndas +
            " [Altura de Ondas] -> " + alturaOndas +
            " [Profundidade] -> " + profundidade +
            " [Data] -> " + date +
            " [Estado] -> " + estado);
        Console.WriteLine("Deseja mudar o estado?");
        var leitura = Console.ReadLine();
        if (leitura == "Sim")
        {
            Console.WriteLine("Escolhe o estado: Associada(1) - Operação(2) - Manutenção(3) - Desativada(4)");
            var resp = Console.ReadLine();
            string respostaFinal = null;
            byte[] answer = null;
            switch (resp)
            {
                case "1":
                    IDcompleto[9] = "Associada";
                    string answer1 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer1);
                    Console.WriteLine("Estado alterado: " + answer1);
                    serverSocket.Send(answer);
                    break;
                case "2":
                    IDcompleto[9] = "Operação";
                    string answer2 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer2);
                    Console.WriteLine("Estado alterado: " + answer2);
                    serverSocket.Send(answer);
                    break;
                case "3":
                    IDcompleto[9] = "Manutenção";
                    string answer3 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer3);
                    Console.WriteLine("Estado alterado: " + answer3);
                    serverSocket.Send(answer);
                    break;
                case "4":
                    IDcompleto[9] = "Associada";
                    string answer4 = String.Join(":", IDcompleto);
                    answer = Encoding.UTF8.GetBytes(answer4);
                    Console.WriteLine("Estado alterado: " + answer4);
                    serverSocket.Send(answer);
                    break;
            }
        } 
    }
    else
    {
        Console.WriteLine("erro");
        return Task.CompletedTask;
    }
        return Task.CompletedTask;
};

await channel.BasicConsumeAsync("ID", autoAck: true, consumer: consumer3);

//receber STATUS

await channel.QueueDeclareAsync(queue: "STATUS", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

var consumer4 = new AsyncEventingBasicConsumer(channel);
consumer4.ReceivedAsync += (model, ea) =>
{
    var body4 = ea.Body.ToArray();
    var message4 = Encoding.UTF8.GetString(body4);
    Console.WriteLine($" [x] Received ID:{message4}");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync("STATUS", autoAck: true, consumer: consumer4);




Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();