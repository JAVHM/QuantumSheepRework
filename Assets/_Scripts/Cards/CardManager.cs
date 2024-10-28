using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<CardSO> availableCards;
    public static CardManager _instance;
    private bool isDragging = false;
    public DragControllerScript _currentDragController;
    public DragControllerScript _previousDragController;
    public GameObject currentDraggable;
    public bool execLock = false;
    private float cooldownTime = 0.05f; // Cooldown de 0.5 segundos
    private float cooldownTimer = 0f;  // Temporizador para controlar el cooldown

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && cooldownTimer <= 0)
        {
            execLock = false;
            DragControllerScript dragController = hit.collider.gameObject.GetComponent<DragControllerScript>();

            if (Input.GetMouseButtonDown(0) && (dragController != null || isDragging == true))
            {
                if (_currentDragController != null && dragController != null)
                    _previousDragController = _currentDragController;

                if (dragController != null)  // Verificar si dragController no es nulo
                {
                    _currentDragController = dragController;
                }
                if (isDragging == false && _currentDragController != null && execLock == false)  // Verificar que _currentDragController no sea nulo
                {
                    (isDragging, currentDraggable) = _currentDragController.HandleMouseDown();
                    execLock = true;
                }

                if (isDragging == true && _currentDragController != null && execLock == false)  // Verificar que _currentDragController no sea nulo
                {
                    if (_previousDragController != null)
                    {
                        _previousDragController.HandleMouseUp(currentDraggable);
                        _previousDragController = null;
                        (isDragging, currentDraggable) = _currentDragController.HandleMouseDown();
                    }
                    else
                    {
                        (isDragging, currentDraggable) = _currentDragController.HandleMouseUp(currentDraggable);
                        Destroy(currentDraggable);
                    } 
                    execLock = true;
                }

                
                cooldownTimer = cooldownTime;
            }
        }

        if (isDragging && _currentDragController != null)  // Verificar que _currentDragController no sea nulo
        {
            _currentDragController.DragObject(currentDraggable);
        }
    }


    public CardSO GetRandomCard()
    {
        int randomIndex = UnityEngine.Random.Range(0, availableCards.Count);
        CardSO selectedCard = availableCards[randomIndex];
        availableCards.RemoveAt(randomIndex);
        return selectedCard;
    }

    public void ReturnCard(CardSO card)
    {
        availableCards.Add(card);
    }
}
