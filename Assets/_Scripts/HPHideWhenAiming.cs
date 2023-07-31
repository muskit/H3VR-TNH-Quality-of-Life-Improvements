using UnityEngine;
using UnityEngine.UI;
using FistVR;

// To be attached to FVRHealthBar object
public class HPHideWhenAiming : MonoBehaviour
{
    CanvasGroup canvasGroup;

    GameObject gObjHUD;
    RawImage background;

    FVRViveHand leftHand;
    FVRViveHand rightHand;

    BoxCollider hudCollider;
    GameObject gObjLeftLine;
    GameObject gObjRightLine;
    LineRenderer leftHandLine;
    LineRenderer rightHandLine;

    //// Testing renderers
    //GameObject gObjColliderRenderer;
    //LineRenderer colliderRenderer;

    // Use this for initialization
    void Start()
    {
        gObjHUD = transform.GetChild(0).gameObject;
        leftHand = MeatKitPlugin.playerCamera.transform.parent.GetChild(1).GetComponent<FVRViveHand>();
        rightHand = MeatKitPlugin.playerCamera.transform.parent.GetChild(0).GetComponent<FVRViveHand>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        var gObjBG = gObjHUD.transform.Find("Background");
        if (gObjBG != null)
            background = gObjBG.GetComponent<RawImage>();

        hudCollider = gObjHUD.AddComponent<BoxCollider>();
        hudCollider.isTrigger = true;
        hudCollider.gameObject.layer = LayerMask.NameToLayer("UI");
        hudCollider.size = new Vector3(50, 30, .01f);

        //// TESTING: collider visuals
        //gObjColliderRenderer = new GameObject();
        //gObjColliderRenderer.transform.SetParent(hudCollider.transform, false);
        //colliderRenderer = gObjColliderRenderer.AddComponent<LineRenderer>();
        //colliderRenderer.SetWidth(.005f, .005f);
        //colliderRenderer.SetColors(Color.blue, Color.blue);
        //colliderRenderer.positionCount = 8;

        //// TESTING: beam from held weapons
        //gObjLeftLine = new GameObject();
        //leftHandLine = gObjLeftLine.AddComponent<LineRenderer>();
        //leftHandLine.positionCount = 2;
        //leftHandLine.SetWidth(.008f, .008f);
        //leftHandLine.SetColors(Color.blue, Color.blue);
        //gObjRightLine = new GameObject();
        //rightHandLine = gObjRightLine.AddComponent<LineRenderer>();
        //rightHandLine.positionCount = 2;
        //rightHandLine.SetWidth(.008f, .008f);
        //rightHandLine.SetColors(Color.blue, Color.blue);
    }

    void FixedUpdate()
    {
        FVRInteractiveObject[] objs = { leftHand.CurrentInteractable, rightHand.CurrentInteractable };
        LineRenderer[] lines = { leftHandLine, rightHandLine };
        
        bool rayHit = false;

        for (int i = 0; i < 2; ++i)
        {
            Transform transMuzzle;
            if (objs[i] is FVRFireArm)
            {
                var firearm = objs[i] as FVRFireArm;
                transMuzzle = firearm.CurrentMuzzle;
            }
            else if (objs[i] is SosigWeaponPlayerInterface)
            {
                var playerInterface = objs[i] as SosigWeaponPlayerInterface;
                var sosigWeapon = playerInterface.gameObject.GetComponent<SosigWeapon>();
                transMuzzle = sosigWeapon.Muzzle;
            }
            else // not a recognized weapon, don't do anything
                continue;

            // TESTING: draw beam from held wpn muzzle
            //Vector3[] beamPos = { transMuzzle.position, transMuzzle.position + 5 * transMuzzle.forward };
            //lines[i].SetPositions(beamPos);

            RaycastHit hitInfo;
            if (hudCollider.Raycast(new Ray(transMuzzle.position, transMuzzle.forward), out hitInfo, 20) ||
                hudCollider.Raycast(new Ray(transMuzzle.position, -transMuzzle.forward), out hitInfo, 20))
                rayHit = true;
        }

        if (rayHit)
        {
            canvasGroup.alpha = MeatKitPlugin.cfgHPAimOpacity.Value;
            if (background != null)
                background.enabled = false;
        }
        else
        {
            canvasGroup.alpha = 1f;
            if (background != null)
                background.enabled = true;
        }

        // TESTING: draw the collider
        //var trans = hudCollider.transform;
        //var min = hudCollider.center - hudCollider.size * 0.5f;
        //var max = hudCollider.center + hudCollider.size * 0.5f;
        //Vector3[] points = {
        //    trans.TransformPoint(new Vector3(min.x, min.y, min.z)),
        //    trans.TransformPoint(new Vector3(min.x, min.y, max.z)),
        //    trans.TransformPoint(new Vector3(min.x, max.y, min.z)),
        //    trans.TransformPoint(new Vector3(min.x, max.y, max.z)),
        //    trans.TransformPoint(new Vector3(max.x, min.y, min.z)),
        //    trans.TransformPoint(new Vector3(max.x, min.y, max.z)),
        //    trans.TransformPoint(new Vector3(max.x, max.y, min.z)),
        //    trans.TransformPoint(new Vector3(max.x, max.y, max.z))
        //};
        //colliderRenderer.SetPositions(points);
    }
}
