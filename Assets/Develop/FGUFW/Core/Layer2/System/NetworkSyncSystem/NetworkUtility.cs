using System;
using UnityEngine;

namespace FGUFW.Core
{
    static public class NetworkUtility
    {
        public const ushort APP_ID = 333;
        public const ushort PACK_HEAD_LENGTH = 6;
        public const ushort PACK_GAMEPLAY_LENGTH = 2;
        public const ushort PACK_APPID_LENGTH = 2;
        public const ushort PACK_LEN_LENGTH = 2;
        public const int BROADCAST_COUNT = 1;

        /// <summary>
        /// 数据包[ appid 2 | length 2 | gameplayid 2 | msgdata ]
        /// </summary>
        /// <returns></returns>
        static public byte[] Encode(ushort appID,ushort gameplayID,byte[] msgData)
        {
            ushort bufferLength = (ushort)(msgData.Length+NetworkUtility.PACK_HEAD_LENGTH);
            byte[] sendBuffer = new byte[bufferLength];
            byte[] appIDBuffer = BitConverter.GetBytes(NetworkUtility.APP_ID);
            byte[] gpIDBuffer = BitConverter.GetBytes(gameplayID);
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

            length = msgData.Length;
            Array.Copy(msgData,0,sendBuffer,index,length);

            return sendBuffer;
        }

        static public bool Decode(byte[] buffer,ref ushort appID,ref ushort length,ref ushort gameplayID)
        {
            if(buffer!=null && buffer.Length<NetworkUtility.PACK_HEAD_LENGTH)
            {
                return false;
            }
            int index=0;
            appID = BitConverter.ToUInt16(buffer,index);
            index += NetworkUtility.PACK_APPID_LENGTH;

            length = BitConverter.ToUInt16(buffer,index);
            index += NetworkUtility.PACK_LEN_LENGTH;

            gameplayID = BitConverter.ToUInt16(buffer,index);
            
            return true;
        }
    }
}