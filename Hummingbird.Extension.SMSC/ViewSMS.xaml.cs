﻿using Hummingbird.TestFramework;
using Hummingbird.TestFramework.Services;
using Hummingbird.TestFramework.Extensibility;
using System;
using System.Globalization;
using System.Windows.Media;
using Hummingbird.TestFramework.Messaging;

namespace Hummingbird.Extension.SMSC
{
    /// <summary>
    /// Interaction logic for ViewSMS.xaml
    /// </summary>

    [MetadataId(new string[] {
        "2bee75e3-527d-48ec-9bad-c7b258259032", //MO - UCP 52
        "35AECAC8-3B1D-41BE-9CEA-53E78CE1F25C", //SR - UCP 53
        "5b0007e7-db18-45f7-b0ff-beeea52fd7d4", //MT - UCP 51
        "461890fe-3aab-4c96-ba04-f3c0e57088a8", //SS - UCP 60
        "982edf07-b4f0-4d92-880d-e3ebc3aa09f3", //MTACK - UCP 52
    })]
    public partial class ViewSMS : CustomMessageViewer
    {
        string trame;
        EmiMessageType messageType = EmiMessageType.SYS;
        public ViewSMS()
        {
            InitializeComponent();
        }


        private void ParseObject(object o)
        {
            EMIProtocol eo = (EMIProtocol)o;
            trame = eo.OriginalTrame;
            txtTrameEMI.Text = trame;
            try
            {
                string operation = eo.OT;
                string response = string.Empty;
                string SR = string.Empty;


                if (operation != null)
                {

                    if (eo.OT == "51")
                    {
                        if (eo.OR == "O") messageType = EmiMessageType.MT;
                        else messageType = EmiMessageType.MT_ACK;

                        //CHECK IF THE MT IS PING OR ACK?
                        string MSISDN = eo.AdC;
                        if (MSISDN == string.Empty)
                        {
                            if (eo.OR == "O") messageType = EmiMessageType.PING;
                            else messageType = EmiMessageType.PING_ACK;
                        }
                    }
                    else if (eo.OT == "52")
                    {
                        if (eo.OR == "O") messageType = EmiMessageType.MO;
                        else messageType = EmiMessageType.MO_ACK;
                    }
                    else if (eo.OT == "53")
                    {
                        if (eo.OR == "O") messageType = EmiMessageType.SR;
                        else messageType = EmiMessageType.SR_ACK;
                    }
                    else if (eo.OT == "60")
                    {
                        if (eo.OR == "O") messageType = EmiMessageType.SESSION;
                        else messageType = EmiMessageType.SESSION_ACK;
                    }

                    Analyse();
                }
            }
            catch
            {
                viewAnalyse.Items.Add(new Variable("Error", "Your Trame EMI is not valide"));
            }
        }

        #region SMSC CONSTANTS
        public static string[] rsns = {
            "000 Unknown subscriber",
            "001 Service temporary not available",
            "002 Service temporary not available",
            "003 Service temporary not available",
            "004 Service temporary not available",
            "005 Service temporary not available",
            "006 Service temporary not available",
            "007 Service temporary not available",
            "008 Service temporary not available",
            "009 Illegal error code",
            "010 Network time-out",
            "100 Facility not supported",
            "101 Unknown subscriber",
            "102 Facility not provided",
            "103 Call barred",
            "104 Operation barred",
            "105 SC congestion",
            "106 Facility not supported",
            "107 Absent subscriber",
            "108 Delivery fail",
            "109 Sc congestion",
            "110 Protocol error",
            "111 MS not equipped",
            "112 Unknown SC",
            "113 SC congestion",
            "114 Illegal MS",
            "115 MS not a subscriber",
            "116 Error in MS",
            "117 SMS lower layer not provisioned",
            "118 System fail",
            "119 PLMN system failure",
            "120 HLR system failure",
            "121 VLR system failure",
            "122 Previous VLR system failure",
            "123 Controlling MSC system failure",
            "124 VMSC system failure",
            "125 EIR system failure",
            "126 System failure",
            "127 Unexpected data value",
            "200 Error in address service centre",
            "201 Invalid absolute Validity Period",
            "202 Short message exceeds maximum",
            "203 Unable to Unpack GSM message",
            "204 Unable to convert to IRA ALPHABET",
            "205 Invalid validity period format",
            "206 Invalid destination address",
            "207 Duplicate message submit",
            "208 Invalid message type indicator",
        };

        public static string[] dsts = {
            "0 - Delivered",
            "1 - Buffered",
            "2 - Not delivered"
        };

        public static string[] ecs = {
            "01 - Checksum error",
            "02 - Syntax error",
            "03 - Operation not supported by system",
            "04 - Operation not allowed",
            "05 - Call barring active",
            "06 - AdC invalid",
            "07 - Authentication failure",
            "08 - Legitimisation code for all calls, failure",
            "09 - GA not valid",
            "10 - Repetition not allowed",
            "11 - Legitimisation code for repetition, failure",
            "12 - Priority call not allowed",
            "13 - Legitimisation code for priority call, failure",
            "14 - Urgent message not allowed",
            "15 - Legitimisation code for urgent message, failure",
            "16 - Reverse charging not allowed",
            "17 - Legitimisation code for rev. charging, failure",
            "18 - Deferred delivery not allowed",
            "19 - New AC not valid",
            "20 - New legitimisation code not valid",
            "21 - Standard text not valid",
            "22 - Time period not valid",
            "23 - Message type not supported by system",
            "24 - Message too long",
            "25 - Requested standard text not valid",
            "26 - Message type not valid for the pager type",
            "27 - Message not found in smsc",
            "30 - Subscriber hang-up",
            "31 - Fax group not supported",
            "32 - Fax message type not supported",
            "33 - Address already in list (60 series)",
            "34 - Address not in list (60 series)",
            "35 - List full, cannot add address to list (60 series)",
            "36 - RPID already in use",
            "37 - Delivery in progress",
            "38 - Message forwarded",
        };

