using UnityEngine;
using DG.Tweening;
using NUnit.Compatibility;

[RequireComponent(typeof(LineRenderer))]
public class DrawCircleWithDotween : MonoBehaviour
{
    public float startRadius = 0.1f;
    public float duration = .2f;
    public int segments = 100;

    private float targetRadius;
    private LineRenderer lineRenderer;
    private Material lineMaterial;
    private float currentRadius;

    private StructureUnit unit => GetComponentInParent<StructureUnit>();

    public void RegisterDefensiveCircle()
    {
        targetRadius = unit.ObjectDetectionRadius;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;

        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // 初始半径
        currentRadius = startRadius;
        DrawCircle();

        // 动画渐变半径
        DOTween.To(() => currentRadius, x =>
        {
            currentRadius = x;
            DrawCircle();
        }, targetRadius, duration).SetEase(Ease.OutQuad).SetLink(gameObject);
    }

    void DrawCircle()
    {
        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * currentRadius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * currentRadius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
            angle += 360f / segments;
        }
    }

    public void ShrinkAndDisappear()
    {
        Sequence shrinkSeq = DOTween.Sequence();

        // 半径变小
        shrinkSeq.Join(DOTween.To(() => currentRadius, x => {
            currentRadius = x;
            DrawCircle();
        }, 0f, duration).SetEase(Ease.InQuad)).SetLink(gameObject);

        // 渐隐透明
        shrinkSeq.Join(DOTween.ToAlpha(() => lineMaterial.color, x => lineMaterial.color = x, 0f, 1f));
    }
}
