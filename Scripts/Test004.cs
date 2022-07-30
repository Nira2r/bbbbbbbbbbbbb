using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test004 : MonoBehaviour
{
    Test002 EditorCore;

    public int uniqueId { get; set; }

    public void OnClicked()
    {
        if (EditorCore.editorMode == Test002.EditorMode.Select)
        {
            EditorCore.SelectNote(uniqueId);
        }
        if(Input.GetMouseButtonUp(1))
        {
            EditorCore.EraseNote(uniqueId);
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        EditorCore = GameObject.Find("EditorCore").GetComponent<Test002>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