        public static string[] nrqs ={
            "0 - NAdC not used",
            "1 - NAdC used"
        };

        public static string[] nts = {
            "0 - default value",
            "1 - Delivery Notification",
            "2 - Non-delivery notification",
            "3 - Delivery + Non-delivery notification",
            "4 - Buffered message notification",
            "5 - Buffered message + Delivery notification",
            "6 - Buffered message + Non-delivery notification",
            "7 - all"
        };

        public static string[] npids = {
            "0100 - Mobile Station",
            "0122 - Fax Group 3",
            "0131 - X.400",
            "0138 - Menu over PSTN",
            "0139 - PC appl. over PSTN (E.164)",
            "0339 - PC appl. over X.25 (X.121)",
            "0439 - PC appl. over ISDN (E.164)",
            "0539 - PC appl. over TCP/IP"
        };

        public static string[] lpids = npids;

        public static string[] rpids = {
            "0000 - SME to SME: implicit, device type is specific to this SC, or can be concluded on the basis of the address",
            "0001 - SME to SME: telex (or teletex reduced to telex format)",
            "0002 - SME to SME: group 3 telefax",
            "0003 - SME to SME: group 4 telefax",
            "0004 - SME to SME: voice telephone (i.e. conversion to speech)",
            "0005 - SME to SME: ERMES (European Radio Messaging System)",
            "0006 - SME to SME: National Paging system (known to the SC)",
            "0007 - SME to SME: Videotex (T.100 [20] /T.101 [21])",
            "0008 - SME to SME: teletex, carrier unspecified",
            "0009 - SME to SME: teletex, in PSPDN",
            "0010 - SME to SME: teletex, in CSPDN",
            "0011 - SME to SME: teletex, in analog PSTN",
            "0012 - SME to SME: teletex, in digital ISDN",
            "0013 - SME to SME: UCI (Universal Computer Interface, ETSI DE/PS 3 01 3)",
            "0016 - SME to SME: a message handling facility (known to the SC)",
            "0017 - SME to SME: any public X.400 based message handling system",
            "0018 - SME to SME: Internet Electronic Mail",
            "0032 - telematic interworking: implicit, device type is specific to this SC, or can be concluded on the basis of the address",
            "0033 - telematic interworking: telex (or teletex reduced to telex format)",
            "0034 - telematic interworking: group 3 telefax",
            "0035 - telematic interworking: group 4 telefax",
            "0036 - telematic interworking: voice telephone (i.e. conversion to speech)",
            "0037 - telematic interworking: ERMES (European Radio Messaging System)",
            "0038 - telematic interworking: National Paging system (known to the SC)",
            "0039 - telematic interworking: Videotex (T.100 [20] /T.101 [21])",
            "0040 - telematic interworking: teletex, carrier unspecified",
            "0041 - telematic interworking: teletex, in PSPDN",
            "0042 - telematic interworking: teletex, in CSPDN",
            "0043 - telematic interworking: teletex, in analog PSTN",
            "0044 - telematic interworking: teletex, in digital ISDN",
            "0045 - telematic interworking: UCI (Universal Computer Interface, ETSI DE/PS 3 01 3)",
            "0048 - telematic interworking: a message handling facility (known to the SC)",
            "0049 - telematic interworking: any public X.400 based message handling system",
            "0050 - telematic interworking: Internet Electronic Mail",
            "0064 - Type 0, user not alerted",
            "0065 - Replace Type 1",
            "0066 - Replace Type 2",
            "0067 - Replace Type 3",
            "0068 - Replace Type 4",
            "0069 - Replace Type 5",
            "0070 - Replace Type 6",
            "0071 - Replace Type 7",
            "0095 - Return Call Message",
            "0124 - ANSI-136 R-DATA",
            "0125 - ME Data download",
            "0126 - ME De-personalization",
            "0127 - SIM Data Download"
        };

        public static string[] mts = {
            "2 - Numeric message",
            "3 - Alphanumeric message encoded into IRA characters.",
            "4 - TD message encoded into IRA characters"
        };

        public static string[] otoas = {
            "1139 - NPI telephone and TON international",
            "5039 - Alphanumeric address"
        };

        public static string[] dirs ={
            "O - Request",
            "R - Reply"
        };

        public static string[] otons ={
            "1 - International number (starts with the country code)",
            "2 - National number (default value if omitted)",
            "6 - Abbreviated number (registered large account identification)"
        };

        public static string[] onpis = {
            "1 - E.164 address (default value if omitted)",
            "3 - X121 address",
            "5 - SMSC specific: Private (TCP/IP address/abbreviated number)"
        };

        public static string[] styps = {
            "1 - open session",
            "2 - reserved",
            "3 - change password",
            "4 - open provisioning session",
            "5 - reserved",
            "6 - change provisioning password"
        };

        public static string[] opids = {
                "00 - Mobile station",
                "39 - PC application"
        };

        public static string[] opers = {
                "51 - MT, Submit Short Message operation",
                "52 - MO, Delivery Short Message operation",
                "53 - SR, Delivery notification operation",
                "54 - Modify Short Message operation",
                "55 - Inquiry message operation",
                "56 - Delete message operation",
                "57 - Response Inquiry message operation",
                "58 - Response delete message operation",
                "60 - Session management operation",
                "61 - Provisioning actions operation"

        };

