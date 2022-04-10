using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[RequireComponent(typeof(PlantFE))]
public class PickUpPlantECS : Interactable
{
    [SerializeField] private Vector3 distInFrontOfPlayer = new Vector3(0, 0.5f, 0.5f);
    [SerializeField] private float shrinkSize = 0.125f; // For picking up the plant. 1/8th the scale of the player

    private EntityManager entityManager;
    private Entity baseEntity;
    private float ShrinkSize { get { return shrinkSize; } set { ShrinkSize = value; } }

    private const float BUFFERTIME = 0.01f; // So that the plant doesn't get picked up immediately after being dropped

    private int oldBenchNum;
    private int oldSegmentNum;
    private float timeAtDrop = 0f;
    private bool IsPickedUp { get; set; } = false;

    public override void Start()
    {
        base.Start();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        baseEntity = GetComponent<GameObjectEntityLink>().BaseEntity;
    }

    public void Update()
    {
        if (IsPickedUp)
        {
            // Makes all Entities follow the pot
            entityManager.SetComponentData(baseEntity, new Translation
            {
                Value = new float3(transform.position.x, transform.position.y, transform.position.z)
            });
            // Need to rotate it as well. Or not. I kind of like the effect
        }
    }

    public override void DoInteraction()
    {
        if (Player.PlayerInstance.CarriedObject == null && Time.unscaledTime - timeAtDrop > BUFFERTIME)
        {
            GetOriginalValues();
            gameObject.layer = ConstantValues.Layers.Default;
            GetComponentInParent<BenchSegment>().RemovePlantFromBench();
            HoverInFrontOfPlayer();
            ShrinkPlant();
            Player.PlayerInstance.CarriedObject = gameObject;
            IsPickedUp = true;
        }
        else
        {
            Debug.Log("Already holding a plant.");
        }
    }

    private void GetOriginalValues()
    {
        if (GetComponentInParent<Workbench>())
        {
            oldBenchNum = -1; // Using this for the workbench
            oldSegmentNum = -1;
        }
        else
        {
            oldBenchNum = GetComponentInParent<Bench>().BenchNum;
            oldSegmentNum = GetComponentInParent<BenchSegment>().GetBenchSegmentNum();
        }
    }

    private void ShrinkPlant()
    {
        transform.localScale *= ShrinkSize;
        // This shrinks all children of baseEntity
        entityManager.SetComponentData(baseEntity, new NonUniformScale 
        { 
            Value = entityManager.GetComponentData<NonUniformScale>(baseEntity).Value * ShrinkSize 
        });
    }

    private void HoverInFrontOfPlayer()
    {
        //gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.layer = ConstantValues.Layers.Default; // So it won't be interacted with again until dropped
        gameObject.transform.parent = Player.PlayerInstance.transform;
        gameObject.transform.localPosition = distInFrontOfPlayer;
    }

    public void DropPlant(GameObject benchSegment)
    {
        benchSegment.GetComponent<BenchSegment>().SetPlantOnBench(gameObject);
        SetPlantValuesToNormal();

        UpdatePlantSaveLocation(benchSegment);
        Player.PlayerInstance.CarriedObject = null;
        timeAtDrop = Time.unscaledTime;

        IsPickedUp = false;
        entityManager.SetComponentData(baseEntity, new Translation
        {
            Value = new float3(transform.position.x, transform.position.y, transform.position.z)
        });
    }

    private void SetPlantValuesToNormal()
    {
        //GetComponent<Rigidbody>().isKinematic = false;
        gameObject.layer = ConstantValues.Layers.Interactable;
        transform.localRotation = Quaternion.identity; // FIXME: This rotates the plant the opposite way when placed sometimes
        transform.localScale /= ShrinkSize;
        entityManager.SetComponentData(baseEntity, new NonUniformScale
        {
            Value = entityManager.GetComponentData<NonUniformScale>(baseEntity).Value / ShrinkSize
        });
    }

    private void UpdatePlantSaveLocation(GameObject benchSegment)
    {
        int newBenchNum = benchSegment.transform.GetComponentInParent<GreenhouseBench>().BenchNum;
        int newSegmentNum = benchSegment.GetComponent<BenchSegment>().GetBenchSegmentNum();
        if (oldBenchNum == -1) // If it was picked up from the workbench
        {
            GlobalControl.Instance.savedValues.WorktablePlant = null;
        }
        else
        {
            GlobalControl.Instance.savedValues.GreenhousePlants[oldBenchNum][oldSegmentNum] = null;
        }
        GlobalControl.Instance.savedValues.GreenhousePlants[newBenchNum][newSegmentNum] = GetComponent<PlantFE>().Plant;
    }
}
