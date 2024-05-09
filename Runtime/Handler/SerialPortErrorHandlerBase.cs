using UnityEngine;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 串口错误处理类
    /// </summary>
    public abstract class SerialPortErrorHandlerBase : ScriptableObject
    {
        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="serialPortComponent">串口组件</param>
        /// <param name="errorMessage">错误消息</param>
        public abstract void Handler(SerialPortComponent serialPortComponent, string errorMessage);
    }
}