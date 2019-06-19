using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace JNET
{

    public class JConnecter
    {
        private JReceiveEvt Receive_Evt = new JReceiveEvt();
        public static string UseName
        {
            get { return _UseName; }
            set { _UseName = value; }
        }
        private static int J_PORT = 20000;
        //포트입력하면 기본 IP와 연결되는것만 구현
        public bool Connect()
        {
            return Connect(J_PORT);
        }
        public bool Connect(int port)
        {
            return Connect(Dns.GetHostName(), port);
        }
        public bool Connect(string host, int port)
        {
            IPHostEntry hostIp = null;
            try
            {
                hostIp = Dns.GetHostEntry(host);
            }
            catch (Exception ex)
            {
                hostIp = Dns.GetHostByAddress(host);
            }
            //ip검색
            foreach (IPAddress addr in hostIp.AddressList)
            {
                //컨티뉴 사용해서 조건문이 맞으면 다음 단계로 넘어가도록 
                if (addr.AddressFamily != AddressFamily.InterNetwork)
                {
                    continue;
                }

                //_ip = addr.ToString();
                _ip = "127.0.0.1";
                _port = port;
                Console.WriteLine("IP " + _ip);
                Console.WriteLine("PORT" + _port);
            }
            return Connection();
        }
        private bool Connection()
        {
            //이미 연결되어있으면 여기서 나가도록 
            if (ClientSocket != null && ClientSocket.Connected)
            {
                return false;
            }
            //소켓 설정
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            ClientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            try
            {
                //비동기설정
                SocketAsyncEventArgs ConnectAsync = new SocketAsyncEventArgs();
                ConnectAsync.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
                ConnectAsync.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectCompleted);
                ClientSocket.ConnectAsync(ConnectAsync);
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection Error : {0}", e.Message);
            }
            //쓰레드 제어
            ThreadEvent.WaitOne();

            return ClientSocket.Connected;
        }

        private void ConnectCompleted(object obj, SocketAsyncEventArgs evt)
        {
            //쓰레드 제어연결할땐 다른 쓰레드 발동ㄴㄴdd
            ThreadEvent.Set();
            Console.WriteLine("Connect Test {0}", ClientSocket.Connected);
            if (ClientSocket.Connected)
            {
                ConnectCondition = true;
                try
                {
                    //연결
                    SocketAsyncEventArgs receiveAsync = new SocketAsyncEventArgs();
                    receiveAsync.SetBuffer(RecvBuf, 0, RecvBuf.Length);
                    receiveAsync.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveCompleted);
                    ClientSocket.ReceiveAsync(receiveAsync);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ConnectedCompleted Error :{0}", e.Message);
                }
                _Message = "연결 완료";
            }
            else
            {
                //재접속
                //Reconnect();
                _Message = "연결중..";
            }
        }

        private void ReceiveCompleted(object obj, SocketAsyncEventArgs evt)
        {
            if (false == ClientSocket.Connected || 0 == evt.BytesTransferred)
            {
                //서버로 부터 데이터를 0받을시
                Reconnect();
                return;
            }
            //전체사이즈 
            int Size = evt.BytesTransferred;
            int CSize = BitConverter.ToUInt16(evt.Buffer, 0);
            //실제 사이즈와 적힌 사이즈가 다를경우 방지
            if (Size == CSize)
            {
                //패킷 생성
                GCHandle Handle = GCHandle.Alloc(evt.Buffer, GCHandleType.Pinned);
                JRecvPacket RecvPacket = (JRecvPacket)Marshal.PtrToStructure(Handle.AddrOfPinnedObject(), typeof(JRecvPacket));
                Handle.Free();
                RecvQue.Clear();
                int count = 0;
                //버퍼->큐에저장
                foreach (byte value in RecvPacket.RecvBuf)
                {
                    RecvQue.Enqueue(value);
                    count++;
                    if (RecvPacket.RecvBuf[count] == 0 && RecvPacket.RecvBuf[count - 1] == 0) break;
                }
                //큐에 저장완료
                RecvSucces = true;
            }
            _Message = "Receive Check OKK";
            evt.SetBuffer(RecvBuf, 0, RecvBuf.Length);
            //비동기 재설정
            ClientSocket.ReceiveAsync(evt);
        }
        public void Reconnect()
        {
            //접속상태 ConnectCompleted 안갔으면 접속할 이유없음
            Console.WriteLine("재접속중...");
            if (true == ConnectCondition)
                return;
            Console.WriteLine("서버와 접속을 하는 중입니다...");
            Connection();
        }
        public void Send<P>(P Packet) where P : JPacket
        {
            //byte[] _Buf = new byte[Packet.size];
            byte[] _Buf = { 8, 0, 0, 0, 119, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0 };

            //패킷 생성
            GCHandle handle = GCHandle.Alloc(_Buf, GCHandleType.Pinned);
            Marshal.StructureToPtr(Packet, handle.AddrOfPinnedObject(), false);
            handle.Free();
            //비동기보내기
            SocketAsyncEventArgs SendAsync = new SocketAsyncEventArgs();

            //SendAsync.SetBuffer(_Buf, 0, Packet.size);
            SendAsync.SetBuffer(_Buf, 0, _Buf.Length);
            SendAsync.Completed += new EventHandler<SocketAsyncEventArgs>(SendCompleted);
            ClientSocket.SendAsync(SendAsync);
        }
        private void SendCompleted(object obj, SocketAsyncEventArgs evt)
        {
            evt.Dispose();
            GC.SuppressFinalize(obj);
        }
        public void Disconnect()
        {
            if (null == ClientSocket || false == ClientSocket.Connected)
                return;
            //재접속 안함
            ConnectCondition = false;
            //송,수신 제어
            ClientSocket.Shutdown(SocketShutdown.Both);
            try
            {
                //연결끊기
                SocketAsyncEventArgs DisconnectAsync = new SocketAsyncEventArgs();
                DisconnectAsync.Completed += new EventHandler<SocketAsyncEventArgs>(DisconnectCompleted);
                ClientSocket.DisconnectAsync(DisconnectAsync);
            }
            catch (Exception e)
            {
                Console.WriteLine("Disconnect Error :: {0}", e.Message);
            }
        }
        private void DisconnectCompleted(object obj, SocketAsyncEventArgs evt)
        {
            evt.Dispose();
            GC.SuppressFinalize(obj);
            _Message = "연결 해제 완료";
        }
        //큐
        public static Queue<byte> JQue { get { return RecvQue; } }
        //큐제거하고 반환
        public static byte Deque() { return RecvQue.Dequeue(); }
        //큐 카운트
        public static int Count { get { return RecvQue.Count; } }
        //상태메시지
        public string _Message
        {
            get { return msg; }
            set { msg = value; }
        }
        //변수들
        public static string _UseName = "";
        public static bool RecvSucces = false;
        private Socket ClientSocket = null;
        public static bool ConnectCondition = false;
        private string _ip = "";
        private int _port = 0;
        private byte[] RecvBuf = new byte[4096];
        private string msg;
        private static Queue<byte> RecvQue = new Queue<byte>();
        //쓰레드 제어
        AutoResetEvent ThreadEvent = new AutoResetEvent(false);
    }
}