        public static string[] MessageTypes = {
                "00 - Short Message (Default)",
                "01 - Delivery Acknowledgement message type (read receipt)",
                "02 - Manual Acknowledgement message type",
            };

        public static string[] UDH = {
            "00 - Concatenated short messages, 8-bit reference number",
            "01 - Special SMS Message Indication",
            "02 - Reserved",
            "03 - Value not used to avoid misinterpretation as <LF> character",
            "04 - Application port addressing scheme, 8 bit address",
            "05 - Application port addressing scheme, 16 bit address",
            "06 - SMSC Control Parameters",
            "07 - UDH Source Indicator ",
            "08 - Concatenated short message, 16-bit reference number",
            "09 - Wireless Control Message Protocol",
            "0A - Text Formatting",
            "0B - Predefined Sound",
            "0C - User Defined Sound (iMelody max 128 bytes)",
            "0D - Predefined Animation",
            "0E - Large Animation (16*16 times 4 = 32*4 =128 bytes)",
            "0F - Small Animation (8*8 times 4 = 8*4 =32 bytes)",
            "10 - Large Picture (32*32 = 128 bytes)",
            "11 - Small Picture (16*16 = 32 bytes)",
            "12 - Variable Picture",
            "13 - User prompt indicator",
            "14 - Extended Object",
            "15 - Reused Extended Object",
            "16 - Compression Control",
            "17 - Object Distribution Indicator",
            "18 - Standard WVG object",
            "19 - Character Size WVG object",
            "1A - Extended Object Data Request Command",
            "1B - Reserved for future EMS features (see subclause 3.10)",
            "1C - Reserved for future EMS features (see subclause 3.10)",
            "1D - Reserved for future EMS features (see subclause 3.10)",
            "1E - Reserved for future EMS features (see subclause 3.10)",
            "1F - Reserved for future EMS features (see subclause 3.10)",
            "20 - RFC 822 E-Mail Header",
            "21 - Hyperlink format element",
            "22 - Reply Address Element",
            "23 - Enhanced Voice Mail Information",
            "24 - National Language Single Shift ",
            "25 - National Language Locking Shift"
        };

        public static string[] DCS = {
            "00 - GSM 7 bit default alphabet, no class meaning",
            "01 - GSM 7 bit default alphabet, no class meaning",
            "02 - GSM 7 bit default alphabet, no class meaning",
            "03 - GSM 7 bit default alphabet, no class meaning",
            "04 - GSM 8 bit data, no class meaning",
            "05 - GSM 8 bit data, no class meaning",
            "06 - GSM 8 bit data, no class meaning",
            "07 - GSM 8 bit data, no class meaning",
            "08 - UCS2 (16bit), no class meaning",
            "09 - UCS2 (16bit), no class meaning",
            "0A - UCS2 (16bit), no class meaning",
            "0B - UCS2 (16bit), no class meaning",
            "10 - GSM 7 bit default alphabet, class 0",
            "11 - GSM 7 bit default alphabet, class 1",
            "12 - GSM 7 bit default alphabet, class 2",
            "13 - GSM 7 bit default alphabet, class 3",
            "14 - GSM 8 bit data, class 0",
            "15 - GSM 8 bit data, class 1",
            "16 - GSM 8 bit data, class 2",
            "17 - GSM 8 bit data, class 3",
            "18 - UCS2 (16bit), class 0",
            "19 - UCS2 (16bit), class 1",
            "1A - UCS2 (16bit), class 2",
            "1B - UCS2 (16bit), class 3",
            "20 - Compressed GSM 7 bit default alphabet, no class meaning",
            "21 - Compressed GSM 7 bit default alphabet, no class meaning",
            "22 - Compressed GSM 7 bit default alphabet, no class meaning",
            "23 - Compressed GSM 7 bit default alphabet, no class meaning",
            "24 - Compressed GSM 8 bit data, no class meaning",
            "25 - Compressed GSM 8 bit data, no class meaning",
            "26 - Compressed GSM 8 bit data, no class meaning",
            "27 - Compressed GSM 8 bit data, no class meaning",
            "28 - Compressed UCS2 (16bit), no class meaning",
            "29 - Compressed UCS2 (16bit), no class meaning",
            "2A - Compressed UCS2 (16bit), no class meaning",
            "2B - Compressed UCS2 (16bit), no class meaning",
            "30 - Compressed GSM 7 bit default alphabet, class 0",
            "31 - Compressed GSM 7 bit default alphabet, class 1",
            "32 - Compressed GSM 7 bit default alphabet, class 2",
            "33 - Compressed GSM 7 bit default alphabet, class 3",
            "34 - Compressed GSM 8 bit data, class 0",
            "35 - Compressed GSM 8 bit data, class 1",
            "36 - Compressed GSM 8 bit data, class 2",
            "37 - Compressed GSM 8 bit data, class 3",
            "38 - Compressed UCS2 (16bit), class 0",
            "39 - Compressed UCS2 (16bit), class 1",
            "3A - Compressed UCS2 (16bit), class 2",
            "3B - Compressed UCS2 (16bit), class 3",
            "D0 - Indication Inactive, Voicemail Message Waiting",
            "D1 - Indication Inactive, Fax Message Waiting",
            "D2 - Indication Inactive, Electronic Mail Message Waiting",
            "D3 - Indication Inactive, Other Message Waiting",
            "D8 - Indication Active, Voicemail Message Waiting",
            "D9 - Indication Active, Fax Message Waiting",
            "DA - Indication Active, VElectronic Mail Message Waiting",
            "DB - Indication Active, Other Message Waiting",
            "F0 - GSM 7bit default alphabet, Class 0",
            "F1 - GSM 7bit default alphabet, Class 1",
            "F2 - GSM 7bit default alphabet, Class 2",
            "F3 - GSM 7bit default alphabet, Class 3",
            "F4 - 8bit data, Class 0",
            "F5 - 8bit data, Class 1",
            "F6 - 8bit data, Class 2",
            "F7 - 8bit data, Class 3",
        };

