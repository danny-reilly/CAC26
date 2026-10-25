using UnityEngine;

public class johnMovement : MonoBehaviour
{
    Vector2 idlePos = new Vector2(5.37f, -1.74f);
    Vector2 goalPos;
    public bool talking;
    float timeY, timeX;
    float goalY, goalX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timeY -= Time.deltaTime;
        if(timeY < 0)
        {
            timeY = Random.Range(0.1f, 0.2f);
            goalY = -Mathf.Sign(goalY)*Random.Range(0.25f, 0.75f);
            goalX = Random.Range(-0.10f, 0.10f);
        }
        if(talking)
        {
            goalPos = idlePos + new Vector2(goalX, goalY);
        } else
        {
            goalPos = idlePos;
        }
        transform.position = Vector2.Lerp(transform.position, goalPos, 8*Time.deltaTime);
    }


    public void ChangeJohnSprite(string spriteName)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/John" + spriteName);
    }


}
