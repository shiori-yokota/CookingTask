using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIGVerse.TurtleBot3
{
	public class TurtleBot3LinkInfo
	{
		public enum LinkType
		{
			Odom,
			BaseFootprint,
			BaseLink,
			CasterBackLeftLink,
			CasterBackRightLink,
			WheelLeftLink,
			WheelRightLink,
			CameraLink,
			CameraRgbFrame,
			CameraDepthFrame,
			CameraRgbOpticalFram,
			CameraDepthOpticalFrame,

			BaseScan,

			Link1,
			Link2,
			Link3,
			Link4,
			Link5,
			GripLink,
			GripLinkSub,
		}

		public LinkType linkType;
		public string   linkName;

		public TurtleBot3LinkInfo(LinkType linkType, string linkName)
		{
			this.linkType = linkType;
			this.linkName = linkName;
		}
	}
}

