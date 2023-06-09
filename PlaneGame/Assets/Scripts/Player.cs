using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject bullet;
    public float mySpeed = 10;
    Rigidbody2D myRigid;

    public Vector2 inputVec;

    float fireDelay = .2f; //0.2초가 상한선
    float currentDelay = 0;

    float bulletSpeed = 7;

    bool hitLeftBox = false;
    bool hitRightBox = false;
    bool hitUpBox = false;
    bool hitDownBox = false;

    float playerHp = 100;
    float playerMaxHp = 100;

    public ObjectManager objManager;
    public Slider playerHpSlider;
    public GameObject playerHpObj;

    public float score = 0;
    public Text scoreNum;
    public GameObject loseText;
    public GameObject restartBtn;
    public GameObject gameOverObj;

    GameObject particle;

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody2D>();
        scoreNum.text = score.ToString();
        playerHpSlider.value = playerMaxHp;
    }

    // Update is called once per frame
    void Update()
    {
        currentDelay += Time.deltaTime;

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        if (hitLeftBox && inputVec.x == -1)
            inputVec.x = 0;
        
        else if (hitRightBox && inputVec.x == 1)
            inputVec.x = 0;

        else if(hitUpBox && inputVec.y == 1)
            inputVec.y = 0;

        else if(hitDownBox && inputVec.y == -1)
            inputVec.y = 0;  

        Fire();

        scoreNum.text = score.ToString();

        
    }

    private void FixedUpdate()
    {
        inputVec = inputVec.normalized * mySpeed * Time.fixedDeltaTime;
        myRigid.MovePosition(myRigid.position + inputVec);
    }

    void Fire()
    {
        if (currentDelay < fireDelay)
            return;

        //Instantiate(bullet, transform.position, transform.rotation); -> gameObject에 담을 수 있음
        //Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>(); -> 하나만 앞으로 나감 bullet은 instantiate해서 복제된 애들이랑 달라서 복제된 애들 getcomponent는 못가져옴

        GameObject bulletInfo = objManager.SelectObj("playerBullet");
        bulletInfo.transform.position = transform.position;
        Rigidbody2D bulletRigid = bulletInfo.GetComponent<Rigidbody2D>();
        bulletRigid.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);

        currentDelay = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {     
        if(collision.transform.tag == "Boundary")
        {
            switch (collision.transform.name)
            {
                case "LeftBoundary":
                    hitLeftBox = true; break;
                case "RightBoundary":
                    hitRightBox= true; break;
                case "UpBoundary":
                    hitUpBox= true; break;
                case "DownBoundary":
                    hitDownBox= true; break;
                default:
                    break;
            }
        }

        if(collision.transform.tag == "enemy" || collision.transform.tag == "enemyBullet")
        {
            particle = objManager.SelectObj("particle");
            particle.transform.position = collision.transform.position;
            collision.gameObject.SetActive(false);
            playerHp-=20;
            playerHpSlider.value = playerHp / playerMaxHp;

            if (playerHp <= 0)
            {
                Destroy(gameObject);
                playerHpObj.SetActive(false);
                gameOverObj.SetActive(true);

                Time.timeScale = 0;
            }
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Boundary")
        {
            switch (collision.transform.name)
            {
                case "LeftBoundary":
                    hitLeftBox = false; break;
                case "RightBoundary":
                    hitRightBox = false; break;
                case "UpBoundary":
                    hitUpBox = false; break;
                case "DownBoundary":
                    hitDownBox = false; break;
                default:
                    break;
            }
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "enemy")
    //    {
    //        collision.gameObject.SetActive(false);
    //        playerHp--;
    //        playerHpSlider.value = playerHp / playerMaxHp;
    //    }
    //}

    
}
