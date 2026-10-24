using UnityEngine;

public class bgMovement : MonoBehaviour
{
    Sprite[] bgImgs = new Sprite[3];
    SpriteRenderer bgRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bgImgs[0] = Resources.Load<Sprite>("Sprites/bg1");
        bgImgs[1] = Resources.Load<Sprite>("Sprites/bg2");
        bgImgs[2] = Resources.Load<Sprite>("Sprites/bg3");

        bgRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        bgRenderer.sprite = bgImgs[Mathf.FloorToInt(System.DateTime.UtcNow.Millisecond/333)];

    }
}
