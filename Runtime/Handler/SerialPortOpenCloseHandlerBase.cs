using System;
using UnityEngine;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 打开串口和关闭串口处理类
    /// </summary>
    public abstract class SerialPortOpenCloseHandlerBase : ScriptableObject
    {
        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="serialPortComponent">串口组件</param>
        /// <param name="message">消息</param>
        /// <param name="isOpen">是否是打开串口</param>
        public abstract void Handler(SerialPortComponent serialPortComponent, string message, bool isOpen);
    }
}