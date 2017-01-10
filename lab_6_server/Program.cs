using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
namespace lab_6_server
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        public static void ReceiveCallback(IAsyncResult AsyncCall)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Byte[] message = encoding.GetBytes("Я занят");

            Socket listener = (Socket)AsyncCall.AsyncState;
            Socket client = listener.EndAccept(AsyncCall);

            Console.WriteLine("Принято соединение от: {0}", client.RemoteEndPoint);
            client.Send(message);

            Console.WriteLine("Закрытие соединения");
            client.Close();

            // После того как завершили соединение, говорим ОС что мы готовы принять новое
            listener.BeginAccept(new AsyncCallback(ReceiveCallback), listener);
        }

        public static void Main()
        {
            String fileName = "D:\\text.txt";
            int fileCount = 0;

            try
            {
                // Application.EnableVisualStyles();
                //  Application.SetCompatibleTextRenderingDefault(false);
                //  Form1 f = new Form1();
                //  Application.Run(f);
                TcpListener listener = new TcpListener(IPAddress.Any, 2200); //Создание объекта класса Socket 
                Socket socket = null;
                //Создание объекта класса NetworkStream 
                NetworkStream ns = null;
                //Создание объекта класса кодировки ASCIIEncoding 
                ASCIIEncoding ae = null;
                while (true)
                {
                    listener.Start();
                    socket = listener.AcceptSocket();

                    if (socket.Connected)
                    {
                        ns = new NetworkStream(socket);
                        Console.WriteLine("new connect");
                        ae = new ASCIIEncoding();
                        ThreadClass threadClass = new ThreadClass();
                        Thread thread = threadClass.Start(ns, fileName, fileCount);
                    }
                }
            }
            catch
            {

            }

        }

        public class ThreadClass
        {
            String fileName;
            int fileCount = 0;

            //Создание объекта класса NetworkStream 
            NetworkStream ns = null;
            //Создание объекта класса кодировки ASCIIEncoding 
            ASCIIEncoding ae = null;
            public Thread Start(NetworkStream ns, String fileName, int fileCountm)
            {
                this.ns = ns;
                ae = new ASCIIEncoding();
                this.fileName = fileName;

                this.fileCount = fileCountm;

                //Создание нового экземпляра класса Thread 
                Thread thread = new Thread(new ThreadStart(ThreadOperations));
                //Запуск потока 
                thread.Start();
                return thread;
            }
            public void ThreadOperations()
            {
                while (true)
                {
                    byte[] received = new byte[256]; //С помощью сетевого потока считываем в переменную received данные от клиента 
                    try
                    {

                        ns.Read(received, 0, received.Length);
                        String s1 = ae.GetString(received);
                        int i = s1.IndexOf("|", 0);
                        if (i > 0)
                        {
                            String cmd = s1.Substring(0, i);
                            s1=s1.Remove(0, i+1);
                            Console.WriteLine(s1);
                            if (cmd.CompareTo("show") == 0)
                            {
                                // Создаем переменную типа byte[] для отправки ответа клиенту 
                                byte[] sent = new byte[256];
                                //Создаем объект класса FileStream для последующего чтения информации из файла 
                                FileStream fstr = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                                StreamReader sr = new StreamReader(fstr);
                                //Запись в переменную sent содержания прочитанного файла 
                                sent = ae.GetBytes(sr.ReadToEnd());
                                sr.Close();
                                fstr.Close(); //Отправка информации клиенту 
                                ns.Write(sent, 0, sent.Length);
                                s1 = "";
                                //listener.BeginAcceptSocket(AsyncCallback())
                            }
                            if (cmd.CompareTo("delete") == 0)
                            {
                                // Создаем переменную типа byte[] для отправки ответа клиенту 
                                byte[] sent = new byte[256];
                                List<String> workersList = System.IO.File.ReadLines("D:\\text.txt").ToList();
                                i = s1.IndexOf("|", 0); ;
                                String position = s1.Substring(0, i);
                                s1 = s1.Remove(0, i + 1);
                                i = s1.IndexOf("|", 0);
                                String FirstName= s1.Substring(0, i);
                                s1 = s1.Remove(0, i + 1);
                                i = s1.IndexOf("|", 0);
                                String SecondName= s1.Substring(0, i);
                                s1 = s1.Remove(0, i + 1);
                                i = s1.IndexOf("|", 0);
                                String Name= s1.Substring(0, i);
                                workersList.ForEach(delegate (String values)
                                {
                                    if (values.ToUpper().Contains(position.ToUpper()))
                                        if (values.ToUpper().Contains(FirstName.ToUpper()))
                                            if (values.Contains(SecondName))
                                                if (values.Contains(Name))
                                                {
                                                    workersList.Remove(values);
                                                    System.IO.File.WriteAllLines("D:\\text.txt", workersList);
                                                }
                                });
                                s1 = "";
                                //Запись в переменную sent содержания прочитанного файла 
                                sent = ae.GetBytes("deleted");
                                ns.Write(sent, 0, sent.Length);
                                //listener.BeginAcceptSocket(AsyncCallback())
                            }
                            if (cmd.CompareTo("add") == 0)
                            {
                                // Создаем переменную типа byte[] для отправки ответа клиенту 
                                byte[] sent = new byte[256];
                                List<String> workersList = System.IO.File.ReadLines("D:\\text.txt").ToList();
                                i = s1.IndexOf("|", 0); ;
                                String position = s1.Substring(0, i);
                                s1.Remove(0, i+1);
                                i = s1.IndexOf("|", 0);
                                String FirstName = s1.Substring(0, i);
                                s1.Remove(0, i+1);
                                i = s1.IndexOf("|", 0);
                                String SecondName = s1.Substring(0, i);
                                s1.Remove(0, i+1);
                                i = s1.IndexOf("|", 0);
                                String Name = s1.Substring(0, i);
                                String newWorker = position + " " + FirstName + " " + SecondName + " " + Name;
                                if (!workersList.Contains(newWorker, StringComparer.CurrentCultureIgnoreCase))
                                {
                                    workersList.Add(newWorker);
                                    System.IO.File.WriteAllLines("D:\\text.txt", workersList);

                                }
                                s1 = "";
                                //Запись в переменную sent содержания прочитанного файла 
                                sent = ae.GetBytes("add");
                                ns.Write(sent, 0, sent.Length);
                                //listener.BeginAcceptSocket(AsyncCallback())
                            }
                            if (cmd.CompareTo("search") == 0)
                            {
                                // Создаем переменную типа byte[] для отправки ответа клиенту 
                                byte[] sent = new byte[256];
                                List<String> workersList = System.IO.File.ReadLines("D:\\text.txt").ToList();
                                i = s1.IndexOf("|", 0); ;
                                String param = s1.Substring(0, i);
                                List<String> workersListSort=new List<String>();
                                String send="";

                                workersList.ForEach(delegate (String values)
                                {
                                    //values.StartsWith
                                    if (values.ToUpper().Contains(param.ToUpper()))
                                    {
                                    send+=(values)+"\n";
                                    }
                                });
                                s1 = "";
                                //Запись в переменную sent содержания прочитанного файла 
                                sent = ae.GetBytes(send);
                                ns.Write(sent, 0, sent.Length);
                                //listener.BeginAcceptSocket(AsyncCallback())
                            }
                        }

                    }
                    catch (Exception t)
                    {
                        //form.info.Clear();
                        //form.info.Text += "failed read";
                    }
                }
            }
        }
    }
}
