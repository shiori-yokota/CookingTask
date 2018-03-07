using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentLoader : MonoBehaviour
{
	public GameObject environmentPrefab;
	private GameObject environment;

    private List<UpdatingTransformData> initialTransforms = new List<UpdatingTransformData>();
    private List<GameObject> targetObjects;
    private List<Rigidbody> targetRigidbodies = new List<Rigidbody>();

    void Awake()
    {
        this.setEnvironment();
    }

    // Use this for initialization
    void Start ()
	{
		
	}

	// Update is called once per frame
	void Update () {
		
	}

	private void setEnvironment()
	{
		this.environment = MonoBehaviour.Instantiate(this.environmentPrefab);
		this.environment.SetActive(true);
    }

	public void resetEnvironment()
	{
		Destroy(this.environment);
		this.setEnvironment();
	}

}
