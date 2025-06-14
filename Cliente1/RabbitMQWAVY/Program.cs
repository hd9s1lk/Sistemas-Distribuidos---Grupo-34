﻿using RabbitMQ.Client;
using System.Text;
using System.Text.Json;



var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();



await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

const string message = "Hello World!";
var body = Encoding.UTF8.GetBytes(message);

await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);
Console.WriteLine($" [x] Sent {message}");

//mandar IP

await channel.QueueDeclareAsync(queue: "IP", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

Console.WriteLine("Insira o IP da WAVY:");
string IP = Console.ReadLine();
var body2 = Encoding.UTF8.GetBytes(IP);

await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "IP", body: body2);
Console.WriteLine($" [x] Sent {IP}");

//dados analiticos
Random random = new Random();
int numeroAleatorioTemp = random.Next(0,32);
int numeroAleatorioOndas = random.Next(0, 30);
int numeroAleatorioAlturaOndas = random.Next(1, 12);
int numeroAleatorioProfundidade = random.Next(0,6000);
var DATA = DateTime.Now.ToString("d/M/yyyy");

EstadoWavy[] estado = (EstadoWavy[])Enum.GetValues(typeof(EstadoWavy));
int indice = random.Next(estado.Length);
EstadoWavy estadoatual = estado[indice];

var EstadoFinal = Convert.ToString(estadoatual);

//mandar WavyID

await channel.QueueDeclareAsync(queue: "ID", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

Console.WriteLine("Que tipo de Dados queres enviar: (1 - Manipulação de Strings) / (2 - Ficheiro TXT) / (3 - Ficheiro CSV) / (4 - Ficheiro JSON)");
string resposta = Console.ReadLine();
if (resposta == "1")
{
    string message3 = "WAVY_ID[1]:Manipulação_Strings:200MB:127.0.0.1:" + numeroAleatorioTemp + ":" + numeroAleatorioOndas + ":" + numeroAleatorioAlturaOndas + ":" + numeroAleatorioProfundidade + ":" + DATA + ":" + EstadoFinal;
    Console.WriteLine(message3);
    var body3 = Encoding.UTF8.GetBytes(message3);

    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "ID", body: body3);
    Console.WriteLine($" [x] Sent {message3}");
} else if (resposta == "2")
{
    string filePath = "dados.txt";
    string[] lines = File.ReadAllLines("dados.txt");
    string wavyID = lines[0];
    var TXTcompleto = lines[0] + ":" + numeroAleatorioTemp + ":" + numeroAleatorioOndas + ":" + numeroAleatorioAlturaOndas + ":" + numeroAleatorioProfundidade + ":" + DATA + ":" + EstadoFinal;
    var body3 = Encoding.UTF8.GetBytes(TXTcompleto);

    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "ID", body: body3);
    Console.WriteLine($" [x] Sent {TXTcompleto}");
} else if(resposta == "3")
{
    string filePath = "dados.csv";
    string[] lines = File.ReadAllLines("dados.csv");
    string wavyID = lines[0] + ":" + numeroAleatorioTemp + ":" + numeroAleatorioOndas + ":" + numeroAleatorioAlturaOndas + ":" + numeroAleatorioProfundidade + ":" + DATA + ":" + EstadoFinal;
    Console.WriteLine(wavyID);
    var body3 = Encoding.UTF8.GetBytes(wavyID);

    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "ID", body: body3);
    Console.WriteLine($" [x] Sent {wavyID}");

} else if(resposta == "4")
{
    string filePath = "dados.json";
    string lines = File.ReadAllText("dados.json");
    DADOS dados = JsonSerializer.Deserialize<DADOS>(lines);
    string JSONdados = ($"{dados.WAVY_ID}:{dados.PRE_PROCESSAMENTO}:{dados.VOLUME_DADOS_ENVIAR}:{dados.SERVIDOR_ASSOCIADO}") + ":" + numeroAleatorioTemp + ":" + numeroAleatorioOndas + ":" + numeroAleatorioAlturaOndas + ":" + numeroAleatorioProfundidade + ":" + DATA + ":" + EstadoFinal;
    var body3 = Encoding.UTF8.GetBytes(JSONdados);

    Console.WriteLine(JSONdados);

    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "ID", body: body3);
    Console.WriteLine($" [x] Sent {JSONdados}");
}





//DADOS PARA JSON

class Dados
{
    public string WAVY_ID { get; set; }
    public string PRE_PROCESSAMENTO { get; set; }
    public string VOLUME_DADOS_ENVIAR { get; set; }
    public string SERVIDOR_ASSOCIADO { get; set; }
    public string STATUS { get; set; }

    public string data_type { get; set; }

    public string Last_Sync { get; set; }
}

public enum EstadoWavy
{
    Associada,
    Manutencao,
    Operacao,
    Desativada
}