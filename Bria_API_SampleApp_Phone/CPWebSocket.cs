using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bria_API_CSharp_SampleApp
{
    class CPWebSocket
   {
      // PUBLIC

      public CPWebSocket(string connectString)
      {
         connectionUri = new Uri(connectString);
         messageQueue = new Queue<string>();
      }

      public void Open()
      {
         Task.Run(async () => await OpenAsync());
      }

      public void Close()
      {
         if (ws != null)
         {
            ws.Dispose();
            ws = null;
         }
      }

      public void Send(string message)
      {
         messageQueue.Enqueue(message);
      }

      public event EventHandler Opened;
      public event EventHandler Closed;

      public class ErrorEventArgs : EventArgs
      {
         public string Text { get; private set; }
         public ErrorEventArgs(string text) { Text = text; }
      }
      public event EventHandler<ErrorEventArgs> Error;

      public class MessageReceivedEventArgs : EventArgs
      {
         public string Message { get; private set; }
         public MessageReceivedEventArgs(string message) { Message = message; }
      }
      public event EventHandler<MessageReceivedEventArgs> MessageReceived;


      // PRIVATE

      private Uri connectionUri;

      private ClientWebSocket ws = null;

      private Queue<string> messageQueue;

      private async Task OpenAsync()
      {
         if (ws == null)
         {
            ws = new ClientWebSocket();
         }

         try
         {
            await ws.ConnectAsync(connectionUri, CancellationToken.None);

            if (ws.State == WebSocketState.Open)
            {
               Opened?.Invoke(this, new EventArgs());

               do
               {
                  await Task.WhenAny(ReceiveAsync(), SendAsync());
                  if (ws.State != WebSocketState.Open)
                  {
                     return;
                  }
               } while (true);

            }
         }
         catch (Exception ex)
         {
            Error?.Invoke(this, new ErrorEventArgs(ex.Message));
         }
         finally
         {
            Closed?.Invoke(this, new EventArgs());

            ws.Dispose();
            ws = null;
         }
      }

      private async Task ReceiveAsync()
      {
         var buffer = new ArraySegment<byte>(new byte[4096]);
         do
         {
            WebSocketReceiveResult result;
            if (ws.State != WebSocketState.Open)
            {
               return;
            }

            using (var ms = new MemoryStream())
            {
               do
               {
                  result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                  ms.Write(buffer.Array, buffer.Offset, result.Count);
               } while (!result.EndOfMessage);

               if (result.MessageType == WebSocketMessageType.Close)
                  break;

               ms.Seek(0, SeekOrigin.Begin);
               using (var reader = new StreamReader(ms, Encoding.UTF8))
               {
                  var message = await reader.ReadToEndAsync();
                  MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
               }
            }

         } while (true);
      }

      private async Task SendAsync()
      {
         do
         {
            if (ws.State != WebSocketState.Open)
            {
               return;
            }

            if (messageQueue.Count > 0)
               {
                  await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageQueue.Dequeue())), WebSocketMessageType.Text, true, CancellationToken.None);
               }
         } while (true);
      }
   }
}
