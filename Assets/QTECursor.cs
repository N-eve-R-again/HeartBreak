using UnityEngine;

public class QTECursor : MonoBehaviour
{
    public float rotateSpeed = 1.5f;
    public float size = 1f;

    public Vector2 dir = Vector2.zero;
    public Vector3 pos = Vector3.zero;
    public Vector2 rawInput = Vector2.zero;
    public float maxSpeed = 10f;
    public float drag = 1f;
    public RectTransform rectTransform;
    public ParticleSystem ps_Start;
    public Animator animator;
    [Space]
    public Vector2 lockOnPosition = Vector2.zero;

    public QTECursorState currState = QTECursorState.LockOn;
    public enum QTECursorState
    {
        Wait,
        LockOn,
        Move
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currState = QTECursorState.Wait;
        rotateSpeed = 1.5f;
        size = 1f;
        gameObject.SetActive(false);
}
    void Update()
    {
        switch (currState)
        {
            case QTECursorState.Wait: break;

            case QTECursorState.LockOn:
                CursorLockOn();
                break;

            case QTECursorState.Move: CursorMove(); 
                break;
        }
        rotateSpeed = Mathf.Lerp(rotateSpeed, 1.5f,Time.deltaTime*5f);
        size = Mathf.Lerp(size, 1f,Time.deltaTime*2f);
        animator.SetFloat("Speed", rotateSpeed);
        rectTransform.localScale = Vector3.one * size;
    }
    public void PlaceCursor(Vector2 _pos)
    {
        pos = _pos;
        rectTransform.localPosition = (Vector3)_pos;
    }
    void CursorMove()
    {
        rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (rawInput.magnitude > 1f) rawInput.Normalize();
        dir = Vector2.Lerp(dir, rawInput, drag * Time.deltaTime);
        if (dir.magnitude < 0.001f) dir = Vector2.zero;
        pos += new Vector3(dir.x, dir.y, 0) * Time.deltaTime * maxSpeed;
        rectTransform.localPosition = (Vector3)pos;
    }

    void CursorLockOn()
    {
        pos = Vector2.Lerp(pos, lockOnPosition, Time.deltaTime * 10f);
        rectTransform.localPosition = (Vector3)pos;
    }
    // Update is called once per frame

    public void StartLockOn(Vector2 Position)
    {
        currState = QTECursorState.LockOn;
        lockOnPosition = Position;
    }

    public void StartMove()
    {
        //playAnim
        ps_Start.Play();
        animator.SetTrigger("startMove");

        currState = QTECursorState.Move;
    }
    public void Stop()
    {
        currState = QTECursorState.Wait;
    }
    public void Success()
    {
        StartLockOn(Vector2.zero);
        animator.SetTrigger("Success");
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("QTE Target"))
        {
            if (!collision.gameObject.GetComponent<QTETarget>().validated)
            {
                collision.gameObject.GetComponent<QTETarget>().Collect();
                size = 1.3f;
                rotateSpeed = 3f;
                ps_Start.Play();
                PlaceCursor(collision.gameObject.transform.localPosition);
            }
            //StartLockOn(collision.gameObject.transform.localPosition);
        }
    }
}
