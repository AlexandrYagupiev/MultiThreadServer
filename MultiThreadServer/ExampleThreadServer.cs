using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace MultiThreadServer
{
    class ExampleThreadServer
    {
        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                var MaxThreadsCount = Environment.ProcessorCount * 4;
                //максимльное количество рабочих потоков
                ThreadPool.SetMaxThreads(MaxThreadsCount,MaxThreadsCount);
                //максимальное количество потоков
                ThreadPool.SetMinThreads(2, 2);

                //порт для TcpListener
                var port = 9595;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                var counter = 0;
                server = new TcpListener(localAddr, port);

                Console.WriteLine("Конфигурация многопоточного сервера");
                Console.WriteLine("IP-адрес: " + localAddr.ToString());
                Console.WriteLine("Порт: "+port.ToString());
                Console.WriteLine("Потоки: "+MaxThreadsCount.ToString());
                Console.WriteLine("\nСервер запущен\n");

                //Запуск TcpListener и прослушка клиента
                server.Start();

                //принимаем клиентов в бесконечном цикле
                while (true)
                {
                    Console.Write("\nОжидаем соединения...");
                    ThreadPool.QueueUserWorkItem(ClientProcessing, server.AcceptTcpClient());
                    //вывод информации о подключении
                    counter++;
                    Console.Write("\nСоединение №"+counter.ToString());
                }
            }
            catch (SocketException e) 
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                //остановка сервера
                server.Stop();
            }

            Console.WriteLine("\nНажмите Enter...");
            Console.Read();
        }

        static void ClientProcessing(object client_obj)
        {
            //буфер принимаймых данных
            var bytes = new byte[256];
            string data = null;

            TcpClient client = client_obj as TcpClient;

            data = null;

            //получаем информацию от клиента
            NetworkStream stream = client.GetStream();

            var i = 0;

            //принимаем данные от клиента
            while((i=stream.Read(bytes,0,bytes.Length))!=0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes,0,i);
                data = data.ToUpper();
                var msg = System.Text.Encoding.ASCII.GetBytes(data);
                //отправляем данные клиенту(ответ)
                stream.Write(msg,0,msg.Length);
            }
            client.Close();
        }
    }
}
