# ParallaxMitigation
An OTD plugin to reduce parallax  currently WIP

requires tilt

Mitigation amount is essentially just the distance from your pen tip to the sensor inside your pen

angle clamping sets the maximum angle of effect

Accuracy defines how close you want to be to no parallax, recommended is 0.3, it will go past 0 if this value is too low, this cannot be set to 0

Tightening amount is the amount the Mitigation amount will increase per update, recommended is 0.05

calibration: when you check the box, click into the console tab, change it to debug,hold your pen on the surface of 
the tablet, and slowly rock the pen left and right, keeping the tip in the same place.
  When you think that the cursor movement is close enough to zero, look at the last value in the console, take your pen off of the tablet, 
put the number into "Mitigation amount"
uncheck calibration, click save, and apply.