        public static string[] PIS = {
            "00 Not Restricted (Default)",
            "01 Restricted",
            "02 Confidential",
            "03 Secret"
        };

        public static string[] UIS = {
            "00 Bulk",
            "01	Normal (Default)",
            "02 Urgent",
            "03 Very Urgent",
        };

        public static string[] ARS = {
            "00 No Acknowledgement requested (Default)",
            "01 Delivery Acknowledgement requested (read receipt)",
            "02 Manual Acknowledgement requested",
            "03 Both delivery and Manual Acknowledgement requested"
        };

        public static string[] MUS = {
            "00 New (Default)",
            "01 Replace in SMSC and SME"
        };

        public static string[] SSIS = {
            "00 non-Single Shot short message (Default)",
            "01 Single Shot short message"
        };

        #endregion

        #region Analyse
        private void Analyse_MessageMT()
        {
            string[] pairs = trame.Split('/');
            string DIR = pairs[2];
            string OPER = pairs[3];
            string AdC = pairs[4];
            string OAdC = pairs[5];
            string AC = pairs[6];
            string NRq = pairs[7];
            string NAdC = pairs[8];
            string NT = pairs[9];
            string NPID = pairs[10];
            string LRq = pairs[11];
            string LRAd = pairs[12];
            string LPID = pairs[13];
            string DD = pairs[14];
            string DDT = pairs[15];
            string VP = pairs[16];
            string RPID = pairs[17];
            string MT = pairs[22];
            string NB = pairs[23];
            string AMsg = pairs[24];
            string NMsg = pairs[24];
            string TMsg = pairs[24];
            string MMS = pairs[25];
            string PR = pairs[26];
            string MCLs = pairs[28];
            string RPI = pairs[29];
            string OTOA = pairs[32];
            string XSer = pairs[34];

            int bit = 8;
            if (XSer.Contains("020108") || XSer.Contains("020109") || XSer.Contains("02010A") || XSer.Contains("02010B") || XSer.Contains("020118") || XSer.Contains("020119") || XSer.Contains("02011A") || XSer.Contains("02011B") || XSer.Contains("020128") || XSer.Contains("020129") || XSer.Contains("02012A") || XSer.Contains("02012B") || XSer.Contains("020138") || XSer.Contains("020139") || XSer.Contains("02013A") || XSer.Contains("02013B"))
            {
                bit = 16;
            }

            string decodedMsg = string.Empty;

            viewAnalyse.Items.Add(new Variable("Direction", getValueFrom(dirs, DIR)));
            viewAnalyse.Items.Add(new Variable("Operation", getValueFrom(opers, OPER)));

            string sAdC = AdC;
            string sOAdC = OAdC;
            try
            {

                int test;
                if (!int.TryParse(AdC, out test))
                {
                    sAdC += " (Decoded value: " + EMIProtocol.GSM7HexToString(AdC.Substring(2)).Substring(0, int.Parse(AdC.Substring(0, 2), NumberStyles.HexNumber) * 8 / 14) + ")";
                }

                if (!int.TryParse(OAdC, out test))
                {
                    sOAdC += " (Decoded value: " + EMIProtocol.GSM7HexToString(OAdC.Substring(2)).Substring(0, int.Parse(OAdC.Substring(0, 2), NumberStyles.HexNumber) * 8 / 14) + ")";
                }
            }
            catch { }

            viewAnalyse.Items.Add(new Variable("AdC - Address code recipient for the SM", sAdC));
            viewAnalyse.Items.Add(new Variable("OAdC - Address code originator", sOAdC));
            viewAnalyse.Items.Add(new Variable("AC - Authentication code originator", AC));
            viewAnalyse.Items.Add(new Variable("NRq - Notification Request", getValueFrom(nrqs, NRq)));
            viewAnalyse.Items.Add(new Variable("NAdC - Notification Address", NAdC));
            viewAnalyse.Items.Add(new Variable("NT - Notification Type", getValueFrom(nts, NT)));
            viewAnalyse.Items.Add(new Variable("NPID - Notification PID value", getValueFrom(npids, NPID)));
            viewAnalyse.Items.Add(new Variable("LRq - Last Resort Address request", LRq));
            viewAnalyse.Items.Add(new Variable("LRAd - Last Resort Address", LRAd));
            viewAnalyse.Items.Add(new Variable("LPID - LRAD PID value", getValueFrom(lpids, LPID)));
            viewAnalyse.Items.Add(new Variable("DD - Deferred Delivery requested", DD));
            viewAnalyse.Items.Add(new Variable("DDT - Deferred delivery time", getTime(DDT)));
            viewAnalyse.Items.Add(new Variable("VP - Validity period", getTime(VP)));
            viewAnalyse.Items.Add(new Variable("RPID - Replace PID value", getValueFrom(rpids, RPID)));
            viewAnalyse.Items.Add(new Variable("MT - Message Type", getValueFrom(mts, MT)));
            switch (MT)
            {
                case "2":
                    //NMsg = message.FriendlyMessage;
                    viewAnalyse.Items.Add(new Variable("NMsg", NMsg));
                    decodedMsg = NMsg;
                    break;
                case "3":
                    //AMsg = message.FriendlyMessage;
                    viewAnalyse.Items.Add(new Variable("AMsg", AMsg));
                    decodedMsg = AMsg;
                    break;
                case "4":
                    //TMsg = message.FriendlyMessage;
                    viewAnalyse.Items.Add(new Variable("NB - No. of bits in Transparent Data (TD) message", NB));
                    viewAnalyse.Items.Add(new Variable("TMsg", TMsg));
                    decodedMsg = TMsg;
                    break;
            }

            if (bit == 16)
            {
                viewAnalyse.Items.Add(new Variable("  (Decoded 16 bit Message)", System.Text.UnicodeEncoding.BigEndianUnicode.GetString(EMIProtocol.decode(decodedMsg.ToCharArray()))));
            }
            else if (bit == 7)
            {
                viewAnalyse.Items.Add(new Variable("  (Decoded compressed 7 bit Message)", EMIProtocol.GSM7HexToString(decodedMsg)));
            }
            else
            {
                viewAnalyse.Items.Add(new Variable("  (Decoded 8 bit (uncompressed 7 bit) Message)", EMIProtocol.GSM8HexToString(decodedMsg)));
            }

            viewAnalyse.Items.Add(new Variable("MMS - More Messages to Send", MMS));
            viewAnalyse.Items.Add(new Variable("PR - Priority Requested", PR));
            viewAnalyse.Items.Add(new Variable("MCLs - Message Class", MCLs));
            viewAnalyse.Items.Add(new Variable("RPI - Reply Path", RPI));
            viewAnalyse.Items.Add(new Variable("OTOA - Originator Type Of Address", getValueFrom(otoas, OTOA)));
            viewAnalyse.Items.Add(new Variable("XSer - Extra Services", XSer));
            AnalyseXser(XSer);
        }

