namespace PhysicsEngine.Colliders;

public interface IPhysicsSceneObjectComponent
{
    SceneObject SceneObject {  get;  }

    Collision CheckCollision(Edge edge);
}