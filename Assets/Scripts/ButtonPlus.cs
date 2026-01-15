using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
// using static ThemeManager;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class ButtonPlus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public Backdrop backdrop;
	public enum ButtonType {Standard, Back, Warning}
	public ButtonType buttonType;
	
    public RectTransform rt;
	public bool isButton;
	public bool buttonEnabled = true;
	public AudioClip clickSound;
	public AudioSource soundSource;
	public float volumeFactor;
	public bool holdingDown = false;
	public bool mouseOverButton = false;
	public bool specialState = false;
	[SerializeField]
    public UnityEvent onClickEvent;
	[SerializeField]
    private UnityEvent onDoubleClickEvent;
	private float timeOfLastClick;
	private int clicksInARow = 0;
	
	public bool moveImageWhenClicked = true;
	public RectTransform buttonImageRT;
	public Vector2 buttonImageDestinationAdditive = new Vector2(0, -2f);
	public float moveImageDuration = 0.05f;
	public RectTransform labelRT;
	
	public bool expandEnabled;
	public float expansionFactor = 1.05f;
	public float expansionDuration = 0.1f;
	
	public bool changeColorEnabled;
	public Image buttonImage;
	public Image shadowImage;
	public float changeColorDuration = 0.1f;
	
	private IEnumerator checkForGlobalMouseUpCoroutine;
	private bool checkingForGlobalMouseUp = false;
	
	public bool tickOnMouseOver = false;
	
	public Label buttonLabel;
	
	void Start()
	{
		/*if(isButton)
		{
			buttonImageOrigin = buttonImageRT.anchoredPosition;
		}*/
	}
	public void UpdateColor()
	{
		if(!changeColorEnabled || backdrop == null)
		{
			return;
        }
		if (buttonEnabled)
		{
			if (specialState)
			{
				backdrop.UpdateBackdropUIElementType(UIElementType.buttonSpecialState);
			}
			else
			{
				switch (buttonType)
				{
					case ButtonType.Standard:
						backdrop.UpdateBackdropUIElementType(UIElementType.standardButtonActive);
						break;
					case ButtonType.Back:
						backdrop.UpdateBackdropUIElementType(UIElementType.backButtonActive);
						break;
					case ButtonType.Warning:
						backdrop.UpdateBackdropUIElementType(UIElementType.warningButtonActive);
						break;
                }
            }
		}
		else
		{
			backdrop.UpdateBackdropUIElementType(UIElementType.buttonDisabled);
		}
	}
	public void ChangeButtonEvent(UnityAction newAction)
	{
		onClickEvent.RemoveAllListeners();
		onClickEvent.AddListener(newAction);
	}
	public void ChangeButtonText(string newText)
	{
		buttonLabel.ChangeText(newText);
	}
	public bool GetButtonEnabled()
	{
		return buttonEnabled;
	}
	
	void OnDisable()
	{
		mouseOverButton = false;
		ResetButton();
	}
	
	public void ResetButton()
	{
/*		if(changingScale)
		{
			StopCoroutine(scaleChangeCoroutine);
			changingScale = false;
		}
		if(changingColor)
		{
			StopCoroutine(colorChangeCoroutine);
			changingColor = false;
		}
		if(movingImage)
		{
			StopCoroutine(moveImageCoroutine);
			movingImage = false;
		}
		rt.localScale = Vector3.one;
		if(isButton)
		{
			buttonImageRT.anchoredPosition = buttonImageOrigin;
		}*/
		backdrop.ResetBackdrop();
        UpdateColor();
        if (backdrop.IsMouseOver())
		{
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Mouse.current.position.ReadValue();
            OnPointerEnter(pointerEventData);
        }
	}
	
	public void SetButtonEnabled(bool newEnabledState)
	{
		if(buttonEnabled == newEnabledState)
		{
			return;
		}
		buttonEnabled = newEnabledState;
		ResetButton();
	}
	
	public void ChangeSpecialState(bool isSpecial)
	{
		// Debug.Log($"{this.name} change specialState called with intent to set special state to {isSpecial}");
		specialState = isSpecial;
		if(buttonEnabled)
		{
			/*if(changingColor)
			{
				StopCoroutine(colorChangeCoroutine);
			}*/
			backdrop.StopChangingColor();
        }
		UpdateColor();
		// Debug.Log($"{this.name} finished change specialState to {specialState}");
	}
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		if(isButton && !buttonEnabled)
		{
			return;
		}
		if(expandEnabled)
		{
			/*if(changingScale)
			{
				StopCoroutine(scaleChangeCoroutine);
			}
			scaleChangeCoroutine = ChangeScale(new Vector3(expansionFactor, expansionFactor, 1f), expansionDuration);
			StartCoroutine(scaleChangeCoroutine);*/
			backdrop.StartChangeScale(new Vector3(expansionFactor, expansionFactor, 1f), expansionDuration);
        }
		if(changeColorEnabled)
		{
			/*if(changingColor)
			{
				StopCoroutine(colorChangeCoroutine);
			}
            colorChangeCoroutine = ChangeColor(backdrop.GetCurrentBaseColor() * r.themeManager.GetColorFromCurrentTheme(UIElementType.buttonMouseOver), changeColorDuration);
			StartCoroutine(colorChangeCoroutine);*/
			backdrop.StartColorDarken(changeColorDuration);
        }
		if(isButton)
		{
			mouseOverButton = true;
			if(holdingDown)
			{
				/*if(movingImage)
				{
					StopCoroutine(moveImageCoroutine);
				}
				moveImageCoroutine = MoveImage(buttonImageOrigin + buttonImageDestinationAdditive, moveImageDuration);
				StartCoroutine(moveImageCoroutine);*/
				backdrop.StartMoveImage(buttonImageDestinationAdditive, moveImageDuration);
				if(checkingForGlobalMouseUp)
				{
					StopCoroutine(checkForGlobalMouseUpCoroutine);
					checkingForGlobalMouseUp = false;
				}
			}
		}
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		MouseExit();
	}
	
	public void MouseExit()
	{
		if(isButton)
		{
			mouseOverButton = false;
		}
		if(isButton && !buttonEnabled)
		{
			return;
		}
		if(expandEnabled)
		{
			/*if(changingScale)
			{
				StopCoroutine(scaleChangeCoroutine);
			}
			scaleChangeCoroutine = ChangeScale(Vector3.one, expansionDuration);
			StartCoroutine(scaleChangeCoroutine);*/
			backdrop.StartChangeScale(Vector3.one, expansionDuration);
        }
		if(changeColorEnabled)
		{
            backdrop.StopChangingColor();
            /*if (changingColor)
			{
				StopCoroutine(colorChangeCoroutine);
				

            }*/
			if(specialState)
			{
				// colorChangeCoroutine = ChangeColor(specialStateColor, changeColorDuration);
				// backdrop.StartChangeColor(specialStateColor, changeColorDuration);
            }
			else
			{
				// colorChangeCoroutine = ChangeColor(baseColor, changeColorDuration);
			}
			backdrop.StartColorReset(changeColorDuration);
            // StartCoroutine(colorChangeCoroutine);
        }
		if(isButton)
		{
			mouseOverButton = false;
			if(holdingDown)
			{
				/*if(movingImage)
				{
					StopCoroutine(moveImageCoroutine);
				}
				moveImageCoroutine = MoveImage(buttonImageOrigin, moveImageDuration);
				StartCoroutine(moveImageCoroutine);*/
				backdrop.StartMoveImage(Vector2.zero, moveImageDuration);
                checkForGlobalMouseUpCoroutine = CheckForGlobalMouseUp();
				StartCoroutine(checkForGlobalMouseUpCoroutine);
			}
		}
	}
	
	public void OnPointerDown(PointerEventData pointerEventData)
	{
		StartClickingButton();
	}
	
	public void StartClickingButton()
	{
		if(!isButton || !buttonEnabled)
		{
			return;
		}
		holdingDown = true;
		if(moveImageWhenClicked)
		{
			/*if(movingImage)
			{
				StopCoroutine(moveImageCoroutine);
			}
			moveImageCoroutine = MoveImage(buttonImageOrigin + buttonImageDestinationAdditive, moveImageDuration);
			StartCoroutine(moveImageCoroutine);*/
			backdrop.StartMoveImage(buttonImageDestinationAdditive, moveImageDuration);
        }
	}
	
	public void ExectuteButtonPress()
	{
		if(moveImageWhenClicked)
		{
			/*if(movingImage)
			{
				StopCoroutine(moveImageCoroutine);
			}
			moveImageCoroutine = MoveImage(buttonImageOrigin, moveImageDuration);
			StartCoroutine(moveImageCoroutine);*/
			backdrop.StartMoveImage(Vector2.zero, moveImageDuration);
        }
		if(!isButton || !buttonEnabled)
		{
			return;
		}
        if (clickSound != null && soundSource != null)
        {
            soundSource.PlayOneShot(clickSound, volumeFactor);
        }
        if (onDoubleClickEvent.GetPersistentEventCount() > 0)
		{
			if(Time.time - timeOfLastClick > 0.4f)
			{
				clicksInARow = 0;
			}
			timeOfLastClick = Time.time;
			clicksInARow++;
			if(mouseOverButton && holdingDown)
			{
				holdingDown = false;
				if(clicksInARow >= 2)
				{
					onDoubleClickEvent.Invoke();
				}
				else
				{
					onClickEvent.Invoke();
				}
			}
		}
		else
		{
			if(mouseOverButton && holdingDown)
			{
				holdingDown = false;
				onClickEvent.Invoke();
			}
		}
	}
	
	public void OnPointerUp(PointerEventData pointerEventData)
	{
		ExectuteButtonPress();
	}
	
