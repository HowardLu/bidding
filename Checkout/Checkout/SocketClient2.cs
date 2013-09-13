using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Checkout
{
    public class SocketClient2
    {
        // The response from the remote device.
        public String m_responseString = String.Empty;
        private TcpClient m_tcpClient;
        private NetworkStream m_nwStream;
        private int m_port = 5566;
        int m_bufferSize = 256;

        public string IP { get; set; }
        public bool IsConnected { get { return m_tcpClient.Connected; } }

        public SocketClient2(string ip, int port)
        {
            IP = ip;
            m_port = port;
        }

        public bool Connect(string ip)
        {
            try
            {
                if (m_tcpClient != null)
                    m_tcpClient.Close();
                m_tcpClient = new TcpClient();
                IP = ip;
                m_tcpClient.Connect(IP, m_port);
                Console.WriteLine("ip " + IP + " port " + m_port.ToString() + " Connected");
                m_nwStream = m_tcpClient.GetStream();
                Send("payment");
                Receive();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(IP + " " + m_port.ToString() + " Connection failed");
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        public void Close()
        {
            if (m_tcpClient != null)
            {
                m_tcpClient.Close();
            }
        }

        public bool Send(string str)
        {
            if (m_tcpClient.Connected && m_nwStream.CanWrite)
            {
                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(str);
                    m_nwStream.Write(buffer, 0, buffer.Length);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }

        public void Receive()
        {
            if (!m_tcpClient.Connected || !m_nwStream.CanRead)
                return;

            List<byte> readBytes = new List<byte>();
            int readCount = 1;
            byte[] buffer = new byte[m_bufferSize];
            while (readCount != 0)
            {
                readCount = m_nwStream.Read(buffer, 0, m_bufferSize);
                if (readCount < m_bufferSize)
                {
                    for (int i = 0; i < readCount; i++)
                        readBytes.Add(buffer[i]);
                    break;
                }
                else
                {
                    readBytes.AddRange(buffer);
                }
            }

            m_responseString = Encoding.UTF8.GetString(readBytes.ToArray(), 0, readBytes.Count);
            Console.WriteLine(m_responseString);
        }
    }
}
