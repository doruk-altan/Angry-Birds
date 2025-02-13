using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;

    [Header("Transform References")]
    [SerializeField] private Transform leftStartPosition;
    [SerializeField] private Transform rightStartPosition;

    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;

    [Header("Slingshot Stats")]
    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float shotForce = 5f;
    [SerializeField] private float timeBetweenBirdRespawns = 2f;



    [Header("Scripts")]
    [SerializeField] private SlingShotArea slingShotArea;
    [SerializeField] private CameraManager cameraManager;

    [Header("Bird")]
    [SerializeField] private AngieBird angieBirdPrefab;
    [SerializeField] private float angieBirdPositionOffset = .275f;

    [Header("Sounds")]
    [SerializeField] private AudioClip ellasticPulledClip;
    [SerializeField] private AudioClip[] ellasticReleasedClips;

    private Vector2 slingshotLinesPosition;
    private Vector2 direction;
    private Vector2 directionNormalized;

    private bool clickedWithinArea;
    private bool birdOnSlingshot;

    private AngieBird spawnedAngieBird;

    private AudioSource audioSource;



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;

        SpawnAngieBird();
    }

    private void Update()
    {
        if(InputManager.WasLeftMouseButtonPressed && slingShotArea.IsWithinSlingshotArea())
        {
            clickedWithinArea = true;

            if (birdOnSlingshot)
            {
                SoundManager.Instance.PlayClip(ellasticPulledClip, audioSource);
                cameraManager.SwitchToFollowCam(spawnedAngieBird.transform);
            }
        }

        if (InputManager.IsLeftMouseButtonPressed && clickedWithinArea && birdOnSlingshot)
        {
            DrawSlingshot();
            PositionAndRotateAngieBird();
        }

        if (InputManager.WasLeftMouseButtonReleased && clickedWithinArea && birdOnSlingshot)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                clickedWithinArea = false;

                spawnedAngieBird.LaunchBird(direction, shotForce);

                SoundManager.Instance.PlayRandomClip(ellasticReleasedClips, audioSource);

                GameManager.instance.UseShot();

                birdOnSlingshot = false;

                SetLines(centerPosition.position);

                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnNewAngieBirdAfterTime());
                }
            }
            
        }
    }
    
    #region Slingshot Methods

    private void DrawSlingshot()
    {

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.mousePosition);

        slingshotLinesPosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);

        SetLines(slingshotLinesPosition);

        direction = (Vector2)centerPosition.position - slingshotLinesPosition;

        directionNormalized = direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (leftLineRenderer.enabled == false && rightLineRenderer.enabled == false)
        {
            leftLineRenderer.enabled = true;
            rightLineRenderer.enabled = true;
        }

        leftLineRenderer.SetPosition(0,position);
        leftLineRenderer.SetPosition(1, leftStartPosition.position);

        rightLineRenderer.SetPosition(0, position);
        rightLineRenderer.SetPosition(1, rightStartPosition.position);
    }

    #endregion

    #region AngieBird Methods

    private void SpawnAngieBird()
    {
        SetLines(idlePosition.position);

        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;

        Vector2 spawnPosition = (Vector2)idlePosition.position + dir * angieBirdPositionOffset;

        spawnedAngieBird = Instantiate(angieBirdPrefab, spawnPosition, Quaternion.identity);

        spawnedAngieBird.transform.right = dir;

        birdOnSlingshot = true;
    }

    private void PositionAndRotateAngieBird()
    {
        spawnedAngieBird.transform.position = slingshotLinesPosition + directionNormalized * angieBirdPositionOffset;
        spawnedAngieBird.transform.right = directionNormalized;
    }

    private IEnumerator SpawnNewAngieBirdAfterTime()
    {

        yield return new WaitForSeconds(timeBetweenBirdRespawns);


        SpawnAngieBird();

        cameraManager.SwitchToIdleCam();
    }


    #endregion
}
