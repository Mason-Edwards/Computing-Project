import time
import board
import busio
import adafruit_adxl34x
from kafka import KafkaProducer
import json
import os

# Sensor setup
i2c = busio.I2C(board.SCL, board.SDA)
accel = adafruit_adxl34x.ADXL345(i2c)

# Kafka setup
# Use argument to pass IP and port?
producer = KafkaProducer(bootstrap_servers='192.168.0.65:9094')

while True:
    # Accelerometres measure acceleration, which is the change in velocity. This is measured in m/s^2.
    # A single G force is 9.8m/s^2, meaning if we divide the output of the sensor by this we can get the Gs.
    # Also due to the fact we know gravity is pulling straight down, we can get orientation from the sensor.
    # For example, if the sensor is flat gravity is pulling down the z axis of the sensor, so if the sensor is measuing
    # 9.8m/s^2 in the z axis we know that the senor is laying flat. If the sensor is measuring 9.8m/s^2 in the poistive 
    # x axis, we know the sensor is tilting forward etc.

    x = {"parameter": "Accelerometre X", "Unit": "something", "value": accel.acceleration[0]}
    y = {"parameter": "Accelerometre Y", "Unit": "something", "value": accel.acceleration[1]}
    z = {"parameter": "Accelerometre Z", "Unit": "something", "value": accel.acceleration[2]}
    a = {"x": round(accel.acceleration[0]/9.8,1), "y": round(accel.acceleration[1]/9.8,1), "z": round(accel.acceleration[2]/9.8, 1)}

    # Convert to json and then bytearray to send to kafka topic. In future should setup schema registry to enforce json schema (May need custom docker setup scripts to enable this).
    data = json.dumps(a)
    data = bytearray(data, "utf-8")

    producer.send('TelemetryData', data)
    print(f"Data sent: {data}")
    time.sleep(0.1)
    os.system("clear")
