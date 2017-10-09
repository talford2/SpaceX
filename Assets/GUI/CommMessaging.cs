using UnityEngine;
using UnityEngine.UI;

public class CommMessaging : MonoBehaviour
{
    public Canvas Canvas;
    public Text AuthorText;
    public Text MessageText;
    public AudioClip Sound;

    private static CommMessaging _current;

    public static CommMessaging Current { get { return _current; } }

    private float _messageCooldown;

    private void Awake()
    {
        _current = this;

        Player.Current.Squadron.OnChangeMember += OnChangeSquadronMember;
    }

    private void Start()
    {
        Canvas.enabled = false;
    }

    public void ShowMessage(GameObject to, string author, string message)
    {
        if (Player.Current.VehicleInstance != null && Player.Current.VehicleInstance.gameObject == to)
        {
            if (_messageCooldown < 4f)
            {
                Canvas.enabled = true;
                AuthorText.text = author;
                MessageText.text = message;
                _messageCooldown = 5f;
                if (Sound != null)
                    AudioSource.PlayClipAtPoint(Sound, Universe.Current.ViewPort.transform.position);
            }
        }
    }

    private void HideMessage()
    {
        Canvas.enabled = false;
    }

    private void OnChangeSquadronMember(GameObject from, GameObject to)
    {
        HideMessage();
    }

    private void Update()
    {
        if (_messageCooldown >= 0f)
        {
            _messageCooldown -= Time.deltaTime;
            if (_messageCooldown < 0f)
            {
                HideMessage();
            }
        }
    }
}
