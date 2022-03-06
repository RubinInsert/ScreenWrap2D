# ScreenWrap
Smooth wrapping of objects on a 2D Orthographic View. Use of 8 other cameras to overlay main camera. Script also handles introducing objects into the scene.
![Example](https://i.imgur.com/7MmyYON.gif)

## Setup
1. Add [ScreenWrapper.cs](ScreenWrapper.cs) to your main camera.
2. Create a new layer and enter it in the `Culling Mask` variable.
3. Add the [ScreenWrappingObject.cs](ScreenWrappingObject.cs) to any objects you want to be wrapped.

## Extra Features
### `OnTeleport` event
Called before an object is teleported (wrapped). The event outs a GameObject variable.
#### Example
```cs
void Awake() {
ScreenWrapper.OnTeleport += DoSomething;
}
void OnDestroy() {
ScreenWrapper.OnTeleport -= DoSomething;
}
void DoSomething(GameObject gO) {
	if(gO = this.gameObject) {
	print("My Last Position before teleporting:" + gO.transform.position);
	}
}
```
### Stationary Camera
If the camera is not expected to move, the bool can be checked to increase performance