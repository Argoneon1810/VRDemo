namespace FakeCamera.Test1
{
    using UnityEngine;

    [ExecuteAlways]
    public class FakeMirror : MonoBehaviour
    {
        public Camera EyeCam;                 // 메인 카메라
        public Camera MirrorCam;              // Mirror Camera
        public GameObject IntersectionIndicator; // 교점 표시용 (디버그용)

        public Renderer MirrorRenderer;       // 거울 메시 렌더러
        public RenderTexture RenderTex;       // Mirror Camera의 전체 RenderTexture
        public RenderTexture ClippedRenderTexture; // 클리핑된 RenderTexture

        public void Update()
        {
            // 1. Mirror Camera의 위치와 방향 설정
            SetupMirrorCamera();

            // 2. 거울 화면 좌표 계산
            var screenPoints = CalculateViewport();

            // 3. UV Stretch 적용
            ApplyUVStretch(screenPoints);

            // 4. 클리핑된 결과를 ClippedRenderTexture에 복사
            CopyToClippedRenderTexture();
        }

        private void SetupMirrorCamera()
        {
            var planeNormal = transform.up;                        // 거울 평면 노멀
            var planeSurfacePoint = transform.position;            // 거울 평면의 한 점
            var cameraForward = EyeCam.transform.forward;          // 메인 카메라 forward
            var cameraPosition = EyeCam.transform.position;        // 메인 카메라 위치

            // 반사 벡터 계산
            var reflectedVector = cameraForward - 2 * (Vector3.Dot(cameraForward, planeNormal)) * planeNormal;

            // 교점 계산
            float numerator = Vector3.Dot(planeNormal, planeSurfacePoint - cameraPosition);
            float denominator = Vector3.Dot(planeNormal, cameraForward);

            if (Mathf.Abs(denominator) <= 1e-6f) return; // 교점 없음 (평행)

            float t = numerator / denominator;
            var intersectionPoint = cameraPosition + t * cameraForward; // 교점 위치

            if (IntersectionIndicator)
                IntersectionIndicator.transform.position = intersectionPoint;

            MirrorCam.transform.position = intersectionPoint - reflectedVector * Vector3.Distance(cameraPosition, intersectionPoint);
            MirrorCam.transform.LookAt(intersectionPoint, planeNormal); // 거울 평면 기준으로 반사 방향 설정

            // RenderTexture 설정
            MirrorCam.targetTexture = RenderTex;
        }

        private Vector3[] CalculateViewport()
        {
            var meshFilter = MirrorRenderer.GetComponent<MeshFilter>();
            var vertices = meshFilter.sharedMesh.vertices;

            Vector3[] screenPoints = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                var worldPoint = MirrorRenderer.transform.TransformPoint(vertices[i]);
                screenPoints[i] = EyeCam.WorldToScreenPoint(worldPoint); // 화면 좌표로 변환
            }

            return screenPoints;
        }

        private void ApplyUVStretch(Vector3[] screenPoints)
        {
            var meshFilter = MirrorRenderer.GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;

            Vector2[] uvPoints = new Vector2[screenPoints.Length];
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            for (int i = 0; i < screenPoints.Length; i++)
            {
                uvPoints[i] = new Vector2(
                    screenPoints[i].x / screenWidth,
                    screenPoints[i].y / screenHeight
                );
            }

            mesh.uv = uvPoints;

            // 머티리얼에 RenderTexture 연결
            //var material = MirrorRenderer.material;
            //material.mainTexture = RenderTex;
        }

        private void CopyToClippedRenderTexture()
        {
            if (RenderTex == null || ClippedRenderTexture == null)
            {
                Debug.LogWarning("RenderTex or ClippedRenderTexture is not assigned.");
                return;
            }

            // RenderTex에서 ClippedRenderTexture로 복사
            Graphics.Blit(RenderTex, ClippedRenderTexture);
        }
    }
}