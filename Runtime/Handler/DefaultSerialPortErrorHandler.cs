using UnityEngine;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 串口错误处理基类
    /// </summary>
    [CreateAssetMenu(fileName = "SerialPortErrorHandler", menuName = "SerialPort/DefaultError", order = 1)]
    public class DefaultSerialPortErrorHandler : SerialPortErrorHandlerBase
    {
        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="serialPortComponent">串口组件</param>
        /// <param name="errorMessage">错误消息</param>
        public override void Handler(SerialPortComponent serialPortComponent, string errorMessage)
        {
            Debug.LogError(errorMessage);
        }
    }
}