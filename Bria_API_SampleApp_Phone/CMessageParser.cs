// (C) Copyright 2020 - CounterPath Corporation. All rights reserved.
//
// THIS SOURCE CODE IS PROVIDED AS A SAMPLE WITH THE SOLE PURPOSE OF DEMONSTRATING A POSSIBLE
// USE OF A COUNTERPATH API. IT IS NOT INTENDED AS A USABLE PRODUCT OR APPLICATION FOR ANY 
// PARTICULAR PURPOSE OR TASK, WHETHER IT BE FOR COMMERCIAL OR PERSONAL USE.
//
// COUNTERPATH DOES NOT REPRESENT OR WARRANT THAT ANY COUNTERPATH APIs OR SAMPLE CODE ARE FREE
// OF INACCURACIES, ERRORS, BUGS, OR INTERRUPTIONS, OR ARE RELIABLE, ACCURATE, COMPLETE, OR 
// OTHERWISE VALID.
//
// THE COUNTERPATH APIs AND ASSOCIATED SAMPLE APPLICATIONS ARE PROVIDED "AS IS" WITH NO WARRANTY, 
// EXPRESS OR IMPLIED, OF ANY KIND AND COUNTERPATH EXPRESSLY DISCLAIMS ANY AND ALL WARRANTIES AND 
// CONDITIONS, INCLUDING, BUT NOT LIMITED TO, ANY IMPLIED WARRANTY OF MERCHANTABILITY, FITNESS FOR 
// A PARTICULAR PURPOSE, AVAILABLILTIY, SECURITY, TITLE AND/OR NON-INFRINGEMENT.  
//
// YOUR USE OF COUNTERPATH APIS AND SAMPLE CODE IS AT YOUR OWN DISCRETION AND RISK, AND YOU WILL 
// BE SOLELY RESPONSIBLE FOR ANY DAMAGE THAT RESULTS FROM THE USE OF ANY COUNTERPATH APIs OR
// SAMPLE CODE INCLUDING, BUT NOT LIMITED TO, ANY DAMAGE TO YOUR COMPUTER SYSTEM OR LOSS OF DATA. 
//
// COUNTERPATH DOES NOT PROVIDE ANY SUPPORT FOR THE SAMPLE APPLICATIONS.
//
// TO VIEW THE OFFICIAL VERSION OF THE TERMS OF USE FOR COUNTERPATH APIs, PLEASE 
// READ IT ON THE WEB_SITE AT: http://www.counterpath.com/bria-desktop-api/


using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Diagnostics;


namespace Bria_API_CSharp_SampleApp
{
   class CMessageParser
   {
      #region PUBLIC_INTERFACE

      #region PUBLIC_TYPE_ENUMS_AND_STRING_CONSTANTS

      public enum ApiMessageType
      {
         UnknownMessageType = 0,
         Response,
         Event,
         Error
      }

      public enum ApiEventType
      {
         UnknownEventType = 0,
         StatusChange
      }

      public enum ApiStatusType
      {
         UnknownStatusType = 0,
         PhoneStatus,
         CallStatus,
         CallHistoryStatus,
         MissedCallStatus,
         VoiceMailStatus,
         AudioPropertiesStatus,
         CallOptionsStatus,
         SystemSettingsStatus
      }

      public class ApiStringConstants
      {
         public class StatusEvents
         {
            public const String Status = "status";
            public const String Event = "event";
            public const String Type = "type";
         }

         public class PhoneStatusResponse
         {
            public const String Phone = "phone";
            public const String State = "state";
            public const String Call = "call";
            public const String CallDisallowedReason = "callDisallowedReason";
            public const String MaxLines = "maxLines";
            public const String AccountStatus = "accountStatus";
            public const String AccountFailureCode = "accountFailureCode";
         }

         public class PhoneState
         {
            public const String Ready = "ready";
            public const String NotReady = "notReady";
         }

         public class CallAllowedState
         {
            public const String Allow = "allow";
            public const String NotAllowed = "notAllow";
         }

         public class PhoneAccountStatus
         {
            public const String Connected = "connected";
            public const String Connecting = "connecting";
            public const String FailureContactingServer = "failureContactingServer";
            public const String FailureAtServer = "failureAtServer";
            public const String Disabled = "disabled";
         }

