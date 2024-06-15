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
                MessageBox.Show("すべてのフィールドに値を入れてください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string path = $"./{fileName}.xlsx";
            if (!int.TryParse(lineNoStr, out int lineNo))
            {
                MessageBox.Show("有効な行番号を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        MessageBox.Show("指定されたシートが見つかりません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    throw new Exception("セルの値が空です。");
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
                                    throw new Exception($"予期しないセルの値: {cellValue}");
                                }
                            }

                            // コマンド送信
                            SendCommand(command);

                            // 待機
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
                MessageBox.Show($"エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void SendCommand(byte[] command)
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
            //byte[] command = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x11, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x2A };
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
            //COMNo
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //送信行

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //レスポンス
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //送信コマンド
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            //ファイル名
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
