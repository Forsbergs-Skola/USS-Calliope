using UnityEngine;
using System.Collections.Generic;

public class Scratch : MonoBehaviour
{
   private enum EnumFruit
    {
        APPLE,
        ORANGE,
        BANANA
    }

    private Dictionary<EnumFruit, int> fruitDict = new Dictionary<EnumFruit, int>();


    private void Start()
    {
        InitializeFruitDict();

        
    }

    private void InitializeFruitDict()
    {
        foreach(EnumFruit fruit in System.Enum.GetValues(typeof(EnumFruit)))
        {
            fruitDict[fruit] = 0;
        }

        Debug.Log(fruitDict[EnumFruit.APPLE]);
        Debug.Log(fruitDict[EnumFruit.ORANGE]);
        Debug.Log(fruitDict[EnumFruit.BANANA]);

    }
}
