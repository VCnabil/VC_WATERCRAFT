using Kvaser.CanLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VC_WATERCRAFT._GLobalz
{
    public class KvsrManager
    {
        private static readonly Lazy<KvsrManager> instance = new Lazy<KvsrManager>(() => new KvsrManager());
        private GCHandle callbackHandle; // Store the GCHandle here to keep the callback alive

        private int handle1 = -1337;
        private int handle2 = -1337;
        private bool _isOnBus1 = false;
        private bool _isOnBus2 = false;
        private int errorsCnt = 0;
        private string _errormessage = "";
        private bool dualChannel = false;

        public delegate void MessageReceivedHandler(string message);
        public event MessageReceivedHandler OnMessageReceived;
        private byte[] datareceivedFFFA;
        private int _channelsFound = 0;

        public static KvsrManager Instance => instance.Value;

        public int GetChannelsFound() => _channelsFound;

        public string GetErrorMessage() => _errormessage;

        public bool GetIsOnBus() => dualChannel ? (_isOnBus1 && _isOnBus2) : _isOnBus1;

        private KvsrManager()
        {
            handle1 = -1337;
            handle2 = -1337;
            _isOnBus1 = false;
            _isOnBus2 = false;
            errorsCnt = 0;
            _channelsFound = 0;
        }

        public void Init(bool useDualChannel)
        {
            dualChannel = useDualChannel;

            if (_isOnBus1 || (dualChannel && _isOnBus2))
            {
                errorsCnt++;
                _errormessage = "Already on bus " + errorsCnt.ToString();
                return;
            }

            Canlib.canInitializeLibrary();
            Canlib.canStatus statusConnected = Canlib.canGetNumberOfChannels(out int numberOfChannels);
            _channelsFound = numberOfChannels;

            if (statusConnected != 0)
            {
                errorsCnt++;
                _errormessage = "No Kvaser device connected" + errorsCnt.ToString();
                return;
            }

            if (numberOfChannels < 2)
            {
                errorsCnt++;
                _errormessage = "At least one physical CAN channel is required " + errorsCnt.ToString();
                if (numberOfChannels < 1)
                    return;
            }

            InitChannel(0, ref handle1, ref _isOnBus1);

            if (dualChannel)
            {
                if (numberOfChannels < 2)
                {
                    errorsCnt++;
                    _errormessage = "Dual channel setup requires at least 2 physical channels" + errorsCnt.ToString();
                    return;
                }
                InitChannel(1, ref handle2, ref _isOnBus2);
            }
        }
        private void InitChannel(int channelIndex, ref int handle, ref bool isOnBus)
        {
            handle = Canlib.canOpenChannel(channelIndex, Canlib.canOPEN_ACCEPT_VIRTUAL);

            if (handle < 0)
            {
                errorsCnt++;
                _errormessage = $"Failed to open channel {channelIndex} - Error count: {errorsCnt}";
                Debug.WriteLine($"[DEBUG] Failed to open channel {channelIndex} - Handle: {handle}");
                return;
            }

            Canlib.canStatus statusSetParams = Canlib.canSetBusParams(handle, Canlib.canBITRATE_250K, 0, 0, 0, 0);
            if (statusSetParams != Canlib.canStatus.canOK)
            {
                errorsCnt++;
                _errormessage = $"Failed to set bus parameters for channel {channelIndex} - Error count: {errorsCnt}";
                Debug.WriteLine($"[DEBUG] Failed to set bus parameters for channel {channelIndex} - Status: {statusSetParams}");
                return;
            }

            Canlib.canStatus statusBusOn = Canlib.canBusOn(handle);
            if (statusBusOn != Canlib.canStatus.canOK)
            {
                errorsCnt++;
                _errormessage = $"Failed to set bus on for channel {channelIndex} - Error count: {errorsCnt}";
                Debug.WriteLine($"[DEBUG] Failed to set bus on for channel {channelIndex} - Status: {statusBusOn}");
                return;
            }

            // Set up the callback function for asynchronous CAN reading
            Canlib.kvCallbackDelegate callback = new Canlib.kvCallbackDelegate(CanEventCallback);
            callbackHandle = GCHandle.Alloc(callback, GCHandleType.Normal); // Prevent garbage collection of the callback

            // Set notify flags to listen for RX events
            int notifyFlags = Canlib.canNOTIFY_RX;
            Canlib.canStatus result = Canlib.kvSetNotifyCallback(handle, callback, IntPtr.Zero, notifyFlags);

            if (result != Canlib.canStatus.canOK)
            {
                errorsCnt++;
                _errormessage = $"Failed to set notify for channel {channelIndex} - Error count: {errorsCnt}";
                Debug.WriteLine($"[DEBUG] Error setting notify for handle {handle} on channel {channelIndex}, Status: {result}");
                return;
            }

            Debug.WriteLine($"[DEBUG] Successfully registered CanEventCallback for channel {channelIndex} on handle {handle}");
            isOnBus = true;
        }
        public void ManualInvokeCallback(int handle, IntPtr context, int notifyEvent)
        {
            // Directly invoke the callback method for testing
            CanEventCallback(handle, context, notifyEvent);
        }

        public int GetTestHandle() => handle1; // Return the active handle; adjust as necessary

        private void CanEventCallback(int handle, IntPtr context, int notifyEvent)
        {
            Debug.WriteLine($"[DEBUG] CanEventCallback triggered with handle: {handle}, event: {notifyEvent}");

            if (notifyEvent == Canlib.canNOTIFY_RX)
            {
                HandleCanRxEvent(handle);
            }
            else
            {
                Debug.WriteLine($"[DEBUG] Unexpected notify event: {notifyEvent}");
            }
        }

        private void HandleCanRxEvent(int handle)
        {
            int id;
            byte[] msg = new byte[8];
            int dlc;
            int flags;
            long timestamp;

            Canlib.canStatus status = Canlib.canRead(handle, out id, msg, out dlc, out flags, out timestamp);

            if (status == Canlib.canStatus.canOK)
            {
                string hexId = id.ToString("X8");

                // string message = $"ID={id}, DLC={dlc}, Data={BitConverter.ToString(msg, 0, dlc)}, Timestamp={timestamp}";
                string message = $"ID={hexId}, DLC={dlc}, Data={BitConverter.ToString(msg, 0, dlc)}, Timestamp={timestamp}";

                Debug.WriteLine($"[DEBUG] Asynchronous CAN message received: {message}");
                OnMessageReceived?.Invoke(message);
            }
            else
            {
                errorsCnt++;
                _errormessage = $"Failed to read CAN message - Error count: {errorsCnt}";
                Debug.WriteLine($"[DEBUG] CAN Read Failed: {status}, Error Count: {errorsCnt}");
                LogErrorStatus(status);
            }
        }

        public void LogErrorStatus(Canlib.canStatus status)
        {
            Canlib.canGetErrorText(status, out string errorText);
            Debug.WriteLine($"[DEBUG] CAN Read Error: {errorText} (Status: {status})");
            MessageBox.Show($"CAN Read Error: {errorText}", "CAN Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        public void Cleanup()
        {
            if (callbackHandle.IsAllocated)
            {
                callbackHandle.Free();
            }
        }
        public void Close()
        {
            if (!_isOnBus1 && !_isOnBus2)
            {
                errorsCnt++;
                _errormessage = "Not on bus" + errorsCnt.ToString();
                return;
            }

            CloseChannel(ref handle1, ref _isOnBus1);
            if (dualChannel)
            {
                CloseChannel(ref handle2, ref _isOnBus2);
            }
        }

        private void CloseChannel(ref int handle, ref bool isOnBus)
        {
            if (!isOnBus)
            {
                errorsCnt++;
                _errormessage = "Channel not on bus" + errorsCnt.ToString();
                return;
            }

            Canlib.canStatus statusBusOff = Canlib.canBusOff(handle);
            if (statusBusOff != 0)
            {
                errorsCnt++;
                _errormessage = "Failed to bus off" + errorsCnt.ToString();
                return;
            }

            Canlib.canStatus statusClose = Canlib.canClose(handle);
            if (statusClose != 0)
            {
                errorsCnt++;
                _errormessage = "Failed to close" + errorsCnt.ToString();
                return;
            }

            isOnBus = false;
            Cleanup();
        }

        public int SendPGN_withStatus(int channel, int pgn, byte[] data)
        {
            bool isOnBus = channel == 1 ? _isOnBus1 : _isOnBus2;
            int handle = channel == 1 ? handle1 : handle2;

            if (!isOnBus)
            {
                errorsCnt++;
                _errormessage = $"Channel {channel} not on bus, can't write" + errorsCnt.ToString();
                return -1;
            }

            Canlib.canStatus statusWrite = Canlib.canWrite(handle, pgn, data, data.Length, Canlib.canMSG_EXT);
            if (statusWrite != 0)
            {
                errorsCnt++;
                _errormessage = $"Failed to write on channel {channel}" + errorsCnt.ToString();
                return -2;
            }

            return 0;
        }

        #region Untested
        private void CheckStatus(Canlib.canStatus status, string method)
        {
            if (status < 0)
            {
                string errorText;
                Canlib.canGetErrorText(status, out errorText);
                // Check if there's an open form
                if (Application.OpenForms.Count > 0)
                {
                    // Use the first open form to invoke the MessageBox
                    Application.OpenForms[0].Invoke(new MethodInvoker(delegate
                    {
                        MessageBox.Show($"{method} failed: {errorText}");
                    }));
                }
                else
                {
                    // Fallback if no form is open
                    MessageBox.Show($"{method} failed: {errorText}");
                }
            }
        }

        bool receivingMessages = true; // Flag to start/stop receiving messages
        public void ReceiveMessage(int channel, bool start)
        {
            int handle = channel == 1 ? handle1 : handle2;
            if (start)
            {
                byte[] data = new byte[8];
                int id;
                int dlc;
                int flags;
                long timestamp;
                Canlib.canStatus status;
                status = Canlib.canReadWait(handle, out id, data, out dlc, out flags, out timestamp, 100);
                if (status == Canlib.canStatus.canOK)
                {
                    string message = $"Received Message: ID={id}, DLC={dlc}, Data={BitConverter.ToString(data, 0, dlc)}, Timestamp={timestamp}";
                    OnMessageReceived?.Invoke(message);
                }
                else if (status != Canlib.canStatus.canERR_NOMSG)
                {
                    CheckStatus(status, "canReadWait");
                }
            }
        }

        public string ReceiveMessageV2(int channel, bool start)
        {
            string msg = ".";
            int handle = channel == 1 ? handle1 : handle2;
            Canlib.canStatus status;
            byte[] data = new byte[8];
            int id;
            int dlc;
            int flags;
            long timestamp;
            bool finished = false;
            Debug.WriteLine("Press the Spacebar to stop receiving messages.");
            status = Canlib.canReadWait(handle, out id, data, out dlc, out flags, out timestamp, 100);
            if (status == Canlib.canStatus.canOK)
            {
                msg = $"Received Message: ID={id}, DLC={dlc}, Data={BitConverter.ToString(data, 0, dlc)}, Timestamp={timestamp}";
            }
            else if (status != Canlib.canStatus.canERR_NOMSG)
            {
                CheckStatus(status, "canReadWait");
                msg = "canReadWait"; // Exit the loop if an error occurs
            }
            return msg;
        }

        public string ReceiveMessageV3(int channel, bool start)
        {
            string msg = ".";
            int handle = channel == 1 ? handle1 : handle2;
            Canlib.canStatus status;
            byte[] data = new byte[8];
            int id;
            int dlc;
            int flags;
            long timestamp;
            bool finished = false;
            Debug.WriteLine("Press the Spacebar to stop receiving messages.");
            status = Canlib.canReadWait(handle, out id, data, out dlc, out flags, out timestamp, 500);
            if (status == Canlib.canStatus.canOK)
            {
                msg = $"Received Message: ID={id}, DLC={dlc}, Data={BitConverter.ToString(data, 0, dlc)}, Timestamp={timestamp}";
            }
            else if (status != Canlib.canStatus.canERR_NOMSG)
            {
                CheckStatus(status, "canReadWait");
                msg = "canReadWait"; // Exit the loop if an error occurs
            }
            return msg;
        }
        #endregion

        #region temp

        #endregion
    }
}
