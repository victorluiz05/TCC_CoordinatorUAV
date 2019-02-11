from __future__ import print_function
from dronekit import connect, VehicleMode
import time
import sys

# Communication Arguments received from Coordinator
con = (sys.argv[1])
ip = (sys.argv[2])
port = (sys.argv[3])
connection_string = con + ':'+ ip + ':' + port

# Connect to the Vehicle.
vehicle = connect(connection_string, wait_ready=False)

currentLocation = vehicle.location.global_relative_frame
currentGroundspeed = vehicle.groundspeed
currentHeading = vehicle.heading
cmds = vehicle.commands
currentWP = cmds.next

#CurrentState = currentLocation.lat + ":" + currentLocation.lon + " " + currentLocation.alt + " " + currentGroundspeed 
print("%s, %s, %s, %s, %s, %s " %(currentLocation.lat, currentLocation.lon, currentLocation.alt, currentGroundspeed, currentHeading, currentWP))

# Close vehicle object before exiting script
vehicle.close()