/*	private IEnumerator ChangeScale(Vector3 destinationScale, float duration)
	{
		changingScale = true;
		Vector3 startingScale = rt.localScale;
		float t = 0;
		while(t < duration)
		{
			t += Time.deltaTime;
			rt.localScale = new Vector3(Mathf.Lerp(startingScale.x, destinationScale.x, t / duration), Mathf.Lerp(startingScale.y, destinationScale.y, t / duration), 1f);
			yield return null;
		}
		rt.localScale = destinationScale;
		changingScale = false;
	}
	
	private IEnumerator ChangeColor(Color destinationColor, float duration)
	{
		changingColor = true;
		Color originColor = buttonImage.color;
		float t = 0;
		while(t < duration)
		{
			t += Time.deltaTime;
			buttonImage.color = Color.Lerp(originColor, destinationColor, t / duration);
			yield return null;
		}
		buttonImage.color = destinationColor;
		changingColor = false;
	}
	
	private IEnumerator MoveImage(Vector2 destination, float duration)
	{
		movingImage = true;
		Vector2 origin = buttonImageRT.anchoredPosition;
		float t = 0;
		while(t < duration)
		{
			t += Time.deltaTime;
			buttonImageRT.anchoredPosition = Vector2.Lerp(origin, destination, t / duration);
			yield return null;
		}
		buttonImageRT.anchoredPosition = destination;
		movingImage = false;
	}*/
	
	private IEnumerator CheckForGlobalMouseUp()
	{
		checkingForGlobalMouseUp = true;
		// while(Input.GetMouseButton(0))
		while(Mouse.current.leftButton.isPressed)
		{
			yield return null;
		}
		holdingDown = false;
		checkingForGlobalMouseUp = false;
	}
}