         public class CallStatusResponse
         {
            public const String Call = "call";
            public const String Id = "id";
            public const String HoldStatus = "holdStatus";
            public const String Participants = "participants";
            public const String Participant = "participant";
         }

         public class CallParticipant
         {
            public const String Number = "number";
            public const String DisplayName = "displayName";
            public const String State = "state";
            public const String TimeInitiated = "timeInitiated";
         }

         public class CallState
         {
            public const String Ringing = "ringing";
            public const String Connecting = "connecting";
            public const String Connected = "connected";
            public const String Failed = "failed";
            public const String Ended = "ended";
         }

         public class CallHoldState
         {
            public const String OffHold = "offHold";
            public const String LocalHold = "localHold";
            public const String RemoteHold = "remoteHold";
         }

         public class CallOptionsStatusResponse
         {
            public const String CallOptions = "callOptions";
            public const String Anonymous = "anonymous";
            public const String LettersToNumbers = "lettersToNumbers";
            public const String AutoAnswer = "autoAnswer";
         }

         public class AudioPropertiesStatusResponse
         {
            public const String AudioProperties = "audioProperties";
            public const String Mute = "mute";
            public const String SpeakerMute = "speakerMute";
            public const String Speaker = "speaker";
            public const String Volume = "volume";
         }

         public class AudioPropertiesVolumeType
         {
            public const String Microphone = "microphone";
            public const String Speaker = "speaker";
         }

         public class MissedCallsStatusResponse
         {
            public const String MissedCalls = "missedCall";
            public const String Count = "count";
         }

         public class VoiceMailStatusResponse
         {
            public const String VoiceMail = "voiceMail";
            public const String AccountId = "accountId";
            public const String AccountName = "accountName";
            public const String Count = "count";
         }

         public class CallHistoryStatusResponse
         {
            public const String CallHistory = "callHistory";
            public const String Type = "type";
            public const String Number = "number";
            public const String DisplayName = "displayName";
            public const String Duration = "duration";
            public const String TimeInitiated = "timeInitiated";
         }

         public class CallHistoryEntryType
         {
            public const String Missed = "missed";
            public const String Received = "received";
            public const String Dialed = "dialed";
         }

         public class SystemSettingsStatusResponse
         {
            public const String SystemSettings = "systemSettings";
            public const String DefaultCallType = "defaultCallType";
            public const String CallRightAwayOnceNumberSelected = "callRightAwayOnceNumberSelected";
         }

         public class GenericEnabledDisabled
         {
            public const String Enabled = "enabled";
            public const String Disabled = "disabled";
         }

         public class GenericTrueFalse
         {
            public const String True = "true";
            public const String False = "false";
         }

         public class GenericAudioVideoConference
         {
            public const String Audio = "audio";
            public const String Video = "video";
            public const String Conference = "conference";
         }
      }

      #endregion

      public CMessageParser(String message)
      {
         m_Message      = message;
         m_MessageType  = ApiMessageType.UnknownMessageType;
         m_EventType    = ApiEventType.UnknownEventType;
      }

      public void ParseMessage()
      {
         if (m_IsParsed == false)
         {
            internalParseMessage();
         }
      }

      #region PUBLIC_MESSAGE_ATTRIBUTES

      public ApiMessageType MessageType   { get { return m_MessageType; } }
      public String TransactionID         { get { return m_TransactionID; } }
      public String UserAgent             { get { return m_UserAgent; } }
      public String ContentType           { get { return m_ContentType; } }
      public String Content               { get { return m_Content; } }
      public Int32 ContentLength          { get { return m_ContentLength; } }
      public ApiEventType EventType       { get { return m_EventType; } }
      public ApiStatusType StatusType     { get { return m_StatusType; } }
      public Int32 ErrorCode              { get { return m_ErrorCode; } }
      public String ErrorText             { get { return m_ErrorText; } }
      public Object Data                  { get { return m_Data; } }

      #endregion

      #endregion

      #region PRIVATE_MESSAGE_PROCESSING

