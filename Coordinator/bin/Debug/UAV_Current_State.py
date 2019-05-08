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
vehicle = connect(connection_string, wait_ready = False, heartbeat_timeout=7)
vehicle.wait_ready(raise_exception=False)

#if vehicle.last_heartbeat > 0.3:
 #   vehicle.close() 
  #  sys.exit()
#else:
currentLocation = vehicle.location.global_relative_frame
currentGroundspeed = vehicle.groundspeed
currentHeading = vehicle.heading
battery = vehicle.battery.level
cmds = vehicle.commands
currentWP = cmds.next

print("%s, %s, %s, %s, %s, %s, %s " %(currentLocation.lat, currentLocation.lon, currentLocation.alt, currentGroundspeed, currentHeading, currentWP, battery))

# Close vehicle object before exiting script
vehicle.close() 
sys.exit()

   
