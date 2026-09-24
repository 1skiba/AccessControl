from mfrc522 import MFRC522
from machine import SPI
import network
import config
import time

# WIFI
wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect(config.WIFI_SSID, config.WIFI_PASSWD)

def connect_to_wifi():
    timeout = 10
    while not wlan.isconnected() and timeout > 0:
        time.sleep(1)
        timeout -= 1
        print("Łączenie z WIFI...")
   
   
connect_to_wifi()
print("Połączono, IP:", wlan.ifconfig()[0])

spi = SPI(2, baudrate=2500000, polarity=0, phase=0)
# Using Hardware SPI pins:
#     sck=18   #
#     mosi=23  #
#     miso=19  #
#     rst=4    #
#     cs=5     #
# *************************
spi.init()
rdr = MFRC522(spi=spi, gpioRst=4, gpioCs=5)

print("Zbliz karte.")
while True:
    if wlan.isconnected() == False:
        print("error: no internet connection")
        connect_to_wifi()
        
    (stat, tag_type) = rdr.request(rdr.REQIDL)
    if stat == rdr.OK:
        (stat, raw_uid) = rdr.anticoll()
        if stat == rdr.OK:
            card_id = "%02x:%02x:%02x:%02x" % (raw_uid[0], raw_uid[1], raw_uid[2], raw_uid[3])
            print(card_id)
