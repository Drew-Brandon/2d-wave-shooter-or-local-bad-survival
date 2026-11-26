namespace AttackInfo;
using CastInfo2D;
using Godot;

/// <summary>
/// Represents a shooter that fires projectiles via hitscan.
/// </summary>
public partial class HitscanProjectileShooter2D : Node2D
{
    [Export(PropertyHint.Range, "radians_as_degrees")]
    private float _maxSpreadRad = Mathf.DegToRad(2f);

    [Export]
    private ProjectileInfo2D _projectileInfo = null;

    [Export]
    private RayCastInfo2D _rayCast = null;

    /// <summary>
    /// Emitted when a projectile is shot.
    /// </summary>
    /// <param name="target">
    /// The position at which the projectile landed.
    /// </param>
    [Signal]
    public delegate void ProjectileShotEventHandler(Vector2 target);

    /// <summary>
    /// Fires a projectile.
    /// </summary>
    public void Fire()
    {
        PreciseFire("");
    }

    private RayCastResults2D CastProjectile(PhysicsRayQueryParameters2D query, StringName targetPart)
    {
        RayCastResults2D results = RayCastInfo2D.IntersectRay(GetWorld2D().DirectSpaceState, query);
        
        if (results.Collider != null)
        {
            _projectileInfo.ApplyProjectileTo(this, (Node2D)results.Collider, results.Position, results.Normal, targetPart);
        }

        return results;
    }    

    private void FireProjectile(Vector2 from, Vector2 to, StringName targetPart)
    {
        PhysicsRayQueryParameters2D query = _rayCast.CreateQuery(from, to);
        RayCastResults2D castResults = CastProjectile(query, targetPart);
        
        // Pierce through the projectile's target if possible.
        for (int i = 0; i < _projectileInfo.PierceCount; i++)
        {
            CollisionObject2D collision = castResults.Collider as CollisionObject2D;
            
            /* 
             * Check if the projectile hit anything.
             * If it did, then check if it can pierce what it hit.
             */
            if (collision == null || (collision.CollisionLayer | _projectileInfo.PierceLayer) == 0)
            {
                break;
            }

            // Continue the projectile's course past the collision.
            query.From = castResults.Position;
            query.Exclude.Add(collision.GetRid());
            castResults = CastProjectile(query, targetPart);
        }
        
        EmitSignal(SignalName.ProjectileShot, castResults.Collider == null ? to : castResults.Position);
    }

    /// <summary>
    /// Fires a projectile that can target specific body parts on skeletal health sources.
    /// </summary>
    /// <param name="targetPart">
    /// The name of the body part to target.
    /// </param>
    public void PreciseFire(StringName targetPart)
    {
        for (int i = 0; i < _projectileInfo.SplitCount; i++)
        {
            // Get the ray cast's starting and end points.
            Vector2 from = GlobalPosition;
            float angleOffset = RandUtils.RandRangef(-_maxSpreadRad, _maxSpreadRad);
            Vector2 direction = GlobalTransform.X.Rotated(angleOffset);
            Vector2 to = from + direction * _projectileInfo.IdealRange;
            
            FireProjectile(from, to, targetPart);
        }
    }
}
