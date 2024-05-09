using System;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 打开串口失败事件参数
    /// </summary>
    public class OpenSerialPortFailureEventArgs : EventArgs
    {
        /// <summary>
        /// 失败信息
        /// </summary>
        private string m_FailureMessage;

        /// <summary>
        /// 获取或设置失败信息
        /// </summary>
        public string FailureMessage
        {
            get => this.m_FailureMessage;
            set => this.m_FailureMessage = value;
        }
    }
}