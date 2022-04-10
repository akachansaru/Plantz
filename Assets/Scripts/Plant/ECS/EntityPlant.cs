using System;
using System.Collections.Generic;
using Unity.Mathematics;

[Serializable]
public class EntityPlant
{
    public List<EntityStem> stems = new List<EntityStem>();
}

[Serializable]
public class EntityStem
{
    public string Name { get; set; }

    // For IDComponent
    public int PlantID { get; set; }
    public char TypeID { get; set; }

    public quaternion Rotation { get; set; }
    public float3 Position { get; set; }
    public float3 Scale { get; set; }

    // SaveIDComponent
    public int ParentID { get; set; } // ID of parent
    public int SaveID { get; set; } // ID of self
    public int RootID { get; set; } // ID of root
    public bool IsEmpty { get; set; }

    public int CurrBranchPatternStep { get; set; }
    public BranchPatterns BranchPattern { get; set; }
    public float3 BranchAngle { get; set; }

    public bool HasGrowingStemTag { get; set; }
    public bool HasGrowNormalTag { get; set; }
    public bool HasGrowBranchTag { get; set; }
    public bool HasGrowContinueTag { get; set; }

    public bool IsRoot { get; set; }
    public bool IsBaseStem { get; set; }

    // Only matters for root. Should probably make two classes here
    public StatECS MaxSize { get; set; }
    public bool IsMaxSize { get; set; }
    public int StemCount { get; set; }

    public StatECS StemGrowthRate { get; set; }
    public float StemGrowthRateCurrTime { get; set; }

    public StatECS WidenTime { get; set; }
    public float WidenCurrTime { get; set; }

    public float GrowTime { get; set; }
    public float GrowCurrTime { get; set; }
}
