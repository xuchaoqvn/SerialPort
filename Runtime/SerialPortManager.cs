using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Data.SqlTypes;
using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 串口管理器
    /// </summary>
    public class SerialPortManager : ISerialPortManager
    {
        #region Delegate
        /// <summary>
        /// 当串口打开成功时
        /// </summary>
        private EventHandler m_OnOpenSerialPortSucceed;

        /// <summary>
        /// 当串口打开失败时
        /// </summary>
        private EventHandler m_OnOpenSerialPortFailure;

        /// <summary>
        /// 当串口发生错误时
        /// </summary>
        private EventHandler m_OnSerialPortError;

        /// <summary>
        /// 当串口接受到数据时
        /// </summary>
        private EventHandler m_SerialPortReceiveData;

        /// <summary>
        /// 当串口关闭时
        /// </summary>
        private EventHandler m_SerialClosed;
        #endregion

        #region Field
        /// <summary>
        /// 串口的通信类
        /// </summary>
        private System.IO.Ports.SerialPort m_SerialPort;

        /// <summary>
        /// COM口
        /// </summary>
        private string m_PortName;

        /// <summary>
        /// 波特率
        /// </summary>
        private int m_BaudRate;

        /// <summary>
        /// 奇偶校验
        /// </summary>
        private Parity m_Parity;

        /// <summary>
        /// 数据位
        /// </summary>
        private int m_DataBits;

        /// <summary>
        /// 停止位
        /// </summary>
        private StopBits m_StopBits;

        /// <summary>
        /// 每次读取数据的长度
        /// </summary>
        private int m_ReadLength;

        /// <summary>
        /// 当每次读取数据的长度不确定时，用于读取数据的间隔
        /// </summary>
        private float m_ReadInterval;

        /// <summary>
        /// 读取超时时长
        /// </summary>
        private int m_ReadTimeout;

        /// <summary>
        /// 写入超时时长
        /// </summary>
        private int m_WriteTimeout;

        /// <summary>
        /// 编码
        /// </summary>
        private Encoding m_Encoding;

        /// <summary>
        /// 是否打开串口
        /// </summary>
        private bool m_IsOpen;

        /// <summary>
        /// 接受的的数据队列
        /// </summary>
        private ConcurrentQueue<byte[]> m_ReceiveDatas;

        /// <summary>
        /// 发送的数据数量
        /// </summary>
        private int m_SendDataCount;

        /// <summary>
        /// 接受到的数据数量
        /// </summary>
        private int m_ReceivedDataCount;

        /// <summary>
        /// 当接收数据长度不确定时，读取的计时器
        /// </summary>
        private float m_Timer = 0.0f;
        #endregion

        #region Property
        /// <summary>
        /// 获取com口
        /// </summary>
        public string PortName => this.m_PortName;

        /// <summary>
        /// 获取波特率
        /// </summary>
        public int BaudRate => this.m_BaudRate;

        /// <summary>
        /// 获取奇偶校验
        /// </summary>
        public Parity Parity => this.m_Parity;

        /// <summary>
        /// 获取数据位
        /// </summary>
        public int DataBits => this.m_DataBits;

        /// <summary>
        /// 获取停止位
        /// </summary>
        public StopBits StopBits => this.m_StopBits;

        /// <summary>
        /// 获取每次读取数据的长度
        /// </summary>
        public int ReadLength => this.m_ReadLength;

        /// <summary>
        /// 获取当每次读取数据的长度不确定时，用于读取数据的间隔
        /// </summary>
        public float ReadInterval => this.m_ReadInterval;

        /// <summary>
        /// 获取读取超时时长
        /// </summary>
        public int ReadTimeout => this.m_ReadTimeout;

        /// <summary>
        /// 获取写入超时时长
        /// </summary>
        public int WriteTimeout => this.m_WriteTimeout;

        /// <summary>
        /// 获取编码
        /// </summary>
        public Encoding Encoding => this.m_Encoding;

        /// <summary>
        /// 获取是否打开串口
        /// </summary>
        public bool IsOpen => this.m_SerialPort != null && this.m_IsOpen;

        /// <summary>
        /// 获取发送的数据数量
        /// </summary>
        public int SendDataCount => this.m_SendDataCount;

        /// <summary>
        /// 获取接受的数据数量
        /// </summary>
        public int ReceivedDataCount => this.m_ReceivedDataCount;
        #endregion

        #region Event
        /// <summary>
        /// 串口打开成功
        /// </summary>
        public event EventHandler OpenSerialPortSucceed
        {
            add => this.m_OnOpenSerialPortSucceed += value;
            remove => this.m_OnOpenSerialPortSucceed -= value;
        }

        /// <summary>
        /// 串口打开失败
        /// </summary>
        public event EventHandler OpenSerialPortFailure
        {
            add => this.m_OnOpenSerialPortFailure += value;
            remove => this.m_OnOpenSerialPortFailure -= value;
        }

        /// <summary>
        /// 串口错误
        /// </summary>
        public event EventHandler SerialPortError
        {
            add => this.m_OnSerialPortError += value;
            remove => this.m_OnSerialPortError -= value;
        }

        /// <summary>
        /// 收到数据
        /// </summary>
        public event EventHandler ReceiveData
        {
            add => this.m_SerialPortReceiveData += value;
            remove => this.m_SerialPortReceiveData -= value;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public event EventHandler Closed
        {
            add => this.m_SerialClosed += value;
            remove => this.m_SerialClosed -= value;
        }
        #endregion

        #region Function
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="portName">com口</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">奇偶校验</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="readLength">每次读取数据的长度,当不确定是填-1</param>
        /// <param name="readInterval">当每次读取数据的长度不确定时，用于读取数据的间隔</param>
        /// <param name="readTimeout">读取超时时长</param>
        /// <param name="writeTimeout">写入超时时长</param>
        /// <param name="encoding">编码</param>
        public void Initialize(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, int readLength, float readInterval, int readTimeout, int writeTimeout, Encoding encoding)
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            bool exist = false;
            for (int i = 0; i < ports.Length; i++)
            {
                string com = ports[i];
                if (portName == com)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                throw new ArgumentException($"no exist com whih the same name for {portName}");
            }

            this.m_PortName = portName;
            this.m_BaudRate = baudRate;
            this.m_Parity = parity;
            this.m_DataBits = dataBits;
            this.m_StopBits = stopBits;
            this.m_ReadLength = readLength;
            this.m_ReadInterval = readInterval;
            this.m_ReadTimeout = readTimeout;
            this.m_WriteTimeout = writeTimeout;
            this.m_Encoding = encoding;
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void OpenPort()
        {
            if (this.m_SerialPort != null && this.m_SerialPort.IsOpen)
            {
                OpenSerialPortFailureEventArgs openSerialPortFailureEventArgs = new OpenSerialPortFailureEventArgs();
                openSerialPortFailureEventArgs.FailureMessage = "Already Open SerialPort...";
                this.m_OnOpenSerialPortFailure?.Invoke(this, openSerialPortFailureEventArgs);
                openSerialPortFailureEventArgs = null;
                return;
            }

            this.m_SerialPort = new System.IO.Ports.SerialPort(this.m_PortName, this.m_BaudRate, this.m_Parity, this.m_DataBits, this.m_StopBits)
            {
                Encoding = this.m_Encoding,
                ReadTimeout = this.m_ReadTimeout,
                WriteTimeout = this.m_WriteTimeout
            };

            this.m_SerialPort.DtrEnable = true;
            this.m_SerialPort.RtsEnable = true;
            //Unity中不支持
            //this.m_SerialPort.ErrorReceived += this.OnErrorReceived;
            //this.m_SerialPort.DataReceived += this.OnDataReceived;

            try
            {
                this.m_SerialPort.Open();
            }
            catch (Exception exception)
            {
                OpenSerialPortFailureEventArgs openSerialPortFailureEventArgs = new OpenSerialPortFailureEventArgs();
                openSerialPortFailureEventArgs.FailureMessage = exception.ToString();
                this.m_OnOpenSerialPortFailure?.Invoke(this, openSerialPortFailureEventArgs);
                openSerialPortFailureEventArgs = null;
                return;
            }

            this.m_IsOpen = true;
            this.m_ReceiveDatas = new ConcurrentQueue<byte[]>();
            this.m_SendDataCount = 0;
            this.m_ReceivedDataCount = 0;
            this.m_Timer = 0.0f;

            this.m_OnOpenSerialPortSucceed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 串口轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (!this.m_IsOpen)
                return;

            this.ReceiveSerialPortData(realElapseSeconds);

            if (this.m_ReceiveDatas.Count < 0)
                return;

            while (this.m_ReceiveDatas.TryDequeue(out byte[] data))
            {
                SerialPortReceiveDataEventArgs serialPortReceiveDataEventArgs = new SerialPortReceiveDataEventArgs();
                serialPortReceiveDataEventArgs.ReceiveData = data;
                this.m_SerialPortReceiveData?.Invoke(this, serialPortReceiveDataEventArgs);
                serialPortReceiveDataEventArgs = null;
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void ClosePort()
        {
            try
            {
                this.m_SerialPort?.Close();
            }
            catch (Exception exception)
            {
                SerialPortErrorEventArgs serialPortErrorEventArgs = new SerialPortErrorEventArgs();
                serialPortErrorEventArgs.ErrorMessage = exception.ToString();
                this.m_OnSerialPortError?.Invoke(this, serialPortErrorEventArgs);
                serialPortErrorEventArgs = null;
                return;
            }

            this.m_IsOpen = false;
            this.m_SerialClosed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="bytes">待写入的字节数组</param>
        public void WriteBytes(byte[] bytes)
        {
            if (!this.m_IsOpen)
                return;

            this.m_SerialPort.Write(bytes, 0, bytes.Length);
            this.m_SendDataCount++;
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="content">待写入的字符串</param>
        public void WriteData(string content)
        {
            if (!this.m_IsOpen)
                return;

            this.m_SerialPort.Write(content);
            this.m_SendDataCount++;
        }

        /// <summary>
        /// 写入char数组
        /// </summary>
        /// <param name="dataStr">待写入的char数组</param>
        public void WriteData(char[] chars)
        {
            if (!this.m_IsOpen)
                return;

            this.m_SerialPort.Write(chars, 0, chars.Length);
            this.m_SendDataCount++;
        }

        /// <summary>
        /// 当串口发生错误时
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            SerialPortErrorEventArgs serialPortErrorEventArgs = new SerialPortErrorEventArgs();
            serialPortErrorEventArgs.ErrorMessage = $"Internal Error, EventType:{e.EventType}";
            this.m_OnSerialPortError?.Invoke(this, serialPortErrorEventArgs);
            serialPortErrorEventArgs = null;
        }

        /// <summary>
        /// 当接受到数据时
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int receiveDataCount = this.m_SerialPort.BytesToRead;
            if (receiveDataCount <= 0)
                return;

            byte[] data = new byte[receiveDataCount];
            this.m_SerialPort.Read(data, 0, data.Length);

            this.m_ReceiveDatas.Enqueue(data);
            this.m_ReceivedDataCount++;
        }

        /// <summary>
        /// 轮询接收数据
        /// </summary>
        private void ReceiveSerialPortData(float realElapseSeconds)
        {
            int receiveDataCount = this.m_SerialPort.BytesToRead;
            int readLength = receiveDataCount;

            //不确定数据长度
            if (this.m_ReadLength == -1)
            {
                if (receiveDataCount <= 0)
                {
                    this.m_Timer = 0.0f;
                    return;
                }

                this.m_Timer += realElapseSeconds;
                if (this.m_Timer < this.m_ReadInterval)
                    return;

                readLength = receiveDataCount;
                this.m_Timer = 0.0f;
            }
            //确定数据长度
            else
            {
                if (receiveDataCount < this.m_ReadLength)
                    return;

                readLength = this.m_ReadLength;
            }

            byte[] data = new byte[readLength];
            this.m_SerialPort.Read(data, 0, data.Length);

            this.m_ReceiveDatas.Enqueue(data);
            this.m_ReceivedDataCount++;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.ClosePort();

            this.m_SerialPort = null;
            this.m_PortName = default;
            this.m_BaudRate = default;
            this.m_Parity = default;
            this.m_DataBits = default;
            this.m_StopBits = default;
            this.m_ReadLength = default;
            this.m_ReadTimeout = default;
            this.m_WriteTimeout = default;
            this.m_ReceiveDatas = null;
            this.m_SendDataCount = default;
            this.m_ReceivedDataCount = default;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            this.Dispose();
        }
        #endregion
    }
}