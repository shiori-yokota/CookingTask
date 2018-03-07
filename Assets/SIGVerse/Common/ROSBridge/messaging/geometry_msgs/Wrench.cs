// Generated by gencs from geometry_msgs/Wrench.msg
// DO NOT EDIT THIS FILE BY HAND!

using System;
using System.Collections;
using System.Collections.Generic;
using SIGVerse.ROSBridge;
using UnityEngine;

using SIGVerse.ROSBridge.geometry_msgs;

namespace SIGVerse.ROSBridge 
{
	namespace geometry_msgs 
	{
		[System.Serializable]
		public class Wrench : ROSMessage
		{
			public UnityEngine.Vector3 force;
			public UnityEngine.Vector3 torque;


			public Wrench()
			{
				this.force = new UnityEngine.Vector3();
				this.torque = new UnityEngine.Vector3();
			}

			public Wrench(UnityEngine.Vector3 force, UnityEngine.Vector3 torque)
			{
				this.force = force;
				this.torque = torque;
			}

			new public static string GetMessageType()
			{
				return "geometry_msgs/Wrench";
			}

			new public static string GetMD5Hash()
			{
				return "4f539cf138b23283b520fd271b567936";
			}
		} // class Wrench
	} // namespace geometry_msgs
} // namespace SIGVerse.ROSBridge

