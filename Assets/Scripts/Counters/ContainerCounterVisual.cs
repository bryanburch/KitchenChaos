using System;
using UnityEngine;

// Responsible for triggering the open-close animation for the container counter.
// Listens for an event to know when to start the animation.
// 
// The animator is attached to the same game object that this script is, so it's 
// easy to grab the animator from here.
public class ContainerCounterVisual : MonoBehaviour
{
    private const string OPEN_CLOSE = "OpenClose";

    [SerializeField] private ContainerCounter containerCounter;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // "Subscribe" to the event that a counter container fires off when a player interacts with it
        containerCounter.OnPlayerGrabbedObject += ContainerCounter_OnPlayerGrabbedObject;
    }

    private void ContainerCounter_OnPlayerGrabbedObject(object sender, EventArgs e)
    {
        animator.SetTrigger(OPEN_CLOSE);
    }
}
