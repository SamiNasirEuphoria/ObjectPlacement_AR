using UnityEngine;
using System.Collections;

public class ChangeMyColor : MonoBehaviour
{
    public Material[] myColor;
    public float delay;
    private MeshRenderer _myrenderer;
    private void Start()
    {
        _myrenderer = GetComponent<MeshRenderer>();
        StartCoroutine(Wait());
    }
    public void ChangeColor()
    {
        var num = Random.RandomRange(0, myColor.Length);
        _myrenderer.material = myColor[num];
    }
    IEnumerator Wait()
    {
        while (true)
        {
            WaitForSeconds _wait = new WaitForSeconds(delay);
            yield return _wait;
            ChangeColor();
        }
    }
}
