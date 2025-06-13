using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Grpc.Net.Client;
using System.Threading.Tasks;
using Servidor;
using Microsoft.Data.SqlClient;

public class ServerClass
{
    public void InstanceMethod()
    {
        Console.WriteLine("Conectado!");
    }
}

class Example
{
    private static Mutex mut = new Mutex();
    private const int numIterations = 1;
    private const int numThreads = 3;

    private static void ThreadProc()
    {
        for(int i=0; i< numIterations; i++)
        {
            UseResource();
        }
    }

    private static void UseResource()
    {
        // Wait until it is safe to enter, and do not enter if the request times out.
        Console.WriteLine("{0} is requesting the mutex", Thread.CurrentThread.Name);
        if (mut.WaitOne(1000))
        {
            Console.WriteLine("{0} has entered the protected area",
                Thread.CurrentThread.Name);

            // Place code to access non-reentrant resources here.

            // Simulate some work.
            Thread.Sleep(5000);

            Console.WriteLine("{0} is leaving the protected area",
                Thread.CurrentThread.Name);

            // Release the Mutex.
            mut.ReleaseMutex();
            Console.WriteLine("{0} has released the mutex",
                              Thread.CurrentThread.Name);
        }
        else
        {
            Console.WriteLine("{0} will not acquire the mutex",
                              Thread.CurrentThread.Name);
        }
    }

    ~Example()
    {
        mut.Dispose();
    }

    private void StartThreads()
    {
        // Create the threads that will use the protected resource.
        for (int i = 0; i < numThreads; i++)
        {
            Thread newThread = new Thread(new ThreadStart(ThreadProc));
            newThread.Name = String.Format("Thread{0}", i + 1);
            newThread.Start();
        }

        // The main thread returns to Main and exits, but the application continues to
        // run until all foreground threads have exited.
    }

    static void Main()
    {

        using var channel = GrpcChannel.ForAddress("http://localhost:5067");
        var client = new Greeter.GreeterClient(channel);
        var reply = client.SayHelloAsync(new HelloRequest { Name = "Servidor" });


        Console.WriteLine("Conectado ao RPC");


        Example ex = new Example();
        ex.StartThreads();

        ServerClass serverObject = new ServerClass();

        Thread InstanceCaller = new Thread(new ThreadStart(serverObject.InstanceMethod));

        InstanceCaller.Start();


        int port = 800;
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(endPoint);
        serverSocket.Listen(5);
        Console.WriteLine($"Servidor a rodar na porta {port}...");

        while (true)
        {
            string filePath = "servidor.txt";
            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("Agregador conectado!");


            //IP WAVY
            byte[] ipwavy = new byte[1024];
            int ipwavyBytes = clientSocket.Receive(ipwavy);
            string ipwavydados = Encoding.UTF8.GetString(ipwavy, 0, ipwavyBytes);
            Console.WriteLine(ipwavydados);


            //recebe wavy id com pre processamento
            byte[] wavyProcessamento = new byte[1024];
            int wavyProcessamentoBytes = clientSocket.Receive(wavyProcessamento);
            string wavyProcessamentoDados = Encoding.UTF8.GetString(wavyProcessamento, 0, wavyProcessamentoBytes);
            string[] wavyProcessamentoFinal = wavyProcessamentoDados.Split(":");
            string wavyProcessamentoSQL = wavyProcessamentoFinal[1];
            var temperatura = Convert.ToInt64(wavyProcessamentoFinal[4]);
            var velocidadeOndas = Convert.ToInt64(wavyProcessamentoFinal[5]);
            var alturaOndas = Convert.ToInt64(wavyProcessamentoFinal[6]);
            var profundidade = Convert.ToInt64(wavyProcessamentoFinal[7]);
            var date = wavyProcessamentoFinal[8];
            var estado = wavyProcessamentoFinal[9];
            Console.WriteLine($"{wavyProcessamentoDados}");


            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SD;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            

          
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO dados_agregador (IPWavy, WavyProcessamento, Temperatura, Velocidade_Ondas, Altura_Ondas, Profundidade, Data_Dados, Estado) " +
                               "VALUES (@IPWavy, @WavyProc, @Temp, @VelOndas, @AlturaOndas, @Profundidade, @Data, @Estado)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IPWavy", ipwavydados.Trim());
                    command.Parameters.AddWithValue("@WavyProc", wavyProcessamentoSQL.Trim());
                    command.Parameters.AddWithValue("@Temp", temperatura);
                    command.Parameters.AddWithValue("@VelOndas", velocidadeOndas);
                    command.Parameters.AddWithValue("@AlturaOndas", alturaOndas);
                    command.Parameters.AddWithValue("@Profundidade", profundidade);
                    command.Parameters.AddWithValue("@Data", date);
                    command.Parameters.AddWithValue("@Estado", estado);

                    Console.WriteLine("Dados inseridos na base de dados.");

                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Linhas afetadas: {rowsAffected}");

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Dados inseridos com sucesso. A verificar...");

                        string checkQuery = "SELECT TOP 1 * FROM dados_agregador ORDER BY Id DESC";
                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            using (SqlDataReader reader = checkCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Console.WriteLine($"Verificação: IPWavy={reader["IPWavy"]}, WavyProcessamento={reader["WavyProcessamento"]}");
                                }
                                else
                                {
                                    Console.WriteLine("Nenhum dado encontrado após inserção.");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nenhuma linha afetada pelo INSERT!");
                    }
                }


            }




        }
    }
}