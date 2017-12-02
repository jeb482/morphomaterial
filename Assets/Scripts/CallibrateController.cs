using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CallibrateController : MonoBehaviour {

    public Vector3 displacement;

	// Use this for initialization
	void Start () {
		
	}
	





	// Update is called once per frame
	void Update () {
		
	}



    /// <summary>
    /// Finds the homogenous 3d point x (with x_4 = 1) to minimize the least squares
    /// error of M_1x - M_2x = 1; Assumes that each M_i is affine4x4.
    /// </summary>
    /// <param name="matrices"></param>
    /// <returns></returns>
    public static Vector3 solveForOffsetVector(List<Matrix4x4> matrices)
    {
        var A = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<float>(matrices.Count * (matrices.Count), 3);
        var B = MathNet.Numerics.LinearAlgebra.CreateMatrix.Dense<float>(matrices.Count * (matrices.Count), 1);
        int row = 0;
        for (int i = 0; i < matrices.Count - 1; i++)
        {

            A.SetRow(row, new float[] {        matrices.ElementAt(i)[0,0] - matrices.ElementAt(i+1)[0,0],
                                               matrices.ElementAt(i)[0,1] - matrices.ElementAt(i+1)[0,1],
                                               matrices.ElementAt(i)[0,2] - matrices.ElementAt(i+1)[0,2]});

            A.SetRow(row + 1, new float[] {    matrices.ElementAt(i)[1,0] - matrices.ElementAt(i+1)[1,0],
                                               matrices.ElementAt(i)[1,1] - matrices.ElementAt(i+1)[1,1],
                                               matrices.ElementAt(i)[1,2] - matrices.ElementAt(i+1)[1,2]});

            A.SetRow(row + 2, new float[] {    matrices.ElementAt(i)[2,0] - matrices.ElementAt(i+1)[2,0],
                                               matrices.ElementAt(i)[2,1] - matrices.ElementAt(i+1)[2,1],
                                               matrices.ElementAt(i)[2,2] - matrices.ElementAt(i+1)[2,2]});

            B.SetRow(row,     new float[] { matrices.ElementAt(i + 1)[0,3] - matrices.ElementAt(i)[0,3]});
            B.SetRow(row + 1, new float[] { matrices.ElementAt(i + 1)[1,3] - matrices.ElementAt(i)[1,3]});
            B.SetRow(row + 2, new float[] { matrices.ElementAt(i + 1)[2,3] - matrices.ElementAt(i)[2,3]});
            row += 3;
        }
        var qr = A.QR();
        var matX = qr.R.Solve(qr.Q.Transpose() * B);
        return new Vector3(matX[0, 0], matX[1, 0], matX[2, 0]);
    }

}
