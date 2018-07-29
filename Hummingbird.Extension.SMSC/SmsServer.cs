﻿using ALAZ.SystemEx.NetEx.SocketsEx;
using Hummingbird.TestFramework.Messaging;
using Hummingbird.TestFramework.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Hummingbird.Extension.SMSC
{
    /// <summary>
    /// Main class of the SMS-C Simulator
    /// </summary>
    /// <seealso cref="Hummingbird.TestFramework.Services.AbstractServer" />
    /// <seealso cref="Hummingbird.TestFramework.Services.ISendRequest" />
    [SingletonServiceAttribute]
    public class SmsServer : AbstractServer, ISendRequest
    {


        private string ErrorMessage { get; set; }

        /// <summary>
        /// Sends the request enabled.
        /// </summary>
        /// <param name="canSendRequest">if set to <c>true</c> [can send request].</param>
        /// <param name="errormessage">The errormessage.</param>
        public void SendRequestEnabled(bool canSendRequest, string errormessage)
        {
            CanSendRequest = canSendRequest;
            this.ErrorMessage = errormessage;
        }

        private const string PARAM_BINDING_ADDRESS = "Binding Address";
        private const string PARAM_PORT = "Port";
        private const string PARAM_MT_BEHAVIOR = "MT Behavior";
        private const string PARAM_NACK_CODE = "NACK Code";
        private const string PARAM_SR_ACTIVE = "Status Report";
        private const string PARAM_SR_DELAY = "Status Report delay";
        private const string PARAM_SR_DST = "Status Report DST";
        private const string PARAM_SR_RSN = "Status Report RSN";
        private const string PARAM_LA = "Large Account";
        private const string PARAM_LAPASSWORD = "Large Account Password";
        private const string PARAM_HIDE_ACKANDPING = "Hide ACK and Empty MTs";

        internal string localIp;
        internal int port;
        internal MTBehavior mtBehavior;
        internal int nackCode;
        internal bool srActive;
        internal int srDelay;
        internal int srDst;
        internal string srRsn;
        internal string largeAccountNumber;
        internal string largeAccountPassword;
        internal bool hidePingACK;

        internal ResponseMetadata MTMetadata;
        internal ResponseMetadata SSMetadata;
        internal ResponseMetadata AckMetadata;
        internal RequestMetadata MOMetadata;
        internal RequestMetadata SRMetadata;

        SocketServer sersock = null;
        internal static EMIService emiservice = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsServer"/> class.
        /// </summary>
        public SmsServer()
        {
            this.Name = "SMS Center";
            this.StrongName = new Guid("9908954e-c54d-4085-b8ee-bc3e7e676f90");
            this.Description = "A standard SMS Center server usin EMI protocol.";
            this.SettingPageType = null;
            this.Parameters = new Dictionary<string, Parameter>
            {
                { PARAM_BINDING_ADDRESS, new Parameter() { Name = PARAM_BINDING_ADDRESS, DefaultValue = "*", ParameterType = ParameterType.IpAddress } },
                { PARAM_PORT, new Parameter() { Name = PARAM_PORT, DefaultValue = "5000", ParameterType = ParameterType.Integer } },
                { PARAM_MT_BEHAVIOR, new Parameter() { Name = PARAM_MT_BEHAVIOR, DefaultValue = MTBehavior.ACK.ToString(), ParameterType = ParameterType.Enumeration, EnumerationType = typeof(MTBehavior), Description = "Reply ACK, NACK or no reply when receiving MT (UCP 51) messages. 'Auto' for auto detection" } },
                { PARAM_NACK_CODE, new Parameter() { Name = PARAM_NACK_CODE, DefaultValue = "99", ParameterType = ParameterType.Integer, Description = "The Nack code to be applied to MT (UCP 51) messages, when 'MT Behavior' is set to NACK" } },
                { PARAM_SR_ACTIVE, new Parameter() { Name = PARAM_SR_ACTIVE, DefaultValue = "True", ParameterType = ParameterType.Boolean, Description = "If requested, the SMSC will resend a Status Report message (UCP 53) to the client." } },
                { PARAM_SR_DELAY, new Parameter() { Name = PARAM_SR_DELAY, DefaultValue = "0", ParameterType = ParameterType.Integer, Description = "The time in seconds to wait until the Status Report (SR) is resent." } },
                { PARAM_SR_DST, new Parameter() { Name = PARAM_SR_DST, DefaultValue = "0", ParameterType = ParameterType.Integer, Description = "The Dst value of the Status Report (SR), please refer to EMI protocol specifications." } },
                { PARAM_SR_RSN, new Parameter() { Name = PARAM_SR_RSN, DefaultValue = "000", ParameterType = ParameterType.String, Description = "The Rsn value of the Status Report (SR), please refer to EMI protocol specifications." } },
                { PARAM_HIDE_ACKANDPING, new Parameter() { Name = PARAM_HIDE_ACKANDPING, DefaultValue = "True", ParameterType = ParameterType.Boolean, Description = "Hide the ACK and Empty MT SMS from message list, while NACK will still be shown." } },
                { PARAM_LA, new Parameter() { Name = PARAM_LA, DefaultValue = "", ParameterType = ParameterType.String, Description = "Large Account used in Session Management (UCP 60). Empty = Accept all connections" } },
                { PARAM_LAPASSWORD, new Parameter() { Name = PARAM_LAPASSWORD, DefaultValue = "", ParameterType = ParameterType.String, Description = "Large Account Password used in Session Management (UCP 60). Empty = Accept all passwords" } }
            };



            try
            {
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri("/Images/smsc.png", UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                this.ImageSource = src;
            }
            catch { }
            MOMetadata = new RequestMetadata()
            {
                Id = new Guid("2bee75e3-527d-48ec-9bad-c7b258259032"),
                Description = "EMI Protocol SMS Mobile originated (MO) message (UCP 52)",
                RequestType = typeof(SmsMo),
                ApplicationName = "Generic",
                ServiceCategory = "SMS Center",
                ServiceName = "Send SMS MO",
            };

            SRMetadata = new RequestMetadata()
            {
                Id = new Guid("35AECAC8-3B1D-41BE-9CEA-53E78CE1F25C"),
                Description = "EMI Protocol SMS Status Report (MO) message (UCP 53)",
                RequestType = typeof(SmsSr),
                ApplicationName = "Generic",
                ServiceCategory = "SMS Center",
                ServiceName = "Send SMS Status Report",
            };

            this.SupportedRequests.Add(MOMetadata); 
            this.SupportedRequests.Add(SRMetadata);

            MTMetadata = new ResponseMetadata(){
                Id = new Guid("5b0007e7-db18-45f7-b0ff-beeea52fd7d4"),
                Description = "EMI Protocol SMS Mobile terminated (MT) message (UCP 51)",
                ApplicationName = "Generic",
                ServiceCategory = "SMS Center",
                ServiceName = "Send SMS MT",
                RequestType = typeof(EMIProtocol),
            };

            SSMetadata = new ResponseMetadata()
            {
                Id = new Guid("461890fe-3aab-4c96-ba04-f3c0e57088a8"),
                Description = "EMI Protocol SMS Mobile terminated (MT) message (UCP 60)",
                ApplicationName = "Generic",
                ServiceCategory = "SMS Center",
                ServiceName = "Open SMS Session",
                RequestType = typeof(EMIProtocol),
            };

            AckMetadata = new ResponseMetadata()
            {
                Id = new Guid("982edf07-b4f0-4d92-880d-e3ebc3aa09f3"),
                Description = "Acknoledgement of the SMS",
                ApplicationName = "Generic",
                ServiceCategory = "SMS Center",
                ServiceName = "Sent SMS Acknoledgement",
                RequestType = typeof(EMIProtocol),
            };

            this.SupportedResponses.Add(MTMetadata);
            this.SupportedResponses.Add(SSMetadata);
            this.SupportedResponses.Add(AckMetadata);
        }

        /// <summary>
        /// Starts the virtualized service. When used as Virtual Server, the service will registers TCP port and services descriptions.
        /// </summary>
        public override void Start()
        {
            emiservice = new EMIService(this);
            sersock = new SocketServer(CallbackThreadType.ctIOThread, emiservice);
            System.Net.IPEndPoint endpoint;
            if (localIp == "*")
            {
                endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, port);
            }
            else
            {
                endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(localIp), port);
            }
            SocketListener sl = sersock.AddListener("EMI", endpoint);
            sl.Start();
            IsRunning = true;
            
        }

        /// <summary>
        /// Stops this virtualized service. When used as Virtual Server, it will release all services descriptions and network resources like TCP Port.
        /// When the service is stopped, the virtual server will not reply anymore.
        /// </summary>
        public override void Stop()
        {
            if (sersock != null)
            {
                sersock.Stop();
                sersock = null;
            }
            IsRunning = false;
            emiservice.Stop();     
        }

        /// <summary>
        /// Applies the settings. used when Test Framework initializes or user changes the settings related to this service.
        /// When this class is inherited. make sure to call <code>base.ApplySettings(appliedParameters)</code> in derived class.
        /// </summary>
        /// <param name="appliedParameters">The parameters and values to take account by the service.</param>
        public override void ApplySettings(IEnumerable<Parameter> appliedParameters)
        { 
            base.ApplySettings(appliedParameters);
            if (Parameters[PARAM_BINDING_ADDRESS].Value != "*")
            {
                this.Information = string.Format("{0}:{1}", Parameters[PARAM_BINDING_ADDRESS].Value, Parameters[PARAM_PORT].Value);
            }
            else
            {
                this.Information = string.Format("Port {0}", Parameters[PARAM_PORT].Value);
            }

            localIp = Parameters[PARAM_BINDING_ADDRESS].Value;
            port = int.Parse(Parameters[PARAM_PORT].Value);
            mtBehavior = (MTBehavior)Enum.Parse(typeof(MTBehavior), Parameters[PARAM_MT_BEHAVIOR].Value);

            nackCode = int.Parse(Parameters[PARAM_NACK_CODE].Value);
            srActive = bool.Parse(Parameters[PARAM_SR_ACTIVE].Value);
            srDelay = int.Parse(Parameters[PARAM_SR_DELAY].Value);
            srDst = int.Parse(Parameters[PARAM_SR_DST].Value);
            srRsn = Parameters[PARAM_SR_RSN].Value;
            largeAccountNumber = Parameters[PARAM_LA].Value;
            largeAccountPassword = Parameters[PARAM_LAPASSWORD].Value;
            hidePingACK = bool.Parse(Parameters[PARAM_HIDE_ACKANDPING].Value);
        }

        void ISendRequest.SendRequest(RequestData requestData)
        {
            RequestMetadata requestMetadata = requestData.Metadata;
            object requestObject = requestData.Data;
            Message message = new Message();

            if (EMIService.Buffers.Count > 0)
            {
                message.Direction = MessageDirection.Outgoing;
                message.Status = MessageStatus.Pending;
                if (requestMetadata == MOMetadata)
                {
                    SmsMo request = (SmsMo)requestObject;
                    string messagemo = EMIProtocol.CreateMO(request.Sender, request.Receiver, request.MessageText, new DateTimeOffset(DateTime.Now), request.MessageFormat, MT.AlphaNumeric);
                    EmiMessage mm = new EmiMessage();
                    mm.CreateDate = new DateTimeOffset(DateTime.Now);
                    mm.Direction = 0;
                    mm.RAWMessage = messagemo;
                    mm.FriendlyMessage = "SMS MO:" + request.Sender + " -> " + request.Receiver + " : " + request.MessageText + " (" + request.MessageText.Length + " chars)";
                    mm.Type = EmiMessageType.MO;
                    mm.Message = message;
                    emiservice.SendMOSRACK(mm, null);
                }
                else if (requestMetadata == SRMetadata)
                {
                    SmsSr request = (SmsSr)requestObject;
                    string messagemo = EMIProtocol.CreateSR(request.OAdC, request.AdC, request.SCTS, request.Dst.ToString(), request.Rsn.ToString(), request.Text);
                    EmiMessage mm = new EmiMessage();
                    mm.CreateDate = new DateTimeOffset(DateTime.Now);
                    mm.Direction = 0;
                    mm.RAWMessage = messagemo;
                    mm.FriendlyMessage = "SMS SR:" + request.OAdC + " -> " + request.AdC + " : " + request.Text + " (" + request.Text.Length + " chars)";
                    mm.Type = EmiMessageType.SR;
                    mm.Message = message;
                    emiservice.SendMOSRACK(mm, null);
                }
            }
            else
            {
                throw new Exception("You must have an active SMS Connection before sending SMSMO Messages.");
            }
        }

        void ISendRequest.SendRequestAsync(RequestData requestData, PropertyChangedEventHandler propertyChanged)
        {
            SendRequest(requestData);
        }


    }

    /// <summary>
    /// Changes the behavior that SMS-C simulator behaves when an MT message is received.
    /// </summary>
    public enum MTBehavior {
        /// <summary>
        /// Reply with a MT ACK
        /// </summary>
        ACK,
        /// <summary>
        /// Reply with a MT NACK
        /// </summary>
        NACK,
        /// <summary>
        /// Does not reply the MT
        /// </summary>
        Nothing
    }
}
