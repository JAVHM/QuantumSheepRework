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
    private float cooldownTime = 0.25f; // Cooldown de 0.5 segundos
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

        if (Input.touchCount > 0 && cooldownTimer <= 0)  // Comprobar si hay algún toque en pantalla
        {
            Touch touch = Input.GetTouch(0);  // Obtener el primer toque
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            if (hit.collider != null)
            {
                execLock = false;
                DragControllerScript dragController = hit.collider.gameObject.GetComponent<DragControllerScript>();

                if (dragController != null || isDragging == true)
                {
                    print("enter");
                    if (_currentDragController != null && dragController != null)
                        _previousDragController = _currentDragController;

                    if (dragController != null)  // Verificar si dragController no es nulo
                    {
                        _currentDragController = dragController;
                    }

                    if (isDragging == false && _currentDragController != null && execLock == false)  // Verificar que _currentDragController no sea nulo
                    {
                        print("1");
                        _currentDragController = dragController;
                        (isDragging, currentDraggable) = _currentDragController.HandleMouseDown();
                        execLock = true;
                    }

                    //print(isDragging + " | " + (_currentDragController != null) + "|" + (execLock == false));
                    if (isDragging == true && _currentDragController != null && execLock == false)  // Verificar que _currentDragController no sea nulo
                    {
                        _currentDragController.transform.position = touchPosition;
                        print(_currentDragController.transform.position);
                        if (_previousDragController != null)
                        {
                            print("2");
                            _previousDragController.HandleMouseUp(currentDraggable, touchPosition);
                            _previousDragController = null;
                            (isDragging, currentDraggable) = _currentDragController.HandleMouseDown();
                        }
                        else
                        {
                            print("3");
                            (isDragging, currentDraggable) = _currentDragController.HandleMouseUp(currentDraggable, touchPosition);
                            Destroy(currentDraggable);
                        }
                        execLock = true;
                    }
                }
                cooldownTimer = cooldownTime;
            }

            if (isDragging && _currentDragController != null)  // Verificar que _currentDragController no sea nulo
            {
                _currentDragController.DragObject(currentDraggable);
            }
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
