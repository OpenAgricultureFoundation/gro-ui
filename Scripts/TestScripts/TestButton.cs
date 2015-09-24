using UnityEngine;
using System.Collections;

public class TestButton : MonoBehaviour 
{

    public Camera mainCamera;

    public void printScreenResolution()
    {
        float h = Screen.height;
        float w = Screen.width;
        print ("Screen\nHeight: " + h.ToString() + "\tWidth: " + w.ToString());
    }

    public void printCameraResolution()
    {
        if(mainCamera == null){
            print ("Error: Set camera to evaluate");
            return;
        }
        float h = mainCamera.pixelHeight;
        float w = mainCamera.pixelWidth;
        print ("Camera\nHeight: " + h.ToString() + "\tWidth: " + w.ToString());

    }

    public void TestCoroutineFunctions(){
        StartCoroutine(func2());
    }

    IEnumerator func1(){
        print ("Started func1");
        int x = 5;
        while(x>0){
            print ("x=" + x);
            x--;
            yield return null;
        }
        print ("func1 finished");
    }

    IEnumerator func2(){
        print ("Started func2");
        yield return StartCoroutine(func1());
        print ("func2 finished");
    }
}
