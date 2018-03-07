// Generated by gencs from sensor_msgs/MultiEchoLaserScan.msg
// DO NOT EDIT THIS FILE BY HAND!

using System;
using System.Collections;
using System.Collections.Generic;
using SIGVerse.ROSBridge;
using UnityEngine;

using SIGVerse.ROSBridge.std_msgs;
using SIGVerse.ROSBridge.sensor_msgs;

namespace SIGVerse.ROSBridge 
{
	namespace sensor_msgs 
	{
		[System.Serializable]
		public class MultiEchoLaserScan : ROSMessage
		{
			public std_msgs.Header header;
			public float angle_min;
			public float angle_max;
			public float angle_increment;
			public float time_increment;
			public float scan_time;
			public float range_min;
			public float range_max;
			public System.Collections.Generic.List<sensor_msgs.LaserEcho>  ranges;
			public System.Collections.Generic.List<sensor_msgs.LaserEcho>  intensities;


			public MultiEchoLaserScan()
			{
				this.header = new std_msgs.Header();
				this.angle_min = 0.0f;
				this.angle_max = 0.0f;
				this.angle_increment = 0.0f;
				this.time_increment = 0.0f;
				this.scan_time = 0.0f;
				this.range_min = 0.0f;
				this.range_max = 0.0f;
				this.ranges = new System.Collections.Generic.List<sensor_msgs.LaserEcho>();
				this.intensities = new System.Collections.Generic.List<sensor_msgs.LaserEcho>();
			}

			public MultiEchoLaserScan(std_msgs.Header header, float angle_min, float angle_max, float angle_increment, float time_increment, float scan_time, float range_min, float range_max, System.Collections.Generic.List<sensor_msgs.LaserEcho>  ranges, System.Collections.Generic.List<sensor_msgs.LaserEcho>  intensities)
			{
				this.header = header;
				this.angle_min = angle_min;
				this.angle_max = angle_max;
				this.angle_increment = angle_increment;
				this.time_increment = time_increment;
				this.scan_time = scan_time;
				this.range_min = range_min;
				this.range_max = range_max;
				this.ranges = ranges;
				this.intensities = intensities;
			}

			new public static string GetMessageType()
			{
				return "sensor_msgs/MultiEchoLaserScan";
			}

			new public static string GetMD5Hash()
			{
				return "6fefb0c6da89d7c8abe4b339f5c2f8fb";
			}
		} // class MultiEchoLaserScan
	} // namespace sensor_msgs
} // namespace SIGVerse.ROSBridge

