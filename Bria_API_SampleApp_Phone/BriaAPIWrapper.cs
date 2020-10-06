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
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Bria_API_CSharp_SampleApp
{
    public class BriaAPI
   {
      #region ENUMS

      public enum AccountStates
      {
         Connected,
         Connecting,
         NetworkError,
         ErrorFromServer,
         Disabled
      };

      public enum CallStates
      {
         Ringing,
         Connecting,
         Connected,
         Failed,
         Ended
      }

      public enum HoldStates
      {
         OffHold,
         RemoteHold,
         LocalHold
      }

      public enum CallHistoryEntryTypes
      {
         Missed,
         Received,
         Dialed
      }

      public enum CallTypes
      {
         // Note the CallTypes enum is used with ToString directly used in message construction
         // so do not alter the 'names' unless the API commands have changed also!
         audio,
         video,
         conference
      }

      public enum CallHistoryFilterTypes
      {
         // Note the CallHistoryFilterTypes enum is used with ToString directly used in message construction
         // so do not alter the 'names' unless the API commands have changed also!
         all,
         missed,
         received,
         dialed
      }

      public enum StatusTypes
      {
         // Note the StatusTypes enum is used with ToString directly used in message construction
         // so do not alter the 'names' unless the API commands have changed also!
         phone,
         call,
         callHistory,
         missedCall,
         voiceMail,
         audioProperties,
         callOptions,
         deviceConfiguration,
         systemSettings,
         unknown
      }

      #endregion

      #region EVENTS_TO_APPLICATION

      public event EventHandler OnConnected;
      public event EventHandler OnError;
      public event EventHandler OnDisconnected;

      public class StatusChangedEventArgs : EventArgs
      {
         public StatusChangedEventArgs(StatusTypes statusType)
         {
            Handled = false;
            StatusType = statusType;
         }

         public StatusTypes StatusType { get; private set; }
         public bool Handled { get; set; }
      }
      public event EventHandler<StatusChangedEventArgs> OnStatusChanged;

      public class PhoneStatusEventArgs : EventArgs
      {
         public PhoneStatusEventArgs(bool isReady, bool callsAllowed, AccountStates accountStatus, short errorCode, short maxLines, string noCallsReason)
         {
            IsReady = isReady;
            CallsAllowed = callsAllowed;
            AccountStatus = accountStatus;
            ErrorCode = errorCode;
            MaxLines = maxLines;
            CallsNotAllowedReason = noCallsReason;
         }

         public bool IsReady { get; private set; }
         public bool CallsAllowed { get; private set; }
         public AccountStates AccountStatus { get; private set; }
         public short ErrorCode { get; private set; }
         public short MaxLines { get; private set; }
         public string CallsNotAllowedReason { get; private set; }
      }
      public event EventHandler<PhoneStatusEventArgs> OnPhoneStatus;

      public class CallParticipant
      {
         public CallParticipant(string number, string displayName, CallStates callState, long timeInitiated)
         {
            Number = number;
            DisplayName = displayName;
            CallState = callState;
            TimeInitiated = timeInitiated;
         }
   
         public string Number { get; private set; }
         public string DisplayName { get; private set; }
         public CallStates CallState { get; private set; }
         public long TimeInitiated { get; private set; }
      }
      public class Call
      {
         public Call(string callId, HoldStates holdState, List<CallParticipant> participantList)
         {
            CallId = callId;
            HoldState = holdState;

            ParticipantList = new List<CallParticipant>(participantList);
         }

         public string CallId { get; private set; }
         public HoldStates HoldState { get; private set; }
         public List<CallParticipant> ParticipantList { get; private set; }
      }
      public class CallStatusEventArgs : EventArgs
      {
         public CallStatusEventArgs(List<Call> callList) 
         {
            CallList = new List<Call>(callList);
         }

         public List<Call> CallList { get; private set; }
      }
      public event EventHandler<CallStatusEventArgs> OnCallStatus;

      public class CallOptionsStatusEventArgs : EventArgs
      {
         public CallOptionsStatusEventArgs(bool anonymousEnabled, bool lettersToNumbersEnabled, bool autoAnswerEnabled)
         {
            IsAnonymousEnabled = anonymousEnabled;
            IsLettersToNumbersEnabled = lettersToNumbersEnabled;
            IsAutoAnswerEnabled = autoAnswerEnabled;
         }

         public bool IsAnonymousEnabled { get; private set; }
         public bool IsLettersToNumbersEnabled { get; private set; }
         public bool IsAutoAnswerEnabled { get; private set; }
      }
      public event EventHandler<CallOptionsStatusEventArgs> OnCallOptionsStatus;

      public class AudioPropertiesStatusEventArgs : EventArgs
      {
         public AudioPropertiesStatusEventArgs(bool speakerMode, bool speakerMuted, bool microphoneMuted, short speakerVolume, short microphoneVolume)
         {
            IsSpeakerModeEnabled = speakerMode;
            IsSpeakerMuted = speakerMuted;
            IsMicrophoneMuted = microphoneMuted;
            SpeakerVolume = speakerVolume;
            MicrophoneVolume = microphoneVolume;
         }

         public bool IsSpeakerModeEnabled { get; private set; }
         public bool IsSpeakerMuted { get; private set; }
         public bool IsMicrophoneMuted { get; private set; }
         public short SpeakerVolume { get; private set; }
         public short MicrophoneVolume { get; private set; }
      }
      public event EventHandler<AudioPropertiesStatusEventArgs> OnAudioPropertiesStatus;

      public class MissedCallsStatusEventArgs : EventArgs
      {
         public MissedCallsStatusEventArgs(int count)
         {
            Count = count;
         }

         public int Count { get; private set; }
      }
      public event EventHandler<MissedCallsStatusEventArgs> OnMissedCallsStatus;

      public class VoiceMailInfo
      {
         public VoiceMailInfo(short accountId, string accountName, int count, bool hasNewVoiceMail)
         {
            AccountId = accountId;
            AccountName = accountName;
            Count = count;
            HasNewVoiceMail = hasNewVoiceMail;
         }

         public short AccountId { get; private set; }
         public string AccountName { get; private set; }
         public int Count { get; private set; }
         public bool HasNewVoiceMail { get; private set; }
      }
      public class VoiceMailStatusEventArgs : EventArgs
      {
         public VoiceMailStatusEventArgs(List<VoiceMailInfo> voiceMailInfoList)
         {
            VoiceMailInfoList = new List<VoiceMailInfo>(voiceMailInfoList);
         }

         public List<VoiceMailInfo> VoiceMailInfoList { get; private set; }
      }
      public event EventHandler<VoiceMailStatusEventArgs> OnVoiceMailStatus;

      public class CallHistoryItem
      {
         public CallHistoryItem(CallHistoryEntryTypes type, string number, string displayName, int duration, long timeInitiated)
         {
            Type = type;
            Number = number;
            DisplayName = displayName;
            Duration = duration;
            TimeInitiated = timeInitiated;
         }

         public CallHistoryEntryTypes Type { get; private set; }
         public string Number { get; private set; }
         public string DisplayName { get; private set; }
         public int Duration { get; private set; }
         public long TimeInitiated { get; private set; }
      }
      public class CallHistoryStatusEventArgs : EventArgs
      {
         public CallHistoryStatusEventArgs(List<CallHistoryItem> callHistoryItemList)
         {
            CallHistoryItemList = new List<CallHistoryItem>(callHistoryItemList);
         }

         public List<CallHistoryItem> CallHistoryItemList { get; private set; }
      }
      public event EventHandler<CallHistoryStatusEventArgs> OnCallHistoryStatus;

      public class SystemSettingsStatusEventArgs : EventArgs
      {
         public SystemSettingsStatusEventArgs(CallTypes defaultCallType, bool callRightAwayOnceNumberSelected)
         {
            DefaultCallType = defaultCallType;
            CallRightAwayOnceNumberSelected = callRightAwayOnceNumberSelected;
         }

         public CallTypes DefaultCallType { get; private set; }
         public bool CallRightAwayOnceNumberSelected { get; private set; }
      }
      public event EventHandler<SystemSettingsStatusEventArgs> OnSystemSettingsStatus;

      public class ErrorReceivedEventArgs : EventArgs
      {
         public ErrorReceivedEventArgs(int errorCode, string errorText, string transactionId)
         {
            ErrorCode = errorCode;
            ErrorText = errorText;
            TransactionId = transactionId;
         }

         public int ErrorCode { get; private set; }
         public string ErrorText { get; private set; }
         public string TransactionId { get; private set; }
      }
      public event EventHandler<ErrorReceivedEventArgs> OnErrorReceived;

      #endregion
      
      #region ABSTRACTED_PUBLIC_INTERFACE

      public void Start()
      {
          StartSocketClient();
      }

      public void Stop()
      {
          StopSocketClient();
      }

      public void RequestSetAnonymousMode(bool enabled)
      {
         string value = (enabled ? CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled : CMessageParser.ApiStringConstants.GenericEnabledDisabled.Disabled);
         API_SetCallOptions(value, null, null);
      }

      public void RequestSetAutoAnswerMode(bool enabled)
      {
         string value = (enabled ? CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled : CMessageParser.ApiStringConstants.GenericEnabledDisabled.Disabled);
         API_SetCallOptions(null, value, null);
      }

      public void RequestSetLettersToNumbersMode(bool enabled)
      {
         string value = (enabled ? CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled : CMessageParser.ApiStringConstants.GenericEnabledDisabled.Disabled);
         API_SetCallOptions(null, null, value);
      }

      public void RequestSetCallOptions(string anonymous, string autoAnswer, string lettersToNumbers)
      {
         API_SetCallOptions(anonymous, autoAnswer, lettersToNumbers);
      }

      public void RequestBringPhoneToFront()
      {
         API_BringToFront();
      }

      public void RequestBringCallHistoryToFront(CallHistoryFilterTypes type, string filterText)
      {
         API_OpenCallHistory(type, filterText);
      }

      public void RequestPlaceCall(string number, CallTypes type)
      {
         API_PlaceCall(number, type);
      }

      public void RequestAnswerCall(string callId, bool withVideo)
      {
         API_AnswerCall(callId, withVideo);
      }

      public void RequestPutCallOnHold(string callId)
      {
         API_HoldCall(callId);
      }

      public void RequestResumeCall(string callId)
      {
         API_ResumeCall(callId);
      }

      public void RequestEndCall(string callId)
      {
         API_EndCall(callId);
      }

      public void RequestCheckVoiceMail(short accountId)
      {
         API_CheckVoiceMail(accountId);
      }

      public void RequestSendDTMF(char key)
      {
         API_SendDTMF(key);
      }

      public void RequestSetSpeakerMode(bool enabled, bool suppressDialTone)
      {
         string value = (enabled ? CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled : CMessageParser.ApiStringConstants.GenericEnabledDisabled.Disabled);
         API_SetAudioProperties(null, null, value, suppressDialTone, null, null);
      }

      public void RequestSetSpeakerMute(bool enabled)
      {
         string value = (enabled ? CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled : CMessageParser.ApiStringConstants.GenericEnabledDisabled.Disabled);
         API_SetAudioProperties(null, value, null, false, null, null);
      }

      public void RequestSetMicrophoneMute(bool enabled)
      {
         string value = (enabled ? CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled : CMessageParser.ApiStringConstants.GenericEnabledDisabled.Disabled);
         API_SetAudioProperties(value, null, null, false, null, null);
      }

      public void RequestSetSpeakerVolume(short volume)
      {
         short safeVolume = volume;

         if (safeVolume < 0) safeVolume = 0;
         if (safeVolume > 100) safeVolume = 100;

         API_SetAudioProperties(null, null, null, false, safeVolume.ToString(), null);
      }

      public void RequestSetMicrophoneVolume(short volume)
      {
         short safeVolume = volume;

         if (safeVolume < 0) safeVolume = 0;
         if (safeVolume > 100) safeVolume = 100;

         API_SetAudioProperties(null, null, null, false, null, safeVolume.ToString());
      }

      public void RequestSetAudioProperties(string mute, string speakerMute, string speaker, bool suppressDialTone, string speakerVolume, string micVolume)
      {
         API_SetAudioProperties(mute, speakerMute, speaker, suppressDialTone, speakerVolume, micVolume);
      }

      public void RequestPhoneStatus()
      {
         API_GetStatus(StatusTypes.phone);
      }

      public void RequestCallStatus()
      {
         API_GetStatus(StatusTypes.call);
      }

      public void RequestCallHistoryStatus(int maxCount, CallHistoryFilterTypes filter)
      {
         API_GetStatusCallHistory(maxCount, filter);
      }

      public void RequestMissedCallStatus()
      {
         API_GetStatus(StatusTypes.missedCall);
      }

      public void RequestVoiceMailStatus()
      {
         API_GetStatus(StatusTypes.voiceMail);
      }

      public void RequestAudioPropertiesStatus()
      {
         API_GetStatus(StatusTypes.audioProperties);
      }

      public void RequestCallOptionsStatus()
      {
         API_GetStatus(StatusTypes.callOptions);
      }

      public void RequestSystemSettingsStatus()
      {
         API_GetStatus(StatusTypes.systemSettings);
      }

      #endregion

      #region WEB_SOCKET_CONNECTION

      private CPWebSocket socketClient = null;

      private void StartSocketClient()
      {
         // Handle any SSL Certificate issues to allow for self-signed or otherwise untrusted server certificate
         ServicePointManager.ServerCertificateValidationCallback =
         delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                       System.Security.Cryptography.X509Certificates.X509Chain chain,
                       System.Net.Security.SslPolicyErrors sslPolicyErrors)
         {
            Debug.WriteLine("------------------- !SECURE CONNECTION ALERT! ---------------------");
            Debug.WriteLine(sslPolicyErrors.ToString());
            Debug.WriteLine("");
            Debug.WriteLine(certificate.ToString());
            Debug.WriteLine("-------------------------------------------------------------------");
            return true; // **** Always accept
         };

         if (socketClient == null)
         {
            socketClient = new CPWebSocket("wss://cpclientapi.softphone.com:9002/counterpath/socketapi/v1/");

            socketClient.Opened += new EventHandler(OnSocketConnectionOpened);
            socketClient.Closed += new EventHandler(OnSocketConnectionClosed);
            socketClient.Error += new EventHandler<CPWebSocket.ErrorEventArgs>(OnSocketConnectionError);
            socketClient.MessageReceived += new EventHandler<CPWebSocket.MessageReceivedEventArgs>(OnSocketReceivedMessage);
         }

         socketClient.Open();

      }

      private void StopSocketClient()
      {
         if (socketClient != null)
         {
            socketClient.Close();
            socketClient = null;
         }
      }

      private void SendSocketMessage(string message)
      {
         Debug.WriteLine("--------------------- SENDING THROUGH SOCKET ----------------------");
         Debug.WriteLine(message);
         Debug.WriteLine("-------------------------------------------------------------------");

         socketClient.Send(message);
      }

      #endregion

      #region WEB_SOCKET_CONNECTION_EVENT_HANDLING

      void OnSocketConnectionOpened(object sender, EventArgs e)
      {
            if (OnConnected != null)
            {
               OnConnected(this, e);
            }
      }

      void OnSocketConnectionClosed(object sender, EventArgs e)
      {
         if (OnDisconnected != null)
         {
            OnDisconnected(this, e);
         }
      }

      void OnSocketConnectionError(object sender, CPWebSocket.ErrorEventArgs e)
      {
            Debug.WriteLine("-------------------------- SOCKET ERROR ---------------------------");
            Debug.WriteLine(e.Text);
            Debug.WriteLine("-------------------------------------------------------------------");

            if (OnError != null)
            {
               OnError(this, new EventArgs());
            }
      }

      void OnSocketReceivedMessage(object sender, CPWebSocket.MessageReceivedEventArgs e)
      {
         Debug.WriteLine("------------------------ SOCKET RECEIVED --------------------------");
         Debug.WriteLine(e.Message);
         Debug.WriteLine("-------------------------------------------------------------------");

         ParseIncomingMessage(e.Message);
      }

      #endregion

      #region BRIA_API_MESSAGE_PARSING

      private StatusTypes ConvertApiStatusTypeToStatusTypes(CMessageParser.ApiStatusType apiStatusType)
      {
         switch (apiStatusType)
         {
            case CMessageParser.ApiStatusType.PhoneStatus:
               return StatusTypes.phone;
            case CMessageParser.ApiStatusType.CallStatus:
               return StatusTypes.call;
            case CMessageParser.ApiStatusType.CallHistoryStatus:
               return StatusTypes.callHistory;
            case CMessageParser.ApiStatusType.MissedCallStatus:
               return StatusTypes.missedCall;
            case CMessageParser.ApiStatusType.VoiceMailStatus:
               return StatusTypes.voiceMail;
            case CMessageParser.ApiStatusType.AudioPropertiesStatus:
               return StatusTypes.audioProperties;
            case CMessageParser.ApiStatusType.CallOptionsStatus:
               return StatusTypes.callOptions;
         }

         return StatusTypes.unknown;
      }

      private void ParseIncomingMessage(string message)
      {
         CMessageParser parseThis = new CMessageParser(message);

         parseThis.ParseMessage();

         switch (parseThis.MessageType)
         {
            case CMessageParser.ApiMessageType.Response:
               if ((parseThis.Content != null) && (parseThis.ContentLength > 0))
               {
                  HandleResponse(parseThis.StatusType, parseThis.Data);
               }
               break;

            case CMessageParser.ApiMessageType.Event:
               if (parseThis.EventType == CMessageParser.ApiEventType.StatusChange)
               {
                  HandleStatusChangeEvent(ConvertApiStatusTypeToStatusTypes(parseThis.StatusType));
               }
               else
               {
                  Debug.WriteLine("Unknown API Event received: " + parseThis.EventType.ToString());
               }
               break;

            case CMessageParser.ApiMessageType.Error:
               HandleErrorReceived(parseThis.ErrorCode, parseThis.ErrorText, parseThis.TransactionID);
               break;

            default:
               // Unknown message type
               Debug.WriteLine("Unknown API message received: \r\n" + message);
               break;
         }
      }
      
      private void HandleStatusChangeEvent(StatusTypes statusType)
      {
         StatusChangedEventArgs eventArgs = new StatusChangedEventArgs(statusType);
         if (OnStatusChanged != null)
         {
            OnStatusChanged(this, eventArgs);
         }

         // Default handling if the application doesn't handle the event on its own
         // is to request detailed status on whatever changed 
         if ((eventArgs.Handled == false) && (statusType != StatusTypes.unknown))
         {
            API_GetStatus(statusType);
         }
      }

      private void HandleResponse(CMessageParser.ApiStatusType statusType, Object data)
      {
         // CMessageParser will store the data for each response type in an appropriate 
         // container depending on the structure required for that date

         switch (statusType)
         {
            case CMessageParser.ApiStatusType.PhoneStatus:
               HandlePhoneStatus(data as Hashtable);
               break;

            case CMessageParser.ApiStatusType.CallStatus:
               HandleCallStatus(data as ArrayList);
               break;

            case CMessageParser.ApiStatusType.CallOptionsStatus:
               HandleCallOptionsStatus(data as Hashtable);
               break;

            case CMessageParser.ApiStatusType.AudioPropertiesStatus:
               HandleAudioPropertiesStatus(data as Hashtable);
               break;

            case CMessageParser.ApiStatusType.MissedCallStatus:
               HandleMissedCallsStatus(data as Hashtable);
               break;

            case CMessageParser.ApiStatusType.VoiceMailStatus:
               HandleVoiceMailStatus(data as ArrayList);
               break;

            case CMessageParser.ApiStatusType.CallHistoryStatus:
               HandleCallHistoryStatus(data as ArrayList);
               break; 

            case CMessageParser.ApiStatusType.SystemSettingsStatus:
               HandleSystemSettingsStatus(data as Hashtable);
               break;
         }
      }

      private void HandleErrorReceived(int errorCode, string errorText, string transactionId)
      {
         if (OnErrorReceived != null)
         {
            OnErrorReceived(this, new ErrorReceivedEventArgs(errorCode, errorText, transactionId));
         }
      }

      private void HandlePhoneStatus(Hashtable data)
      {
         bool isReady = false;
         bool allowCalls = false;
         AccountStates accountStatus = AccountStates.Disabled;
         short errorCode = 0;
         short maxLines = 0;

         string phoneState = (string)data[CMessageParser.ApiStringConstants.PhoneStatusResponse.State];
         if (phoneState != null) isReady = (phoneState == CMessageParser.ApiStringConstants.PhoneState.Ready);

         string callsAllowed = (string)data[CMessageParser.ApiStringConstants.PhoneStatusResponse.Call];
         if (callsAllowed != null) allowCalls = (callsAllowed == CMessageParser.ApiStringConstants.CallAllowedState.Allow);

         string accountStatusStr = (string)data[CMessageParser.ApiStringConstants.PhoneStatusResponse.AccountStatus];
         if (accountStatusStr != null)
         {
            switch (accountStatusStr)
            {
               case CMessageParser.ApiStringConstants.PhoneAccountStatus.Connected:
                  accountStatus = AccountStates.Connected;
                  break;
               case CMessageParser.ApiStringConstants.PhoneAccountStatus.Connecting:
                  accountStatus = AccountStates.Connecting;
                  break;
               case CMessageParser.ApiStringConstants.PhoneAccountStatus.Disabled:
                  accountStatus = AccountStates.Disabled;
                  break;
               case CMessageParser.ApiStringConstants.PhoneAccountStatus.FailureAtServer:
                  accountStatus = AccountStates.ErrorFromServer;
                  break;
               case CMessageParser.ApiStringConstants.PhoneAccountStatus.FailureContactingServer:
                  accountStatus = AccountStates.NetworkError;
                  break;
            }
         }

         string errorCodeStr = (string)data[CMessageParser.ApiStringConstants.PhoneStatusResponse.AccountFailureCode];
         if (errorCodeStr != null) errorCode = System.Convert.ToInt16(errorCodeStr);

         string maxLinesStr = (string)data[CMessageParser.ApiStringConstants.PhoneStatusResponse.MaxLines];
         if (maxLinesStr != null) maxLines = System.Convert.ToInt16(maxLinesStr);

         string callNotAllowedReason = (string)data[CMessageParser.ApiStringConstants.PhoneStatusResponse.CallDisallowedReason];

         if (OnPhoneStatus != null)
         {
            OnPhoneStatus(this, new PhoneStatusEventArgs(isReady, allowCalls, accountStatus, errorCode, maxLines, callNotAllowedReason));
         }
      }

      private HoldStates StringToHoldState(string stateString)
      {
         if (stateString != null)
         {
            switch (stateString)
            {
               case CMessageParser.ApiStringConstants.CallHoldState.LocalHold:
                  return HoldStates.LocalHold;
               case CMessageParser.ApiStringConstants.CallHoldState.RemoteHold:
                  return HoldStates.RemoteHold;
               case CMessageParser.ApiStringConstants.CallHoldState.OffHold:
                  return HoldStates.OffHold;
            }
         }

         return HoldStates.OffHold;
      }

      private CallStates StringToCallState(string callStateString)
      {
         if (callStateString != null)
         {
            switch (callStateString)
            {
               case CMessageParser.ApiStringConstants.CallState.Ringing:
                  return CallStates.Ringing;
               case CMessageParser.ApiStringConstants.CallState.Connected:
                  return CallStates.Connected;
               case CMessageParser.ApiStringConstants.CallState.Connecting:
                  return CallStates.Connecting;
               case CMessageParser.ApiStringConstants.CallState.Failed:
                  return CallStates.Failed;
               case CMessageParser.ApiStringConstants.CallState.Ended:
                  return CallStates.Ended;
            }
         }

         return CallStates.Ended;
      }
      
      private void HandleCallStatus(ArrayList calls)
      {
         List<Call> callList = new List<Call>();

         foreach (Hashtable call in calls)
         {
            string callId = (string)call[CMessageParser.ApiStringConstants.CallStatusResponse.Id];
            HoldStates holdStatus = StringToHoldState((string)call[CMessageParser.ApiStringConstants.CallStatusResponse.HoldStatus]);

            List<CallParticipant> callParticipantList = new List<CallParticipant>();

            ArrayList participants = call[CMessageParser.ApiStringConstants.CallStatusResponse.Participants] as ArrayList;
            foreach (Hashtable participant in participants)
            {
               string number = (string)participant[CMessageParser.ApiStringConstants.CallParticipant.Number];
               string displayName = (string)participant[CMessageParser.ApiStringConstants.CallParticipant.DisplayName];
               CallStates state = StringToCallState((string)participant[CMessageParser.ApiStringConstants.CallParticipant.State]);
               long timeInitiated = System.Convert.ToInt64((string)participant[CMessageParser.ApiStringConstants.CallParticipant.TimeInitiated]);

               CallParticipant callParticipant = new CallParticipant(number, displayName, state, timeInitiated);
               callParticipantList.Add(callParticipant);
            }
            
            Call callData = new Call(callId, holdStatus, callParticipantList);
            callList.Add(callData);
         }

         if (OnCallStatus != null)
         {
            OnCallStatus(this, new CallStatusEventArgs(callList));
         }
      }
      
      private void HandleCallOptionsStatus(Hashtable data)
      {
         bool anonymous = false;
         bool lettersToNumbers = false;
         bool autoAnswer = false;

         string anonymousState = (string)data[CMessageParser.ApiStringConstants.CallOptionsStatusResponse.Anonymous];
         if (anonymousState != null) anonymous = (anonymousState == CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled);

         string lettersToNumbersStr = (string)data[CMessageParser.ApiStringConstants.CallOptionsStatusResponse.LettersToNumbers];
         if (lettersToNumbersStr != null) lettersToNumbers = (lettersToNumbersStr == CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled);

         string autoAnswerState = (string)data[CMessageParser.ApiStringConstants.CallOptionsStatusResponse.AutoAnswer];
         if (autoAnswerState != null) autoAnswer = (autoAnswerState == CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled);

         if (OnCallOptionsStatus != null)
         {
            OnCallOptionsStatus(this, new CallOptionsStatusEventArgs(anonymous, lettersToNumbers, autoAnswer));
         }
      }
      
      private void HandleAudioPropertiesStatus(Hashtable data)
      {
         bool speakerModeEnabled = false;
         bool speakerMuted = false;
         bool microphoneMuted = false;
         short speakerVolume = 0;
         short microphoneVolume = 0;

         string speakerModeState = (string)data[CMessageParser.ApiStringConstants.AudioPropertiesStatusResponse.Speaker];
         if (speakerModeState != null) speakerModeEnabled = (speakerModeState == CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled);

         string microphoneMuteState = (string)data[CMessageParser.ApiStringConstants.AudioPropertiesStatusResponse.Mute];
         if (microphoneMuteState != null) microphoneMuted = (microphoneMuteState == CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled);

         string speakerMuteState = (string)data[CMessageParser.ApiStringConstants.AudioPropertiesStatusResponse.SpeakerMute];
         if (speakerMuteState != null) speakerMuted = (speakerMuteState == CMessageParser.ApiStringConstants.GenericEnabledDisabled.Enabled);

         Hashtable volumeData = (Hashtable)data[CMessageParser.ApiStringConstants.AudioPropertiesStatusResponse.Volume];
         if (volumeData != null)
         {
            string speakerVolumeStr = (string)volumeData[CMessageParser.ApiStringConstants.AudioPropertiesVolumeType.Speaker];
            if (speakerVolumeStr != null) speakerVolume = System.Convert.ToInt16(speakerVolumeStr);

            string microphoneVolumeStr = (string)volumeData[CMessageParser.ApiStringConstants.AudioPropertiesVolumeType.Microphone];
            if (microphoneVolumeStr != null) microphoneVolume = System.Convert.ToInt16(microphoneVolumeStr);
         }

         if (OnAudioPropertiesStatus != null)
         {
            OnAudioPropertiesStatus(this, new AudioPropertiesStatusEventArgs(speakerModeEnabled, speakerMuted, microphoneMuted, speakerVolume, microphoneVolume));
         }
       }

      private void HandleMissedCallsStatus(Hashtable data)
      {
         int count = 0;

         string countStr = (string)data[CMessageParser.ApiStringConstants.MissedCallsStatusResponse.Count];
         if (countStr != null) count = System.Convert.ToInt32(countStr);

         if (OnMissedCallsStatus != null)
         {
            OnMissedCallsStatus(this, new MissedCallsStatusEventArgs(count));
         }
      }

      private void HandleVoiceMailStatus(ArrayList voiceMailsList)
      {
         List<VoiceMailInfo> voiceMailInfoList = new List<VoiceMailInfo>();

         foreach (Hashtable voiceMail in voiceMailsList)
         {
            short accountId = 0;
            int count = 0;
            bool hasNewVoiceMail = false;

            string accountIdStr = (string)voiceMail[CMessageParser.ApiStringConstants.VoiceMailStatusResponse.AccountId];
            if (accountIdStr != null) accountId = System.Convert.ToInt16(accountIdStr);

            string accountName = (string)voiceMail[CMessageParser.ApiStringConstants.VoiceMailStatusResponse.AccountName];

            string countStr = (string)voiceMail[CMessageParser.ApiStringConstants.VoiceMailStatusResponse.Count];
            if (countStr != null) 
            {
               count = System.Convert.ToInt32(countStr);
               if (count != 0) hasNewVoiceMail = true;
            }

            VoiceMailInfo voiceMailInfo = new VoiceMailInfo(accountId, accountName, count, hasNewVoiceMail);
            voiceMailInfoList.Add(voiceMailInfo);
         }

         if (OnVoiceMailStatus != null)
         {
            OnVoiceMailStatus(this, new VoiceMailStatusEventArgs(voiceMailInfoList));
         }
      }

      private void HandleCallHistoryStatus(ArrayList callHistoryList)
      {
         List<CallHistoryItem> callHistoryItemList = new List<CallHistoryItem>();

         foreach (Hashtable callHistory in callHistoryList)
         {
            CallHistoryEntryTypes type = CallHistoryEntryTypes.Missed;
            int duration = 0;
            long timeInitiated = 0;

            string typeStr = (string)callHistory[CMessageParser.ApiStringConstants.CallHistoryStatusResponse.Type];
            if (typeStr != null)
            {
               switch (typeStr)
               {
                  case CMessageParser.ApiStringConstants.CallHistoryEntryType.Missed:
                     type = CallHistoryEntryTypes.Missed;
                     break;
                  case CMessageParser.ApiStringConstants.CallHistoryEntryType.Received:
                     type = CallHistoryEntryTypes.Received;
                     break;
                  case CMessageParser.ApiStringConstants.CallHistoryEntryType.Dialed:
                     type = CallHistoryEntryTypes.Dialed;
                     break;
               }
            }

            string number = (string)callHistory[CMessageParser.ApiStringConstants.CallHistoryStatusResponse.Number];

            string displayName = (string)callHistory[CMessageParser.ApiStringConstants.CallHistoryStatusResponse.DisplayName];

            string durationStr = (string)callHistory[CMessageParser.ApiStringConstants.CallHistoryStatusResponse.Duration];
            if (durationStr != null) duration = System.Convert.ToInt32(durationStr);

            string timeInitiatedStr = (string)callHistory[CMessageParser.ApiStringConstants.CallHistoryStatusResponse.TimeInitiated];
            if (timeInitiatedStr != null) timeInitiated = System.Convert.ToInt64(timeInitiatedStr);

            CallHistoryItem callHistoryItem = new CallHistoryItem(type, number, displayName, duration, timeInitiated);
            callHistoryItemList.Add(callHistoryItem);
         }

         if (OnCallHistoryStatus != null)
         {
            OnCallHistoryStatus(this, new CallHistoryStatusEventArgs(callHistoryItemList));
         }
      }

      private void HandleSystemSettingsStatus(Hashtable data)
      {
         bool callRightAway = false;
         CallTypes defaultCallType = CallTypes.audio;

         string callRightAwayStr = (string)data[CMessageParser.ApiStringConstants.SystemSettingsStatusResponse.CallRightAwayOnceNumberSelected];
         if (callRightAwayStr != null) callRightAway = (callRightAwayStr == CMessageParser.ApiStringConstants.GenericTrueFalse.True);

         string defaultCallTypeStr = (string)data[CMessageParser.ApiStringConstants.SystemSettingsStatusResponse.DefaultCallType];
         if (defaultCallTypeStr != null)
         {
            switch (defaultCallTypeStr)
            {
               case CMessageParser.ApiStringConstants.GenericAudioVideoConference.Audio:
                  defaultCallType = CallTypes.audio;
                  break;
               case CMessageParser.ApiStringConstants.GenericAudioVideoConference.Video:
                  defaultCallType = CallTypes.video;
                  break;
               case CMessageParser.ApiStringConstants.GenericAudioVideoConference.Conference:
                  defaultCallType = CallTypes.conference;
                  break;
            }
         }

         if (OnSystemSettingsStatus != null)
         {
            OnSystemSettingsStatus(this, new SystemSettingsStatusEventArgs(defaultCallType, callRightAway));
         }
      }

      #endregion
      
      #region BRIA_API_MESSAGE_CONSTRUCTION

      private enum ApiRequests
      {
         status,
         checkVoiceMail,
         showHistory,
         audioProperties,
         dtmf,
         callOptions,
         bringToFront,
         call,
         im,
         callVoicemail,
         checkVoicemail,
         endCall,
         hold,
         resume,
         answer
      }

      private string GetNextTransactionID()
      {
         m_LastTransactionID++;
         return m_LastTransactionID.ToString();
      }

      private string ConstructApiMessage(ApiRequests request, string content)
      {
         string msg = "GET /";
         msg += request.ToString();
         msg += "\r\nUser-Agent: Bria Desktop API C# Sample App (by CounterPath)";
         msg += "\r\nTransaction-ID: ";
         msg += GetNextTransactionID();
         msg += "\r\nContent-Type: application/xml";
         msg += "\r\nContent-Length: ";

         if (content != null)
         {
            msg += content.Length.ToString();
            msg += "\r\n\r\n";
            msg += content;
         }
         else
         {
            msg += "0";
         }

         return msg;
      }

      #endregion

      #region BRIA_API_COMMANDS

      private void SendMessage(string msg)
      {
          SendSocketMessage(msg);
      }

      private const string XmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n";

      private void API_GetStatus(StatusTypes type)
      {
         string content = XmlDeclaration + "<status>\r\n <type>" + type.ToString() + "</type>\r\n</status>";
         string msg = ConstructApiMessage(ApiRequests.status, content);
         SendMessage(msg);
      }

      private void API_GetStatusCallHistory(int maxCount, CallHistoryFilterTypes filter)
      {
         string content = XmlDeclaration + "<status>\r\n";
         content += " <type>" + StatusTypes.callHistory.ToString() + "</type>\r\n";
         content += " <count>" + maxCount.ToString() + "</count>\r\n";
         content += " <entryType>" + filter.ToString() + "</entryType>\r\n";
         content += "</status>";
         string msg = ConstructApiMessage(ApiRequests.status, content);
         SendMessage(msg);
      }

      private void API_SetCallOptions(string anonymous, string autoAnswer, string lettersToNumbers)
      {
         string content = XmlDeclaration + "<callOptions>\r\n";

         if (anonymous != null)
         {
            content += " <anonymous>" + anonymous + "</anonymous>\r\n";
         }

         if (lettersToNumbers != null)
         {
            content += " <lettersToNumbers>" + lettersToNumbers + "</lettersToNumbers>\r\n"; 
         }

         if (autoAnswer != null)
         {
            content += " <autoAnswer>" + autoAnswer + "</autoAnswer>\r\n";
         }

         content += "</callOptions>";

         string msg = ConstructApiMessage(ApiRequests.callOptions, content);
         SendMessage(msg);
      }

      private void API_BringToFront()
      {
         string msg = ConstructApiMessage(ApiRequests.bringToFront, null);
         SendMessage(msg);
      }

      private void API_OpenCallHistory(CallHistoryFilterTypes type, string filter)
      {
         string content = XmlDeclaration + "<filter>\r\n <type>" + type.ToString() + "</type>\r\n <text>" + filter + "</text>\r\n</filter>";
         string msg = ConstructApiMessage(ApiRequests.showHistory, content);
         SendMessage(msg);
      }

      private void API_PlaceCall(string target, CallTypes type)
      {
         string content = XmlDeclaration + "<dial type=\"" + type.ToString() + "\">\r\n <number>" + target + "</number>\r\n <displayName></displayName>\r\n</dial>";
         string msg = ConstructApiMessage(ApiRequests.call, content);
         SendMessage(msg);
      }

      private void API_AnswerCall(string callId, bool withVideo)
      {
         string content = XmlDeclaration + "<answerCall>\r\n <callId>" + callId + "</callId>\r\n <withVideo>" + withVideo.ToString() + "</withVideo>\r\n</answerCall>";
         string msg = ConstructApiMessage(ApiRequests.answer, content);
         SendMessage(msg);
      }

      private void API_HoldCall(string callId)
      {
         string content = XmlDeclaration + "<holdCall>\r\n <callId>" + callId + "</callId>\r\n</holdCall>";
         string msg = ConstructApiMessage(ApiRequests.hold, content);
         SendMessage(msg);
      }

      private void API_ResumeCall(string callId)
      {
         string content = XmlDeclaration + "<resumeCall>\r\n <callId>" + callId + "</callId>\r\n</resumeCall>";
         string msg = ConstructApiMessage(ApiRequests.resume, content);
         SendMessage(msg);
      }

      private void API_SendDTMF(Char key)
      {
         string content = XmlDeclaration + "<dtmf>\r\n <tone digit=\"" + key + "\">pulse</tone>\r\n</dtmf>";
         string msg = ConstructApiMessage(ApiRequests.dtmf, content);
         SendMessage(msg);
      }

      private void API_SetAudioProperties(string mute, string speakerMute, string speaker, bool speakerSupressDialtone, string volumeSpeaker, string volumeMicrophone)
      {
         string content = XmlDeclaration + "<audioProperties>\r\n";

         if (mute != null)
         {
            content += " <mute>" + mute + "</mute>\r\n";
         }

         if (speakerMute != null)
         {
            content += " <speakerMute>" + speakerMute + "</speakerMute>\r\n";
         }

         if (speaker != null)
         {
            content += " <speaker supressDialtone=\"" + speakerSupressDialtone.ToString().ToLower() + "\">" + speaker + "</speaker>\r\n";
         }

         if (volumeSpeaker != null)
         {
            content += " <volume type=\"speaker\">" + volumeSpeaker + "</volume>\r\n";
         }

         if (volumeMicrophone != null)
         {
            content += " <volume type=\"microphone\">" + volumeMicrophone + "</volume>\r\n";
         }

         content += "</audioProperties>";

         string msg = ConstructApiMessage(ApiRequests.audioProperties, content);
         SendMessage(msg);
      }

      private void API_EndCall(string callId)
      {
         string content = XmlDeclaration + "<endCall>\r\n <callId>" + callId + "</callId>\r\n</endCall>";
         string msg = ConstructApiMessage(ApiRequests.endCall, content);
         SendMessage(msg);
      }

      private void API_CheckVoiceMail(Int16 accountId)
      {
         string content = XmlDeclaration + "<checkVoiceMail>\r\n <accountId>" + accountId.ToString() + "</accountId>\r\n</checkVoiceMail>";
         string msg = ConstructApiMessage(ApiRequests.checkVoiceMail, content);
         SendMessage(msg);
      }

      #endregion

      #region PRIVATE_MEMBER_VARIABLES

      private long m_LastTransactionID = 0;

      #endregion
   }
}
