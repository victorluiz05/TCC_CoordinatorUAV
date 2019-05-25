from __future__ import print_function
import time
import sys
from dronekit import connect, VehicleMode

#parameters receveid from Coordinator
con = (sys.argv[1])
ip = (sys.argv[2])
port = (sys.argv[3])
connection_string = con + ':'+ ip + ':' + port

#Connect to the Vehicle
vehicle = connect(connection_string, wait_ready=True)


"""
vehicle.commands.clear()
vehicle.upload()
while not vehicle.is_armable:
        print (" Waiting for vehicle to initialise...")
        time.sleep(1)
"""     
vehicle.mode = VehicleMode("GUIDED")
vehicle.armed = True

while not vehicle.armed:
        print (" Waiting for arming...")
        time.sleep(1)

aTargetAltitude = 9
vehicle.simple_takeoff(aTargetAltitude)

while True:
        #Break and return from function just below target altitude.
        if vehicle.location.global_relative_frame.alt>=aTargetAltitude*0.70:
            print ("Reached target altitude")
            break
        time.sleep(1)

vehicle.mode = VehicleMode("AUTO")
print("Flight Started")

# Close vehicle object before exiting script
vehicle.close()
