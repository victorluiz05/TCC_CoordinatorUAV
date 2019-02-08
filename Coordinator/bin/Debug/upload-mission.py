from __future__ import print_function
import sys
from dronekit import connect, Command
import time

con = (sys.argv[1])
ip = (sys.argv[2])
port = (sys.argv[3])
connection_string = con + ':'+ ip + ':' + port

# Connect to the Vehicle
print('Connecting to vehicle on: %s' % connection_string)
vehicle = connect(connection_string, wait_ready=False)

# Check that vehicle is armable. 
# This ensures home_location is set (needed when saving WP file)

while not vehicle.is_armable:
    print(" Waiting for vehicle to initialise...")
    time.sleep(1)


def readmission(aFileName):
    """
    Load a mission from a file into a list. The mission definition is in the Waypoint file
    format (http://qgroundcontrol.org/mavlink/waypoint_protocol#waypoint_file_format).

    This function is used by upload_mission().
    """
    print("\nReading mission from file: %s" % aFileName)
    cmds = vehicle.commands
    missionlist=[]
    with open(aFileName) as f:
        for i, line in enumerate(f):
            if i==0:
                if not line.startswith('QGC WPL 110'):
                    raise Exception('File is not supported WP version')
            else:
                linearray=line.split('\t')
                ln_index=int(linearray[0])
                ln_currentwp=int(linearray[1])
                ln_frame=int(linearray[2])
                ln_command=int(linearray[3])
                ln_param1=float(linearray[4])
                ln_param2=float(linearray[5])
                ln_param3=float(linearray[6])
                ln_param4=float(linearray[7])
                ln_param5=float(linearray[8])
                ln_param6=float(linearray[9])
                ln_param7=float(linearray[10])
                ln_autocontinue=int(linearray[11].strip())
                cmd = Command( 0, 0, 0, ln_frame, ln_command, ln_currentwp, ln_autocontinue, ln_param1, ln_param2, ln_param3, ln_param4, ln_param5, ln_param6, ln_param7)
                missionlist.append(cmd)
    return missionlist


def upload_mission(aFileName):
    """
    Upload a mission from a file. 
    """
    #Read mission from file
    missionlist = readmission(aFileName)
    
    print("\nUpload mission from a file: %s" % import_mission_filename)
    #Clear existing mission from vehicle
    print(' Clear mission')
    cmds = vehicle.commands
    cmds.clear()
    #Add new mission to vehicle
    for command in missionlist:
        cmds.add(command)
    print(' Upload mission')
    vehicle.commands.upload()

def printfile(aFileName):
    
   # Print a mission file to demonstrate "round trip"
    
    print("\nMission file: %s" % aFileName)
    with open(aFileName) as f:
        for line in f:
            print(' %s' % line.strip())        

#export_mission_filename = 'exportedmission.txt'

path = (sys.argv[4])
import_mission_filename = path
#import_mission_filename = 'Missions\M1.txt'   #VICTOR - Here is where the path os the mission file is written    



#Upload mission from file
upload_mission(import_mission_filename)

#Download mission we just uploaded and save to a file
#save_mission(export_mission_filename)

#Close vehicle object before exiting script
print("Close vehicle object")
vehicle.close()

# Shut down simulator if it was started.
if sitl is not None:
    sitl.stop()
