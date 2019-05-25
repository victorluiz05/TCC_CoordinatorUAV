from __future__ import print_function
import time
import sys
from dronekit import connect, VehicleMode

con = (sys.argv[1])
ip = (sys.argv[2])
port = (sys.argv[3])
connection_string = con + ':'+ ip + ':' + port

# Connect to the Vehicle.
vehicle = connect(connection_string, wait_ready=False)

vehicle.mode = VehicleMode("GUIDED")

def arm_and_takeoff(aTargetAltitude):
    """
    Arms vehicle and fly to aTargetAltitude.
    """

    #print ("Basic pre-arm checks")
    # Don't try to arm until autopilot is ready
    while not vehicle.is_armable:
         #vehicle.armed   = True
         print (" Waiting for vehicle to initialise...")
         time.sleep(1)

    print ("Arming motors")
    # Copter should arm in GUIDED mode
    #vehicle.commands.clear()
    vehicle.armed = True
    time.sleep(1)
    #vehicle.mode = VehicleMode("GUIDED")
    
        
    # Confirm vehicle armed before attempting to take off
    while not vehicle.armed:
        print (" Waiting for arming...")
        time.sleep(1)

    print ("Taking off!")
    vehicle.simple_takeoff(aTargetAltitude) # Take off to target altitude

    while True:
        #Break and return from function just below target altitude.
        if vehicle.location.global_relative_frame.alt>=aTargetAltitude*0.50:
            print ("Reached target altitude")
            break
        vehicle.mode = VehicleMode("AUTO")
        time.sleep(1)
    

arm_and_takeoff(5)

# Get some vehicle attributes (state)
print ("Change mode: Starting AUTO mode...")
vehicle.mode = VehicleMode("AUTO")

# Close vehicle object before exiting script
vehicle.close()

print("Flight Started")
