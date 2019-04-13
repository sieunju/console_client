using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
//여기는 구조체 입니다. 

namespace JNET
{
    //오 멋있는데? JNETㅋㅋㅋㅋㅋㅋㅋㅋㅋ
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class JPacket
    {
        private enum Enum
        {
            NO_TYPE = 0, MONEY = 10, CASH = 11, TOPAZ = 12, LEV = 13,
            STR_TYPE = 84
        }
        public JPacket() { }
        public JPacket(ushort type)
        {
            this.size = 4096;
            this.type = type;
            Clear();

        }
        //내부적 한정자
        internal ushort size = 0;
        internal ushort type = 0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4092)]
        internal byte[] SendBuf = new byte[4092];
        public void Clear()
        {
            Sucess = true;
            Offset = 0;
        }

        public ushort Add(string Value, ushort Type)
        {
            if (Sucess == false)
                return 0;

            ushort HeadSize = (ushort)Marshal.SizeOf(typeof(ushort));
            ushort Size = (ushort)(Encoding.Unicode.GetByteCount(Value));

            //데이터를 더이상 추가 못할경우 ㅈㅈ
            if (SendBuf.Length < (Offset + Size))
            {
                Sucess = false;
                return 0;
            }
            //크기 입력
            BitConverter.GetBytes(Size).CopyTo(SendBuf, Offset);
            Offset += HeadSize;
            switch (Type)
            {
                case (ushort)Enum.NO_TYPE:
                    break;
                case (ushort)Enum.MONEY:
                    SendBuf[Offset] = (byte)Enum.MONEY;
                    break;
                case (ushort)Enum.CASH:
                    SendBuf[Offset] = (byte)Enum.CASH;
                    break;
                case (ushort)Enum.TOPAZ:
                    SendBuf[Offset] = (byte)Enum.TOPAZ;
                    break;
                case (ushort)Enum.LEV:
                    SendBuf[Offset] = (byte)Enum.LEV;
                    break;
                case (ushort)Enum.STR_TYPE:
                    SendBuf[Offset] = (byte)Enum.STR_TYPE;
                    break;
                default:
                    break;
            }
            Offset += 2;
            //문자열
            Buffer.BlockCopy(Encoding.Unicode.GetBytes(Value), 0, SendBuf, Offset, Size);
            Offset += Size;
            return (ushort)Offset;
        }
        [MarshalAs(UnmanagedType.I1)]
        internal bool Sucess = true;
        internal ushort Offset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class JRecvPacket
    {
        internal ushort size = 0;
        internal ushort type = 0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4092)]
        internal byte[] RecvBuf = new byte[4092];
    }
}

