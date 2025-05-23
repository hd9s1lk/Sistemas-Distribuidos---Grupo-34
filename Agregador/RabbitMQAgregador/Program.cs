using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using AgregadorClient;


var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

//RPC
using var channelRPC = GrpcChannel.ForAddress("http://localhost:5062");
var client = new Greeter.GreeterClient(channelRPC);
var reply = await client.SayHelloAsync(new HelloRequest { Name = "AgregadorClient" });

Console.WriteLine("Boas: (MENSAGEM RPC)" + reply.Message);


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
    Console.WriteLine($" [x] Received IP:{message2}");
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
    Console.WriteLine($" [x] Received ID:{message3}");
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