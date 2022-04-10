public class ConstantValues
{
    public static class PlantConsts
    {
        public static Vector3Serializable LeafPrefabScale { get; } = new Vector3Serializable(1, 0.05f, 0.57f);
        // TODO can I look up the prefab here and get the scale directly?
        public static Vector3Serializable FlowerPrefabScale { get; } = new Vector3Serializable(0.5f, 0.5f, 0.5f);
        public static Vector3Serializable FruitPrefabScale { get; } = new Vector3Serializable(1f, 1f, 1f);

        public static float GrowthAmt { get; } = 0.1f; // The amount that leaves and flowers grow each time they grow
        public static float StemGrowthPercent { get; } = 0.1f;
    }

    public static class StatID
    {
        public static string LeafMaxSize { get; } = "LMS";
        public static string LeafGrowthRate { get; } = "LGR";
        public static string StemMaxSize { get; } = "SMS";
        public static string StemGrowthRate { get; } = "SGR";
        public static string StemInitialSize { get; } = "SIS";
        public static string TrunkRotationX { get; } = "TRX";
        public static string TrunkRotationY { get; } = "TRY";
        public static string TrunkRotationZ { get; } = "TRZ";


    }

    public static class Prefabs
    {
        private static readonly string prefabPath = "Prefabs";

        public static string Stem { get; } = prefabPath + "/Plant/Stem";
        public static string Leaf { get; } = prefabPath + "/Plant/Leaf";
        public static string Flower { get; } = prefabPath + "/Plant/Flower";
        public static string Fruit { get; } = prefabPath + "/Plant/Fruit";
        public static string Plant { get; } = prefabPath + "/Plant/Plant";
        public static string PlantECSBase { get; } = prefabPath + "/Plant/PlantECSBase";

        public static string PollenUI { get; } = prefabPath + "/UI/PollenUI";
        public static string SeedButton { get; } = prefabPath + "/UI/SeedButton";
        public static string PotUI { get; } = prefabPath + "/UI/PotUI";
        public static string SoilUI { get; } = prefabPath + "/UI/SoilUI";

    }

    public static class Materials
    {
        private static string materialPath = "Materials";

        public static string Grass { get; } = materialPath + "/Grass";
        public static string Sand { get; } = materialPath + "/Sand";
        public static string Swamp { get; } = materialPath + "/Swamp";

    }

    public static class SaveLists
    {
        public static string Greenhouse { get; } = "Greenhouse";
        public static string Forest { get; } = "Forest";
        public static string Desert { get; } = "Desert";
        public static string Swamp { get; } = "Swamp";
    }

    public static class Scenes
    {
        public static string Outdoors { get; } = "Outdoors";
        public static string Worktable { get; } = "Worktable";
        public static string Loading { get; } = "Loading";
    }

    public static class Tags
    {
        public static string Player { get; } = "Player";
        //public static string Barrier { get; } = "Barrier";
        public static string WindArea { get; } = "WindArea";
        public static string Boundary { get; } = "Boundary";
    }

    public static class Layers
    {
        public static int Default { get; } = 0;
        public static int Worktable { get; } = 8;
        public static int Barrier { get; } = 9;
        public static int Ground { get; } = 10;
        public static int Interactable { get; } = 11;
        public static int Invisible { get; } = 12;
        public static int Terrain { get; } = 13;
        public static int PlantComp { get; } = 14;
        public static int Player { get; } = 15;
    }

    public static class LayerMasks
    {
        public static int Default { get; } = 1 << 0;
        public static int Worktable { get; } = 1 << 8;
        public static int Barrier { get; } = 1 << 9;
        public static int Ground { get; } = 1 << 10;
        public static int Interactable { get; } = 1 << 11;
        public static int Invisible { get; } = 1 << 12;
        public static int Terrain { get; } = 1 << 13;
        public static int PlantComp { get; } = 1 << 14;
        public static int Player { get; } = 1 << 15;
    }
}