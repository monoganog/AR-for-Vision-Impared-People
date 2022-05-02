using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GetColor : MonoBehaviour
{

    public Texture2D environmentDepthTexture;

    public Color col;
    public Image debugImg;
    public AROcclusionManager OM;

    public RenderTexture depthRT;
    public Texture2D envDepth;
    private int depthGradientMaterial;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Texture2D tex = OM.environmentDepthTexture;
        if (tex)
        {
            col = tex.GetPixel(50, 50);
        }
        debugImg.color = col;



        depthRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat);
        envDepth = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBAFloat, false);

        /*        Graphics.Blit(OM.environmentDepthTexture, depthRT, depthGradientMaterial);
                envDepth.ReadPixels(new Rect(0, 0, depthRT.width, depthRT.height), 0, 0);
                envDepth.Apply();*/

    }
}
