using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArrowRegion : MonoBehaviour
{
	void Start ()
	{
		gameObject.GetComponent<Image>().color = Color.clear;
	}
}