        private void Analyse_MessageMO()
        {
            string[] pairs = trame.Split('/');
            string DIR = pairs[2];
            string OPER = pairs[3];
            string AdC = pairs[4];
            string OAdC = pairs[5];
            string RPID = pairs[17];
            string SCTS = pairs[18];
            string MT = pairs[22];
            string NB = pairs[23];
            string AMsg = pairs[24];
            string NMsg = pairs[24];
            string TMsg = pairs[24];
            string MMS = pairs[25];
            string DCs = pairs[27];
            string MCLs = pairs[28];
            string RPI = pairs[29];
            string HPLMN = pairs[33];
            string XSer = pairs[34];

            int bit = 8;
            if (XSer.Contains("020108") || XSer.Contains("020109") || XSer.Contains("02010A") || XSer.Contains("02010B") || XSer.Contains("020118") || XSer.Contains("020119") || XSer.Contains("02011A") || XSer.Contains("02011B") || XSer.Contains("020128") || XSer.Contains("020129") || XSer.Contains("02012A") || XSer.Contains("02012B") || XSer.Contains("020138") || XSer.Contains("020139") || XSer.Contains("02013A") || XSer.Contains("02013B"))
            {
                bit = 16;
            }

            string decodedMsg = string.Empty;


            viewAnalyse.Items.Add(new Variable("Direction", getValueFrom(dirs, DIR)));
            viewAnalyse.Items.Add(new Variable("Operation", getValueFrom(opers, OPER)));
            viewAnalyse.Items.Add(new Variable("AdC - Address code recipient for the SM", AdC));
            viewAnalyse.Items.Add(new Variable("OAdC - Address code originator", OAdC));
            viewAnalyse.Items.Add(new Variable("RPID - Replace PID value", getValueFrom(rpids, RPID)));
            viewAnalyse.Items.Add(new Variable("SCTS - Service Centre Time Stamp", getTime(SCTS)));
            viewAnalyse.Items.Add(new Variable("MT - Message Type", MT));
            switch (MT)
            {
                case "2":
                    //NMsg = message.FriendlyMessage;
                    viewAnalyse.Items.Add(new Variable("NMsg - Numeric message", NMsg));
                    decodedMsg = NMsg;
                    break;
                case "3":
                    //AMsg = message.FriendlyMessage;
                    viewAnalyse.Items.Add(new Variable("AMsg - Alphanumeric message encoded into IRA characters.", AMsg));
                    decodedMsg = AMsg;
                    break;
                case "4":
                    //TMsg = message.FriendlyMessage;
                    viewAnalyse.Items.Add(new Variable("NB - No. of bits in Transparent Data (TD) message", NB));
                    viewAnalyse.Items.Add(new Variable("TMsg - TD message encoded into IRA characters", TMsg));
                    decodedMsg = TMsg;
                    break;
            }

            if (bit == 16)
            {
                viewAnalyse.Items.Add(new Variable("  (Decoded 16 bit Message)", System.Text.UnicodeEncoding.BigEndianUnicode.GetString(EMIProtocol.decode(decodedMsg.ToCharArray()))));
            }
            else if (bit == 7)
            {
                viewAnalyse.Items.Add(new Variable("  (Decoded compressed 7 bit Message)", EMIProtocol.GSM7HexToString(decodedMsg)));
            }
            else
            {
                viewAnalyse.Items.Add(new Variable("  (Decoded 8 bit (uncompressed 7 bit) Message)", EMIProtocol.GSM8HexToString(decodedMsg)));
            }

            viewAnalyse.Items.Add(new Variable("MMS - More Messages to Send", MMS));
            viewAnalyse.Items.Add(new Variable("DCs - Deprecated", DCs));
            viewAnalyse.Items.Add(new Variable("MCLs - Message Class", MCLs));
            viewAnalyse.Items.Add(new Variable("RPI - Reply Path", RPI));
            //viewAnalyse.Items.Add(new Variable("OTOA - Originator Type Of Address",OTOA));
            viewAnalyse.Items.Add(new Variable("HPLMN - Home PLMN Address", HPLMN));
            viewAnalyse.Items.Add(new Variable("XSer - Extra Services", XSer));
            AnalyseXser(XSer);
        }

