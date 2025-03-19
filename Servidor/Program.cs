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

class SocketServer
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
        // Wait until it is safe to enter.
        Console.WriteLine("{0} is requesting the mutex",
                          Thread.CurrentThread.Name);
        mut.WaitOne();

        Console.WriteLine("{0} has entered the protected area",
                          Thread.CurrentThread.Name);

        // Place code to access non-reentrant resources here.

        // Simulate some work.
        Thread.Sleep(500);

        Console.WriteLine("{0} is leaving the protected area",
            Thread.CurrentThread.Name);

        // Release the Mutex.
        mut.ReleaseMutex();
        Console.WriteLine("{0} has released the mutex",
            Thread.CurrentThread.Name);
    }

static void Main()
    {
        for (int i=0; i< numThreads; i++)
        {
            Thread newThread = new Thread(new ThreadStart(ThreadProc));
            newThread.Name = String.Format("Thread{0}", i + 1);
            newThread.Start();
        }

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
            Console.WriteLine("Cliente conectado!");

            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string receivedText = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

            byte[] calculo = new byte[1024];
            int somaBytes = clientSocket.Receive(calculo);
            string receivedCalculo = Encoding.UTF8.GetString(calculo, 0, somaBytes);
            string[] soma = receivedCalculo.Split(" ");
            if (soma[0] == "SOMA")
            {
                int n1 = Convert.ToInt32(soma[1]);
                int n2 = Convert.ToInt32(soma[2]);
                string total = Convert.ToString(n1 + n2);
                byte[] resposta = Encoding.UTF8.GetBytes(total);
                clientSocket.Send(resposta);
            }
            else if (soma[0]== "MUL"){
                int n1 = Convert.ToInt32(soma[1]);
                int n2 = Convert.ToInt32(soma[2]);
                string total = Convert.ToString(n1 * n2);
                byte[] resposta = Encoding.UTF8.GetBytes(total);
                clientSocket.Send(resposta);
            } else if (soma[0] == "SUB")
            {
                int n1 = Convert.ToInt32(soma[1]);
                int n2 = Convert.ToInt32(soma[2]);
                string total = Convert.ToString(n1 - n2);
                byte[] resposta = Encoding.UTF8.GetBytes(total);
                clientSocket.Send(resposta);
            } else if (soma[0] == "DIV")
            {
                int n1 = Convert.ToInt32(soma[1]);
                int n2 = Convert.ToInt32(soma[2]);
                string total = Convert.ToString(n1 / n2);
                byte[] resposta = Encoding.UTF8.GetBytes(total);
                clientSocket.Send(resposta);
            } else
            {
                string respostaFinal = "ERRO";
                byte[] resposta = Encoding.UTF8.GetBytes(respostaFinal);
                clientSocket.Send(resposta);
            }

                Console.WriteLine($"Mensagem recebida: {receivedText}");

            string response = "Mensagem recebida com sucesso!";
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            clientSocket.Send(responseBytes);

            clientSocket.Close();
        }
    }
}