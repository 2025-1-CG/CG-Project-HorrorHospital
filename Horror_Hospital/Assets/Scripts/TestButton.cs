// Scripts/Interaction/TestButton.cs
using UnityEngine;

public class TestButton : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("버튼이 눌렸습니다!");
    }
}