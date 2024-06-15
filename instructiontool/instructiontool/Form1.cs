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
            // �e�L�X�g�{�b�N�X����COM�|�[�g�ԍ����擾
            string comNo = textBox2.Text.Trim();
            if (string.IsNullOrEmpty(comNo) || !int.TryParse(comNo, out _))
            {
                MessageBox.Show("�L����COM�|�[�g�ԍ�����͂��Ă��������B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string portName = $"COM{comNo}"; // �g�p����COM�|�[�g
            //string portName = "COM3"; // �g�p����COM�|�[�g
            int baudRate = 9600;      // �{�[���[�g

            // ���M����R�}���h�o�C�g��
            byte[] command = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x11, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x2A };
            byte[] response = new byte[11];

            try
            {
                using (SerialPort serialPort = new SerialPort(portName, baudRate))
                {
                    // �V���A���|�[�g�̐ݒ�
                    serialPort.Parity = Parity.None;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = StopBits.One;
                    serialPort.Handshake = Handshake.None;

                    // �V���A���|�[�g���J��
                    serialPort.Open();

                    // �R�}���h���M
                    serialPort.Write(command, 0, command.Length);
                    MessageBox.Show("�R�}���h�����M����܂����B", "���", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox3.Text = BitConverter.ToString(command);

                    // �R�}���h���M
                    serialPort.Read(response, 0, response.Length);
                    textBox4.Text = BitConverter.ToString(response);

                    // �V���A���|�[�g�����
                    serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�G���[: {ex.Message}", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
