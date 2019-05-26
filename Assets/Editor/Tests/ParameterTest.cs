using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class ParameterTest
{
    [Test]
    public void ArrayTransferMode()
    {
        int originValue = 0;
        int targetValue = 1;

        int[] ints = new int[] { originValue };
        ArrayTransferModeSub(ints, targetValue);
        Debug.Log("初始值：" + originValue + "，调用后值：" + ints[0] + "，" + (ints[0] == targetValue ? "数组是传引用的" : "数组是传值的"));
    }
     void ArrayTransferModeSub(int[] ints, int value)
    {
        for (int i = 0; i < ints.Length; i++)
            ints[i] = value;
    }
}
