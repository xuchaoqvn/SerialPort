using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text;

namespace SimpleFramework.SerialPort
{
    // <summary>
    /// 串口管理器接口
    /// </summary>
    public interface ISerialPortManager : IDisposable
    {
        #region Property
        /// <summary>
        /// 获取com口
        /// </summary>
        string PortName
        {
            get;
        }

        /// <summary>
        /// 获取波特率
        /// </summary>
        int BaudRate
        {
            get;
        }

        /// <summary>
        /// 获取奇偶校验
        /// </summary>
        Parity Parity
        {
            get;
        }

        /// <summary>
        /// 获取数据位
        /// </summary>
        int DataBits
        {
            get;
        }

        /// <summary>
        /// 获取停止位
        /// </summary>
        StopBits StopBits
        {
            get;
        }

        /// <summary>
        /// 获取每次读取数据的长度
        /// </summary>
        int ReadLength
        {
            get;
        }

        /// <summary>
        /// 获取是否打开串口
        /// </summary>
        bool IsOpen
        {
            get;
        }

        /// <summary>
        /// 获取读取超时时长
        /// </summary>
        int ReadTimeout
        {
            get;
        }

        /// <summary>
        /// 获取写入超时时长
        /// </summary>
        int WriteTimeout
        {
            get;
        }

        /// <summary>
        /// 获取发送的数据数量
        /// </summary>
        int SendDataCount
        {
            get;
        }

        /// <summary>
        /// 获取接受的数据数量
        /// </summary>
        int ReceivedDataCount
        {
            get;
        }
        #endregion

        #region Event
        /// <summary>
        /// 串口打开成功
        /// </summary>
        event EventHandler OpenSerialPortSucceed;

        /// <summary>
        /// 串口打开失败
        /// </summary>
        event EventHandler OpenSerialPortFailure;

        /// <summary>
        /// 串口错误
        /// </summary>
        event EventHandler SerialPortError;

        /// <summary>
        /// 收到数据
        /// </summary>
        event EventHandler ReceiveData;

        /// <summary>
        /// 关闭
        /// </summary>
        event EventHandler Closed;
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
        void Initialize(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, int readLength, float readInterval, int readTimeout, int writeTimeout, Encoding encoding);

        /// <summary>
        /// 串口轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 打开串口
        /// </summary>
        void OpenPort();

        /// <summary>
        /// 关闭串口
        /// </summary>
        void ClosePort();

        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="bytes">待写入的字节数组</param>
        void WriteBytes(byte[] bytes);

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="content">待写入的字符串</param>
        void WriteData(string content);

        /// <summary>
        /// 写入char数组
        /// </summary>
        /// <param name="dataStr">待写入的char数组</param>
        void WriteData(char[] chars);
        #endregion
    }
}