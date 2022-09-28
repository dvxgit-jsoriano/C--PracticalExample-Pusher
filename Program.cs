using Newtonsoft.Json;
using PusherClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        public class Message
        {
            public string test_message { get; set; }
        }
        public static void Main(string[] args)
        {
            MainAsync().Wait();
            Console.ReadKey();
        }

        public static async Task MainAsync()
        {
            Pusher pusher = new Pusher("4ad1d0c8e978c0c9769f",
                new PusherOptions
                {
                    Cluster = "ap1",
                    Encrypted = true,
                });

            pusher.Connected += OnConnected;
            pusher.Disconnected += OnDisconnected;
            pusher.Error += ErrorHandler;
            pusher.Subscribed += SubscribedHandler;

            void OnConnected(object sender)
            {
                Console.WriteLine("Connected: " + ((Pusher)sender).SocketID);
            }

            void OnDisconnected(object sender)
            {
                Console.WriteLine("Disconnected: " + ((Pusher)sender).SocketID);
            }

            // Connect
            await pusher.ConnectAsync().ConfigureAwait(false);

            // Subscribe
            try
            {
                Channel channel = await pusher.SubscribeAsync("rtm-channel").ConfigureAwait(false);
                //Assert.AreEqual(true, channel.IsSubscribed);


            }
            catch (Exception)
            {
                // Handle error
            }

            // Disconnect
            //await pusher.DisconnectAsync().ConfigureAwait(false);
            // pusher.State will now be ConnectionState.Disconnected


            void ErrorHandler(object sender, PusherException error)
            {
                if ((int)error.PusherCode < 5000)
                {
                    // Error recevied from Pusher cluster, use PusherCode to filter.
                }
                else
                {
                    if (error is ChannelUnauthorizedException unauthorizedAccess)
                    {
                        // Private and Presence channel failed authorization with Forbidden (403)
                    }
                    else if (error is ChannelAuthorizationFailureException httpError)
                    {
                        // Authorization endpoint returned an HTTP error other than Forbidden (403)
                    }
                    else if (error is OperationTimeoutException timeoutError)
                    {
                        // A client operation has timed-out. Governed by PusherOptions.ClientTimeout
                    }
                    else if (error is ChannelDecryptionException decryptionError)
                    {
                        // Failed to decrypt the data for a private encrypted channel
                    }
                    else
                    {
                        // Handle other errors
                    }
                }
            }

            // Subscribed event handler
            void SubscribedHandler(object sender, Channel channel)
            {
                if (channel.Name == "rtm-channel")
                {
                    Console.WriteLine("channel rtm-channel!");

                    // Bind event listener to channel
                    channel.Bind("rtm-event", ChannelListener);
                }
            }

            // Channel event listener
            void ChannelListener(PusherEvent eventData)
            {
                //Message data = JsonConvert.DeserializeObject<Message>(eventData.Data);
                string data = eventData.Data;
                //Trace.TraceInformation($"Message from '{data.Name}': {data.Message}");
                Console.WriteLine(data);
            }

        }
    }
}
