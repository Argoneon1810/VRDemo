using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

namespace FakeMirror.Test2
{
    public class MirrorCamera : MonoBehaviour
    {
        public Camera mainCamera, mirrorCamera;
        public RenderTexture mirrorTexture;
        public Renderer mirrorRenderer;
        public GameObject IntersectionIndicator;

        private void Awake()
        {
            if (!mirrorTexture)
                mirrorTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        }

        private void Start()
        {
            mirrorRenderer.material.mainTexture = mirrorTexture;
        }

        private void OnEnable()
        {
            RenderPipeline.beginCameraRendering += UpdateCamera;
        }

        private void OnDisable()
        {
            RenderPipeline.beginCameraRendering -= UpdateCamera;
        }

        void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
        {
            if (mirrorRenderer.isVisible)
            {
                mirrorCamera.targetTexture = mirrorTexture;
                RenderCamera(SRC);
            }
        }

        private void RenderCamera(ScriptableRenderContext SRC)
        {
            Transform inTransform = transform;

            Transform cameraTransform = mirrorCamera.transform;
            cameraTransform.position = transform.position;
            cameraTransform.rotation = transform.rotation;

            Plane p = new Plane(transform.up, transform.position);      // 거울 평면
            var cameraForward = mainCamera.transform.forward;           // 메인 카메라 forward
            var cameraPosition = mainCamera.transform.position;         // 메인 카메라 위치

            // 반사 벡터 계산
            var reflectedVector = cameraForward - 2 * (Vector3.Dot(cameraForward, p.normal)) * p.normal;

            // 교점 계산
            float numerator = Vector3.Dot(p.normal, p.GetPoint() - cameraPosition);
            float denominator = Vector3.Dot(p.normal, cameraForward);

            if (Mathf.Abs(denominator) <= 1e-6f) return; // 교점 없음 (평행)

            float t = numerator / denominator;
            var intersectionPoint = cameraPosition + t * cameraForward; // 교점 위치

            if (IntersectionIndicator)
                IntersectionIndicator.transform.position = intersectionPoint;

            //거울상 위치에 카메라 배치
            mirrorCamera.transform.position = intersectionPoint - reflectedVector * Vector3.Distance(cameraPosition, intersectionPoint);
            var projectedUp = Vector3.ProjectOnPlane(Vector3.up, intersectionPoint - mirrorCamera.transform.position).normalized;
            //메인 카메라와 동일한 방법으로 거울을 쳐다보게
            mirrorCamera.transform.LookAt(intersectionPoint, projectedUp);

            // Set the camera's oblique view frustum.
            //Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
            //Vector4 clipPlaneCameraSpace =
                //Matrix4x4.Transpose(Matrix4x4.Inverse(mirrorCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;
                //Matrix4x4.Inverse(mirrorCamera.worldToCameraMatrix) * clipPlaneWorldSpace;

            //var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            //mirrorCamera.projectionMatrix = newMatrix;

            // Render the camera to its render target.
            UniversalRenderPipeline.RenderSingleCamera(SRC, mirrorCamera);
        }
    }
}