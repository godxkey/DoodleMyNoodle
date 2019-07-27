using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    SimTransform simTransform;

    public Text[] texts;

    private void Start()
    {
        simTransform = GetComponent<SimTransform>();
    }

    private void Update()
    {
        FixMatrix.b = simTransform.worldToLocalMatrix;
        FixMatrix.a = transform.worldToLocalMatrix;

        texts[0].text = ($"({FixMatrix.b.M11:F1}, {FixMatrix.b.M12:F1}, {FixMatrix.b.M13:F1}, {FixMatrix.b.M14:F1})");
        texts[1].text = ($"({FixMatrix.b.M21:F1}, {FixMatrix.b.M22:F1}, {FixMatrix.b.M23:F1}, {FixMatrix.b.M24:F1})");
        texts[2].text = ($"({FixMatrix.b.M31:F1}, {FixMatrix.b.M32:F1}, {FixMatrix.b.M33:F1}, {FixMatrix.b.M34:F1})");
        texts[3].text = ($"({FixMatrix.b.M41:F1}, {FixMatrix.b.M42:F1}, {FixMatrix.b.M43:F1}, {FixMatrix.b.M44:F1})");
        texts[4].text = (FixMatrix.a.GetRow(0).ToString());
        texts[5].text = (FixMatrix.a.GetRow(1).ToString());
        texts[6].text = (FixMatrix.a.GetRow(2).ToString());
        texts[7].text = (FixMatrix.a.GetRow(3).ToString());
    }
}