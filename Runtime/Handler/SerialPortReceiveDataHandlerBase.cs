using UnityEngine;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 串口接受的数据处理类
    /// </summary>
    public abstract class SerialPortReceiveDataHandlerBase : ScriptableObject
    {
        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="serialPortComponent">串口组件</param>
        /// <param name="data">接受到的数据</param>
        public abstract void Handler(SerialPortComponent serialPortComponent, byte[] data);
    }
}