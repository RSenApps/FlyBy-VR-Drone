# FlyBy-VR-Drone
A virtual cockpit for the AR Drone using Oculus head tracking and Leap Motion

Built at Hacking Generation Y with Logan Taylor, Matthew Linker, and Andrew Liu

Uses a Node.js server with node-ar-drone to control the drone. This server then communicates with Unity3d game engine over sockets. Within the Unity3d game engine we use LeapMotionVR assets and a built-from-scratch 3d cockpit to create a immersive 3d experience. Using Oculus head tracking and accelerometer data from FlyBy-VR-Drone-Android, commands are relayed to the Node.js server.

