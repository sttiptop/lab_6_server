using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace lab_6_server
{
    public partial class Form1 : Form
    {
        int ccc = 0;
        String fileName = "D:\\text.txt";
        int fileCount = 0;
        //Создание объекта класса TcpListener 
        TcpListener listener = null; //Создание объекта класса Socket 
        Socket socket = null;
        //Создание объекта класса NetworkStream 
        NetworkStream ns = null;
        //Создание объекта класса кодировки ASCIIEncoding 
        ASCIIEncoding ae = null;

        public Form1()
        {
            InitializeComponent();
            listener = new TcpListener(IPAddress.Any, 5555);
            try
            {


            }
            catch (Exception q)
            {
                info.Text += q.ToString();
            }


        }
        private void start_button(object sender, EventArgs e)
        {
            info.Clear();
            listener.Start();
            socket = listener.AcceptSocket();
            info.Text += "start lestinng\n";
            if (socket.Connected)
            {
                info.Text += "new connect";
                ns = new NetworkStream(socket);
                ae = new ASCIIEncoding();
                //Создаем новый экземпляр класса ThreadClass 
                ThreadClass threadClass = new ThreadClass();
                //Создаем новый поток 
                Thread thread = threadClass.Start(ns, fileName, fileCount, this);
            }

        }
        public class ThreadClass
        {
            String fileName;
            int fileCount = 0;
            TcpListener listener = null; //Создание объекта класса Socket 
            Socket socket = null;
            //Создание объекта класса NetworkStream 
            NetworkStream ns = null;
            //Создание объекта класса кодировки ASCIIEncoding 
            ASCIIEncoding ae = null;
            Form1 form = null;
            public Thread Start(NetworkStream ns, String fileName, int fileCount, Form1 form)
            {
                this.ns = ns;
                ae = new ASCIIEncoding();
                this.fileName = fileName;

                this.fileCount = fileCount;
                this.form = form;
                //Создание нового экземпляра класса Thread 
                Thread thread = new Thread(new ThreadStart(ThreadOperations));
                //Запуск потока 
                thread.Start();
                return thread;
            }
            public void ThreadOperations()
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
                        if (cmd.CompareTo("view") == 0)
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
                        }
                    }
                }catch(Exception t)
                {
                    //form.info.Clear();
                    form.info.Text += "failed read";
                }
            }


        }
    }
}