        private void Analyse_MessageSR()
        {
            string[] pairs = trame.Split('/');
            string DIR = pairs[2];
            string OPER = pairs[3];
            string AdC = pairs[4];
            string OAdC = pairs[5];
            string RPID = pairs[17];
            string SCTS = pairs[18];
            string Dst = pairs[19];
            string Rsn = pairs[20];
            string DSCTS = pairs[21];
            string MT = pairs[22];
            string NB = pairs[23];
            string AMsg = pairs[24];
            string NMsg = pairs[24];
            string TMsg = pairs[24];
            string MMS = pairs[25];
            string HPLMN = pairs[33];
            string XSer = pairs[34];

            int bit = 8;
            if (XSer.Contains("020108") || XSer.Contains("020109") || XSer.Contains("02010A") || XSer.Contains("02010B") || XSer.Contains("020118") || XSer.Contains("020119") || XSer.Contains("02011A") || XSer.Contains("02011B") || XSer.Contains("020128") || XSer.Contains("020129") || XSer.Contains("02012A") || XSer.Contains("02012B") || XSer.Contains("020138") || XSer.Contains("020139") || XSer.Contains("02013A") || XSer.Contains("02013B"))
            {
                bit = 16;
            }

            string decodedMsg = string.Empty;

            string sAdC = AdC;
            string sOAdC = OAdC;
            try
            {

                int test;
                if (!int.TryParse(AdC, out test))
                {
                    sAdC += " (Decoded value: " + EMIProtocol.GSM7HexToString(AdC.Substring(2)).Substring(0, int.Parse(AdC.Substring(0, 2), NumberStyles.HexNumber) * 8 / 14) + ")";
                }

                if (!int.TryParse(OAdC, out test))
                {
                    sOAdC += " (Decoded value: " + EMIProtocol.GSM7HexToString(OAdC.Substring(2)).Substring(0, int.Parse(OAdC.Substring(0, 2), NumberStyles.HexNumber) * 8 / 14) + ")";
                }
            }
            catch { }


            viewAnalyse.Items.Add(new Variable("Direction", getValueFrom(dirs, DIR)));
            viewAnalyse.Items.Add(new Variable("Operation", getValueFrom(opers, OPER)));
            viewAnalyse.Items.Add(new Variable("AdC - Address code recipient for the SM", sAdC));
            viewAnalyse.Items.Add(new Variable("OAdC - Address code originator", sOAdC));
            viewAnalyse.Items.Add(new Variable("RPID - Replace PID value", getValueFrom(rpids, RPID)));
            viewAnalyse.Items.Add(new Variable("SCTS - Service Centre Time Stamp", getTime(SCTS)));
            viewAnalyse.Items.Add(new Variable("Dst - Delivery status", getDst(Dst)));
            if (Dst != "0")
            {
                viewAnalyse.Items.Add(new Variable("Rsn - Reason code", getRsn(Rsn)));
            }
            else
            {
                viewAnalyse.Items.Add(new Variable("Rsn - Reason code", Rsn));
            }
            viewAnalyse.Items.Add(new Variable("DSCTS - Delivery time stamp", getTime(DSCTS)));

            viewAnalyse.Items.Add(new Variable("MT - Message Type", MT));
            switch (MT)
            {
                case "2":
                    //NMsg = message.FriendlyMessage;
                    viewAnalyse.Items.Add(new Variable("NMsg - Numeric message", NMsg));
                    decodedMsg = NMsg;
                    break;
                case "3":
                    //AMsg = message.FriendlyMessage;
                    viewAnalyse.Items.Add(new Variable("AMsg - Alphanumeric message encoded into IRA characters.", AMsg));
                    decodedMsg = AMsg;
                    break;
                case "4":
                    //TMsg = message.FriendlyMessage;
                    viewAnalyse.Items.Add(new Variable("NB - No. of bits in Transparent Data (TD) message", NB));
                    viewAnalyse.Items.Add(new Variable("TMsg - TD message encoded into IRA characters", TMsg));
                    decodedMsg = TMsg;
                    break;
            }

            if (bit == 16)
            {
                viewAnalyse.Items.Add(new Variable("  (Decoded 16 bit Message)", System.Text.UnicodeEncoding.BigEndianUnicode.GetString(EMIProtocol.decode(decodedMsg.ToCharArray()))));
            }
            else if (bit == 7)
            {
                viewAnalyse.Items.Add(new Variable("  (Decoded compressed 7 bit Message)", EMIProtocol.GSM7HexToString(decodedMsg)));
            }
            else
            {
                viewAnalyse.Items.Add(new Variable("  (Decoded 8 bit (uncompressed 7 bit) Message)", EMIProtocol.GSM8HexToString(decodedMsg)));
            }

            viewAnalyse.Items.Add(new Variable("MMS - More Messages to Send", MMS));
            viewAnalyse.Items.Add(new Variable("HPLMN - Home PLMN Address", HPLMN));
            viewAnalyse.Items.Add(new Variable("XSer - Extra Services", XSer));
            AnalyseXser(XSer);
        }

