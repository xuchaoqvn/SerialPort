using System;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 串口接收到数据事件参数
    /// </summary>
    public class SerialPortReceiveDataEventArgs : EventArgs
    {
        /// <summary>
        /// 数据
        /// </summary>
        private byte[] m_Data;

        /// <summary>
        /// 获取或设置失败信息
        /// </summary>
        public byte[] ReceiveData
        {
            get => this.m_Data;
            set => this.m_Data = value;
        }
    }
}