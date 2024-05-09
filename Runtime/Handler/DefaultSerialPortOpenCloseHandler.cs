
using UnityEngine;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 打开串口和关闭串口处理基类
    /// </summary>
    [CreateAssetMenu(fileName = "SerialPortOpenCloseHandler", menuName = "SerialPort/DefaultOpenClose", order = 1)]
    public class DefaultSerialPortOpenCloseHandler : SerialPortOpenCloseHandlerBase
    {
        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="serialPortComponent">串口组件</param>
        /// <param name="message">消息</param>
        /// <param name="isOpen">是否是打开串口</param>
        public override void Handler(SerialPortComponent serialPortComponent, string message, bool isOpen)
        {
            Debug.Log(message);
        }
    }
}