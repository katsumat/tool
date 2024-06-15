using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using OfficeOpenXml;

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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string fileName = textBox5.Text.Trim();
            string sheetName = textBox6.Text.Trim();
            string lineNoStr = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(sheetName) || string.IsNullOrEmpty(lineNoStr))
            {
                MessageBox.Show("���ׂẴt�B�[���h�ɒl�����Ă�������", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string path = $"./{fileName}.xlsx";
            if (!int.TryParse(lineNoStr, out int lineNo))
            {
                MessageBox.Show("�L���ȍs�ԍ�����͂��Ă��������B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var package = new ExcelPackage(new System.IO.FileInfo(path)))
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets[sheetName];
                    if (worksheet == null)
                    {
                        MessageBox.Show("�w�肳�ꂽ�V�[�g��������܂���B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    for (int i = 1; i <= rowCount; i++)
                    {
                        if (worksheet.Cells[i, 5].Value != null && int.TryParse(worksheet.Cells[i, 5].Text, out int cellLineNo) && cellLineNo == lineNo)
                        {
                            byte[] command = new byte[13];
                            for (int j = 0; j < 13; j++)
                            {
                                var cellValue = worksheet.Cells[i, 6 + j].Value;
                                if (cellValue == null)
                                {
                                    throw new Exception("�Z���̒l����ł��B");
                                }

                                if (cellValue is string)
                                {
                                    command[j] = Convert.ToByte(cellValue.ToString(), 16);
                                }
                                else if (cellValue is int || cellValue is double)
                                {
                                    command[j] = Convert.ToByte(cellValue);
                                }
                                else
                                {
                                    throw new Exception($"�\�����Ȃ��Z���̒l: {cellValue}");
                                }
                            }

                            // �R�}���h���M
                            SendCommand(command);

                            // �ҋ@
                            if (worksheet.Cells[i, 19].Value != null && double.TryParse(worksheet.Cells[i, 19].Text, out double delay))
                            {
                                Thread.Sleep((int)(delay * 1000));
                            }
                        }
                    }

                    package.SaveAs(new System.IO.FileInfo("output.xlsx"));
                }

                System.Diagnostics.Process.Start("output.xlsx");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�G���[: {ex.Message}", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void SendCommand(byte[] command)
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
            //byte[] command = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x11, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x2A };
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
            //COMNo
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //���M�s

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //���X�|���X
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //���M�R�}���h
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            //�t�@�C����
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
