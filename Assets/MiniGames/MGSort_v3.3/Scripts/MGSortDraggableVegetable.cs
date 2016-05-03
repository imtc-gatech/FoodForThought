using UnityEngine;
using System.Collections;

public class MGSortDraggableVegetable : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
	

    void OnMouseDrag()
    {
        GetComponent<Rigidbody>().isKinematic = true; //allow collision with the other (currently) non-kinematic vegetables.
        Vector3 point = transform.parent.GetComponent<MGSortSortingGame>().MinigameHolder.GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition); //start from the vegetable's original location
        point.z = 8; //keep z constant
        gameObject.transform.position = point; //sets the new position of the vegetable with the drag of the mouse.
        Cursor.visible = false; //hide the mouse while dragging
        GetComponent<Rigidbody>().isKinematic = false; //reset the vegetable to non-kinematic when dragging is finished.
    }

    void OnMouseUp()
    {
        Cursor.visible = true; //show the cursor
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0); //stop rotation of the vegetable, because the player set the vegetable down on the table.
        GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 0) - GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange); //opposing force to stop velocity because the player set the vegetable down.
    }

}

