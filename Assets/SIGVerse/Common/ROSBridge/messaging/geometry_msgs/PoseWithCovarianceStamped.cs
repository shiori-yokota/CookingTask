// Generated by gencs from geometry_msgs/PoseWithCovarianceStamped.msg
// DO NOT EDIT THIS FILE BY HAND!

using System;
using System.Collections;
using System.Collections.Generic;
using SIGVerse.ROSBridge;
using UnityEngine;

using SIGVerse.ROSBridge.std_msgs;
using SIGVerse.ROSBridge.geometry_msgs;

namespace SIGVerse.ROSBridge 
{
	namespace geometry_msgs 
	{
		[System.Serializable]
		public class PoseWithCovarianceStamped : ROSMessage
		{
			public std_msgs.Header header;
			public geometry_msgs.PoseWithCovariance pose;


			public PoseWithCovarianceStamped()
			{
				this.header = new std_msgs.Header();
				this.pose = new geometry_msgs.PoseWithCovariance();
			}

			public PoseWithCovarianceStamped(std_msgs.Header header, geometry_msgs.PoseWithCovariance pose)
			{
				this.header = header;
				this.pose = pose;
			}

			new public static string GetMessageType()
			{
				return "geometry_msgs/PoseWithCovarianceStamped";
			}

			new public static string GetMD5Hash()
			{
				return "953b798c0f514ff060a53a3498ce6246";
			}
		} // class PoseWithCovarianceStamped
	} // namespace geometry_msgs
} // namespace SIGVerse.ROSBridge

