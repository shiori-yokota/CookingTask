using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdatingTransformData {
    public Transform UpdatingTransform { get; set; }

    public Vector3 LocalPosition { get; set; }
    public Vector3 LocalRotation { get; set; }
    public Vector3? LocalScale { get; set; }

    public void UpdateTransform()
    {
        this.UpdatingTransform.localPosition = this.LocalPosition;
        this.UpdatingTransform.localEulerAngles = this.LocalRotation;

        if (this.LocalScale != null)
        {
            this.UpdatingTransform.localScale = this.LocalScale.Value;
        }
    }
}

public class PlaybackerCommon : MonoBehaviour {

    public const int TypeDefMotion = 10;
    public const int TypeDefObject = 20;
    public const int TypeValMotion = 11;
    public const int TypeValObject = 21;

    public List<GameObject> targetObjects;

    private List<UpdatingTransformData> initialTransforms = new List<UpdatingTransformData>();

    private List<Rigidbody> targetRigidbodies = new List<Rigidbody>();

    private void Awake()
    {
        GameObject tools = GameObject.Find("CookingTools");
        targetObjects.Add(tools);

        foreach (GameObject targetObj in targetObjects)
        {
            Transform[] transforms = targetObj.GetComponentsInChildren<Transform>();

            foreach (Transform transform in transforms)
            {
                UpdatingTransformData initialTransform = new UpdatingTransformData();
                initialTransform.UpdatingTransform = transform;

                initialTransform.LocalPosition = transform.localPosition;
                initialTransform.LocalRotation = transform.localEulerAngles;
                initialTransform.LocalScale = transform.localScale;

                this.initialTransforms.Add(initialTransform);
            }

            Rigidbody[] rigidbodies = targetObj.transform.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rigidbody in rigidbodies)
            {
                this.targetRigidbodies.Add(rigidbody);
            }
        }
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static string GetLinkPath (Transform transform)
    {
        string path = transform.name;

        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }

    public List<GameObject> GetTargetObjects()
    {
        return targetObjects;
    }

    public void ResetObjects()
    {
        this.RestoreInitialTransformOfTargets();
    }

    private void RestoreInitialTransformOfTargets()
    {
        foreach (UpdatingTransformData initialTransform in this.initialTransforms)
        {
            initialTransform.UpdateTransform();
        }

        foreach (Rigidbody rigidbody in this.targetRigidbodies)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
