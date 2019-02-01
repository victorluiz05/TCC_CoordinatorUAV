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

# Get some vehicle attributes (state)
print ("Returning to Home (RTL)")
vehicle.mode = VehicleMode("RTL")

# Close vehicle object before exiting script
vehicle.close()
