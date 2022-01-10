using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIPlayerLook playerLook;
    public UIPlayerLook PlayerLook => playerLook;

    [SerializeField] private UIPlayerMove playerMove;
    public UIPlayerMove PlayerMove => playerMove;
}