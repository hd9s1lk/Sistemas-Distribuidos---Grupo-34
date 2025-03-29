using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;


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
            string[] ipwavyFinal = ipwavydados.Split(":");
            Console.WriteLine($"{ipwavydados}");

            //recebe wavy id com pre processamento
            byte[] wavyProcessamento = new byte[1024];
            int wavyProcessamentoBytes = clientSocket.Receive(wavyProcessamento);
            string wavyProcessamentoDados = Encoding.UTF8.GetString(wavyProcessamento, 0, wavyProcessamentoBytes);
            string[] wavyProcessamentoFinal = wavyProcessamentoDados.Split(":");
            Console.WriteLine($"{wavyProcessamentoDados}");


            //Status da Wavy recebido pelo agregador
            byte[] agregadorStatus = new byte[1024];
            int agregadorStatusBytes = clientSocket.Receive(agregadorStatus);
            string agregadorStatusDados = Encoding.UTF8.GetString(agregadorStatus, 0, agregadorStatusBytes);
            string[] agregadorFinal = agregadorStatusDados.Split(":");
            Console.WriteLine($"{agregadorStatusDados}");


            if (File.Exists(filePath))
            {
                Console.WriteLine("Dados guardados no ficheiro");
                string dados = $"{agregadorStatusDados}{Environment.NewLine}{wavyProcessamentoDados}";
                File.WriteAllText(filePath, dados);
            }
            else
            {
                Console.WriteLine("Ficheiro não existe");
            }



        }
    }
}