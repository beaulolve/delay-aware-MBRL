//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Mavros
{
    [Serializable]
    public class GlobalPositionTargetMsg : Message
    {
        public const string k_RosMessageName = "mavros_msgs/GlobalPositionTarget";
        public override string RosMessageName => k_RosMessageName;

        //  Message for SET_POSITION_TARGET_GLOBAL_INT
        // 
        //  https://mavlink.io/en/messages/common.html#SET_POSITION_TARGET_GLOBAL_INT
        //  Some complex system requires all feautures that mavlink
        //  message provide. See issue #402, #415.
        public Std.HeaderMsg header;
        public byte coordinate_frame;
        public const byte FRAME_GLOBAL_INT = 5;
        public const byte FRAME_GLOBAL_REL_ALT = 6;
        public const byte FRAME_GLOBAL_TERRAIN_ALT = 11;
        public ushort type_mask;
        public const ushort IGNORE_LATITUDE = 1; //  Position ignore flags
        public const ushort IGNORE_LONGITUDE = 2;
        public const ushort IGNORE_ALTITUDE = 4;
        public const ushort IGNORE_VX = 8; //  Velocity vector ignore flags
        public const ushort IGNORE_VY = 16;
        public const ushort IGNORE_VZ = 32;
        public const ushort IGNORE_AFX = 64; //  Acceleration/Force vector ignore flags
        public const ushort IGNORE_AFY = 128;
        public const ushort IGNORE_AFZ = 256;
        public const ushort FORCE = 512; //  Force in af vector flag
        public const ushort IGNORE_YAW = 1024;
        public const ushort IGNORE_YAW_RATE = 2048;
        public double latitude;
        public double longitude;
        public float altitude;
        //  in meters, AMSL or above terrain
        public Geometry.Vector3Msg velocity;
        public Geometry.Vector3Msg acceleration_or_force;
        public float yaw;
        public float yaw_rate;

        public GlobalPositionTargetMsg()
        {
            this.header = new Std.HeaderMsg();
            this.coordinate_frame = 0;
            this.type_mask = 0;
            this.latitude = 0.0;
            this.longitude = 0.0;
            this.altitude = 0.0f;
            this.velocity = new Geometry.Vector3Msg();
            this.acceleration_or_force = new Geometry.Vector3Msg();
            this.yaw = 0.0f;
            this.yaw_rate = 0.0f;
        }

        public GlobalPositionTargetMsg(Std.HeaderMsg header, byte coordinate_frame, ushort type_mask, double latitude, double longitude, float altitude, Geometry.Vector3Msg velocity, Geometry.Vector3Msg acceleration_or_force, float yaw, float yaw_rate)
        {
            this.header = header;
            this.coordinate_frame = coordinate_frame;
            this.type_mask = type_mask;
            this.latitude = latitude;
            this.longitude = longitude;
            this.altitude = altitude;
            this.velocity = velocity;
            this.acceleration_or_force = acceleration_or_force;
            this.yaw = yaw;
            this.yaw_rate = yaw_rate;
        }

        public static GlobalPositionTargetMsg Deserialize(MessageDeserializer deserializer) => new GlobalPositionTargetMsg(deserializer);

        private GlobalPositionTargetMsg(MessageDeserializer deserializer)
        {
            this.header = Std.HeaderMsg.Deserialize(deserializer);
            deserializer.Read(out this.coordinate_frame);
            deserializer.Read(out this.type_mask);
            deserializer.Read(out this.latitude);
            deserializer.Read(out this.longitude);
            deserializer.Read(out this.altitude);
            this.velocity = Geometry.Vector3Msg.Deserialize(deserializer);
            this.acceleration_or_force = Geometry.Vector3Msg.Deserialize(deserializer);
            deserializer.Read(out this.yaw);
            deserializer.Read(out this.yaw_rate);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.coordinate_frame);
            serializer.Write(this.type_mask);
            serializer.Write(this.latitude);
            serializer.Write(this.longitude);
            serializer.Write(this.altitude);
            serializer.Write(this.velocity);
            serializer.Write(this.acceleration_or_force);
            serializer.Write(this.yaw);
            serializer.Write(this.yaw_rate);
        }

        public override string ToString()
        {
            return "GlobalPositionTargetMsg: " +
            "\nheader: " + header.ToString() +
            "\ncoordinate_frame: " + coordinate_frame.ToString() +
            "\ntype_mask: " + type_mask.ToString() +
            "\nlatitude: " + latitude.ToString() +
            "\nlongitude: " + longitude.ToString() +
            "\naltitude: " + altitude.ToString() +
            "\nvelocity: " + velocity.ToString() +
            "\nacceleration_or_force: " + acceleration_or_force.ToString() +
            "\nyaw: " + yaw.ToString() +
            "\nyaw_rate: " + yaw_rate.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
