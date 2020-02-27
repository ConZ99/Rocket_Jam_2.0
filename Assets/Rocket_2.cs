using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket_2 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    enum States { Alive, Dead, Load}
    States state = States.Alive;

    Rigidbody rigidBody;
    AudioSource audioSource;

    int sceneNo;
    bool gameMode = false;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == States.Alive)
        {
            Thrust();
            Rotate();
        }
        else
        {
            audioSource.Stop();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != States.Alive) return; //if dead ignore collisions

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Fuel":
                print("Got fuel");
                //TODO fuel
                break;
            case "Finish":
                print("You Won!");
                state = States.Load;
                Invoke("LoadNextLevel", 1f);
                break;
            case "Singleplayer":
                print("You chose singleplayer");
                gameMode = false;
                state = States.Load;
                Invoke("LoadNextLevelS", 1f);
                break;
            case "Multiplayer":
                print("You chose multiplayer");
                gameMode = true;
                state = States.Load;
                Invoke("LoadNextLevelM", 1f);
                break;
            default:
                print("Dead");
                state = States.Load;
                Invoke("LoadFirstLevel", 1f);
                break;
        }
    }

    private void LoadNextLevel()
    {
        sceneNo = SceneManager.GetActiveScene().buildIndex;
        sceneNo ++;
        if(sceneNo < SceneManager.sceneCountInBuildSettings/2 && gameMode == false)
            SceneManager.LoadScene(sceneNo);
        else if(sceneNo < SceneManager.sceneCountInBuildSettings && gameMode == true)
            SceneManager.LoadScene(sceneNo);
        else
            SceneManager.LoadScene(0);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void Thrust()
    {
        float thrustPower = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustPower);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true; //manual control
        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }

        rigidBody.freezeRotation = false; //resume physiscs
    }
}

