# Video-Device-Control-Windows
Detect audio device changes and select the right device for your video call.

# Setup

1. In Project settings -> Build, make sure "Prefer 32-bit" is unchecked.
2. Get an APIKEY, SessionId, Token and insert in MainWindow.xml.cs file
3. Build and Run

# Notes

1. When the application starts, it will display list of detected devices in the combo box.
2. A publisher is also created but doesn't publish the video to the session yet
3. You can see the status of the session on the Screen.
4. Click on "Connect" button to connect to the session.
5. Add a new audio input / output device - you should see a prompt asking you if you want to use the newly added device.
6. Remove a device - you should see the device drop down menu automatically refresh
