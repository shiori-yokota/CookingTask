using UnityEngine;

using System.Collections;
using SIGVerse.ROSBridge.sensor_msgs;
using SIGVerse.ROSBridge.std_msgs;
using SIGVerse.Common;
using SIGVerse.SIGVerseROSBridge;


namespace SIGVerse.TurtleBot
{
	public class TurtleBotPubRGB : MonoBehaviour
	{
		public string rosbridgeIP;
		public int    sigverseBridgePort;

		public string topicNameCameraInfo = "/camera/rgb/camera_info";
		public string topicNameImage      = "/camera/rgb/image_raw";

		//--------------------------------------------------

		System.Net.Sockets.TcpClient tcpClient = null;
		private System.Net.Sockets.NetworkStream networkStream = null;

		SIGVerseROSBridgeMessage<CameraInfoForSIGVerseBridge> cameraInfoMsg = null;
		SIGVerseROSBridgeMessage<ImageForSIGVerseBridge> imageMsg = null;

		// Xtion
		private Camera xtionRGBCamera;
		private Texture2D imageTexture;

		// TimeStamp
		private Header header;

		private CameraInfoForSIGVerseBridge cameraInfoData;
		private ImageForSIGVerseBridge imageData;


		void Start()
		{
			if (this.rosbridgeIP.Equals(string.Empty))
			{
				this.rosbridgeIP = ConfigManager.Instance.configInfo.rosbridgeIP;
			}
			if (this.sigverseBridgePort==0)
			{
				this.sigverseBridgePort = ConfigManager.Instance.configInfo.sigverseBridgePort;
			}


			this.tcpClient = new System.Net.Sockets.TcpClient(this.rosbridgeIP, this.sigverseBridgePort);

			this.networkStream = this.tcpClient.GetStream();

			this.networkStream.ReadTimeout  = 100000;
			this.networkStream.WriteTimeout = 100000;


			// RGB Camera
			this.xtionRGBCamera = this.transform.Find("Xtion_rgb").GetComponent<Camera>();

			int imageWidth  = this.xtionRGBCamera.targetTexture.width;
			int imageHeight = this.xtionRGBCamera.targetTexture.height;

			this.imageTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);


			//  [camera/rgb/CameraInfo]
			string distortionModel = "plumb_bob";
			double[] D = { 0.0, 0.0, 0.0, 0.0, 0.0 };
			double[] K = { 570.3422241210938, 0.0, 319.5, 0.0, 570.3422241210938, 239.5, 0.0, 0.0, 1.0 };
			double[] R = { 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0 };
			double[] P = { 570.3422241210938, 0.0, 319.5, 0.0, 0.0, 570.3422241210938, 239.5, 0.0, 0.0, 0.0, 1.0, 0.0 };
			RegionOfInterest roi = new RegionOfInterest(0, 0, 0, 0, false);

			this.cameraInfoData = new CameraInfoForSIGVerseBridge(null, (uint)imageHeight, (uint)imageWidth, distortionModel, D, K, R, P, 0, 0, roi);
			
			//  [camera/rgb/Image_raw]
			string encoding = "rgb8";
			byte isBigendian = 0;
			uint step = (uint)imageWidth * 3;

			this.imageData = new ImageForSIGVerseBridge(null, (uint)imageHeight, (uint)imageWidth, encoding, isBigendian, step, null);

			this.header = new Header(0, new SIGVerse.ROSBridge.msg_helpers.Time(0, 0), "camera_rgb_optical_frame");


			this.cameraInfoMsg = new SIGVerseROSBridgeMessage<CameraInfoForSIGVerseBridge>("publish", this.topicNameCameraInfo, CameraInfoForSIGVerseBridge.GetMessageType(), this.cameraInfoData);
			this.imageMsg      = new SIGVerseROSBridgeMessage<ImageForSIGVerseBridge>     ("publish", this.topicNameImage,      ImageForSIGVerseBridge.GetMessageType(),      this.imageData);
		}

		void OnApplicationQuit()
		{
			if (this.networkStream != null) { this.networkStream.Close(); }
			if (this.tcpClient != null) { this.tcpClient.Close(); }
		}

		void Update()
		{
			base.StartCoroutine(GenerateDepthBuffer());
		}


		private bool canStartCoroutine = true;

		public IEnumerator GenerateDepthBuffer()
		{
			if (!this.canStartCoroutine)
			{
				yield break;
			}

			this.canStartCoroutine = false;


//			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
//			sw.Start();

			// Set a terget texture as a target of rendering
			RenderTexture.active = this.xtionRGBCamera.targetTexture;

//			yield return new WaitForEndOfFrame();

			// Apply rgb information to 2D texture
			this.imageTexture.ReadPixels(new Rect(0, 0, this.imageTexture.width, this.imageTexture.height), 0, 0);

			this.imageTexture.Apply();

			// Convert pixel values to depth buffer for ROS message
			byte[] rgbBytes = this.imageTexture.GetRawTextureData();


			this.header.Update();

			//  [camera/rgb/CameraInfo]
			this.cameraInfoData.header = this.header;
			this.cameraInfoMsg.msg = this.cameraInfoData;

			this.cameraInfoMsg.sendMsg(this.networkStream);

//			yield return null;

			//  [camera/rgb/Image_raw]
			this.imageData.header = this.header;
			this.imageData.data = rgbBytes;
			this.imageMsg.msg = this.imageData;

			this.imageMsg.sendMsg(this.networkStream);

//			sw.Stop();
//			UnityEngine.Debug.Log("time5=" + sw.Elapsed);

			this.canStartCoroutine = true;
		}
	}
}