      private void parseMessageType(String line)
      {
         String header = line;

         // First determine the type of message
         if (header.StartsWith("HTTP/1.1"))
         {
            header = header.Remove(0, 8).TrimStart(' ', '\t');
            if (header.StartsWith("200"))
            {
               m_MessageType = ApiMessageType.Response;
            }
            else if (header.StartsWith("4") || header.StartsWith("5"))
            {
               m_MessageType = ApiMessageType.Error;
               String errorCodeStr = header.Substring(0, 3);
               m_ErrorCode = System.Convert.ToInt32(errorCodeStr);
               m_ErrorText = header.Substring(4);
            }
         }
         else if (header.StartsWith("POST"))
         {
            m_MessageType = ApiMessageType.Event;
            header = header.Remove(0, 4).Trim();
            if (header.StartsWith("/statusChange"))
            {
               m_EventType = ApiEventType.StatusChange;
            }
         }
         else
         {
            m_MessageType = ApiMessageType.UnknownMessageType;
         }
      }

      private Boolean extractNextHeader(String line)
      {
         // Extract the header information, if they are present, assume content starts if the line begins with < ...
         if (line.StartsWith("Transaction-ID:"))
         {
            m_TransactionID = line.Substring(15).Trim();
            return true;
         }
         else if (line.StartsWith("User-Agent:"))
         {
            m_UserAgent = line.Substring(11).Trim();
            return true;
         }
         else if (line.StartsWith("Content-Type:"))
         {
            m_ContentType = line.Substring(13).Trim();
            return true;
         }
         else if (line.StartsWith("Content-Length:"))
         {
            m_ContentLength = System.Convert.ToInt32(line.Substring(15).Trim());
            return true;
         }
         else if (line.StartsWith("<"))
         {
            // This looks like a content line
            return false;
         }
         else
         {
            // Ignore any unkonwn/new headers
            return true;
         }
      }

      private void internalParseMessage()
      {
         // Should the message use Windows CR/LF for line-endings, replace them with plain '\n'
         m_Message = m_Message.Replace("\r\n", "\n");

         // Break into individual lines
         var lines = m_Message.Split(new string[] { "\n" }, System.StringSplitOptions.None);

         parseMessageType(lines[0]);

         int i=0;

         for (i=1; i < lines.Length; i++)
         {
            // Parse each of the headers
            if (extractNextHeader(lines[i]) == false)
            {
               break;
            }
         }

         if (i < lines.Length)
         {
            // Remaining lines must be content, put them back together with their original line-breaks
            for (int u = i; u < lines.Length; u++)
            {
               m_Content += lines[u];
               if (u < (lines.Length - 1))
               {
                  m_Content += "\n";
               }
            }

            if (m_ContentLength > 0)
            {
               internalParseContent();
            }
         }

         m_IsParsed = true;
      }

      private void internalParseContent()
      {
         switch (m_MessageType)
         {
            case ApiMessageType.Response:
               internalParseResponse();
               break;

            case ApiMessageType.Event:
               internalParseEvent();
               break;
         }
      }

      private void internalParseResponse()
      {
         using (StringReader stringReader = new StringReader(m_Content))
         using (XmlTextReader reader = new XmlTextReader(stringReader))
         {
            try
            {
               while (reader.Read())
               {
                  if (reader.IsStartElement())
                  {
                     if (reader.Name == ApiStringConstants.StatusEvents.Status)
                     {
                        String statusType = reader[ApiStringConstants.StatusEvents.Type];

                        switch (statusType)
                        {
                           case ApiStringConstants.PhoneStatusResponse.Phone:
                              m_StatusType = ApiStatusType.PhoneStatus;
                              internalParsePhoneStatusResponse(reader);
                              break;
                           case ApiStringConstants.CallStatusResponse.Call:
                              m_StatusType = ApiStatusType.CallStatus;
                              internalParseCallStatusResponse(reader);
                              break;
                           case ApiStringConstants.CallHistoryStatusResponse.CallHistory:
                              m_StatusType = ApiStatusType.CallHistoryStatus;
                              internalParseCallHistoryStatusResponse(reader);
                              break;
                           case ApiStringConstants.MissedCallsStatusResponse.MissedCalls:
                              m_StatusType = ApiStatusType.MissedCallStatus;
                              internalParseMissedCallStatusResponse(reader);
                              break;
                           case ApiStringConstants.VoiceMailStatusResponse.VoiceMail:
                              m_StatusType = ApiStatusType.VoiceMailStatus;
                              internalParseVoiceMailStatusResponse(reader);
                              break;
                           case ApiStringConstants.AudioPropertiesStatusResponse.AudioProperties:
                              m_StatusType = ApiStatusType.AudioPropertiesStatus;
                              internalParseAudioPropertiesStatusResponse(reader);
                              break;
                           case ApiStringConstants.CallOptionsStatusResponse.CallOptions:
                              m_StatusType = ApiStatusType.CallOptionsStatus;
                              internalParseCallOptionsStatusResponse(reader);
                              break;
                           case ApiStringConstants.SystemSettingsStatusResponse.SystemSettings:
                              m_StatusType = ApiStatusType.SystemSettingsStatus;
                              internalParseSystemSettingsStatusResponse(reader);
                              break;
                              
                        }
                     }
                  }
               }
            }
            catch (Exception ex)
            {
               Debug.WriteLine(ex.ToString());
            }
         }
      }

