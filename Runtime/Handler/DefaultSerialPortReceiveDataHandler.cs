using UnityEngine;

namespace SimpleFramework.SerialPort
{
    /// <summary>
    /// 串口接受的数据处理基类
    /// </summary>
    [CreateAssetMenu(fileName = "SerialPortReceiveDataHandler", menuName = "SerialPort/DefaultReceiveData", order = 1)]
    public class DefaultSerialPortReceiveDataHandler : SerialPortReceiveDataHandlerBase
    {
        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="serialPortComponent">串口组件</param>
        /// <param name="data">接受到的数据</param>
        public override void Handler(SerialPortComponent serialPortComponent, byte[] data)
        {
            Debug.Log($"SerialPort Receive Data, Byte Array Length: {data.Length}");
        }
    }
}
