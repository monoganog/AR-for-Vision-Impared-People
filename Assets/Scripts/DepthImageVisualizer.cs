using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DepthImageVisualizer : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The UI RawImage used to display the image on screen.")]
    public Image hello;

    public Color col;

    public Text colorText;

    public AudioSource AS;


    public ARCameraManager CameraManager
    {
        get => _cameraManager;
        set => _cameraManager = value;
    }

    [SerializeField]
    [Tooltip("The ARCameraManager which will produce camera frame events.")]
    private ARCameraManager _cameraManager;


    public AROcclusionManager OcclusionManager
    {
        get => _occlusionManager;
        set => _occlusionManager = value;
    }

    [SerializeField]
    [Tooltip("The AROcclusionManager which will produce depth textures.")]
    private AROcclusionManager _occlusionManager;

    public RawImage RawImage
    {
        get => _rawImage;
        set => _rawImage = value;
    }

    [SerializeField]
    [Tooltip("The UI RawImage used to display the image on screen.")]
    private RawImage _rawImage;

    float timeToWait = 0.255f;
    float currentTime;

    public GameObject feedback;
    public Transform spawn;
    public Canvas canv;
    void Update()
    {
        if (OcclusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            using (image)
            {
                // Use the texture.
                col = UpdateRawImage(_rawImage, image);
                hello.color = col;

                float greys = col.grayscale;

                currentTime += Time.deltaTime;
                if (currentTime > timeToWait)
                {

                    timeToWait = Vibration.Remap(col.r, 4f, 0.3f, 1, 0);



                    float clammped = Mathf.Clamp(timeToWait, 0, 1);


                    colorText.text = "Color is: " + col + "\nVibration in milliseconds is: 10ms " + "\nInterval is: " + timeToWait + "\nCurrent time: " + currentTime;

                    Vibration.Vibrate(50);

                    GameObject feedbackInstance = Instantiate(feedback, spawn.position, Quaternion.identity, canv.transform);
                    Destroy(feedbackInstance, 0.1f);
                    AS.Play();
                    currentTime = 0;
                }



            }
        }
    }




    private static Color UpdateRawImage(RawImage rawImage, XRCpuImage cpuImage)
    {
        // Get the texture associated with the UI.RawImage that we wish to display on screen.
        var texture = rawImage.texture as Texture2D;

        // If the texture hasn't yet been created, or if its dimensions have changed, (re)create the texture.
        // Note: Although texture dimensions do not normally change frame-to-frame, they can change in response to
        //    a change in the camera resolution (for camera images) or changes to the quality of the human depth
        //    and human stencil buffers.
        if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
        {
            texture = new Texture2D(cpuImage.width, cpuImage.height, cpuImage.format.AsTextureFormat(), false);
            rawImage.texture = texture;
        }

        // For display, we need to mirror about the vertical access.
        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, cpuImage.format.AsTextureFormat(), XRCpuImage.Transformation.MirrorY);

        //Debug.Log("Texture format: " + cpuImage.format.AsTextureFormat()); -> RFloat

        // Get the Texture2D's underlying pixel buffer.
        var rawTextureData = texture.GetRawTextureData<byte>();

        // Make sure the destination buffer is large enough to hold the converted data (they should be the same size)
        Debug.Assert(rawTextureData.Length == cpuImage.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat),
            "The Texture2D is not the same size as the converted data.");

        // Perform the conversion.
        cpuImage.Convert(conversionParams, rawTextureData);

        // "Apply" the new pixel data to the Texture2D.
        texture.Apply();




        // Get the aspect ratio for the current texture.
        var textureAspectRatio = (float)texture.width / texture.height;

        // Determine the raw image rectSize preserving the texture aspect ratio, matching the screen orientation,
        // and keeping a minimum dimension size.
        const float minDimension = 480.0f;
        var maxDimension = Mathf.Round(minDimension * textureAspectRatio);
        var rectSize = new Vector2(maxDimension, minDimension);
        //var rectSize = new Vector2(minDimension, maxDimension);   //Portrait
        rawImage.rectTransform.sizeDelta = rectSize;

        return texture.GetPixel(texture.width / 2, texture.height / 2);



    }


}
