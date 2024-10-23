//Form1.cs
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace ESP8266Controller
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
        }

        private void InitializeSerialPort()
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = 115200;
            serialPort.DataReceived += SerialPort_DataReceived;

            RefreshComPorts();
        }

        private void RefreshComPorts()
        {
            comboBoxPorts.Items.Clear();
            foreach (string port in SerialPort.GetPortNames())
            {
                comboBoxPorts.Items.Add(port);
            }
            if (comboBoxPorts.Items.Count > 0)
            {
                comboBoxPorts.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                DisconnectPort();
            }
            else
            {
                ConnectPort();
            }
        }

        private void ConnectPort()
        {
            if (comboBoxPorts.SelectedItem == null)
            {
                MessageBox.Show("���� �����, ������� COM-����.");
                return;
            }

            try
            {
                serialPort.PortName = comboBoxPorts.SelectedItem.ToString();
                serialPort.Open();
                buttonConnect.Text = "³��������";
                comboBoxPorts.Enabled = false;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("������ �� ����� ����������. �������, ���� ��� ��������������� ����� ��������� ��� � ��� ����������� ����.");
                ClosePortIfOpen();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������� ��� ������� �����: {ex.Message}");
                ClosePortIfOpen();
            }
        }

        private void DisconnectPort()
        {
            ClosePortIfOpen();
            buttonConnect.Text = "ϳ��������";
            comboBoxPorts.Enabled = true;
        }

        private void ClosePortIfOpen()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private void buttonLED1_Click(object sender, EventArgs e)
        {
            SendCommand(buttonLED1.Text == "LED 1 ON" ? "5ON" : "5OFF");
            buttonLED1.Text = buttonLED1.Text == "LED 1 ON" ? "LED 1 OFF" : "LED 1 ON";
        }

        private void buttonLED2_Click(object sender, EventArgs e)
        {
            SendCommand(buttonLED2.Text == "LED 2 ON" ? "4ON" : "4OFF");
            buttonLED2.Text = buttonLED2.Text == "LED 2 ON" ? "LED 2 OFF" : "LED 2 ON";
        }

        private void SendCommand(string command)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.WriteLine(command);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������� ��� �������� �������: {ex.Message}");
                    DisconnectPort();
                }
            }
            else
            {
                MessageBox.Show("���� �� ��������. ���� �����, ����������� �� COM-�����.");
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine();
                this.Invoke((MethodInvoker)delegate {
                    textBoxReceivedData.AppendText(data + Environment.NewLine);
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show($"������� ��� �������� �����: {ex.Message}");
                    DisconnectPort();
                });
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClosePortIfOpen();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshComPorts();
        }
    }
}