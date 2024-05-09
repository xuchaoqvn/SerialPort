using System;
using UnityEngine;
using System.IO.Ports;
using System.Text;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 串口组件
    /// </summary>
    public class SerialPortComponent : MonoBehaviour
    {
        #region Field
        /// <summary>
        /// 串口管理器
        /// </summary>
        private ISerialPortManager m_SerialPortManager;

        /// <summary>
        /// COM口
        /// </summary>
        [SerializeField]
        private string m_PortName = "COM1";

        /// <summary>
        /// 波特率
        /// </summary>
        [SerializeField]
        private int m_BaudRate = 9600;

        /// <summary>
        /// 奇偶校验
        /// </summary>
        [SerializeField]
        private Parity m_Parity = Parity.None;

        /// <summary>
        /// 数据位
        /// </summary>
        [SerializeField]
        private int m_DataBits = 8;

        /// <summary>
        /// 停止位
        /// </summary>
        [SerializeField]
        private StopBits m_StopBits = StopBits.One;

        /// <summary>
        /// 每次读取数据的长度
        /// </summary>
        [SerializeField]
        private int m_ReadLength = -1;

        /// <summary>
        /// 当每次读取数据的长度不确定时，用于读取数据的间隔
        /// </summary>
        [SerializeField]
        private float m_ReadInterval = 0.15f;

        /// <summary>
        /// 读取超时时长
        /// </summary>
        [SerializeField]
        private int m_ReadTimeout = 100;

        /// <summary>
        /// 写入超时时长
        /// </summary>
        [SerializeField]
        private int m_WriteTimeout = 100;

        /// <summary>
        /// 编码
        /// </summary>
        private Encoding m_Encoding = Encoding.UTF8;

        /// <summary>
        /// 打开串口和关闭串口处理类
        /// </summary>
        [SerializeField]
        private SerialPortOpenCloseHandlerBase m_SerialPortOpenCloseHandler;

        /// <summary>
        /// 串口接受数据处理类
        /// </summary>
        [SerializeField]
        private SerialPortReceiveDataHandlerBase m_ReceiveDataHandler;

        /// <summary>
        /// 串口错误处理类
        /// </summary>
        [SerializeField]
        private SerialPortErrorHandlerBase m_SerialPortErrorHandler;
        #endregion

        #region Property
        /// <summary>
        /// 获取com口
        /// </summary>
        public string PortName => this.m_SerialPortManager.PortName;

        /// <summary>
        /// 获取波特率
        /// </summary>
        public int BaudRate => this.m_SerialPortManager.BaudRate;

        /// <summary>
        /// 获取奇偶校验
        /// </summary>
        public Parity Parity => this.m_SerialPortManager.Parity;

        /// <summary>
        /// 获取数据位
        /// </summary>
        public int DataBits => this.m_SerialPortManager.DataBits;

        /// <summary>
        /// 获取停止位
        /// </summary>
        public StopBits StopBits => this.m_SerialPortManager.StopBits;

        /// <summary>
        /// 获取每次读取数据的长度
        /// </summary>
        public int ReadLength => this.m_SerialPortManager.ReadLength;

        /// <summary>
        /// 获取是否打开串口
        /// </summary>
        public bool IsOpen => this.m_SerialPortManager.IsOpen;

        /// <summary>
        /// 获取读取超时时长
        /// </summary>
        public int ReadTimeout => this.m_SerialPortManager.ReadTimeout;

        /// <summary>
        /// 获取写入超时时长
        /// </summary>
        public int WritTtimeout => this.m_SerialPortManager.WriteTimeout;

        /// <summary>
        /// 获取发送的数据数量
        /// </summary>
        public int SendDataCount => this.m_SerialPortManager.SendDataCount;

        /// <summary>
        /// 获取接受的数据数量
        /// </summary>
        public int ReceivedDataCount => this.m_SerialPortManager.ReceivedDataCount;
        #endregion

        #region MonoFunction
        private void OnEnable()
        {
            this.m_SerialPortManager = new SerialPortManager();

            this.m_SerialPortManager.OpenSerialPortSucceed += this.SerialPortOpenSucceed;
            this.m_SerialPortManager.OpenSerialPortFailure += this.SerialPortOpenFailure;
            this.m_SerialPortManager.SerialPortError += this.SerialPortError;
            this.m_SerialPortManager.ReceiveData += this.ReceiveData;
            this.m_SerialPortManager.Closed += this.SerialPortClosed;
        }

        private void Update()
        {
            if (!this.m_SerialPortManager.IsOpen)
                this.m_SerialPortManager.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDisable()
        {
            if (!this.m_SerialPortManager.IsOpen)
                return;

            this.m_SerialPortManager.OpenSerialPortSucceed -= this.SerialPortOpenSucceed;
            this.m_SerialPortManager.OpenSerialPortFailure -= this.SerialPortOpenFailure;
            this.m_SerialPortManager.SerialPortError -= this.SerialPortError;
            this.m_SerialPortManager.ReceiveData -= this.ReceiveData;
            this.m_SerialPortManager.Closed -= this.SerialPortClosed;

            this.ClosePort();
            this.m_SerialPortManager.Dispose();
        }
        #endregion

        #region Function
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize() => this.m_SerialPortManager.Initialize(this.m_PortName, this.m_BaudRate, this.m_Parity, this.m_DataBits, this.m_StopBits,
                                    this.m_ReadLength, this.m_ReadInterval, this.m_ReadTimeout, this.m_WriteTimeout, this.m_Encoding);

        /// <summary>
        /// 打开串口
        /// </summary>
        public void OpenPort() => this.m_SerialPortManager.OpenPort();

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void ClosePort() => this.m_SerialPortManager.ClosePort();

        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="bytes">待写入的字节数组</param>
        public void WriteBytes(byte[] bytes) => this.m_SerialPortManager.WriteBytes(bytes);

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="content">待写入的字符串</param>
        public void WriteData(string content) => this.m_SerialPortManager.WriteData(content);

        /// <summary>
        /// 写入char数组
        /// </summary>
        /// <param name="dataStr">待写入的char数组</param>
        public void WriteData(char[] chars) => this.m_SerialPortManager.WriteData(chars);

        /// <summary>
        /// 当串口打开成功时
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void SerialPortOpenSucceed(object sender, EventArgs e) => this.m_SerialPortOpenCloseHandler.Handler(this, "Open SerialPort Succeed.", true);

        /// <summary>
        /// 当串口打开失败时
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void SerialPortOpenFailure(object sender, EventArgs e)
        {
            OpenSerialPortFailureEventArgs openSerialPortFailureEventArgs = e as OpenSerialPortFailureEventArgs;
            if (openSerialPortFailureEventArgs == null)
                return;

            this.m_SerialPortOpenCloseHandler.Handler(this, $"Open SerialPort Failure, Error Message:{openSerialPortFailureEventArgs.FailureMessage}", true);
        }

        /// <summary>
        /// 当串口发生错误时
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void SerialPortError(object sender, EventArgs e)
        {
            SerialPortErrorEventArgs serialPortErrorEventArgs = e as SerialPortErrorEventArgs;
            if (serialPortErrorEventArgs == null)
                return;

            this.m_SerialPortErrorHandler.Handler(this, $"SerialPort Error, Error Message:{serialPortErrorEventArgs.ErrorMessage}");
        }

        /// <summary>
        /// 当串口接收到数据时
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void ReceiveData(object sender, EventArgs e)
        {
            SerialPortReceiveDataEventArgs serialPortReceiveDataEventArgs = e as SerialPortReceiveDataEventArgs;
            if (serialPortReceiveDataEventArgs == null)
                return;

            this.m_ReceiveDataHandler.Handler(this, serialPortReceiveDataEventArgs.ReceiveData);
        }

        /// <summary>
        /// 当串口关闭时
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void SerialPortClosed(object sender, EventArgs e) => this.m_SerialPortOpenCloseHandler.Handler(this, "SerialPort Closed.", false);
        #endregion
    }
}