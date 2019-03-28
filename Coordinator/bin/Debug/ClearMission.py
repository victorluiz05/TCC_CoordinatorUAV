from __future__ import print_function
import sys
from dronekit import connect, Command
import time
from pymavlink import mavutil

con = (sys.argv[1])
ip = (sys.argv[2])
port = (sys.argv[3])
connection_string = con + ':'+ ip + ':' + port

# Connect to the Vehicle
#print('Connecting to vehicle on: %s' % connection_string)
vehicle = connect(connection_string, wait_ready=False)

# Check that vehicle is armable. 
# This ensures home_location is set (needed when saving WP file)

while not vehicle.is_armable:
    #print(" Waiting for vehicle to initialise...")
    time.sleep(1)

print("Clearing Mission")
#vehicle.commands.clear()
cmds = vehicle.commands
vehicle.commands.clear()
vehicle.flush()

# After clearing the mission you MUST re-download the mission from the vehicle
# before vehicle.commands can be used again
cmds = vehicle.commands
cmds.download()
cmds.wait_ready()

#print("Close vehicle object %s" % NN)
vehicle.close()