      private void internalParseEvent()
      {
         m_StatusType = ApiStatusType.UnknownStatusType;

         using (StringReader stringReader = new StringReader(m_Content))
         using (XmlTextReader reader = new XmlTextReader(stringReader))
         {
            try
            {
               while (reader.Read())
               {
                  if (reader.IsStartElement())
                  {
                     if (reader.Name == ApiStringConstants.StatusEvents.Event)
                     {
                        String statusType = reader[ApiStringConstants.StatusEvents.Type];

                        switch (statusType)
                        {
                           case ApiStringConstants.PhoneStatusResponse.Phone:
                              m_StatusType = ApiStatusType.PhoneStatus;
                              break;
                           case ApiStringConstants.CallStatusResponse.Call:
                              m_StatusType = ApiStatusType.CallStatus;
                              break;
                           case ApiStringConstants.CallHistoryStatusResponse.CallHistory:
                              m_StatusType = ApiStatusType.CallHistoryStatus;
                              break;
                           case ApiStringConstants.MissedCallsStatusResponse.MissedCalls:
                              m_StatusType = ApiStatusType.MissedCallStatus;
                              break;
                           case ApiStringConstants.VoiceMailStatusResponse.VoiceMail:
                              m_StatusType = ApiStatusType.VoiceMailStatus;
                              break;
                           case ApiStringConstants.AudioPropertiesStatusResponse.AudioProperties:
                              m_StatusType = ApiStatusType.AudioPropertiesStatus;
                              break;
                           case ApiStringConstants.CallOptionsStatusResponse.CallOptions:
                              m_StatusType = ApiStatusType.CallOptionsStatus;
                              break;
                        }
                     }
                  }
               }
            }
            catch (Exception ex)
            {
               Debug.WriteLine(ex.ToString());
            }
         }

      }

      private void internalParseGenericKeyValuePair(XmlTextReader reader)
      {
         Hashtable data = new Hashtable();

         while (reader.Read())
         {
            if (reader.IsStartElement())
            {
               // Store each element as key,value pairs in Hashtable
               data.Add(reader.Name, reader.ReadElementContentAsString());
            }

            m_Data = data;
         }
      }

      private void internalParsePhoneStatusResponse(XmlTextReader reader)
      {
         // This response is a straight-up key-value pair so we can parse as such
         internalParseGenericKeyValuePair(reader); 
      }

