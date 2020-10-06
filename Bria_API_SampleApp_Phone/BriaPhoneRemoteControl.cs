// (C) Copyright 2014 - CounterPath Corporation. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace Bria_API_CSharp_SampleApp
{
   public partial class BriaPhoneRemoteControl : Form
   {
      #region MAIN_FORM_APP_DEFINITON

      BriaAPI briaAPI = new BriaAPI();

      public BriaPhoneRemoteControl()
      {
         InitializeComponent();
      }

      ~BriaPhoneRemoteControl()
      {
         briaAPI.Stop();
      }

      private void BriaPhoneRemoteControl_Load(object sender, EventArgs e)
      {
         // Use delegates to handle events to ensure GUI updates happen on the Main Thread
         onConnectedDelegate = new OnConnectedDelegate(MainThread_OnConnected);
         onErrorDelegate = new OnErrorDelegate(MainThread_OnError);
         onDisconnectedDelegate = new OnDisconnectedDelegate(MainThread_OnDisconnected);

         onPhoneStatusDelegate = new OnPhoneStatusDelegate(MainThread_OnPhoneStatus);
         onCallStatusDelegate = new OnCallStatusDelegate(MainThread_OnCallStatus);
         onCallOptionsStatusDelegate = new OnCallOptionsStatusDelegate(MainThread_OnCallOptionsStatus);
         onAudioPropertiesStatusDelegate = new OnAudioPropertiesStatusDelegate(MainThread_OnAudioPropertiesStatus);
         onMissedCallsStatusDelegate = new OnMissedCallsStatusDelegate(MainThread_OnMissedCallsStatus);
         onVoiceMailStatusDelegate = new OnVoiceMailStatusDelegate(MainThread_OnVoiceMailStatus);
         onCallHistoryStatusDelegate = new OnCallHistoryStatusDelegate(MainThread_OnCallHistoryStatus);
         onSystemSettingsStatusDelegate = new OnSystemSettingsStatusDelegate(MainThread_OnSystemSettingsStatus);
         onErrorReceivedDelegate = new OnErrorReceivedDelegate(MainThread_OnErrorReceived);

         briaAPI.OnStatusChanged += new EventHandler<BriaAPI.StatusChangedEventArgs>(OnStatusChanged);

         briaAPI.OnPhoneStatus += new EventHandler<BriaAPI.PhoneStatusEventArgs>(OnPhoneStatus);
         briaAPI.OnCallStatus += new EventHandler<BriaAPI.CallStatusEventArgs>(OnCallStatus);
         briaAPI.OnCallOptionsStatus += new EventHandler<BriaAPI.CallOptionsStatusEventArgs>(OnCallOptionsStatus);
         briaAPI.OnAudioPropertiesStatus += new EventHandler<BriaAPI.AudioPropertiesStatusEventArgs>(OnAudioPropertiesStatus);
         briaAPI.OnMissedCallsStatus += new EventHandler<BriaAPI.MissedCallsStatusEventArgs>(OnMissedCallsStatus);
         briaAPI.OnVoiceMailStatus += new EventHandler<BriaAPI.VoiceMailStatusEventArgs>(OnVoiceMailStatus);
         briaAPI.OnCallHistoryStatus += new EventHandler<BriaAPI.CallHistoryStatusEventArgs>(OnCallHistoryStatus);
         briaAPI.OnSystemSettingsStatus += new EventHandler<BriaAPI.SystemSettingsStatusEventArgs>(OnSystemSettingsStatus);
         briaAPI.OnErrorReceived += new EventHandler<BriaAPI.ErrorReceivedEventArgs>(OnErrorReceived);

         briaAPI.OnError += new EventHandler(OnError);
         briaAPI.OnDisconnected += new EventHandler(OnDisconnected);
         briaAPI.OnConnected += new EventHandler(OnConnected);

         ConnectToBria();
      }

      private void ConnectToBria()
      {
         SetPhoneStatusText("Attempting to connect to Bria...");
         briaAPI.Start();
      }

      #endregion

      #region EVENTS_DELEGATE_DEFINITIONS

      private delegate void OnConnectedDelegate();
      private OnConnectedDelegate onConnectedDelegate;

      private delegate void OnErrorDelegate();
      private OnErrorDelegate onErrorDelegate;

      private delegate void OnDisconnectedDelegate();
      private OnDisconnectedDelegate onDisconnectedDelegate;

      private delegate void OnPhoneStatusDelegate(BriaAPI.PhoneStatusEventArgs args);
      private OnPhoneStatusDelegate onPhoneStatusDelegate;

      private delegate void OnCallStatusDelegate(BriaAPI.CallStatusEventArgs args);
      private OnCallStatusDelegate onCallStatusDelegate;

      private delegate void OnCallOptionsStatusDelegate(BriaAPI.CallOptionsStatusEventArgs args);
      private OnCallOptionsStatusDelegate onCallOptionsStatusDelegate;

      private delegate void OnAudioPropertiesStatusDelegate(BriaAPI.AudioPropertiesStatusEventArgs args);
      private OnAudioPropertiesStatusDelegate onAudioPropertiesStatusDelegate;

      private delegate void OnMissedCallsStatusDelegate(BriaAPI.MissedCallsStatusEventArgs args);
      private OnMissedCallsStatusDelegate onMissedCallsStatusDelegate;

      private delegate void OnVoiceMailStatusDelegate(BriaAPI.VoiceMailStatusEventArgs args);
      private OnVoiceMailStatusDelegate onVoiceMailStatusDelegate;

      private delegate void OnCallHistoryStatusDelegate(BriaAPI.CallHistoryStatusEventArgs args);
      private OnCallHistoryStatusDelegate onCallHistoryStatusDelegate;

      private delegate void OnSystemSettingsStatusDelegate(BriaAPI.SystemSettingsStatusEventArgs args);
      private OnSystemSettingsStatusDelegate onSystemSettingsStatusDelegate;

      private delegate void OnErrorReceivedDelegate(BriaAPI.ErrorReceivedEventArgs args);
      private OnErrorReceivedDelegate onErrorReceivedDelegate;

      #endregion
      
      #region GUI_STATE_VARIABLES

      // General State
      private bool m_IsConnectedToBria = false;
      private bool m_IsActiveOnThePhone = false;
      private String m_lastNumberEntered = "";

      // Phone State
      private bool m_IsPhoneReady = false;
      private bool m_AreCallsAllowed = false;
      private Int16 m_ErrorCode;
      private Int16 m_MaxLines;
      private String m_CallNotAllowedReason;
      private BriaAPI.AccountStates m_AccountStatus;

      // Phone Lines
      public class RemoteParty
      {
         public String Number;
         public String DisplayName;
         public BriaAPI.CallStates State;
         public Int64 TimeInitiated;
      }
      public class PhoneLine
      {
         public String Id;
         public BriaAPI.HoldStates HoldState;
         public List<RemoteParty> RemoteParties;

         public PhoneLine(String id)
         {
            Id = id;
            RemoteParties = new List<RemoteParty>();
         }

         public Boolean IsRinging;
      }
      private PhoneLine[] phoneLines = new PhoneLine[6];

      // Call Options
      private bool m_IsAnonymousEnabled = false;
      private bool m_IsLettersToNumbersEnabled = false;
      private bool m_IsAutoAnswerEnabled = false;

      // Audio Properties
      private bool m_SpeakerModeEnabled = false;
      private bool m_MicrophoneIsMuted = false;
      private bool m_SpeakerIsMuted = false;
      private Int16 m_SpeakerVolume = 100;
      private Int16 m_MicrophoneVolume = 100;

      // Missed Calls
      private int m_MissedCallsCount = 0;

      // VoiceMail
      private bool m_MessagesAreWaiting = false;

      #endregion

      #region BRIA_API_WRAPPER_EVENT_HANDLERS

      // Most of these event handlers are used only to 'transport' the event onto the Main Thread
      // using a Delegate pattern. Only 'OnStatusChanged' does not follow this model.

      private void OnConnected(object sender, EventArgs args)
      {
         this.BeginInvoke(onConnectedDelegate);
      }

      private void OnError(object sender, EventArgs args)
      {
         this.BeginInvoke(onErrorDelegate);
      }

      private void OnDisconnected(object sender, EventArgs args)
      {
         this.BeginInvoke(onDisconnectedDelegate);
      }

      private void OnStatusChanged(object sender, BriaAPI.StatusChangedEventArgs args)
      {
         // This event can largely be handled by the Wrapper and the default handling there,
         // but for the CallHistoryStatusChanged we don't want to automtaically fetch the
         // entire list on every change, only when we want to display it, so that event we
         // claim to handle here

         // Adding the SystemSettingsStatusChanged to this category also

         if ((args.StatusType == BriaAPI.StatusTypes.callHistory)
            || (args.StatusType == BriaAPI.StatusTypes.systemSettings))
         {
            args.Handled = true;
         }
      }
      
      private void OnPhoneStatus(object sender, BriaAPI.PhoneStatusEventArgs args)
      {
         this.BeginInvoke(onPhoneStatusDelegate, new Object[] { args });
      }

      private void OnCallStatus(object sender, BriaAPI.CallStatusEventArgs args)
      {
         this.BeginInvoke(onCallStatusDelegate, new Object[] { args });
      }

      private void OnCallOptionsStatus(object sender, BriaAPI.CallOptionsStatusEventArgs args)
      {
         this.BeginInvoke(onCallOptionsStatusDelegate, new Object[] { args });
      }

      private void OnAudioPropertiesStatus(object sender, BriaAPI.AudioPropertiesStatusEventArgs args)
      {
         this.BeginInvoke(onAudioPropertiesStatusDelegate, new Object[] { args });
      }

      private void OnMissedCallsStatus(object sender, BriaAPI.MissedCallsStatusEventArgs args)
      {
         this.BeginInvoke(onMissedCallsStatusDelegate, new Object[] { args });
      }

      private void OnVoiceMailStatus(object sender, BriaAPI.VoiceMailStatusEventArgs args)
      {
         this.BeginInvoke(onVoiceMailStatusDelegate, new Object[] { args });
      }

      private void OnCallHistoryStatus(object sender, BriaAPI.CallHistoryStatusEventArgs args)
      {
         this.BeginInvoke(onCallHistoryStatusDelegate, new Object[] { args });
      }

      private void OnSystemSettingsStatus(object sender, BriaAPI.SystemSettingsStatusEventArgs args)
      {
         this.BeginInvoke(onSystemSettingsStatusDelegate, new Object[] { args });
      }

      private void OnErrorReceived(object sender, BriaAPI.ErrorReceivedEventArgs args)
      {
         this.BeginInvoke(onErrorReceivedDelegate, new Object[] { args });
      }

      #endregion

      #region MAIN_THREAD_EVENT_METHODS

      // The event handlers running on the Main Thread are used to extract the  
      // necessary information from the events and update the GUI as needed

      private void MainThread_OnConnected()
      {
         m_IsConnectedToBria = true;

         ClearErrorMessage();
         UpdatePhoneState();
         SetPhoneStatusText("Connected to Bria!");

         // "Poke" Bria to send updated status for things we want to reflect properly on the GUI 
         briaAPI.RequestAudioPropertiesStatus();
         briaAPI.RequestCallOptionsStatus();
         briaAPI.RequestMissedCallStatus();
         briaAPI.RequestVoiceMailStatus();
         briaAPI.RequestCallStatus();
      }

      Timer reconnectDelayTimer;
      private void OnReconnectDelayTimer(object sender, EventArgs e)
      {
         reconnectDelayTimer.Stop();
         reconnectDelayTimer = null;
         ConnectToBria();
      }

      private void StartReconnectDelayTimer(int timeout)
      {
         if (reconnectDelayTimer == null)
         {
            reconnectDelayTimer = new Timer();
            reconnectDelayTimer.Tick += new EventHandler(OnReconnectDelayTimer);
            reconnectDelayTimer.Interval = timeout;
            reconnectDelayTimer.Start();
         }
      }

      private void MainThread_OnError()
      {
         m_IsConnectedToBria = false;
         UpdatePhoneState();

         SetPhoneStatusText("Connecting to Bria failed. Waiting 5 seconds before reconnect...");
         StartReconnectDelayTimer(5000);
      }

      private void MainThread_OnDisconnected()
      {
         if (m_IsConnectedToBria == true)
         {
             SetPhoneStatusText("Disconnected from Bria. Waiting 5 seconds before reconnect...");
         }

         m_IsConnectedToBria = false;
         UpdatePhoneState();
          
         StartReconnectDelayTimer(5000);
      }

      private void MainThread_OnPhoneStatus(BriaAPI.PhoneStatusEventArgs args)
      {
         m_IsPhoneReady          = args.IsReady;
         m_AreCallsAllowed       = args.CallsAllowed;
         m_ErrorCode             = args.ErrorCode;
         m_MaxLines              = args.MaxLines;
         m_CallNotAllowedReason  = args.CallsNotAllowedReason;
         m_AccountStatus         = args.AccountStatus;

         UpdatePhoneState();
      }

      private void MainThread_OnCallStatus(BriaAPI.CallStatusEventArgs args)
      {
         Boolean[] lineInUse = new Boolean[6];

         List<BriaAPI.Call> callList = args.CallList;

         foreach (BriaAPI.Call call in callList)
         {
            Boolean existingCall = false;
            PhoneLine phoneLine = null;

            // Check if the call was previously in the list
            for (int i = 0; i < 6; i++)
            {
               phoneLine = phoneLines[i];

               if ((phoneLine != null) && (phoneLine.Id == call.CallId))
               {
                  phoneLine.RemoteParties.Clear();

                  lineInUse[i] = true;
                  existingCall = true;
                  break;
               }
            }

            // If the call is not already existing, we have to add as new call
            if (!existingCall)
            {
               PhoneLine newPhoneLine = new PhoneLine(call.CallId);
               phoneLine = newPhoneLine;

               // Find empty slot to put it in
               for (int i = 0; i < 6; i++)
               {
                  if (phoneLines[i] == null)
                  {
                     phoneLines[i] = phoneLine;
                     lineInUse[i] = true;
                     break;
                  }
               }
            }

            // And fill in the information
            phoneLine.HoldState = call.HoldState;

            foreach (BriaAPI.CallParticipant participant in call.ParticipantList)
            {
               RemoteParty remoteParty = new RemoteParty();
               remoteParty.Number = participant.Number;
               remoteParty.DisplayName = participant.DisplayName;
               remoteParty.TimeInitiated = participant.TimeInitiated;
               remoteParty.State = participant.CallState;

               phoneLine.RemoteParties.Add(remoteParty);

               if ((phoneLine.RemoteParties.Count == 1) && (remoteParty.State == BriaAPI.CallStates.Ringing))
               {
                  phoneLine.IsRinging = true;
               }
               else
               {
                  phoneLine.IsRinging = false;
               }
            }
         }

         // Finally clear out any phoneLine that is no longer active
         for (int i = 0; i < 6; i++)
         {
            if (lineInUse[i] == false)
            {
               phoneLines[i] = null;
            }
         }

         UpdateCallStates();
      }

      private void MainThread_OnCallOptionsStatus(BriaAPI.CallOptionsStatusEventArgs args)
      {
         m_IsAnonymousEnabled = args.IsAnonymousEnabled;
         m_IsLettersToNumbersEnabled = args.IsLettersToNumbersEnabled;
         m_IsAutoAnswerEnabled = args.IsAutoAnswerEnabled;

         UpdateFunctionButtonStates();
      }

      private void MainThread_OnAudioPropertiesStatus(BriaAPI.AudioPropertiesStatusEventArgs args)
      {
         m_SpeakerModeEnabled = args.IsSpeakerModeEnabled;
         m_MicrophoneIsMuted = args.IsMicrophoneMuted;
         m_SpeakerIsMuted = args.IsSpeakerMuted;
         m_SpeakerVolume = args.SpeakerVolume;
         m_MicrophoneVolume = args.MicrophoneVolume;

         UpdateAudioControls();
      }

      private void MainThread_OnMissedCallsStatus(BriaAPI.MissedCallsStatusEventArgs args)
      {
         m_MissedCallsCount = args.Count;

         UpdatePhoneState();
         UpdateFunctionButtonStates();
      }

      private void MainThread_OnVoiceMailStatus(BriaAPI.VoiceMailStatusEventArgs args)
      {
         // Advanced: If multiple SIP accounts are in use, each account can report messages waiting
         //           individually. To support that, the application would need to maintain and 
         //           display information per account. This can in turn be extended to use with the
         //           'checkVoiceMail' facility so an account number can be passed along there.
         //
         //           For this example we will just check if there are messages waiting on any account.

         bool messagesAreWaiting = false;

         foreach (BriaAPI.VoiceMailInfo voiceMailInfo in args.VoiceMailInfoList)
         {
            messagesAreWaiting |= voiceMailInfo.HasNewVoiceMail;
         }

         this.m_MessagesAreWaiting = messagesAreWaiting;

         UpdatePhoneState();
         UpdateFunctionButtonStates();
      }

      private void MainThread_OnCallHistoryStatus(BriaAPI.CallHistoryStatusEventArgs args)
      {
         List<BriaAPI.CallHistoryItem> callhistoryItemList = new List<BriaAPI.CallHistoryItem>(args.CallHistoryItemList);

         CallHistoryView view = new CallHistoryView(callhistoryItemList);

         view.ShowDialog();

         view.Dispose();
      }

      private void MainThread_OnSystemSettingsStatus(BriaAPI.SystemSettingsStatusEventArgs args)
      {
         String text = "Default Call Type: " + args.DefaultCallType.ToString() + "\r\nCall Right Away Once Number Selected: " + args.CallRightAwayOnceNumberSelected.ToString();
         
         MessageBox.Show(text, "Bria System Settings");
      }

      private void MainThread_OnErrorReceived(BriaAPI.ErrorReceivedEventArgs args)
      {
         this.ErrorMessage_Text.Text = "Last error received: " + args.ErrorCode.ToString() + " " + args.ErrorText + " (for transactionId " + args.TransactionId + ")";
         this.ClearError_Button.Visible = true;
         this.Update();
      }

      #endregion

      #region CONTROL_STATES

      private void SetPhoneStatusText(String text)
      {
         this.PhoneStatus_Text.Text = text;
         this.Update();
      }

      private void SetDialpadState(Boolean state)
      {
         this.Dialpad_0.Enabled = state;
         this.Dialpad_1.Enabled = state;
         this.Dialpad_2.Enabled = state;
         this.Dialpad_3.Enabled = state;
         this.Dialpad_4.Enabled = state;
         this.Dialpad_5.Enabled = state;
         this.Dialpad_6.Enabled = state;
         this.Dialpad_7.Enabled = state;
         this.Dialpad_8.Enabled = state;
         this.Dialpad_9.Enabled = state;
         this.Dialpad_Star.Enabled = state;
         this.Dialpad_Hash.Enabled = state;

         this.UseVideo_CheckBox.Enabled = state;
         this.Dial_Button.Enabled = !(m_IsActiveOnThePhone);
      }

      private void SetFunctionButtonStates(Boolean state)
      {
         this.Speaker_Button.Enabled = state;
         this.AutoAnswer_Button.Enabled = state;
         this.LettersToNumbers_Button.Enabled = state;
         this.HideID_Button.Enabled = state;
         this.VoiceMail_Button.Enabled = state;
         this.GetCallHistory_Button.Enabled = state;
         this.ShowMissedCalls_Button.Enabled = state;
         this.BringToFront_Button.Enabled = state;
      }

      private void SetAudioControlStates(Boolean state)
      {
         this.MicVolume_TrackBar.Enabled = state;
         this.MicMute_Button.Enabled = state;
         this.SpeakerVolume_TrackBar.Enabled = state;
         this.SpkMute_Button.Enabled = state;
      }

      private void UpdatePhoneState()
      {
         SetDialpadState(m_IsConnectedToBria);
         SetFunctionButtonStates(m_IsConnectedToBria);
         SetAudioControlStates(m_IsConnectedToBria);

         if (this.m_MessagesAreWaiting)
         {
            this.MessagesWaiting_Text.Text = "There are new Voice Mail messages waiting!";
         }
         else
         {
            this.MessagesWaiting_Text.Text = "";
         }

         if (this.m_MissedCallsCount > 0)
         {
            this.MissedCalls_Text.Text = "You have " + this.m_MissedCallsCount.ToString() + " missed calls!";
         }
         else
         {
            this.MissedCalls_Text.Text = "";
         }

         this.Update();
      }

      private void UpdateAudioControls()
      {
         if (m_MicrophoneIsMuted)
         {
            MicMute_Button.BackColor = Color.Red;
         }
         else
         {
            MicMute_Button.BackColor = DefaultBackColor;
         }

         if (m_SpeakerIsMuted)
         {
            SpkMute_Button.BackColor = Color.Red;
         }
         else
         {
            SpkMute_Button.BackColor = DefaultBackColor;
         }

         if (m_SpeakerModeEnabled)
         {
            Speaker_Button.BackColor = Color.Red;
         }
         else
         {
            Speaker_Button.BackColor = DefaultBackColor;
         }

         MicVolume_TrackBar.Value = m_MicrophoneVolume;
         SpeakerVolume_TrackBar.Value = m_SpeakerVolume;

         this.Update();
      }

      private void UpdateFunctionButtonStates()
      {
         if (m_IsAnonymousEnabled)
         {
            this.HideID_Button.BackColor = Color.Red;
         }
         else
         {
            this.HideID_Button.BackColor = DefaultBackColor;
         }

         if (m_IsAutoAnswerEnabled)
         {
            this.AutoAnswer_Button.BackColor = Color.Red;
         }
         else
         {
            this.AutoAnswer_Button.BackColor = DefaultBackColor;
         }

         if (m_IsLettersToNumbersEnabled)
         {
            this.LettersToNumbers_Button.BackColor = Color.Red;
         }
         else
         {
            this.LettersToNumbers_Button.BackColor = DefaultBackColor;
         }

         if (m_MessagesAreWaiting)
         {
            this.VoiceMail_Button.BackColor = Color.Yellow;
         }
         else
         {
            this.VoiceMail_Button.BackColor = DefaultBackColor;
         }

         if (m_MissedCallsCount > 0)
         {
            this.ShowMissedCalls_Button.BackColor = Color.Yellow;
         }
         else
         {
            this.ShowMissedCalls_Button.BackColor = DefaultBackColor;
         }

         this.Update();
      }

      private bool UpdatePhoneLine(TextBox tb, Button holdBt, Button resumeBt, Button endBt, PhoneLine phoneLine)
      {
         bool isActive = false;

         if (phoneLine == null)
         {
            tb.Text = "Idle";
            holdBt.Enabled = false;
            resumeBt.Enabled = false;
            endBt.Enabled = false;
         }
         else
         {
            String statusText = "";

            bool isRinging = false;
            bool isEnded = false;
            Int16 remoteParticipantCount = (Int16)phoneLine.RemoteParties.Count;

            if (remoteParticipantCount == 1)
            {
               RemoteParty remoteParty = phoneLine.RemoteParties[0];

               if (remoteParty.State == BriaAPI.CallStates.Ringing)
               {
                  statusText = "Incoming call from: ";
                  isRinging = true;
               }
               else if (remoteParty.State == BriaAPI.CallStates.Connecting)
               {
                  statusText = "Outbound call to: ";
               }
               else if (remoteParty.State == BriaAPI.CallStates.Connected)
               {
                  if (phoneLine.HoldState == BriaAPI.HoldStates.LocalHold)
                  {
                     statusText = "Call put on Hold: ";
                  }
                  else if (phoneLine.HoldState == BriaAPI.HoldStates.RemoteHold)
                  {
                     statusText = "Call held by remote: ";
                  }
                  else
                  {
                     statusText = "Active call with: ";
                     isActive = true;
                  }
               }
               else if (remoteParty.State == BriaAPI.CallStates.Failed)
               {
                  statusText = "Call Failed [";
                  isEnded = true;
               }
               else if (remoteParty.State == BriaAPI.CallStates.Ended)
               {
                  statusText = "Call Ended [";
                  isEnded = true;
               }

               statusText += remoteParty.DisplayName;
               statusText += " (" + remoteParty.Number + ")";

               if (isEnded)
               {
                  statusText += "]";
               }
            }
            else
            {
               statusText  = "Conference call with ";
               statusText += remoteParticipantCount.ToString();
               statusText += " participants";

               if (phoneLine.HoldState == BriaAPI.HoldStates.LocalHold)
               {
                  statusText += " on Hold";
               }
               else if (phoneLine.HoldState == BriaAPI.HoldStates.RemoteHold)
               {
                  statusText = " held by remote";
               }
               else
               {
                  isActive = true;
               }
            }

            tb.Text = statusText;

            if ((isRinging) || (phoneLine.HoldState == BriaAPI.HoldStates.LocalHold))
            {
               holdBt.Enabled = false;
               resumeBt.Enabled = true;
            }
            else
            {
               holdBt.Enabled = true;
               resumeBt.Enabled = false;
            }

            endBt.Enabled = true;
         }

         return isActive;
      }

      private void UpdateCallStates()
      {
         bool active1 = UpdatePhoneLine(this.PhoneLine1Status_Text, this.PhoneLine1Hold_Button, this.PhoneLine1Resume_Button, this.PhoneLine1End_Button, phoneLines[0]);
         bool active2 = UpdatePhoneLine(this.PhoneLine2Status_Text, this.PhoneLine2Hold_Button, this.PhoneLine2Resume_Button, this.PhoneLine2End_Button, phoneLines[1]);
         bool active3 = UpdatePhoneLine(this.PhoneLine3Status_Text, this.PhoneLine3Hold_Button, this.PhoneLine3Resume_Button, this.PhoneLine3End_Button, phoneLines[2]);
         bool active4 = UpdatePhoneLine(this.PhoneLine4Status_Text, this.PhoneLine4Hold_Button, this.PhoneLine4Resume_Button, this.PhoneLine4End_Button, phoneLines[3]);
         bool active5 = UpdatePhoneLine(this.PhoneLine5Status_Text, this.PhoneLine5Hold_Button, this.PhoneLine5Resume_Button, this.PhoneLine5End_Button, phoneLines[4]);
         bool active6 = UpdatePhoneLine(this.PhoneLine6Status_Text, this.PhoneLine6Hold_Button, this.PhoneLine6Resume_Button, this.PhoneLine6End_Button, phoneLines[5]);

         m_IsActiveOnThePhone = active1 || active2 || active3 || active4 || active5 || active6;

         UpdatePhoneState();

         this.Update();
      }

      private void ClearErrorMessage()
      {
         this.ErrorMessage_Text.Text = "";
         this.ClearError_Button.Visible = false;
         this.Update();
      }

      #endregion

      #region DIALPAD_BUTTONS

      private void DialpadButtonPressed(char key)
      {
         if (m_IsActiveOnThePhone)
         {
            briaAPI.RequestSendDTMF(key);
         }
         else
         {
            this.NumberEntry_Text.Text += key;
            this.NumberEntry_Text.Update();
         }
      }

      private void Dialpad_1_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('1');
      }

      private void Dialpad_2_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('2');
      }

      private void Dialpad_3_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('3');
      }

      private void Dialpad_4_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('4');
      }

      private void Dialpad_5_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('5');
      }

      private void Dialpad_6_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('6');
      }

      private void Dialpad_7_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('7');
      }

      private void Dialpad_8_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('8');
      }

      private void Dialpad_9_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('9');
      }

      private void Dialpad_Star_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('*');
      }

      private void Dialpad_0_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('0');
      }

      private void Dialpad_Hash_Click(object sender, EventArgs e)
      {
         DialpadButtonPressed('#');
      }

      private void Dial_Button_Click(object sender, EventArgs e)
      {
         String number = this.NumberEntry_Text.Text;

         if (number.Length == 0)
         {
            // Redial - load last called number
            this.NumberEntry_Text.Text = m_lastNumberEntered;
         }
         else
         {
            m_lastNumberEntered = number;
            this.NumberEntry_Text.Text = "";
            if (this.UseVideo_CheckBox.Checked == true)
            {
               briaAPI.RequestPlaceCall(number, BriaAPI.CallTypes.video);
            }
            else
            {
               briaAPI.RequestPlaceCall(number, BriaAPI.CallTypes.audio);
            }
         }
      }

      private void ClearInput_Button_Click(object sender, EventArgs e)
      {
         this.NumberEntry_Text.Text = "";
      }

      #endregion

      #region AUDIO_CONTROLS

      private void SpkMute_Button_Click(object sender, EventArgs e)
      {
         if (m_SpeakerIsMuted)
         {
            briaAPI.RequestSetSpeakerMute(false);
         }
         else
         {
            briaAPI.RequestSetSpeakerMute(true);
         }
      }

      private void MicMute_Button_Click(object sender, EventArgs e)
      {
         if (m_MicrophoneIsMuted)
         {
            briaAPI.RequestSetMicrophoneMute(false);
         }
         else
         {
            briaAPI.RequestSetMicrophoneMute(true);
         }
      }

      private void MicVolTrackBar_ValueChanged(object sender, EventArgs e)
      {
         StartMicVolTrackBar_ValueChangedTimer();
      }

      Timer MicVolTrackBar_ValueChangedTimer;
      private void StartMicVolTrackBar_ValueChangedTimer()
      {
         if (MicVolTrackBar_ValueChangedTimer == null)
         {
            MicVolTrackBar_ValueChangedTimer = new Timer();
            MicVolTrackBar_ValueChangedTimer.Tick += new EventHandler(OnMicVolTrackBar_ValueChangedTimer);
         }
         else
         {
            MicVolTrackBar_ValueChangedTimer.Stop();
         }
         
         MicVolTrackBar_ValueChangedTimer.Interval = 250;
         MicVolTrackBar_ValueChangedTimer.Start();
      }

      private void OnMicVolTrackBar_ValueChangedTimer(object sender, EventArgs e)
      {
         MicVolTrackBar_ValueChangedTimer.Stop();
         MicVolTrackBar_ValueChangedTimer = null;
         
         m_MicrophoneVolume = (Int16)MicVolume_TrackBar.Value;
         briaAPI.RequestSetMicrophoneVolume(m_MicrophoneVolume); 
      }

      Timer SpkVolTrackBar_ValueChangedTimer;
      private void StartSpkVolTrackBar_ValueChangedTimer()
      {
         if (SpkVolTrackBar_ValueChangedTimer == null)
         {
            SpkVolTrackBar_ValueChangedTimer = new Timer();
            SpkVolTrackBar_ValueChangedTimer.Tick += new EventHandler(OnSpkVolTrackBar_ValueChangedTimer);
         }
         else
         {
            SpkVolTrackBar_ValueChangedTimer.Stop();
         }

         SpkVolTrackBar_ValueChangedTimer.Interval = 250;
         SpkVolTrackBar_ValueChangedTimer.Start();
      }

      private void SpkVolTrackBar_ValueChanged(object sender, EventArgs e)
      {
         StartSpkVolTrackBar_ValueChangedTimer();
      }

      private void OnSpkVolTrackBar_ValueChangedTimer(object sender, EventArgs e)
      {
         SpkVolTrackBar_ValueChangedTimer.Stop();
         SpkVolTrackBar_ValueChangedTimer = null;

         m_SpeakerVolume = (Int16)SpeakerVolume_TrackBar.Value;
         briaAPI.RequestSetSpeakerVolume(m_SpeakerVolume);
      }

      #endregion

      #region FUNCTION_BUTTONS

      private void ClearError_Button_Click(object sender, EventArgs e)
      {
         ClearErrorMessage();
      }

      private void ShowSystemSettings_Button_Click(object sender, EventArgs e)
      {
         briaAPI.RequestSystemSettingsStatus();
      }

      private void Speaker_Button_Click(object sender, EventArgs e)
      {
         if (m_SpeakerModeEnabled)
         {
            briaAPI.RequestSetSpeakerMode(false, false);
         }
         else
         {
            briaAPI.RequestSetSpeakerMode(true, true);
         }
      }

      private void AutoAnswer_Button_Click(object sender, EventArgs e)
      {
         if (m_IsAutoAnswerEnabled)
         {
            briaAPI.RequestSetAutoAnswerMode(false);
         }
         else
         {
            briaAPI.RequestSetAutoAnswerMode(true);
         }
      }

      private void LettersToNumbers_Button_Click(object sender, EventArgs e)
      {
         if (m_IsLettersToNumbersEnabled)
         {
            briaAPI.RequestSetLettersToNumbersMode(false);
         }
         else
         {
            briaAPI.RequestSetLettersToNumbersMode(true);
         }
      }

      private void HideID_Button_Click(object sender, EventArgs e)
      {
         if (m_IsAnonymousEnabled)
         {
            briaAPI.RequestSetAnonymousMode(false);
         }
         else
         {
            briaAPI.RequestSetAnonymousMode(true);
         }
      }

      private void VoiceMail_Button_Click(object sender, EventArgs e)
      {
         // Advanced: Bria could be instructed to "check" voice mail on any of several
         // SIP accounts, but for this sample app we'll just assume only a single SIP
         // account in account 0. The request will only result in a call to the VM number
         // if there are messages waiting as far as Bria is aware
         briaAPI.RequestCheckVoiceMail(0);
      }

      private void GetCallHistory_Button_Click(object sender, EventArgs e)
      {
         // Advanced: Here always requestion 50 entries (max) which is the most Bria
         // will store, i.e. get the entire History, and also getting ALL the entry
         // types. Parameters could be tweaked to retrieve filtered information
         briaAPI.RequestCallHistoryStatus(50, BriaAPI.CallHistoryFilterTypes.all);
      }

      private void ShowMissedCalls_Button_Click(object sender, EventArgs e)
      {
         // Advanced: If needed there could be a Filter Text input and some kind of selection of History Type on the GUI
         briaAPI.RequestBringCallHistoryToFront(BriaAPI.CallHistoryFilterTypes.missed, "");
      }

      private void BringToFront_Button_Click(object sender, EventArgs e)
      {
         briaAPI.RequestBringPhoneToFront();
      }
      
      #endregion

      #region PHONELINE_HANDLING

      private void PhoneLine_HoldClickHelper(int line)
      {
         PhoneLine phoneLine = phoneLines[line] as PhoneLine;
         briaAPI.RequestPutCallOnHold(phoneLine.Id);
      }

      private void PhoneLine_ResumeClickHelper(int line)
      {
         PhoneLine phoneLine = phoneLines[line] as PhoneLine;

         if (phoneLine.IsRinging)
         {
            briaAPI.RequestAnswerCall(phoneLine.Id, false);
         }
         else
         {
            briaAPI.RequestResumeCall(phoneLine.Id);
         }
      }

      private void PhoneLine_EndClickHelper(int line)
      {
         PhoneLine phoneLine = phoneLines[line] as PhoneLine;
         briaAPI.RequestEndCall(phoneLine.Id);
      }

      private void PhoneLine1Hold_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_HoldClickHelper(0);
      }

      private void PhoneLine1Resume_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_ResumeClickHelper(0);
      }

      private void PhoneLine1End_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_EndClickHelper(0);
      }

      private void PhoneLine2Hold_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_HoldClickHelper(1);
      }

      private void PhoneLine2Resume_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_ResumeClickHelper(1);
      }

      private void PhoneLine2End_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_EndClickHelper(1);
      }

      private void PhoneLine3Hold_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_HoldClickHelper(2);
      }

      private void PhoneLine3Resume_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_ResumeClickHelper(2);
      }

      private void PhoneLine3End_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_EndClickHelper(2);
      }

      private void PhoneLine4Hold_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_HoldClickHelper(3);
      }

      private void PhoneLine4Resume_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_ResumeClickHelper(3);
      }

      private void PhoneLine4End_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_EndClickHelper(3);
      }

      private void PhoneLine5Hold_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_HoldClickHelper(4);
      }

      private void PhoneLine5Resume_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_ResumeClickHelper(4);
      }

      private void PhoneLine5End_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_EndClickHelper(4);
      }

      private void PhoneLine6Hold_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_HoldClickHelper(5);
      }

      private void PhoneLine6Resume_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_ResumeClickHelper(5);   
      }

      private void PhoneLine6End_Button_Click(object sender, EventArgs e)
      {
         PhoneLine_EndClickHelper(5);
      }

      #endregion
   }
}
