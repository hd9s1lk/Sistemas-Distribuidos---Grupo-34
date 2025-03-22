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
        Console.WriteLine("Boas");
        Console.WriteLine("Vamos");
        Console.WriteLine("Ganha-la?");
        Thread.Sleep(3000);
        Console.WriteLine("O Thread acabou");
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
            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("Agregador conectado!");

            string response = "Mensagem recebida com sucesso!";
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            clientSocket.Send(responseBytes);

            clientSocket.Close();
        }
    }
}