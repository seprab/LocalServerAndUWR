# Simple C# Local File Server & Unity UWR file downloader

Here are two projects for testing UWR for downloading files from a local file server.

## How you can test it?
1. Clone this.
2. Open the server project under "Pechito"
3. Open the Unity client project under "UWR"
4. From the Unity Client project, click on "Tools/Generate Files" and click on "Generate Files". This will create the test files in "ServerFiles".
From the server program edit the directoryPath variable so it points to "ServerFiles".
5. Build and run the server program
6. In the Unity client project, open the SampleScene, enter play mode, and Click on the buttons generated on the screen. This will print different information to the console (including response content encoding and timing).