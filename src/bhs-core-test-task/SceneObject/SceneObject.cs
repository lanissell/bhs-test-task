using System.Numerics;

public abstract class SceneObject
{
    public Vector2 Position { get; private set; }

    public float RotationDegrees { get; private set; }

    public Edge[] Edges { get; init; }

    public void SetPosition(Vector2 newPosition)
    {
        var delta = newPosition - Position;
        Position = newPosition;

        for(int index = 0; index < Edges.Length; index++)
        {
            Edge edge = Edges[index];
            edge.VertexA += delta;
            edge.VertexB += delta;
            Edges[index] = edge;
        }
    }

    public void SetRotation(float degrees)
    {
        float deltaAngle = degrees - RotationDegrees;
        RotationDegrees = degrees;

        Vector2 pivot = Position;

        for (int index = 0; index < Edges.Length; index++)
        {
            Edge edge = Edges[index];
            edge.VertexA = RotatePoint(edge.VertexA, pivot, deltaAngle);
            edge.VertexB = RotatePoint(edge.VertexB, pivot, deltaAngle);
            Edges[index] = edge;
        }
    }

    private Vector2 RotatePoint(Vector2 point, Vector2 pivot, float degrees)
    {
        float angle = degrees * (float)Math.PI / 180f;
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);

        float x = point.X - pivot.X;
        float y = point.Y - pivot.Y;

        float rotatedX = x * cos - y * sin;
        float rotatedY = x * sin + y * cos;

        return new Vector2(rotatedX + pivot.X, rotatedY + pivot.Y);
    }
}
