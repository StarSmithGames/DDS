using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mug : MonoBehaviour
{
    public bool isEnable = true;

    [SerializeField] private Transform liquid;
    [SerializeField] private Transform liquidMesh;

    [SerializeField] private int sloshSpeed = 60;
    [SerializeField] private int rotateSpeed = 15;

    [SerializeField] private float difference = 25;


    private void Update()
    {
        if (isEnable)
        {
            Slosh();//Motion

            liquidMesh.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.Self);//Rotation
        }
    }

    private void Slosh()
    {
        Quaternion inverseRotation = Quaternion.Inverse(transform.localRotation);

        Vector3 rotateTo = Quaternion.RotateTowards(liquid.localRotation, inverseRotation, sloshSpeed * Time.deltaTime).eulerAngles;

        rotateTo.x = ClampRotaton(rotateTo.x);
        rotateTo.z = ClampRotaton(rotateTo.z);

        liquid.localEulerAngles = rotateTo;
    }
    private void MoreSlosh() { }
    private float ClampRotaton(float value)
    {
        return value > 100 ? Mathf.Clamp(value, 360 - difference, 360) : Mathf.Clamp(value, 0, difference);
    }
}