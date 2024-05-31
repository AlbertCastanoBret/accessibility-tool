using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Test
{
    private GameObject gameObject;
    
    [SetUp]
    void Setup()
    {
        gameObject = new GameObject();
        //gameObject.AddComponent(ACC_AccessibilityManager);
    }
    
    [Test]
    public void TestSimplePasses()
    {
        
    }
}
