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

        Console.WriteLine($"Agregador na porta {listenPort}...");

        while (true)
        {
            Socket clientSocket = middlemanSocket.Accept();
            Console.WriteLine("Cliente conectado ao Agregador!");

            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string receivedText = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            Console.WriteLine($"Mensagem recebida do cliente: {receivedText}");

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                serverSocket.Connect(new IPEndPoint(IPAddress.Parse(serverIP), serverPort));
                serverSocket.Send(buffer, receivedBytes, SocketFlags.None);

                // Receber info do cliente
                //byte[] responseBuffer = new byte[1024];
                //int responseBytes = clientSocket.Receive(responseBuffer);
                //string serverResponse = Encoding.UTF8.GetString(responseBuffer, 0, responseBytes);
                //Console.WriteLine($"Resposta do cliente: {serverResponse}");


                //Trocar IP's
                byte[] calculo = new byte[1024];
                int somaBytes = clientSocket.Receive(calculo);
                string receivedCalculo = Encoding.UTF8.GetString(calculo, 0, somaBytes);
                string[] soma = receivedCalculo.Split(" ");
                if (soma[0] == "Dados")
                {
                    string respostaFinal = "IP AGREGADOR: 127.0.0.1:800";
                    byte[] resposta = Encoding.UTF8.GetBytes(respostaFinal);
                    clientSocket.Send(resposta);
                    serverSocket.Send(resposta);
                } else
                {
                    string respostaFinal = "Dados Não Recebidos";
                    byte[] resposta = Encoding.UTF8.GetBytes(respostaFinal);
                    clientSocket.Send(resposta);
                }


                //receber e enviar wavyID
                byte[] wavyID = new byte[1024];
                int wavyIDBytes = clientSocket.Receive(wavyID);
                string wavyIDdados = Encoding.UTF8.GetString(wavyID, 0, wavyIDBytes);

                //trabalhar o wavyID


                string[] wavyIDFINAL = wavyIDdados.Split(":");
                if (wavyIDFINAL[0] == "WAVY_ID[1]" || wavyIDFINAL[0] == "WAVY_ID[2]" || wavyIDFINAL[0] == "WAVY_ID[1234]" || wavyIDFINAL[0] == "3456")
                {
                    Console.WriteLine($"{wavyIDdados}");
                    string respostaFinal = wavyIDdados;
                    byte[] resposta = Encoding.UTF8.GetBytes(respostaFinal);
                    clientSocket.Send(resposta);
                    serverSocket.Send(resposta);
                } else
                {
                    string respostaFinal = "WavyID não recebida";
                    byte[] resposta = Encoding.UTF8.GetBytes(respostaFinal);
                    clientSocket.Send(resposta);
                }


                //receber e enviar wavyStatus
                byte[] wavyStatus = new byte[1024]; 
                int wavyStatusBytes = clientSocket.Receive(wavyStatus);
                string wavyStatusdados = Encoding.UTF8.GetString(wavyStatus, 0, wavyStatusBytes);

                //Trabalhar wavyStatusDados



                string[] wavyStatusFINAL = wavyStatusdados.Split(":");
                Console.WriteLine($"{wavyStatusdados}");
                if (wavyStatusFINAL[0] == "WAVY_ID[1]" || wavyStatusFINAL[0] == "WAVY_ID[2]" || wavyStatusFINAL[0] == "WAVY_ID[1234]" || wavyStatusFINAL[0] == "3456")
                {
                    while (true)
                    {
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
                                    wavyStatusFINAL[1] = "Associada";
                                    Console.WriteLine($"{wavyStatusFINAL}");
                                    string answer1 = String.Join(":", wavyStatusFINAL);
                                    answer = Encoding.UTF8.GetBytes(answer1);
                                    clientSocket.Send(answer);
                                    Console.WriteLine("Estado alterado: " + answer1);
                                    serverSocket.Send(answer);
                                    break;
                                case "2":
                                    wavyStatusFINAL[1] = "Operação";
                                    string answer2 = String.Join(":", wavyStatusFINAL);
                                    answer = Encoding.UTF8.GetBytes(answer2);
                                    clientSocket.Send(answer);
                                    Console.WriteLine("Estado alterado " + answer2);
                                    serverSocket.Send(answer);
                                    break;
                                case "3":
                                    wavyStatusFINAL[1] = "Manutenção";
                                    string answer3 = String.Join(":", wavyStatusFINAL);
                                    answer = Encoding.UTF8.GetBytes(answer3);
                                    clientSocket.Send(answer);
                                    Console.WriteLine("Estado alterado " + answer3);
                                    serverSocket.Send(answer);
                                    break;
                                case "4":
                                    wavyStatusFINAL[1] = "Desativada";
                                    string answer4 = String.Join(":", wavyStatusFINAL);
                                    answer = Encoding.UTF8.GetBytes(answer4);
                                    clientSocket.Send(answer);
                                    Console.WriteLine("Estado alterado " + answer4);
                                    serverSocket.Send(answer);
                                    break;

                                default:
                                    continue;
                            }
                            break;
                        } else if (leitura != "Sim")
                        {
                            string respostaFinal = wavyStatusdados;
                            byte[] resposta = Encoding.UTF8.GetBytes(respostaFinal);
                            clientSocket.Send(resposta);
                            break;

                        }
                    }
                    
                }
                else
                {
                    string respostaFinal = "WavyID não recebida";
                    byte[] resposta = Encoding.UTF8.GetBytes(respostaFinal);
                    clientSocket.Send(resposta);
                }

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