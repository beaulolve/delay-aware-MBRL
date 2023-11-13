#!/usr/bin/env python

import rospy
import socket
import re
from sensor_msgs.msg import Image

from ros_tcp_endpoint import TcpServer, RosPublisher, RosSubscriber, RosService

def callback1(data):
    rospy.loginfo(rospy.get_caller_id() + "I heard image0:\n%s", data.data)

def callback2(data):
    pass

def callback3(data):
    pass

def main():
    rospy.init_node("image_test", anonymous=True)  # 初始化节点
    rospy.Subscriber("/image0_1", Image, callback1)
    rospy.Subscriber("/image1", Image, callback2)
    rospy.Subscriber("/image2", Image, callback3)
    rospy.spin()


if __name__ == "__main__":
    main()