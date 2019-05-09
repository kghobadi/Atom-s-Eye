using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour {

    protected FirstPersonController fpc;
    //UI walking
    protected Texture2D originalCursor;
    public Texture2D[] cursors; 
    public float withinClickDist = 10;

    protected bool changedSprites;

    void Awake () {
        fpc = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        changedSprites = false;
    }

    protected virtual void OnMouseOver()
    {
        float dist = Vector3.Distance(transform.position, fpc.transform.position);
        if ( dist < withinClickDist && !changedSprites)
        {
            RandomIcon();
            changedSprites = true;
        }
        Debug.Log("on mouse over");
    }

    protected virtual void OnMouseDown()
    {
        if (changedSprites)
        {
            Interact();
        }
    }

    protected virtual void OnMouseExit()
    {
        changedSprites = false;
    }

    protected virtual void RandomIcon()
    {
        int randomCursor = Random.Range(0, cursors.Length);
        Vector2 cursorHotspot = new Vector2(cursors[randomCursor].width / 2, cursors[randomCursor].height / 2);
        Cursor.SetCursor(cursors[randomCursor], cursorHotspot, CursorMode.Auto);
    }

    public virtual void Interact()
    {
        //do whatever the hell you want
    }
}
