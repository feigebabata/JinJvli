using System;
using UnityEngine;
using Google.Protobuf;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace FGUFW.Core
{
    static public class NetworkUtility
    {
        public const ushort APP_ID = 333;
        public const ushort PACK_HEAD_LENGTH = 16;
        public const ushort PACK_GAMEPLAY_LENGTH = 8;
        public const ushort PACK_APPID_LENGTH = 2;
        public const ushort PACK_LEN_LENGTH = 2;
        public const ushort PACK_CMD_LENGTH = 4;
        public const int BROADCAST_COUNT = 3;
        public const int GAMELOBBY_GPID = 0;
        
        public const uint GAMESTART_CMD=1;
        public const uint GAMEREADY_CMD=2;

        /// <summary>
        /// 数据包[ appid 2 | length 2 | gameplayid 8 |cmd 4| msgdata ]
        /// </summary>
        static public byte[] EncodeU(ushort appID,long gameplayID,uint cmd,byte[] buffer)
        {
            ushort bufferLength = (ushort)(buffer.Length+NetworkUtility.PACK_HEAD_LENGTH);
            byte[] sendBuffer = new byte[bufferLength];
            byte[] appIDBuffer = BitConverter.GetBytes(NetworkUtility.APP_ID);
            byte[] gpIDBuffer = BitConverter.GetBytes(gameplayID);
            byte[] cmdBuffer = BitConverter.GetBytes(cmd);
            byte[] lengthBuffer = BitConverter.GetBytes(bufferLength);

            int index = 0,length=0;

            length = appIDBuffer.Length;
            Array.Copy(appIDBuffer,0,sendBuffer,index,length);
            index+=length;

            length = lengthBuffer.Length;
            Array.Copy(lengthBuffer,0,sendBuffer,index,length);
            index+=length;

            length = gpIDBuffer.Length;
            Array.Copy(gpIDBuffer,0,sendBuffer,index,length);
            index+=length;

            length = cmdBuffer.Length;
            Array.Copy(cmdBuffer,0,sendBuffer,index,length);
            index+=length;

            length = buffer.Length;
            Array.Copy(buffer,0,sendBuffer,index,length);

            return sendBuffer;
        }

        static public bool DecodeU(ref ushort appID,ref ushort length,ref long gameplayID,ref uint cmd,byte[] buffer,int index,int bufLen)
        {
            if(buffer!=null && bufLen<NetworkUtility.PACK_HEAD_LENGTH)
            {
                return false;
            }
            appID = BitConverter.ToUInt16(buffer,index);
            index += NetworkUtility.PACK_APPID_LENGTH;

            length = BitConverter.ToUInt16(buffer,index);
            index += NetworkUtility.PACK_LEN_LENGTH;

            gameplayID = BitConverter.ToInt64(buffer,index);
            index += NetworkUtility.PACK_GAMEPLAY_LENGTH;

            cmd = BitConverter.ToUInt32(buffer,index);
            index += NetworkUtility.PACK_CMD_LENGTH;
            
            return true;
        }

        static public byte[] EncodeT(uint cmd,byte[] buffer)
        {
            ushort bufferLength = (ushort)(buffer.Length+NetworkUtility.PACK_CMD_LENGTH);
            byte[] sendBuffer = new byte[bufferLength];
            byte[] cmdBuffer = BitConverter.GetBytes(cmd);
            byte[] lengthBuffer = BitConverter.GetBytes(bufferLength);

            int index = 0,length=0;

            length = lengthBuffer.Length;
            Array.Copy(lengthBuffer,0,sendBuffer,index,length);
            index+=length;

            length = cmdBuffer.Length;
            Array.Copy(cmdBuffer,0,sendBuffer,index,length);
            index+=length;

            length = buffer.Length;
            Array.Copy(buffer,0,sendBuffer,index,length);

            return sendBuffer;
        }

        static public bool DecodeT(ref ushort length,ref uint cmd,byte[] buffer,int index,int bufLen)
        {
            if(buffer!=null && bufLen<NetworkUtility.PACK_CMD_LENGTH+PACK_LEN_LENGTH)
            {
                return false;
            }

            length = BitConverter.ToUInt16(buffer,index);
            index += NetworkUtility.PACK_LEN_LENGTH;

            cmd = BitConverter.ToUInt32(buffer,index);
            index += NetworkUtility.PACK_CMD_LENGTH;
            
            return true;
        }
        
        public static IPAddress GetIP(AddressFamily Addfam=AddressFamily.InterNetwork)
        {
            if (Addfam == AddressFamily.InterNetworkV6 && !Socket.OSSupportsIPv6)
            {
                return null;
            }
 
            //本地地址 忽略
            string ignore = "127.0.0.1 ::1";
 
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
                NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;
 
                if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
                #endif 
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == Addfam && !ignore.Contains(ip.Address.ToString()))
                        {
                            return ip.Address;
                        }
                    }
                }
            }
            return null;
        }
 
        
        public static List<IPAddress> GetIPs()
        {
 
            //本地地址 忽略
            string ignore = "127.0.0.1 ::1";
 
            List<IPAddress> outputs = new List<IPAddress>();
 
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
                NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;
 
                if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
                #endif 
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if(!ignore.Contains(ip.Address.ToString()))
                        {
                            outputs.Add(ip.Address);
                        }
                    }
                }
            }
            return outputs;
        }
 
    
        
    }
}