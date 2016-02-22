using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CommMessaging : MonoBehaviour
{
    public Canvas Canvas;
    public Text AuthorText;
    public Text MessageText;

    private static CommMessaging _current;

    public static CommMessaging Current { get { return _current; } }

    private float _messageCooldown;

    private void Awake()
    {
        _current = this;
    }

    private void Start()
    {
        Canvas.enabled = false;
    }

    public void ShowMessage(string author, string message)
    {
        if (_messageCooldown < 4f)
        {
            AuthorText.text = author;
            MessageText.text = message;
            _messageCooldown = 5f;
            enabled = true;
        }
    }

    private void Update()
    {
        if (_messageCooldown >= 0f)
        {
            Canvas.enabled = true;
            _messageCooldown -= Time.deltaTime;
            if (_messageCooldown < 0f)
            {
                Canvas.enabled = false;
                enabled = false;
            }
        }
    }
}