        private void Analyse_ACKNACK()
        {
            string[] pairs = trame.Split('/');
            string DIR = pairs[2];
            string OPER = pairs[3];
            string ACK = pairs[4];


            viewAnalyse.Items.Add(new Variable("Direction", getValueFrom(dirs, DIR)));
            viewAnalyse.Items.Add(new Variable("Operation", getValueFrom(opers, OPER)));
            viewAnalyse.Items.Add(new Variable("ACK/NACK", (ACK.ToUpper() == "A" ? "A - ACK" : "N - NACK")));
            if (OPER == "60" || OPER == "61")
            {
                if (ACK.ToUpper() == "A")
                {
                    string SM = pairs[5];
                    viewAnalyse.Items.Add(new Variable("SM - System message", SM));
                }
                else
                {
                    string EC = pairs[5];
                    string SM = pairs[6];
                    viewAnalyse.Items.Add(new Variable("EC - Error code", getEC(EC)));
                    viewAnalyse.Items.Add(new Variable("SM - System message", SM));
                }

            }
            else
            {
                string EC = pairs[5];
                string MVP = pairs[5];
                string SM = pairs[6];
                if (ACK.ToUpper() == "A")
                {
                    viewAnalyse.Items.Add(new Variable("MVP - Modified validity period", MVP));
                }
                else
                {
                    viewAnalyse.Items.Add(new Variable("EC - Error code", getEC(EC)));
                }
                viewAnalyse.Items.Add(new Variable("SM - System message", SM));
                if (SM != string.Empty)
                {
                    char SEP = ':';
                    string[] seps = SM.Split(SEP);
                    string AdC = seps[0];
                    string SCTS = seps[1];
                    viewAnalyse.Items.Add(new Variable(" SM AdC", AdC));
                    viewAnalyse.Items.Add(new Variable(" SM SCTS", getTime(SCTS)));
                }
            }
        }

        private void Analyse_Nothing()
        {

        }

        private void AnalyseSession()
        {
            string[] pairs = trame.Split('/');
            string DIR = pairs[2];
            string OPERATION = pairs[3];
            string OAdC = pairs[4];
            string OTON = pairs[5];
            string ONPI = pairs[6];
            string STYP = pairs[7];
            string PWD = System.Text.ASCIIEncoding.ASCII.GetString(EMIProtocol.HexStringToByteArray(pairs[8]));
            string NPWD = System.Text.ASCIIEncoding.ASCII.GetString(EMIProtocol.HexStringToByteArray(pairs[9]));
            string VERS = pairs[10];
            string OPID = pairs[14];

            viewAnalyse.Items.Add(new Variable("Direction", getValueFrom(dirs, DIR)));
            viewAnalyse.Items.Add(new Variable("Operation", getValueFrom(opers, OPERATION)));
            viewAnalyse.Items.Add(new Variable("OAdC - TCP/IP or abbreviated address,", OAdC));
            viewAnalyse.Items.Add(new Variable("OTON - Originator Type of Number", getValueFrom(otons, OTON)));
            viewAnalyse.Items.Add(new Variable("ONPI - Originator Numbering Plan Id", getValueFrom(onpis, ONPI)));
            viewAnalyse.Items.Add(new Variable("STYP - Subtype of operation", getValueFrom(styps, STYP)));
            viewAnalyse.Items.Add(new Variable("PWD - Password", PWD));
            viewAnalyse.Items.Add(new Variable("NPWD - New Password", NPWD));
            viewAnalyse.Items.Add(new Variable("VERS - Version Number", VERS));
            viewAnalyse.Items.Add(new Variable("OPID - Originator Protocol Identifier:", getValueFrom(opids, OPID)));
        }


        private void AnalyseXser(string xser)
        {
            int position = 0;
            string type = null;
            string length = null;
            string content = null;
            int count = 1;
            while (position < xser.Length)
            {
                type = xser.Substring(position, 2);
                position += 2;
                length = xser.Substring(position, 2);
                position += 2;
                int contentLength = int.Parse(length, System.Globalization.NumberStyles.HexNumber);
                content = xser.Substring(position, contentLength * 2);
                position += (contentLength * 2);
                string value;
                viewAnalyse.Items.Add(new Variable(" XSer Type " + count, getXserType(type, content, out value)));
                if (value == null)
                {
                    if (type == "01")
                    {
                        viewAnalyse.Items.Add(new Variable("  UDH RAW Value", content));
                        AnalyseUDH(content.Substring(2));
                    }
                    else
                    {
                        viewAnalyse.Items.Add(new Variable(" XSer Value " + count, content));
                    }
                }
                else
                {
                    viewAnalyse.Items.Add(new Variable(" XSer Value " + count, value));
                }
                count++;
            }
        }



