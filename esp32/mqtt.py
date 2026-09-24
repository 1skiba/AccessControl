# https://github.com/micropython/micropython-lib

from umqtt.simple import MQTTClient
import config

mqtt = MQTTClient(
    config.MQTT_CLIENT_ID,
    config.MQTT_SERVER
)

def connect_mqtt():
    try:
        mqtt.connect()
        
        print("connected to MQTT broker")
        print("pub: ", config.TOPIC_PUB)
        return True
    
    except Exception as e:
        print("error: ", e)
        return False
    
    
def publish_card(card_id):
    try:
        mqtt.publish(
            config.TOPIC_PUB,
            card_id.encode()
        )
        print("sent: ", card_id)
        return True
    
    except Exception as e:
        print("error: ", e)
        return False
    
    
def check_mqtt():
    try:
        mqtt.check_msg()
        return True
    
    except Exception as e:
        print("error: ", e)
        return False
    
    
    
    
    
    
    
    
    