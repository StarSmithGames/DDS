using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using Sirenix.OdinInspector;
using Zenject.ReflectionBaking.Mono.Cecil.Cil;

namespace Xamin
{
    [RequireComponent(typeof(AudioSource))]
    public class CircleSelector : MonoBehaviour
    {
        public bool IsOpened => isOpened;
        private bool isOpened;

        public GameObject CurrentSegment
        {
            get => currentSegment;
            set
            {
                if (value == null || value == CurrentSegment) return;
                currentSegment = value;

                //if (segmentChangedSound == null || !(audioCoolDown <= 0)) return;
                //audioSource.PlayOneShot(segmentChangedSound);
            }
        }
        private GameObject currentSegment;

        [SerializeField] private Image background;
        [SerializeField] private Image cursor;//fillamount
        [SerializeField] private GameObject separatorsParent;
        [SerializeField] private GameObject buttonsParent;

        [SerializeField] private GameObject separatorPrefab;

        [Header("Customization")]
        [SerializeField] private Color accentColor;
        [SerializeField] private Color disabledColor, backgroundColor;
        [Space(10)]
        [SerializeField] private bool useSeparators;

        [Header("Animations")]
        [Range(0.0001f, 1)]
        [SerializeField] private float lerpAmount = .145f;
        private float desiredFill;
        private float radius = 120f;

        [Header("Interaction")]
        [SerializeField] private List<GameObject> buttons = new List<GameObject>();
        
        private List<Xamin.Button> buttonsInstances = new List<Xamin.Button>();

        [SerializeField] private bool snap, tiltTowardsMouse;
        [SerializeField] private float tiltAmount = 15;
        
        private string gamepadAxisX, gamepadAxisY;//rm

        private int buttonCount;
        private int startButtonCount;

        private GlobalSettings globalSettings;

        [Inject]
        private void Construct(GlobalSettings globalSettings)
        {
            this.globalSettings = globalSettings;
        }

        private void Start()
        {
            Close();

            buttonCount = buttons.Count;
            foreach (Xamin.Button b in buttonsInstances)
                Destroy(b.gameObject);
            buttonsInstances.Clear();
            if (buttonCount > 0 && buttonCount < 11)
            {
                #region Arrange Buttons
                startButtonCount = buttonCount;
                desiredFill = 1f / (float)buttonCount;
                float fillRadius = desiredFill * 360f;
                float previousRotation = 0;
                foreach (Transform sep in separatorsParent.transform)
                    Destroy(sep.gameObject);
                for (int i = 0; i < buttonCount; i++)
                {
                    //TIP   y=sin(angle)
                    //      x=cos(angle)
                    GameObject b = Instantiate(buttons[i], Vector2.zero, transform.rotation) as GameObject;

                    b.transform.SetParent(buttonsParent.transform);
                    float bRot = previousRotation + fillRadius / 2;
                    previousRotation = bRot + fillRadius / 2;
                    GameObject separator = Instantiate(separatorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    separator.transform.parent = separatorsParent.transform;
                    separator.transform.localScale = Vector3.one;
                    separator.transform.localPosition = Vector3.zero;
                    separator.transform.localRotation = Quaternion.Euler(0, 0, previousRotation);

                    b.transform.localPosition = new Vector2(radius * Mathf.Cos((bRot - 90) * Mathf.Deg2Rad), -radius * Mathf.Sin((bRot - 90) * Mathf.Deg2Rad));
                    b.transform.localScale = Vector3.one;
                    if (bRot > 360)
                        bRot -= 360;
                    b.name = bRot.ToString();

                    buttonsInstances.Add(b.GetComponent<Button>());
                }
                #endregion
            }
        }
        [Button]
        public void Open()
        {
            isOpened = true;
        }
        
        [Button]
        public void Close()
        {
            isOpened = false;
        }

        public Xamin.Button GetButtonWithId(string id)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Xamin.Button b = buttonsInstances[i];
                if (b.id == id)
                    return b;
            }
            return null;
        }

        void Update()
        {
            if (isOpened)
            {
                background.color = backgroundColor;
                separatorsParent.SetActive(useSeparators);
                #region Check if should re-arrange
                buttonCount = buttons.Count;
                if (startButtonCount != buttonCount)
                {
                    Start();
                    return;
                }
                #endregion
                #region Update the mouse rotation and extimate cursor rotation
                cursor.fillAmount = Mathf.Lerp(cursor.fillAmount, desiredFill, .2f);
                //orientamento cursore
                Vector3 screenBounds = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
                Vector3 vector = UnityEngine.Input.mousePosition - screenBounds;
                if (tiltTowardsMouse)
                {
                    float x = vector.x / screenBounds.x, y = vector.y / screenBounds.y;
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(x, y, 0) * -tiltAmount), lerpAmount);
                }
                float mouseRotation = ((globalSettings.projectSettings.platform == PlatformType.Consoles) ? Mathf.Atan2(Input.GetAxis(gamepadAxisX), Input.GetAxis(gamepadAxisY)) : Mathf.Atan2(vector.x, vector.y)) * 57.29578f;
                if (mouseRotation < 0f)
                    mouseRotation += 360f;
                float cursorRotation = -(mouseRotation - cursor.fillAmount * 360f / 2f);
                #endregion

                #region Find and color the selected button
                float difference = 9999;
                GameObject nearest = null;
                for (int i = 0; i < buttonCount; i++)
                {
                    GameObject b = buttonsInstances[i].gameObject;
                    b.transform.localScale = Vector3.one;
                    float rotation = System.Convert.ToSingle(b.name);// - 360 / buttonCount / 2;
                    if (Mathf.Abs(rotation - mouseRotation) < difference)
                    {
                        nearest = b;
                        difference = Mathf.Abs(rotation - mouseRotation);
                    }
                }
                CurrentSegment = nearest;

                if (snap)
                    cursorRotation = -(System.Convert.ToSingle(CurrentSegment.name) - cursor.fillAmount * 360f / 2f);
                cursor.transform.localRotation = Quaternion.Slerp(cursor.transform.localRotation, Quaternion.Euler(0, 0, cursorRotation), lerpAmount);
                #endregion


                #region Call the selected button action
                if (Input.GetMouseButton(0))
                {
                    cursor.rectTransform.localPosition = Vector3.Lerp(cursor.rectTransform.localPosition, new Vector3(0, 0, -10), lerpAmount);
                    if (CurrentSegment.GetComponent<Xamin.Button>().IsUnlocked)
                    {
                        CurrentSegment.transform.localScale = new Vector2(.8f, .8f);
                    }
                }
                else
                    cursor.rectTransform.localPosition = Vector3.Lerp(cursor.rectTransform.localPosition, Vector3.zero, lerpAmount);
                if (Input.GetMouseButtonUp(0))
                {
                    if (CurrentSegment.GetComponent<Xamin.Button>().IsUnlocked)
                    {
                        CurrentSegment.GetComponent<Xamin.Button>().ExecuteAction();
                        //audioSource.PlayOneShot(segmentClickedSound);
                        Close();
                    }
                }
                #endregion
            }
        }
    }

    public enum AnimationType { zoomIn, zoomOut }

    /// <summary>
    /// Button source
    /// <para>use prefabs in a menu where you want to add or remove elements at runtime</para>
    /// <para>use scene if you want a static menu that you can only modify on the editor</para>
    /// </summary>
    public enum ButtonSource { prefabs, scene }
}