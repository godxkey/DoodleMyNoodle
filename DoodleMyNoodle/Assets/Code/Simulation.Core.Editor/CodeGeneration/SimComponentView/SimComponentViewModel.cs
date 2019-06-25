using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public static partial class SimComponentViewCodeGenerator
{
    public static class Model
    {
        static StringBuilder stringBuilder = new StringBuilder();

        public static void Generate(StreamWriter writer, Type simType, bool clear)
        {
            FieldInfo[] fields = SimCodeGenUtility.GetSimComponentViewFieldsToGenerate(simType);

            string viewClassName = SimCodeGenUtility.GetSimComponentViewName(simType);
            string simClassName = simType.Name;
            string viewBaseClassName = SimCodeGenUtility.GetSimComponentViewName(simType.BaseType);


            // _________________________________________ Field Declarations _________________________________________ //
            foreach (FieldInfo field in fields)
            {
                stringBuilder.AppendLine($"    [SerializeField] {field.FieldType} {field.Name};");
            }
            string fieldDeclarations = stringBuilder.ToString();
            stringBuilder.Clear();


            // _________________________________________ Field Apply From Sim _________________________________________ //
            foreach (FieldInfo field in fields)
            {
                stringBuilder.AppendLine($"        {field.Name} = comp.{field.Name};");
            }
            string fieldApplyFromSim = stringBuilder.ToString(); 
            stringBuilder.Clear();


            // _________________________________________ Field Apply To Sim _________________________________________ //
            foreach (FieldInfo field in fields)
            {
                stringBuilder.AppendLine($"        comp.{field.Name} = {field.Name};");
            }
            string fieldApplyToSim = stringBuilder.ToString();
            stringBuilder.Clear();


            writer.Flush();
            if (clear)
            {
                writer.Write(
$@"// THIS CODE IS GENERATED
// DO NOT MODIFY IT
using UnityEngine;

public class {viewClassName} : MonoBehaviour
{{
}}
");
            }
            else
            {
                writer.Write(
$@"// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using UnityEngine;

public class {viewClassName} : {viewBaseClassName}
{{
{fieldDeclarations}
    public override Type simComponentType => typeof({simClassName});
    
    protected override SimComponent CreateComponentFromSerializedData()
    {{
        {simClassName} comp = new {simClassName}();

        ApplySerializedDataToSim(comp);

        return comp;
    }}

#if UNITY_EDITOR
    public override void ApplySimToSerializedData(SimComponent baseComp)
    {{
        base.ApplySimToSerializedData(baseComp);
        {simClassName} comp = ({simClassName})baseComp;
{fieldApplyFromSim}
    }}
#endif
    public override void ApplySerializedDataToSim(SimComponent baseComp)
    {{
        base.ApplySerializedDataToSim(baseComp);
        {simClassName} comp = ({simClassName})baseComp;
{fieldApplyToSim}
    }}
}}
");

            }
        }
    }
}
