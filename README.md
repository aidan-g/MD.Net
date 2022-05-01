# MD.Net

A package containing binaries and a managed wrapper API for minidisc devices.
The binaries are from this project: https://github.com/gavinbenda/platinum-md

A simple example;

```c
//Read device and disc info.
var device = DeviceManager.Default.GetDevices().SingleOrDefault();
var currentDisc = DiscManager.Default.GetDisc(device);

//Create an updatable copy of the disc.
var updatedDisc = currentDisc.Clone();

//Set the title and erase existing tracks.
updatedDisc.Title = "MD.Net.Tests - " + Math.Abs(DateTime.Now.Ticks);
updatedDisc.Tracks.Clear();

//Add 3 new tracks (SP).
foreach (var fileName in new[] { "Track_001.wav", "Track_002.wav", "Track_003.wav" })
{
    var track = updatedDisc.Tracks.Add(fileName, Compression.None);
    track.Name = "MD.Net.Tests - " + updatedDisc.Tracks.Count;
}

//Compile the actions, these may be inspected before writing.
var actions = ActionBuilder.Default.GetActions(device, currentDisc, updatedDisc);
//Apply changes.
var result = DiscManager.Default.ApplyActions(device, actions, Status.None, true);
```

Input files must be WAVE (or ATRAC so long as it matches the specified compression), 44.1kHz, 16 bit, stereo. They may be converted depending on the Compression flag (SP, LP2, LP4).
Progress for various operations (Action, Transfer, Encode) are emitted to your IStatus implementation. 
