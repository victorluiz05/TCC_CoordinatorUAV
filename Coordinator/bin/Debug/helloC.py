print ("Start simulator (SITL)")
import dronekit_sitl
import sys
sitl = dronekit_sitl.start_default()
#connection_string = sitl.connection_string()

con = (sys.argv[1])
ip = (sys.argv[2])
port = (sys.argv[3])
connection_string = con + ':'+ ip + ':' + port
#connection_string = 'tcp:127.0.0.1:5760'

# Import DroneKit-Python
from dronekit import connect, VehicleMode

# Connect to the Vehicle.
print("Connecting to vehicle on: %s" % (connection_string,))
vehicle = connect(connection_string, wait_ready=True)

# Get some vehicle attributes (state)
print ("Get some vehicle attribute values:")
print (" GPS: %s" % vehicle.gps_0)
print (" Battery: %s" % vehicle.battery)
print (" Last Heartbeat: %s" % vehicle.last_heartbeat)
print (" Is Armable?: %s" % vehicle.is_armable)
print (" System status: %s" % vehicle.system_status.state)
print (" Mode: %s" % vehicle.mode.name)    # settable

# Close vehicle object before exiting script
vehicle.close()

# Shut down simulator
sitl.stop()
print("Completed")