        public static string getXserType(string XSER, string v, out string Value)
        {
            switch (XSER)
            {
                case "01":
                    Value = null;
                    return "01 - GSM UDH information";
                case "02":
                    Value = getValueFrom(DCS, v);
                    return "02 - GSM DCS information";
                case "03":
                    Value = getValueFrom(MessageTypes, v);
                    return "03 - Message Type";
                case "04":
                    Value = null;
                    return "04 - Message Reference";
                case "05":
                    Value = getValueFrom(PIS, v);
                    return "05 - Privacy indicator";
                case "06":
                    Value = getValueFrom(UIS, v);
                    return "06 - Urgency Indicator";
                case "07":
                    Value = getValueFrom(ARS, v);
                    return "07 - Acknowledgement Request";
                case "08":
                    Value = getValueFrom(MUS, v);
                    return "08 - Message Updating";
                case "09":
                    Value = null;
                    return "09 - Call Back Number";
                case "0A":
                    Value = null;
                    return "0A - Response Code";
                case "0B":
                    Value = null;
                    return "0B - Teleservice Identifier";
                case "0C":
                    Value = null;
                    return "0C - Billing Identifier";
                case "0D":
                    Value = getValueFrom(SSIS, v);
                    return "0D - Single Shot indicator";
                default:
                    Value = null;
                    return XSER;

            }
        }


        private void AnalyseUDH(string v)
        {
            int max = 0;
            viewAnalyse.Items.Add(new Variable("   UDH Type", getValueFrom(UDH, v.Substring(0, 2))));
            switch (v.Substring(0, 2))
            {
                case "00":
                    viewAnalyse.Items.Add(new Variable("    Concatenated SMS Ref Number", v.Substring(4, 2)));
                    viewAnalyse.Items.Add(new Variable("    Concatenated SMS Max Number", v.Substring(6, 2)));
                    viewAnalyse.Items.Add(new Variable("    Concatenated SMS Sequence Number", v.Substring(8, 2)));
                    max = 10;
                    break;
                case "08":
                    viewAnalyse.Items.Add(new Variable("    Concatenated SMS Ref Number", v.Substring(4, 4)));
                    viewAnalyse.Items.Add(new Variable("    Concatenated SMS Max Number", v.Substring(8, 2)));
                    viewAnalyse.Items.Add(new Variable("    Concatenated SMS Sequence Number", v.Substring(10, 2)));
                    max = 12;
                    break;
                case "05":
                    viewAnalyse.Items.Add(new Variable("    Destination Port", int.Parse(v.Substring(4, 4), NumberStyles.HexNumber).ToString() + "  (HEX Value: 0x" + v.Substring(4, 4) + ")"));
                    viewAnalyse.Items.Add(new Variable("    Originator Port", int.Parse(v.Substring(8, 4), NumberStyles.HexNumber).ToString() + "  (HEX Value: 0x" + v.Substring(8, 4) + ")"));
                    max = 12;
                    break;
                case "04":
                    viewAnalyse.Items.Add(new Variable("    Destination Port", int.Parse(v.Substring(4, 2), NumberStyles.HexNumber).ToString() + "  (HEX Value: 0x" + v.Substring(4, 2) + ")"));
                    viewAnalyse.Items.Add(new Variable("    Originator Port", int.Parse(v.Substring(6, 2), NumberStyles.HexNumber).ToString() + "  (HEX Value: 0x" + v.Substring(6, 2) + ")"));
                    max = 8;
                    break;
                default:
                    int length = int.Parse(v.Substring(2, 2), NumberStyles.HexNumber);
                    max = 2 + length;
                    viewAnalyse.Items.Add(new Variable("    UDH RAW Value", v.Substring(4, length)));
                    break;
            }
            if (v.Length > max)
            {
                AnalyseUDH(v.Substring(max));
            }
        }

        public string getTime(string v)
        {
            try
            {
                int day = int.Parse(v.Substring(0, 2));
                int month = int.Parse(v.Substring(2, 2)); ;
                int year = 2000 + int.Parse(v.Substring(4, 2)); ;
                int hour = int.Parse(v.Substring(6, 2)); ;
                int minute = int.Parse(v.Substring(8, 2)); ;

                int second = 0;
                try
                {
                    second = int.Parse(v.Substring(10, 2)); ;
                }
                catch { }

                return v + string.Format("   (Decoded Value: {0})", new DateTime(year, month, day, hour, minute, second).ToString());
            }
            catch
            {
                return v;
            }
        }

        public string getDst(string Value)
        {
            return getValueFrom(dsts, Value);
        }

        public string getRsn(string Value)
        {
            return getValueFrom(rsns, Value);
        }

        public string getEC(string Value)
        {
            return getValueFrom(ecs, Value);
        }

        public static string getValueFrom(string[] array, string v)
        {
            if (!String.IsNullOrEmpty(v))
            {
                foreach (string s in array)
                {
                    if (s.StartsWith(v, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return s;
                    }
                }
            }
            return v;
        }

        #endregion

        private void Analyse()
        {
            viewAnalyse.Items.Clear();
            switch (messageType)
            {
                case EmiMessageType.SESSION:
                    AnalyseSession();
                    break;
                case EmiMessageType.PING:
                case EmiMessageType.MT:
                    Analyse_MessageMT();
                    break;
                case EmiMessageType.MO:
                    Analyse_MessageMO();
                    break;
                case EmiMessageType.SR:
                    Analyse_MessageSR();
                    break;
                case EmiMessageType.MO_ACK:
                case EmiMessageType.MT_ACK:
                case EmiMessageType.SR_ACK:
                case EmiMessageType.SESSION_ACK:
                case EmiMessageType.PING_ACK:
                    Analyse_ACKNACK();
                    break;
                default:
                    Analyse_Nothing();
                    break;
            }
            //ShowResult();
            viewAnalyse.Background = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.viewAnalyse.Items.Clear();
            }
        }

        public override void ParseMessage(Message message)
        {
            //Message.Tag is the RAW trame message
            string tag = message.RequestText;
            var obj = new EMIProtocol(tag, (EMIService.referredServer));
            ParseObject(obj);
        }
    }

}