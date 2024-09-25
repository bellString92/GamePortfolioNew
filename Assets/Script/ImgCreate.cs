using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImgCreate : MonoBehaviour
{
    public static Camera gameCamera; // 게임 뷰 카메라
    public static RenderTexture renderTexture;

    private void Start()
    {
        // RenderTexture 초기화
        renderTexture = new RenderTexture(1920, 1080, 24);
    }

    [MenuItem("Tools/Copy Scene View Camera to Game Camera")]
    public static void OnCamera()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        gameCamera = Camera.main;

        if (gameCamera != null && sceneView != null)
        {
            gameCamera.transform.position = sceneView.camera.transform.position;
            gameCamera.transform.rotation = sceneView.camera.transform.rotation;
            ImgSave();
        }
        else
        {
            Debug.LogWarning("No active SceneView or main camera found.");
        }
    }

    public static void ImgSave()
    {
        // RenderTexture가 null일 경우 초기화
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(1920, 1080, 24);
        }

        // 모든 게임 오브젝트 비활성화

        // 특정 게임 오브젝트만 활성화 (여기서 targetObject를 설정하거나 매개변수로 받아야 함)

        // 카메라의 Clear Flags를 Solid Color로 설정
        gameCamera.clearFlags = CameraClearFlags.SolidColor;
        gameCamera.backgroundColor = new Color(0, 0, 0, 0); // 투명 배경

        // 카메라의 타겟을 RenderTexture로 설정
        gameCamera.targetTexture = renderTexture;
        gameCamera.Render();

        // RenderTexture에서 Texture2D로 변환
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // 카메라의 타겟 텍스처 초기화
        gameCamera.targetTexture = null;
        RenderTexture.active = null;

        // PNG로 변환 후 파일로 저장
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, "CapturedSpecificGameObject.png");
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Captured specific image saved to: " + filePath);

        // 원래 상태로 복원
    }
}