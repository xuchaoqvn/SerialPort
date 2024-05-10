using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleFramework.SerialPort;
using System.Reflection;
using System;

namespace SimpleFramework.SerialPort.Editor
{
    /// <summary>
    /// 串口组件Inspector面板类
    /// </summary>
    [CustomEditor(typeof(SerialPortComponent), true)]
    [CanEditMultipleObjects]
    public class SerialPortComponentEditor : UnityEditor.Editor
    {
        #region Field
        private SerialPortComponent m_SerialPortComponent;

        /// <summary>
        /// COM口
        /// </summary>
        private SerializedProperty m_PortName;

        /// <summary>
        /// 波特率
        /// </summary>
        private SerializedProperty m_BaudRate;

        /// <summary>
        /// 奇偶校验
        /// </summary>
        private SerializedProperty m_Parity;

        /// <summary>
        /// 数据位
        /// </summary>
        private SerializedProperty m_DataBits;

        /// <summary>
        /// 停止位
        /// </summary>
        private SerializedProperty m_StopBits;

        /// <summary>
        /// 每次读取数据的长度
        /// </summary>
        private SerializedProperty m_ReadLength;

        /// <summary>
        /// 当每次读取数据的长度不确定时，用于读取数据的间隔
        /// </summary>
        private SerializedProperty m_ReadInterval;

        /// <summary>
        /// 读取超时时长
        /// </summary>
        private SerializedProperty m_ReadTimeout;

        /// <summary>
        /// 写入超时时长
        /// </summary>
        private SerializedProperty m_WriteTimeout;

        /// <summary>
        /// 打开串口和关闭串口处理类
        /// </summary>
        private SerializedProperty m_SerialPortOpenCloseHandler;

        /// <summary>
        /// 串口接受数据处理类
        /// </summary>
        private SerializedProperty m_ReceiveDataHandler;

        /// <summary>
        /// 串口错误处理类
        /// </summary>
        private SerializedProperty m_SerialPortErrorHandler;
        #endregion

        #region Function
        protected virtual void OnEnable()
        {
            this.m_SerialPortComponent = this.serializedObject.targetObject as SerialPortComponent;

            this.m_PortName = this.serializedObject.FindProperty("m_PortName");
            this.m_BaudRate = this.serializedObject.FindProperty("m_BaudRate");
            this.m_Parity = this.serializedObject.FindProperty("m_Parity");
            this.m_DataBits = this.serializedObject.FindProperty("m_DataBits");
            this.m_StopBits = this.serializedObject.FindProperty("m_StopBits");
            this.m_ReadLength = this.serializedObject.FindProperty("m_ReadLength");
            this.m_ReadInterval = this.serializedObject.FindProperty("m_ReadInterval");
            this.m_ReadTimeout = this.serializedObject.FindProperty("m_ReadTimeout");
            this.m_WriteTimeout = this.serializedObject.FindProperty("m_WriteTimeout");
            this.m_SerialPortOpenCloseHandler = this.serializedObject.FindProperty("m_SerialPortOpenCloseHandler");
            this.m_ReceiveDataHandler = this.serializedObject.FindProperty("m_ReceiveDataHandler");
            this.m_SerialPortErrorHandler = this.serializedObject.FindProperty("m_SerialPortErrorHandler");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            if (EditorApplication.isPlaying)
            {
                if (!this.m_SerialPortComponent.IsOpen)
                    this.DrawNoOpenGUI();
                else
                    this.DrawOpenGUI();
            }
            else
                this.DrawNoOpenGUI();

            this.serializedObject.ApplyModifiedProperties();
        }

        private void DrawOpenGUI()
        {
            EditorGUILayout.LabelField($"COM口：{this.m_SerialPortComponent.PortName}");
            EditorGUILayout.LabelField($"波特率：{this.m_SerialPortComponent.BaudRate}");
            EditorGUILayout.LabelField($"奇偶校验：{this.m_SerialPortComponent.Parity}");
            EditorGUILayout.LabelField($"数据位：{this.m_SerialPortComponent.DataBits}");
            EditorGUILayout.LabelField($"停止位：{this.m_SerialPortComponent.StopBits}");
            //接收的数据长度不确定
            if (this.m_SerialPortComponent.ReadLength == -1)
                EditorGUILayout.LabelField($"读取读取数据的间隔：{this.m_SerialPortComponent.ReadInterval}");
            //接收的数据长度确定
            else
                EditorGUILayout.LabelField($"每次读取数据的长度：{this.m_SerialPortComponent.ReadLength}");
            EditorGUILayout.LabelField($"读取超时时长：{this.m_SerialPortComponent.ReadTimeout}");
            EditorGUILayout.LabelField($"写入超时时长：{this.m_SerialPortComponent.WritTtimeout}");
            EditorGUILayout.LabelField($"发送的数据量：{this.m_SerialPortComponent.SendDataCount}");
            EditorGUILayout.LabelField($"接收的数据量：{this.m_SerialPortComponent.ReceivedDataCount}");
        }

        private void DrawNoOpenGUI()
        {
            EditorGUILayout.PropertyField(this.m_PortName, new GUIContent("COM口"));
            EditorGUILayout.PropertyField(this.m_BaudRate, new GUIContent("波特率"));
            EditorGUILayout.PropertyField(this.m_Parity, new GUIContent("奇偶校验"));
            EditorGUILayout.PropertyField(this.m_DataBits, new GUIContent("数据位"));
            EditorGUILayout.PropertyField(this.m_StopBits, new GUIContent("停止位"));
            EditorGUILayout.PropertyField(this.m_ReadLength, new GUIContent("每次读取数据的长度，不确定为-1"));
            EditorGUILayout.PropertyField(this.m_ReadInterval, new GUIContent("当每次读取数据的长度不确定时，用于读取数据的间隔(单位/秒)"));
            EditorGUILayout.PropertyField(this.m_ReadTimeout, new GUIContent("读取超时时长(单位/毫秒)"));
            EditorGUILayout.PropertyField(this.m_WriteTimeout, new GUIContent("写入超时时长(单位/毫秒)"));
            EditorGUILayout.PropertyField(this.m_SerialPortOpenCloseHandler, new GUIContent("打开串口和关闭串口处理类"));
            EditorGUILayout.PropertyField(this.m_ReceiveDataHandler, new GUIContent("串口接受数据处理类"));
            EditorGUILayout.PropertyField(this.m_SerialPortErrorHandler, new GUIContent("串口错误处理类"));
        }

        private string[] GetReceiveDataHandler()
        {
            List<string> typeNames = new List<string>();

            Type receiveDataHandler = typeof(SerialPortReceiveDataHandlerBase);
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            System.Type[] types = assembly.GetTypes();
            foreach (System.Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && receiveDataHandler.IsAssignableFrom(type))
                    typeNames.Add(type.FullName);
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }
        #endregion
    }
}