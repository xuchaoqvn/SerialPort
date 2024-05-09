using System;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 串口错误事件参数
    /// </summary>
    public class SerialPortErrorEventArgs : EventArgs
    {
        /// <summary>
        /// 失败信息
        /// </summary>
        private string m_ErrorMessage;

        /// <summary>
        /// 获取或设置错误信息
        /// </summary>
        public string ErrorMessage
        {
            get => this.m_ErrorMessage;
            set => this.m_ErrorMessage = value;
        }
    }
}