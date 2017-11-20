using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public void OnPointerEnter(PointerEventData eventData) {
		//Debug.Log("OnPointerEnter");
		if(eventData.pointerDrag == null)
			return;

		Draggable1 d = eventData.pointerDrag.GetComponent<Draggable1>();
		if(d != null) {
			d.placeholderParent = this.transform;
			d.parentToReturnTo = this.transform;
			d.createPlaceholder ();
		}
	}
	
	public void OnPointerExit(PointerEventData eventData) {
		//Debug.Log("OnPointerExit");
		if(eventData.pointerDrag == null)
			return;

		Draggable1 d = eventData.pointerDrag.GetComponent<Draggable1>();
		if(d != null && d.placeholderParent==this.transform) {
			d.placeholderParent = d.parentToReturnTo;
		}
	}
	
	public void OnDrop(PointerEventData eventData) {
		//Debug.Log (eventData.pointerDrag.name + " was dropped on " + gameObject.name);

		Draggable1 d = eventData.pointerDrag.GetComponent<Draggable1>();
		if(d != null) {
			d.parentToReturnTo = this.transform;
			d.dropInDropZone = true;
		}

	}
}