      private void internalParseCallStatusResponse(XmlTextReader reader)
      {
         ArrayList data = new ArrayList();

         while (reader.Read())
         {
            if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallStatusResponse.Call))
            {
               Hashtable call = new Hashtable();
               ArrayList participants = new ArrayList();

               while (!((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == ApiStringConstants.CallStatusResponse.Call)))
               {
                  reader.Read();

                  if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallStatusResponse.Id))
                  {
                     call.Add(ApiStringConstants.CallStatusResponse.Id, reader.ReadElementContentAsString());
                  }
                  else if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallStatusResponse.HoldStatus))
                  {
                     call.Add(ApiStringConstants.CallStatusResponse.HoldStatus, reader.ReadElementContentAsString());
                  }
                  else if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallStatusResponse.Participants))
                  {
                     while (!((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == ApiStringConstants.CallStatusResponse.Participants)))
                     {
                        reader.Read();

                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallStatusResponse.Participant))
                        {
                           Hashtable participant = new Hashtable();

                           while (!((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == ApiStringConstants.CallStatusResponse.Participant)))
                           {
                              reader.Read();

                              if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallParticipant.Number))
                              {
                                 participant.Add(ApiStringConstants.CallParticipant.Number, reader.ReadElementContentAsString());
                              }
                              else if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallParticipant.DisplayName))
                              {
                                 participant.Add(ApiStringConstants.CallParticipant.DisplayName, reader.ReadElementContentAsString());
                              }
                              else if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallParticipant.State))
                              {
                                 participant.Add(ApiStringConstants.CallParticipant.State, reader.ReadElementContentAsString());
                              }
                              else if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallParticipant.TimeInitiated))
                              {
                                 participant.Add(ApiStringConstants.CallParticipant.TimeInitiated, reader.ReadElementContentAsString());
                              }
                           }

                           participants.Add(participant);
                        }
                     }

                     call.Add(ApiStringConstants.CallStatusResponse.Participants, participants);
                  }
               }

               data.Add(call);
            }
         }

         m_Data = data;
      }

      private void internalParseMissedCallStatusResponse(XmlTextReader reader)
      {
         // This response is a straight-up key-value pair so we can parse as such
         internalParseGenericKeyValuePair(reader); 
      }

      private void internalParseVoiceMailStatusResponse(XmlTextReader reader)
      {
         ArrayList data = new ArrayList();

         while (reader.Read())
         {
            if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.VoiceMailStatusResponse.VoiceMail))
            {
               Hashtable voiceMailInfo = new Hashtable();

               while (!((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == ApiStringConstants.VoiceMailStatusResponse.VoiceMail)))
               {
                  reader.Read();

                  if (reader.NodeType == XmlNodeType.Element)
                  {
                     voiceMailInfo.Add(reader.Name, reader.ReadElementContentAsString());
                  }
               }

               data.Add(voiceMailInfo);
            }
         }

         m_Data = data;
      }

      private void internalParseAudioPropertiesStatusResponse(XmlTextReader reader)
      {
         Hashtable data = new Hashtable();
         Hashtable volumeData = new Hashtable();

         while (reader.Read())
         {
            if (reader.IsStartElement())
            {
               if (reader.Name == ApiStringConstants.AudioPropertiesStatusResponse.Volume)
               {
                  // The volume elements can be 'speaker' or 'microphone' distinguished with an attribute
                  if (reader.HasAttributes)
                  {
                     reader.MoveToFirstAttribute();
                     String attributeType = reader.Value;

                     reader.MoveToElement();
                     volumeData.Add(attributeType, reader.ReadElementContentAsString());
                  }
               }
               else
               {
                  // Store other elements as plain key, value pairs
                  data.Add(reader.Name, reader.ReadElementContentAsString());
               }
            }
         }

         data.Add(ApiStringConstants.AudioPropertiesStatusResponse.Volume, volumeData);

         m_Data = data;
      }

      private void internalParseCallOptionsStatusResponse(XmlTextReader reader)
      {
         // This response is a straight-up key-value pair so we can parse as such
         internalParseGenericKeyValuePair(reader); 
      }

      private void internalParseCallHistoryStatusResponse(XmlTextReader reader)
      {
         ArrayList data = new ArrayList();

         while (reader.Read())
         {
            if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == ApiStringConstants.CallHistoryStatusResponse.CallHistory))
            {
               Hashtable voiceMailInfo = new Hashtable();

               while (!((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == ApiStringConstants.CallHistoryStatusResponse.CallHistory)))
               {
                  reader.Read();

                  if (reader.NodeType == XmlNodeType.Element)
                  {
                     voiceMailInfo.Add(reader.Name, reader.ReadElementContentAsString());
                  }
               }

               data.Add(voiceMailInfo);
            }
         }

         m_Data = data;
      }

      private void internalParseSystemSettingsStatusResponse(XmlTextReader reader)
      {
         // This response is a straight-up key-value pair so we can parse as such
         internalParseGenericKeyValuePair(reader); 
      }

      #endregion

      #region PRIVATE_MEMBER_VARIABLES

      private ApiMessageType  m_MessageType;
      private String          m_TransactionID;
      private String          m_UserAgent;
      private String          m_ContentType;
      private Int32           m_ContentLength;
      private String          m_Content;
      private ApiEventType    m_EventType;

      private ApiStatusType   m_StatusType;

      private Int32           m_ErrorCode;
      private String          m_ErrorText;

      private Object          m_Data;

      private Boolean         m_IsParsed = false;
      private String          m_Message;

      #endregion
   }
}
