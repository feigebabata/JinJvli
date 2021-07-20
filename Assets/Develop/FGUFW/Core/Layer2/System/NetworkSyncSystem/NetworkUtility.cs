using System;
using UnityEngine;
using Google.Protobuf;

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
        public const int BROADCAST_COUNT = 1;
        public const int GAMELOBBY_GPID = 0;
        public const uint GAMESTART_CMD=1;

        /// <summary>
        /// 数据包[ appid 2 | length 2 | gameplayid 8 |cmd 4| msgdata ]
        /// </summary>
        static public byte[] Encode(ushort appID,long gameplayID,uint cmd,byte[] msgData)
        {
            ushort bufferLength = (ushort)(msgData.Length+NetworkUtility.PACK_HEAD_LENGTH);
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

            length = msgData.Length;
            Array.Copy(msgData,0,sendBuffer,index,length);

            return sendBuffer;
        }

        static public bool Decode(byte[] buffer,ref ushort appID,ref ushort length,ref long gameplayID,ref uint cmd)
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

            gameplayID = BitConverter.ToInt64(buffer,index);
            index += NetworkUtility.PACK_GAMEPLAY_LENGTH;

            cmd = BitConverter.ToUInt32(buffer,index);
            index += NetworkUtility.PACK_CMD_LENGTH;
            
            return true;
        }
        
    }
}