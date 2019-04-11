# SimfeedbackXP11
A plugin for Simfeedback for the simulator X-Plane 11

In X-Plane----
* Go to Settings->Data Output
* Enable indexes 17 (Pitch, roll, & headings) and 135 (Motion platform stats) for the 'Network via UDP'
by ticking the box.
* Make sure to setup the desired IP adress, port and UDP-rate.

The XP11.config file-----  
Make sure this file exists in the provider directory along with the XP11.dll file and is in this format:  
SERVERIP=127.0.0.1  
PORT=49001  
PPS=20  

The above data is default if the file is not found, or it is in the wrong format.
You can freely change the data, but it must have the same values as set in X-Plane 11.
PPS in this case is the UDP-rate in X-Plane Data Output settings.

Current available datastreams, which is added in the XML file, from X-Plane is:  
<b>Pitch</b> (degrees)  
<b>Roll</b> (degrees)  
<b>PitchSpeed</b> (rad/s)  
<b>RollSpeed</b> (rad/s)  
<b>YawSpeed</b> (rad/s)  
<b>AccelerationX</b> (m/s^2) Left<>Right  
<b>AccelerationY</b> (m/s^2) Up<>Down  
<b>AccelerationZ</b> (m/s^2) Forward<>Backward  