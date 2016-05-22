using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class InputIndicator : MonoBehaviour
{
	public Image[] Renderers { get; private set; }
	#region Private Methods
	
	void Awake()
	{
		GameObject canvas = GameObject.Find("ArrowRegion");
		transform.SetParent(canvas.transform, false);
		transform.localPosition = Vector3.zero;

		Renderers = new Image[1];
		Renderers[0] = GetComponent<Image>();
		//Renderers[1] = transform.GetChild(0).GetComponent<Image>();
	}
	
	#endregion
}