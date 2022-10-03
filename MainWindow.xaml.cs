using OpenTok;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace BasicVideoChat
{
    public partial class MainWindow : Window
    {
        public const string API_KEY = "46183452";
        public const string SESSION_ID = "2_MX40NjE4MzQ1Mn5-MTY2NDI4NzU2NzM2Nn42bGZYUXlXMjRwR1pmOFVTc0xERWpDK1V-fg"; 
        public const string TOKEN = "T1==cGFydG5lcl9pZD00NjE4MzQ1MiZzaWc9NTM5MTFhNWQwY2UzYzlmYjMzMmMwYTEwOTIzODkwMGJkNjJmMjg0YjpzZXNzaW9uX2lkPTJfTVg0ME5qRTRNelExTW41LU1UWTJOREk0TnpVMk56TTJObjQyYkdaWVVYbFhNalJ3UjFwbU9GVlRjMHhFUldwREsxVi1mZyZjcmVhdGVfdGltZT0xNjY0Mjg3NTY3Jm5vbmNlPTAuOTIzNTE1MTk0ODA0MTI2MiZyb2xlPW1vZGVyYXRvciZleHBpcmVfdGltZT0xNjY2ODc5NTY3JmluaXRpYWxfbGF5b3V0X2NsYXNzX2xpc3Q9";
       
        private readonly Context context;
        Session Session;
        Publisher Publisher;
        Subscriber subscriber;
        AudioDevice.Notifications AudioNotifications;
        List<AudioDevice.InputAudioDevice> inputDeviceNames = new List<AudioDevice.InputAudioDevice>();
        List<AudioDevice.OutputAudioDevice> outputDeviceNames = new List<AudioDevice.OutputAudioDevice>();
        public MainWindow()
        {
            InitializeComponent();
            context = new Context(new DedicatedWorkerDispatcher());
            // Uncomment following line to get debug logging
            //LogUtil.Instance.EnableLogging();
            spkBox.SelectionChanged += AudioOutputDeviceSelected;
            micBox.SelectionChanged += AudioInputDeviceSelected;

            AudioNotifications = new AudioDevice.Notifications(new DedicatedWorkerDispatcher());
            AudioNotifications.InputDeviceAdded += AudioInput_Device_Added;
            AudioNotifications.InputDeviceRemoved += AudioInput_Device_Removed;

            AudioNotifications.OutputDeviceAdded += AudioOutput_Device_Added;
            AudioNotifications.OutputDeviceRemoved += AudioOutput_Device_Removed;

            AudioNotifications.DefaultInputDeviceChanged += Audio_DefaultInput_Device_Changed;
            AudioNotifications.DefaultOutputDeviceChanged += Audio_DefaultOutput_Device_Changed;

            ConnectButton.Click += Connect_Button_Click;
            DisconnectButton.Click += Disconnect_Button_Click;

            Publisher = new Publisher.Builder(context)
            {
                Renderer = PublisherVideo
            }.Build();
            Populate_Device_List();
        }

        private void AudioInputDeviceSelected(object sender, SelectionChangedEventArgs e)
        {
            if (micBox.SelectedItem != null)
            {
                AudioDevice.SetInputAudioDevice(((AudioDevice.InputAudioDevice)micBox.SelectedItem));
            }
        }

        private void AudioOutputDeviceSelected(object sender, SelectionChangedEventArgs e)
        {
            if (spkBox.SelectedItem != null)
            {
                AudioDevice.SetOutputAudioDevice(((AudioDevice.OutputAudioDevice)spkBox.SelectedItem));
            }
        }
        private void AudioInput_Device_Added(object sender, AudioDevice.Notifications.InputAudioDeviceEventArgs e)
        {
            Console.WriteLine("Input Device added: " + e.Device.Id);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Populate_Device_List();
                MessageBoxResult yesNo = MessageBox.Show(this,string.Format("Input Device {0} added",e.Device.Name), "Input Device added",MessageBoxButton.YesNo);
                if (yesNo == MessageBoxResult.Yes)
                {
                    micBox.SelectedItem = e.Device;
                }
            }));
        }
        private void AudioInput_Device_Removed(object sender, AudioDevice.Notifications.InputAudioDeviceEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //MessageBox.Show(this, string.Format("device {0} removed", e.Device.Name), "input removed", MessageBoxButton.YesNo);
                Populate_Device_List();
            }));
        }
        private void AudioOutput_Device_Added(object sender, AudioDevice.Notifications.OutputAudioDeviceEventArgs e)
        {
            Console.WriteLine("Output Device added: " + e.Device.Id);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Populate_Device_List();
                MessageBoxResult yesNo = MessageBox.Show(this, string.Format("Output Device {0} added", e.Device.Name), "Output Device Added", MessageBoxButton.YesNo);
                if (yesNo == MessageBoxResult.Yes)
                {
                    spkBox.SelectedItem = e.Device;
                }
            }));
        }
        private void AudioOutput_Device_Removed(object sender, AudioDevice.Notifications.OutputAudioDeviceEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //MessageBox.Show(this, string.Format("device {0} removed", e.Device.Name), "output removed", MessageBoxButton.YesNo);
                Populate_Device_List();
            }));
        }
        private void Audio_DefaultInput_Device_Changed(object sender, AudioDevice.Notifications.InputAudioDeviceEventArgs e)
        {
            Console.WriteLine("Default audio input device changed: " + e.Device);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                micBox.SelectedItem = e.Device;
            }));
        }
        private void Audio_DefaultOutput_Device_Changed(object sender, AudioDevice.Notifications.OutputAudioDeviceEventArgs e)
        {
            Console.WriteLine("Default audio output device changed: " + e.Device);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                spkBox.SelectedItem = e.Device;
            }));
        }
        private void Populate_Device_List()
        {
            IList<OpenTok.AudioDevice.InputAudioDevice> inputs = AudioDevice.EnumerateInputAudioDevices();
            if (inputs == null || inputs.Count == 0)
            {
                Console.WriteLine("No audio inputs detected");
                micBox.ItemsSource = null;
            }
            else
            {
                AudioDevice.InputAudioDevice currentSelection = (AudioDevice.InputAudioDevice)micBox.SelectedItem;
                micBox.ItemsSource = inputs;
                if (currentSelection != null)
                {
                    micBox.SelectedItem = currentSelection;
                }
                if (micBox.SelectedItem == null)
                {
                    micBox.SelectedItem = AudioDevice.GetDefaultInputAudioDevice();
                }
                if (micBox.SelectedItem == null)
                {
                    micBox.SelectedIndex = 0;
                }
            }

            IList<OpenTok.AudioDevice.OutputAudioDevice> outputs = AudioDevice.EnumerateOutputAudioDevices();
            if (outputs == null || outputs.Count == 0)
            {
                Console.WriteLine("No audio outputs detected");
                spkBox.ItemsSource = null;
                return;
            }
            else
            {
                AudioDevice.OutputAudioDevice currentSelection = (AudioDevice.OutputAudioDevice)spkBox.SelectedItem;
                spkBox.ItemsSource = outputs;
                if (currentSelection != null)
                {
                    spkBox.SelectedItem = currentSelection;
                }
                if (spkBox.SelectedItem == null)
                {
                    spkBox.SelectedItem = AudioDevice.GetDefaultOutputAudioDevice();
                }
                if (spkBox.SelectedItem == null)
                {
                    spkBox.SelectedIndex = 0;
                }
            }
        }
        private void Connect_Button_Click(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusLbl.Content = "Connecting...";
            }));
            Session = new Session.Builder(context, API_KEY, SESSION_ID).Build();
            Session.Connected += Session_Connected;
            Session.Disconnected += Session_Disconnected;
            Session.Error += Session_Error;
            Session.StreamReceived += Session_StreamReceived;
            Session.StreamDropped += Session_StreamDropped;
            Session.Connect(TOKEN);

        }

        private void Disconnect_Button_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Unpublish(Publisher);
                Session.Disconnect();
            }
            catch (OpenTokException ex)
            {
                Console.WriteLine("OpenTokException " + ex.ToString());
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SubscriberVideo.Visibility = Visibility.Hidden;
                StatusLbl.Content = "Disconnected";
            }));
        }
        private void Session_Connected(object sender, System.EventArgs e)
        {
            Session.Publish(Publisher);
            
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusLbl.Content = "Connected";
            }));
        }
 
        private void Session_Disconnected(object sender, System.EventArgs e)
        {
            Trace.WriteLine("Session disconnected.");
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusLbl.Content = "Disconnected";
            }));
        }

        private void Session_Error(object sender, Session.ErrorEventArgs e)
        {
            Trace.WriteLine("Session error:" + e.ErrorCode);
        }

        private void Session_StreamReceived(object sender, Session.StreamEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            { 
                   SubscriberVideo.Visibility = Visibility.Visible;
            }));
            subscriber = new Subscriber.Builder(new Context(new WPFDispatcher()), e.Stream)
            {
                Renderer = SubscriberVideo
            }.Build();
            Session.Subscribe(subscriber);
        }

        private void Session_StreamDropped(object sender, Session.StreamEventArgs e)
        {
            try
            {
                Session.Unsubscribe(subscriber);
            }
            catch (OpenTokException ex)
            {
                Console.WriteLine("OpenTokException " + ex.ToString());
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SubscriberVideo.Visibility = Visibility.Hidden;
            }));
        }
    }
}
