using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace instructiontool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // テキストボックスからCOMポート番号を取得
            string comNo = textBox2.Text.Trim();
            if (string.IsNullOrEmpty(comNo) || !int.TryParse(comNo, out _))
            {
                MessageBox.Show("有効なCOMポート番号を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string portName = $"COM{comNo}"; // 使用するCOMポート
            //string portName = "COM3"; // 使用するCOMポート
            int baudRate = 9600;      // ボーレート

            // 送信するコマンドバイト列
            byte[] command = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x11, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x2A };
            byte[] response = new byte[11];

            try
            {
                using (SerialPort serialPort = new SerialPort(portName, baudRate))
                {
                    // シリアルポートの設定
                    serialPort.Parity = Parity.None;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = StopBits.One;
                    serialPort.Handshake = Handshake.None;

                    // シリアルポートを開く
                    serialPort.Open();

                    // コマンド送信
                    serialPort.Write(command, 0, command.Length);
                    MessageBox.Show("コマンドが送信されました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox3.Text = BitConverter.ToString(command);

                    // コマンド送信
                    serialPort.Read(response, 0, response.Length);
                    textBox4.Text = BitConverter.ToString(response);

                    // シリアルポートを閉じる
                    serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
