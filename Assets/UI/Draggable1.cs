using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable1 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
	public Transform parentToReturnTo = null;
	public Transform placeholderParent = null;
	public bool dropInDropZone = false;

	GameObject placeholder = null;
	Draggable1 draggable = null;
	bool isDropZone = true;
	
	public void OnBeginDrag(PointerEventData eventData) {
		//Debug.Log ("OnBeginDrag");

		isDropZone = true;
		if (this.transform.parent.GetComponent<DropZone> () == null) { 
			draggable = Instantiate (this, this.transform.parent);
			draggable.transform.SetSiblingIndex (this.transform.GetSiblingIndex ());
			isDropZone = false;
		}

		if (isDropZone) {
			placeholder = new GameObject();
			placeholder.transform.SetParent( this.transform.parent );
			LayoutElement le = placeholder.AddComponent<LayoutElement>();
			le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
			le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
			le.flexibleWidth = 0;
			le.flexibleHeight = 0;

			placeholder.transform.SetSiblingIndex (this.transform.GetSiblingIndex ());
			parentToReturnTo = this.transform.parent;
			placeholderParent = parentToReturnTo;
		}
		dropInDropZone = false;
		this.transform.SetParent( this.transform.parent.parent );
		
		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	public void OnDrag(PointerEventData eventData) {
		//Debug.Log ("OnDrag");
		
		this.transform.position = eventData.position;

		if (placeholder != null) {
			if(placeholder.transform.parent != placeholderParent)
				placeholder.transform.SetParent(placeholderParent);

			int newSiblingIndex = placeholderParent.childCount;

			for(int i=0; i < placeholderParent.childCount; i++) {
				if(this.transform.position.y > placeholderParent.GetChild(i).position.y) {

					newSiblingIndex = i;
					if(placeholder.transform.GetSiblingIndex() < newSiblingIndex)
						newSiblingIndex--;

					break;
				}
			}

			placeholder.transform.SetSiblingIndex(newSiblingIndex);
			//Debug.Log ("Placeholder index " + placeholder.transform.GetSiblingIndex ());
		}

	}
	
	public void OnEndDrag(PointerEventData eventData) {
		//Debug.Log ("OnEndDrag");
		if (placeholder != null) {
			if (dropInDropZone) {
				this.transform.SetParent (parentToReturnTo);
				this.transform.SetSiblingIndex (placeholder.transform.GetSiblingIndex ());
				GetComponent<CanvasGroup> ().blocksRaycasts = true;

				Destroy (placeholder);
			} else {
				Destroy (placeholder);
				Destroy (this.gameObject);
			}
		} else {
			Destroy (this.gameObject);
		}
	}

	public void createPlaceholder() {

		if (placeholder == null && placeholderParent != null) {
			placeholder = new GameObject ();
			placeholder.transform.SetParent (placeholderParent);
			LayoutElement le = placeholder.AddComponent<LayoutElement> ();
			le.preferredWidth = this.GetComponent<LayoutElement> ().preferredWidth;
			le.preferredHeight = this.GetComponent<LayoutElement> ().preferredHeight;
			le.flexibleWidth = 0;
			le.flexibleHeight = 0;
		}
	}
	
	
}
