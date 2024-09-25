using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImgCreate : MonoBehaviour
{
    public static Camera gameCamera; // ���� �� ī�޶�
    public static RenderTexture renderTexture;

    private void Start()
    {
        // RenderTexture �ʱ�ȭ
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
        // RenderTexture�� null�� ��� �ʱ�ȭ
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(1920, 1080, 24);
        }

        // ��� ���� ������Ʈ ��Ȱ��ȭ

        // Ư�� ���� ������Ʈ�� Ȱ��ȭ (���⼭ targetObject�� �����ϰų� �Ű������� �޾ƾ� ��)

        // ī�޶��� Clear Flags�� Solid Color�� ����
        gameCamera.clearFlags = CameraClearFlags.SolidColor;
        gameCamera.backgroundColor = new Color(0, 0, 0, 0); // ���� ���

        // ī�޶��� Ÿ���� RenderTexture�� ����
        gameCamera.targetTexture = renderTexture;
        gameCamera.Render();

        // RenderTexture���� Texture2D�� ��ȯ
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // ī�޶��� Ÿ�� �ؽ�ó �ʱ�ȭ
        gameCamera.targetTexture = null;
        RenderTexture.active = null;

        // PNG�� ��ȯ �� ���Ϸ� ����
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, "CapturedSpecificGameObject.png");
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Captured specific image saved to: " + filePath);

        // ���� ���·� ����
    }
}