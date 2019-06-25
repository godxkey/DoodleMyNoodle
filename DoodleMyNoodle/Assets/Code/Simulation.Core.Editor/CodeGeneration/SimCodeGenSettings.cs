using System.Collections;
using System.Collections.Generic;

public static class SimCodeGenSettings
{
    public static Dictionary<string, string> targetAssemblies = new Dictionary<string, string>()
    {
        {"Simulation.Base", "Assets/Code/Simulation.Base.View" + ComponentView_GenerationFolder}
        , {"Simulation.Impl", "Assets/Code/Simulation.Impl.View" + ComponentView_GenerationFolder}
    };


    public const string MenuName_Generate             = "CodeGen/Simulation/Generate";
    public const string MenuName_Clear                = "CodeGen/Simulation/Clear";

    public const string ComponentView_GenerationFolder = "/Generated/SimComponentViews";
}
