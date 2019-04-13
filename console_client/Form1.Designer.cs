namespace console_client
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
        }

        #endregion
    }

    /**
     * public partial class Form1 : Form
    {
        JNET.JConnecter Connecter = new JNET.JConnecter();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Connecter.Connect();
            //Connecter.Connect("220.83.146.196",3773);
            Thread.Sleep(10);
            outPut_TextBox.AppendText(Connecter._Message + "\n");
            //접속이 안될시
            Thread.Sleep(100);
            if(!JNET.JConnecter.ConnectCondition)
            {
                outPut_TextBox.AppendText("서버와의 접속이 안됐습니다. 재접속 버튼을 눌러주시길 바랍니다.\n");
            }
        }
        private enum Enum
        {
            NO_TYPE = 0,
            MONEY = 10, CASH = 11, LEV = 12 ,
            STR_TYPE = 84
        }
        private void reconnect_Click1(object sender, EventArgs e)
        {
            outPut_TextBox.AppendText("재접속 시도중입니다...\n");
            Connecter.Reconnect();
            if(JNET.JConnecter.ConnectCondition)
            {
                outPut_TextBox.AppendText("접속 완료입니다. \n");
            }else if(!JNET.JConnecter.ConnectCondition)
            {
                outPut_TextBox.AppendText("서버와의 접속이 안됐습니다. 재접속 버튼을 눌러주시길 바랍니다.\n");
            }
        }
        //메시지 전송
        private void sendMessage_Click(object sender, EventArgs e)
        {
            JNET.JPacket packet = new JNET.JPacket(100);
            packet.Add(message_box.Text, (ushort)Enum.STR_TYPE);

            //for (int i = 0; i < 100000; i++)
                Connecter.Send(packet);

            message_box.Text = "";
            Thread.Sleep(10);
            String str = "";
            JNET.JReceiveEvt.Receive(ref str);
            outPut_TextBox.AppendText("메시지 : " + str + "\n");

        }
        //s=4 전송
        private void packet4_send_Click(object sender, EventArgs e)
        {
            //Connecter.Send(sendEvt.send_fourPacket());
        }
        //s=8 전송
        private void packet8_send_Click(object sender, EventArgs e)
        {
            //Connecter.Send(sendEvt.send_eightPacket());
        }
        //다른사람에게 보내기
        private void send_other_Click(object sender, EventArgs e)
        {

        }
        //텍스트 입력하고 엔터만 눌렀을때 되게끔.
        private void message_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                sendMessage_Click(null, null);
            }
        }
        //연결 해제
        private void disconnect_Click(object sender, EventArgs e)
        {
            Connecter.Disconnect();
            Thread.Sleep(10);
            outPut_TextBox.AppendText(Connecter._Message + "\n");
        }
        //회원가입
        private void sign_up_Click(object sender, EventArgs e)
        {
            JNET.JPacket packet = new JNET.JPacket(200);
            packet.Add(name_Box.Text, (ushort)Enum.NO_TYPE);
            Connecter.Send(packet);

            name_Box.Text = "";
            name_Box.Focus();
        }
        //DB에 데이터 가져오기
        private void db_import_Click(object sender, EventArgs e)
        {
            JNET.JPacket packet = new JNET.JPacket(201);
            packet.Add(name_Box.Text, (ushort)Enum.NO_TYPE);
            //for (int i = 0; i < 100000; i++)
            Connecter.Send(packet);
            name_Box.Text = "";
            name_Box.Focus();
            bool _bstart = true;
            while(_bstart)
            {
                if (JNET.JConnecter.RecvSucces)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        String str = "";
                        JNET.JReceiveEvt.Receive(ref str);
                        outPut_TextBox.AppendText("회원정보 : " + str + "\n");
                    }
                    JNET.JConnecter.RecvSucces = false;
                    _bstart = false;
                }
            }
            
            
         }
        //DB업데이트
        private void db_update_Click(object sender, EventArgs e)
        {
            JNET.JPacket packet = new JNET.JPacket(203);
            String str = "";
            for(int i=0; i< 20; i++)
            {
                str += "가나다라다달댜저뱌ㅐ거ㅕㅂㅈ댜ㅐ겹ㅈㄷㄱㅂㅈㄷㄱㅂㅈㄷㄱ";
            }
            packet.Add(str, (ushort)Enum.NO_TYPE);
            //packet.Add(name_Box.Text, (ushort)Enum.NO_TYPE);

            if (!money_Box.Text.Equals(""))
            {
                packet.Add(money_Box.Text, (ushort)Enum.MONEY);
            }
            if (!cash_Box.Text.Equals(""))
            {
                packet.Add(cash_Box.Text, (ushort)Enum.CASH);
            }
            if (!lev_Box.Text.Equals(""))
            {
                packet.Add(lev_Box.Text, (ushort)Enum.LEV);
            }
            //for(int i = 0; i<100000; i++)
            Connecter.Send(packet);

            name_Box.Text = "";
            money_Box.Text = "";
            cash_Box.Text = "";
            lev_Box.Text = "";
        }
        //영웅정보가져오기
        private void hero_Import_Click(object sender, EventArgs e)
        {
            JNET.JPacket packet = new JNET.JPacket(300);
            packet.Add(name_Box.Text, (ushort)Enum.NO_TYPE);
            Connecter.Send(packet);
            bool _bstart = true;
            while (_bstart)
            {
                if (JNET.JConnecter.RecvSucces)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        String str = "";
                        JNET.JReceiveEvt.Receive(ref str);
                        outPut_TextBox.AppendText("영웅정보 : " + str + "\n");
                    }
                    JNET.JConnecter.RecvSucces = false;
                    _bstart = false;
                }
            }
        }
    }
     */
}

