using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using EasyCsv;

public class EasyCsvTests {

	[Test]
	public void EasyCSVTestsSimplePasses() {
        Csv myCsv = new Csv();
        myCsv[0][0] = "" + 5;
        myCsv[5][6] = "Hi There!";
        Assert.AreEqual(myCsv[0][0], "5");
        for (int i = 0; i < 6; i++)
            for (int j = 0; j < 7; j++)
                if ((i != 0 && j != 0) && (i != 5 && j != 6))
                    Assert.AreEqual(myCsv[i][j], "");
        Assert.AreEqual(myCsv[5][6], "Hi There!");
    }

    [Test]
    public void WriteCsvTest()
    {
        Csv myCsv = new Csv();
        myCsv[0][0] = "" + 5;
        myCsv[5][6] = "Hi There!";
        myCsv[2][4] = "Hello, World!";
        myCsv.WriteToFile("D:/workspace/lame.csv");
    }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	//[UnityTest]
	//public IEnumerator EasyCSVTestsWithEnumeratorPasses() {
	//	// Use the Assert class to test conditions.
	//	// yield to skip a frame
	//	yield return null;
	//}
}
