from __future__ import print_function
import time
import sys
from dronekit import connect, VehicleMode, LocationGlobalRelative, Command, LocationGlobal
from pymavlink import mavutil

con = (sys.argv[1])
ip = (sys.argv[2])
port = (sys.argv[3])
connection_string = con + ':'+ ip + ':' + port

"""
# Set up option parsing to get connection string
import argparse
parser = argparse.ArgumentParser(description='Commands vehicle using vehicle.simple_goto.')
parser.add_argument('--connect',
                    help="Vehicle connection target string. If not specified, SITL automatically started and used.")
args = parser.parse_args()

connection_string = args.connect
"""

# Connect to the Vehicle.
print("Connecting to vehicle on: %s" % (connection_string,))
vehicle = connect(connection_string, wait_ready=False)

# Arm and Takeoff Function
def arm_and_takeoff(aTargetAltitude):
    #print ("Basic pre-arm checks")
    # Don't try to arm until autopilot is ready
    while not vehicle.is_armable:
        #print (" Waiting for vehicle to initialise...")
        time.sleep(1)

    #print ("Arming motors")
    # Copter should arm in GUIDED mode
    #print ("Change mode: Starting GUIDED mode...")
    vehicle.mode    = VehicleMode("GUIDED")
    vehicle.armed   = True

    # Confirm vehicle armed before attempting to take off
    while not vehicle.armed:
        #print (" Waiting for arming...")
        time.sleep(1)

    #print ("Taking off!")
    vehicle.simple_takeoff(aTargetAltitude) # Take off to target altitude

    # Wait until the vehicle reaches a safe height before processing the goto (otherwise the command
    #  after Vehicle.simple_takeoff will execute immediately).
    while True:
        #print (" Altitude: ", vehicle.location.global_relative_frame.alt)
        #Break and return from function just below target altitude.
        if vehicle.location.global_relative_frame.alt>=aTargetAltitude*0.95:
            #print ("Reached target altitude")
            break
        time.sleep(1)

# Clear Mission Function
def clear_mission(vehicle):
    cmds = vehicle.commands
    vehicle.commands.clear()
    vehicle.flush()

    # After clearing the mission you MUST re-download the mission from the vehicle
    # before vehicle.commands can be used again
    cmds = vehicle.commands
    cmds.download()
    cmds.wait_ready()

# Download Mission Function
def download_mission(vehicle):
    cmds = vehicle.commands
    cmds.download()
    cmds.wait_ready() # wait until download is complete.

# Function to get info about the current mission (wp list and number of wp)
def get_current_mission(vehicle):
    #print ("Downloading mission")
    download_mission(vehicle)
    missionList = []
    n_WP        = 0
    for wp in vehicle.commands:
        missionList.append(wp)
        n_WP += 1 
        
    return n_WP, missionList    
    
# Function that adds home as last waypoint, then when the mission is finished the UAV return to launch automatically
def add_last_waypoint_to_mission(vehicle, wp_Last_Latitude, wp_Last_Longitude, wp_Last_Altitude):  
    # Get the set of commands from the vehicle
    cmds = vehicle.commands
    cmds.download()
    cmds.wait_ready()

    # Save the vehicle commands to a list
    missionlist=[]
    for cmd in cmds:
        missionlist.append(cmd)

    # Modify the mission as needed. For example, here we change the
    wpLastObject = Command( 0, 0, 0, mavutil.mavlink.MAV_FRAME_GLOBAL_RELATIVE_ALT, mavutil.mavlink.MAV_CMD_NAV_WAYPOINT, 0, 0, 0, 0, 0, 0, 
                           wp_Last_Latitude, wp_Last_Longitude, wp_Last_Altitude)
    missionlist.append(wpLastObject)

    # Clear the current mission (command is sent when we call upload())
    cmds.clear()

    #Write the modified mission and flush to the vehicle
    for cmd in missionlist:
        cmds.add(cmd)
        cmds.upload()
    
    return (cmds.count)

def ChangeState(vehicle, mode):
    while vehicle.mode != VehicleMode(mode):
            vehicle.mode = VehicleMode(mode)
            time.sleep(1)
    return True

def DeliveryRoutine(vehicle):
    vehicle.mode = VehicleMode("LOITER")
    time.sleep(15)
   
gnd_speed = 5
state = 'UPDATINGMISSION'
# MAIN FUNCTION
while True:

    if state == 'UPDATINGMISSION':
       np_WP, missionList = get_current_mission(vehicle)
       time.sleep(2)
       if np_WP > 0:
           state = 'TAKEOFF'

    elif state == 'TAKEOFF':
         add_last_waypoint_to_mission(vehicle, vehicle.location.global_relative_frame.lat, vehicle.location.global_relative_frame.lon, vehicle.location.global_relative_frame.alt)
         arm_and_takeoff(20)
         vehicle.groundspeed = gnd_speed
         vehicle.mode = VehicleMode("AUTO")
         state = 'MISSION'

    elif state == 'MISSION':
       if vehicle.commands.next == vehicle.commands.count:  
          DeliveryRoutine(vehicle)
          clear_mission(vehicle)
          vehicle.mode = VehicleMode("RTL")
          state = 'END'

    elif state == 'END':
        if vehicle.location.global_relative_frame.alt < 1:
            print ("Vehicle has ended its mission!")
            break
            
vehicle.close()

