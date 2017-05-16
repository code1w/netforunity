using KaiGeX.Bitswarm;
using KaiGeX.Entities.Data;
using KaiGeX.Exceptions;
using KaiGeX.Logging;
using KaiGeX.Protocol;
using KaiGeX.Util;
using System;
namespace KaiGeX.Core
{
	public class SFSProtocolCodec : IProtocolCodec
	{
        private static readonly string CONTROLLER_ID = "c";
        private static readonly string ACTION_ID = "a";
        private static readonly string PARAM_ID = "p";
        private static readonly string USER_ID = "u";
        private static readonly string UDP_PACKET_ID = "i";
        private IoHandler ioHandler = null;
        private Logger log;
        private BitSwarmClient bitSwarm;
        public IoHandler IOHandler
        {
            get
            {
                return this.ioHandler;
            }
            set
            {
                if (this.ioHandler != null)
                {
                    throw new SFSError("IOHandler is already defined for thir ProtocolHandler instance: " + this);
                }
                this.ioHandler = value;
            }
        }
        public SFSProtocolCodec(IoHandler ioHandler, BitSwarmClient bitSwarm)
		{
			this.ioHandler = ioHandler;
			this.log = bitSwarm.Log;
			this.bitSwarm = bitSwarm;
		}
		public void OnPacketRead(ByteArray packet)
		{
			ISFSObject requestObject = SFSObject.NewFromBinaryData(packet);
			this.DispatchRequest(requestObject);
		}
		public void OnPacketRead(ISFSObject packet)
		{
			this.DispatchRequest(packet);
		}
        public void OnPacketRead(ProtoBuf.IExtensible message)
        {
            this.DispatchRequest(message);

        }
		public void OnPacketWrite(IMessage message)
		{
			if (this.bitSwarm.Debug)
			{
				this.log.Debug(new string[]
				{
					"Writing message " + message.Content.GetHexDump()
				});
			}
			ISFSObject content;
			if (message.IsUDP)
			{
				content = this.PrepareUDPPacket(message);
			}
			else
			{
				content = this.PrepareTCPPacket(message);
			}
			message.Content = content;
			this.ioHandler.OnDataWrite(message);
		}
        //-------------------------------------------
        // ProtoBuf Packet to tcp or udp 2014-1-14 zxb
        public void OnProtoBufPacketWrite(ProtoBuf.IExtensible message)
        {
            if (this.bitSwarm.Debug)
            {
                this.log.Debug(new string[]
				{
					"Writing message " 
				});
            }
            this.ioHandler.OnProtoBufDataWrite(message);
        }
		private ISFSObject PrepareTCPPacket(IMessage message)
		{
			ISFSObject iSFSObject = new SFSObject();
			iSFSObject.PutByte(SFSProtocolCodec.CONTROLLER_ID, Convert.ToByte(message.TargetController));
			iSFSObject.PutShort(SFSProtocolCodec.ACTION_ID, Convert.ToInt16(message.Id));
			iSFSObject.PutSFSObject(SFSProtocolCodec.PARAM_ID, message.Content);
			return iSFSObject;
		}
		private ISFSObject PrepareUDPPacket(IMessage message)
		{
			ISFSObject iSFSObject = new SFSObject();
            iSFSObject.PutByte(SFSProtocolCodec.CONTROLLER_ID, Convert.ToByte(message.TargetController));
            iSFSObject.PutInt(SFSProtocolCodec.USER_ID, (this.bitSwarm.Sfs.MySelf == null) ? -1 : this.bitSwarm.Sfs.MySelf.Id);
			iSFSObject.PutLong(SFSProtocolCodec.UDP_PACKET_ID, this.bitSwarm.NextUdpPacketId());
			iSFSObject.PutSFSObject(SFSProtocolCodec.PARAM_ID, message.Content);
			return iSFSObject;
		}
		private void DispatchRequest(ISFSObject requestObject)
		{
			IMessage message = new Message();
			if (requestObject.IsNull(SFSProtocolCodec.CONTROLLER_ID))
			{
				throw new SFSCodecError("Request rejected: No Controller ID in request!");
			}
			if (requestObject.IsNull(SFSProtocolCodec.ACTION_ID))
			{
				throw new SFSCodecError("Request rejected: No Action ID in request!");
			}
			message.Id = Convert.ToInt32(requestObject.GetShort(SFSProtocolCodec.ACTION_ID));
			message.Content = requestObject.GetSFSObject(SFSProtocolCodec.PARAM_ID);
			message.IsUDP = requestObject.ContainsKey(SFSProtocolCodec.UDP_PACKET_ID);
			if (message.IsUDP)
			{
				message.PacketId = requestObject.GetLong(SFSProtocolCodec.UDP_PACKET_ID);
			}
			int @byte = (int)requestObject.GetByte(SFSProtocolCodec.CONTROLLER_ID);
            UnityEngine.Debug.Log("@byte" + @byte);
			IController controller = this.bitSwarm.GetController(@byte);
			if (controller == null)
			{
				throw new SFSError("Cannot handle server response. Unknown controller, id: " + @byte);
			}
			controller.HandleMessage(message);
		}
        private void DispatchRequest(ProtoBuf.IExtensible message)
        {
            IController controller = this.bitSwarm.GetController(2);
            if (controller == null)
            {
                throw new SFSError("Cannot handle server response. Unknown controller, id: " + 2);
            }
            controller.HandleMessage(message);
        }
	}
}
