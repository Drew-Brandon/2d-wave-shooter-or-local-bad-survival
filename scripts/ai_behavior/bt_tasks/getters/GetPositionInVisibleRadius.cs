namespace AIBehavior.BTTasks;
using Godot;
using CastInfo2D;
using System.Collections.Generic;

[Tool]
public partial class GetPositionInVisibleRadius : BTAction
{
    [Export]
    private int _rayCount = 16;

    [Export]
    private BBVariant _minRange = new();

    [Export]
    private BBVariant _maxRange = new();
    
    [Export]
    private StringName _targPosName = $"target_position";

    [Export]
    private RayCastInfo2D _rayCastInfo = null;

    private PolarCoords GetPolarFromRay(float angle, float radius, Node2D aiBody, PhysicsRayQueryParameters2D query)
    {
        Vector2 dir = Vector2.FromAngle(angle);
        Vector2 to = query.From + dir * radius;
        query.To = to;

        RayCastResults2D results = RayCastInfo2D.IntersectRay(aiBody.GetWorld2D().DirectSpaceState, query);
        PolarCoords newPCoords = new()
        {
            Angle = angle,
            Radius = query.From.DistanceTo(results.Position)
        };

        return newPCoords;
    }

    private Vector2 GetPositionFromPolar(bool isShort, float angleStep, float minRange, Vector2 origin, PolarCoords pCoords)
    {
        float angle = pCoords.Angle + RandUtils.RandRangef(-angleStep / 2f, angleStep / 2f);
        float radius = RandUtils.RandRangef(isShort ? 0f : minRange, pCoords.Radius);
        return origin + Vector2.FromAngle(angle) * radius;
    }

    public override string _GenerateName()
    {
        return $@"GetPositionInVisibleRadius(
                Ray Count: {_rayCount},
                Radius: [{BBUtils.GetBlackboardValueString(Blackboard, _minRange)}:{BBUtils.GetBlackboardValueString(Blackboard, _maxRange)}]
                ) -> {LimboUtility.DecorateOutputVar(_targPosName)}";
    }

    public override void _Enter()
    {
        bool hasPriority = false;
        float angleStep = 1f / _rayCount * Mathf.Tau;
        float minRange = BBUtils.GetBlackboardValue<float>(Blackboard, _minRange);
        float maxRange = BBUtils.GetBlackboardValue<float>(Blackboard, _maxRange);
        Node2D aiBody = (Node2D)Agent;
        List<PolarCoords> coords = new();
        Vector2 from = aiBody.GlobalPosition;
        PhysicsRayQueryParameters2D query = _rayCastInfo.CreateQuery(from, Vector2.Zero, [aiBody.GetCanvasItem()]);

        for (int i = 0; i < _rayCount; i++)
        {
            float curAngle = i * angleStep;
            PolarCoords newPCoords = GetPolarFromRay(curAngle, maxRange, aiBody, query);

            if (hasPriority)
            {
                if (newPCoords.Radius >= minRange)
                {
                    coords.Add(newPCoords);
                }
            }
            else
            {
                if (newPCoords.Radius >= minRange)
                {
                    hasPriority = true;
                    coords.Clear();
                }

                coords.Add(newPCoords);
            }
        }

        PolarCoords chosenPCoords = coords[GD.RandRange(0, coords.Count - 1)];
        Vector2 chosenPos = GetPositionFromPolar(!hasPriority, angleStep, minRange, from, chosenPCoords);
        Blackboard.SetVar(_targPosName, chosenPos);
    }

    public override Status _Tick(double delta)
    {
        return Status.Success;
    }
}
