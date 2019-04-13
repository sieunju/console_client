using System;
using System.Text;
namespace JNET
{
    public class JReceiveEvt
    {
        private enum Enum
        {
            STR_TYPE = 84
        }
        public static bool Receive(ref String Msg)
        {
            if (JConnecter.Count == 1 || JConnecter.Count == 0) { Msg = "NULL"; return false; }
            int Cnt = 0;
            byte[] sizeBuf = new byte[2];
            sizeBuf[0] = JConnecter.Deque();
            sizeBuf[1] = JConnecter.Deque();
            byte[] typeBuf = new byte[2];
            typeBuf[0] = JConnecter.Deque();
            typeBuf[1] = JConnecter.Deque();

            short Size = BitConverter.ToInt16(sizeBuf, 0);
            short Type = BitConverter.ToInt16(typeBuf, 0);
            //int Size = JConnecter.Deque();
            //int Type = JConnecter.Deque();
            byte[] RecvBuf = new byte[Size];
            //RecvBuf 에다가 원하는 만큼 큐값 반환
            foreach (byte Value in JConnecter.JQue)
            {
                if (Cnt == Size)
                {
                    for (int i = 0; i < Size; i++) { JConnecter.Deque(); }
                    break;
                }
                RecvBuf[Cnt++] = Value;
            }
            switch (Type)
            {
                case (int)Enum.STR_TYPE:
                    Msg = Encoding.Unicode.GetString(RecvBuf).Trim('\0');
                    break;
                default:
                    Msg = Encoding.ASCII.GetString(RecvBuf).Trim('\0');
                    break;
            }
            return true;
        }
    }
}