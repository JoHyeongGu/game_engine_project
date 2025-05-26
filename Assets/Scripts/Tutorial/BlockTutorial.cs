using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BlockTutorial : MonoBehaviour
{
    public Block.COLOR color;
    public bool target = false;
    public GameObject sideObject;
    public GameObject[] togethers;
    public StoreTutorial store;

    private Vector3 mousePos;
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isClicked = false;
    private bool moving = false;
    private float rate = 0.0f;
    private Renderer render;
    private Color colorValue;

    void Start()
    {
        SetColor();
        if (this.store != null && this.togethers != null)
        {
            this.store.datas[this.color] = 10 - (this.togethers.Length + 1);
        }
        SetTargetsColor();
    }

    void Update()
    {
        if (target)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
            else if (isClicked && Input.GetMouseButtonUp(0))
            {
                Vector3 dir = (Input.mousePosition - mousePos);
                dir.Normalize();
                if (dir.x >= 0.7f && dir.y <= 0.4f)
                {
                    startPos = this.transform.position;
                    targetPos = this.transform.position + (Vector3.right * 1);
                    moving = true;
                }
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 0.2f);
            }
        }
        if (moving && this.transform.position != targetPos)
        {
            rate += (Time.deltaTime * 5);
            this.transform.position = Vector3.Lerp(startPos, targetPos, rate);
            sideObject.transform.position = Vector3.Lerp(startPos, targetPos, 1 - rate);
        }

        if (this.transform.position == targetPos)
        {
            this.Finish();
        }
    }

    public void SetColor()
    {
        render = this.GetComponent<Renderer>();
        switch (this.color)
        {
            default:
            case Block.COLOR.RED:
                colorValue = Color.red; break;//new Color(1.0f, 0.5f, 0.5f); break;
            case Block.COLOR.BLUE:
                colorValue = Color.blue; break;
            case Block.COLOR.YELLOW:
                colorValue = Color.yellow; break;
            case Block.COLOR.GREEN:
                colorValue = Color.green; break;
            case Block.COLOR.MAGENTA:
                colorValue = Color.magenta; break;
            case Block.COLOR.ORANGE:
                colorValue = new Color(1.0f, 0.46f, 0.0f); break;
        }
        if (!this.target) colorValue = Color.grey * Random.Range(1.2f, 1.6f);
        else
        {
            this.sideObject.GetComponent<Renderer>().material.color = Color.grey * 1.7f;
        }
        render.material.color = colorValue;
    }

    private void OnClick()
    {
        isClicked = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                mousePos = Input.mousePosition;
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 0.2f);
                isClicked = true;
            }
        }
    }

    private async void Finish()
    {
        this.enabled = false;
        await Task.Delay(100);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 0.2f);
        sideObject.transform.position = new Vector3(sideObject.transform.position.x, sideObject.transform.position.y, sideObject.transform.position.z + 0.2f);
        foreach (GameObject together in this.togethers)
        {
            together.GetComponent<Renderer>().material.color *= 0.5f;
        }
        render.material.color *= 0.5f;
        await Task.Delay(300);
        this.store.datas[this.color] += (this.togethers.Length + 1);
        foreach (GameObject together in this.togethers)
        {
            Destroy(together);
        }
        Destroy(this.gameObject);
    }

    private async void SetTargetsColor()
    {
        await Task.Delay(100);
        foreach (GameObject together in this.togethers)
        {
            together.GetComponent<Renderer>().material.color = colorValue;
            Debug.Log(together.GetComponent<Renderer>().material.color);
        }
    }
}