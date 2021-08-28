## Keys
pull
 - `bool`, true for trigger pull, false for trigger release
vibro
 - `bool`, true for vibration feedback on, false for off
indic
 - `bool`, true for hit indicators on, false for off
timing
 - `float`, value for the sensitivity of the trigger

## Example commands
 - Open app with all parameters specified
`adb shell am start -n com.UniversityofWyoming.ThrowingDemo/com.unity3d.player.UnityPlayerActivity --ez pull true --ez vibro true --ez indic false --ef timing 20.3f`
 - Close app
`adb shell pm clear com.UniversityofWyoming.ThrowingDemo`

TODO
 - Accuracy and precision output from target
 - Put center of target at HMD height
 - Change to flat target
 - Out data to CSV (coordinates of collision, traits of throw)