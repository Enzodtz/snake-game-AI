using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class GameManager : Agent {  
  public int gameSizeX;
  public int gameSizeY;
  public int squareSize;
  public GameObject applePrefab;
  public GameObject backgroundPrefab;
  public GameObject snakePartPrefab;
  public GameObject wallPrefab;
  public float timeSpeed = 100;

  private int snakeDirection;
  // 0: top
  // 1: right
  // 2: bottom
  // 3: left

  private List<Vector2> snakePositions = new List<Vector2>();
  private List<GameObject> snakeParts = new List<GameObject>();
  
  private GameObject apple;
  private Vector2 applePosition;
  private int applesCatched = 0;
  private int squaresWalked = 0;
  private int squaresWithoutApple = 0;
  private bool finished;
  private bool isHeuristic;

  void InstantiateBackground() {
    Vector3 backgroundTransform = gameObject.transform.parent.transform.position;
    backgroundTransform.z = 1;
    GameObject background = Instantiate(backgroundPrefab, backgroundTransform, Quaternion.identity);
    background.transform.parent = gameObject.transform.parent;
    background.transform.localScale = new Vector3(gameSizeX * squareSize, gameSizeY * squareSize);
  }

  void InstantiateWalls() {
    GameObject wall = Instantiate(wallPrefab, gameObject.transform.parent.transform.position, Quaternion.identity);
    wall.transform.parent = gameObject.transform.parent;
    wall.transform.localScale = new Vector3(squareSize, gameSizeY * squareSize);
    wall.transform.localPosition = new Vector3((gameSizeX/2 + 0.5f) * squareSize, 0f, 0f);

    wall = Instantiate(wallPrefab, gameObject.transform.parent.transform.position, Quaternion.identity);
    wall.transform.parent = gameObject.transform.parent;
    wall.transform.localScale = new Vector3(squareSize, gameSizeY * squareSize);
    wall.transform.localPosition = new Vector3((-gameSizeX/2 - 0.5f) * squareSize, 0f, 0f);

    wall = Instantiate(wallPrefab, gameObject.transform.parent.transform.position, Quaternion.identity);
    wall.transform.parent = gameObject.transform.parent;
    wall.transform.localScale = new Vector3(gameSizeX * squareSize, squareSize);
    wall.transform.localPosition = new Vector3(0f, (-gameSizeY/2 - 0.5f) * squareSize, 0f);

    wall = Instantiate(wallPrefab, gameObject.transform.parent.transform.position, Quaternion.identity);
    wall.transform.parent = gameObject.transform.parent;
    wall.transform.localScale = new Vector3(gameSizeX * squareSize, squareSize);
    wall.transform.localPosition = new Vector3(0f, (gameSizeY/2 + 0.5f) * squareSize, 0f);
  }

  void InstantiateApple() {
    apple = Instantiate(applePrefab, gameObject.transform.parent.transform.position, Quaternion.identity);
    apple.transform.parent = gameObject.transform.parent;
    apple.transform.localScale = new Vector3(squareSize, squareSize, 1);
  }

  void AddRandomSnakeHead() {
    float newHeadPositionX = Random.Range(-gameSizeX/2, gameSizeX/2 - 1) + 0.5f;
    float newHeadPositionY = Random.Range(-gameSizeY/2, gameSizeY/2 - 1) + 0.5f;

    Vector2 newHeadPosition = new Vector2(newHeadPositionX, newHeadPositionY);
    snakePositions.Add(newHeadPosition);  

    gameObject.transform.localScale = new Vector3(squareSize, squareSize, 1);
    gameObject.transform.localPosition = new Vector3(newHeadPosition.x * squareSize, newHeadPosition.y * squareSize, -1);
    snakeParts.Add(gameObject);
  }

  void AddRandomSnakePart(Vector2 cantBeInThisPosition) {
    Vector2 snakeTail = snakePositions[snakePositions.Count - 1];
    Vector2 newPosition = new Vector2(Random.Range(-1, 2) + snakeTail.x, Random.Range(-1, 2) + snakeTail.y);

    while(
      (newPosition.x > gameSizeX/2 - .5 || newPosition.x < -gameSizeX/2 + .5) ||
      (newPosition.y > gameSizeY/2 - .5 || newPosition.y < -gameSizeY/2 + .5) ||
      (newPosition == snakeTail) ||
      (newPosition.x != snakeTail.x && newPosition.y != snakeTail.y) ||
      (newPosition == cantBeInThisPosition)
    ) {
      newPosition = new Vector2(Random.Range(-1, 2) + snakeTail.x, Random.Range(-1, 2) + snakeTail.y);
    }
    
    snakePositions.Add(newPosition);

    GameObject newSnakePart = Instantiate(snakePartPrefab, gameObject.transform.parent.transform.position, Quaternion.identity);
    newSnakePart.transform.parent = gameObject.transform.parent;
    newSnakePart.transform.localScale = new Vector3(squareSize, squareSize, 1);
    newSnakePart.transform.localPosition = new Vector3(newPosition.x * squareSize, newPosition.y * squareSize, -1);
    snakeParts.Add(newSnakePart);
  }

  Vector2 NextHeadPosition() {
    Vector2 newPosition = new Vector2(snakePositions[0].x, snakePositions[0].y);

    switch (snakeDirection) {
      case 0:
        newPosition.y += 1;
        break;
      case 1:
        newPosition.x += 1;
        break;
      case 2:
        newPosition.y -= 1;
        break;
      case 3:
        newPosition.x -= 1;
        break;
      default:
        break;
    }

    return newPosition;
  }

  void NewApplePosition() {
    float applePositionX = Random.Range(-gameSizeX/2, gameSizeX/2 - 1) + 0.5f;
    float applePositionY = Random.Range(-gameSizeY/2, gameSizeY/2 - 1) + 0.5f;

    bool isDifferentPosition;

    while(true) {
      isDifferentPosition = true;
      foreach (var position in snakePositions) {
        if (position.x == applePositionX && position.y == applePositionY) {
          isDifferentPosition = false;
        }
      }
      if (isDifferentPosition) {
        break;
      } else {
        applePositionX = Random.Range(-gameSizeX/2, gameSizeX/2 - 1) + 0.5f;
        applePositionY = Random.Range(-gameSizeY/2, gameSizeY/2 - 1) + 0.5f;
      }
    }

    applePosition = new Vector2(applePositionX, applePositionY);
    apple.transform.localPosition = new Vector3(applePosition.x * squareSize, applePosition.y * squareSize, -1);
  }

  void AddPartFromTail(Vector2 snakeTail) {
    snakePositions.Add(snakeTail);

    GameObject newSnakePart = Instantiate(snakePartPrefab, gameObject.transform.parent.transform.position, Quaternion.identity);
    newSnakePart.transform.parent = gameObject.transform.parent;
    newSnakePart.transform.localScale = new Vector3(squareSize, squareSize, 1);
    newSnakePart.transform.localPosition = new Vector3(0, 0, 1);
    snakeParts.Add(newSnakePart);
  }

  void CheckIfHitApple(Vector2 snakeTail) {
    if(snakePositions[0] == applePosition) {
      applesCatched += 1;
      squaresWithoutApple = 0;
      NewApplePosition();
      AddPartFromTail(snakeTail);
    } else {
      squaresWithoutApple += 1;
    }
  }

  void CheckIfHitWall() {
    if(
      snakePositions[0].x > gameSizeX/2 - 0.5 || snakePositions[0].x < -gameSizeX/2 + 0.5 ||
      snakePositions[0].y > gameSizeY/2 - 0.5 || snakePositions[0].y < -gameSizeY/2 + 0.5
    ) {
      OnEpisodeEnd();
    }
  }

  void CheckIfHitTail() {
    List<Vector2> snakeBody = snakePositions.GetRange(1, snakePositions.Count - 1);
    if(snakeBody.Contains(snakePositions[0])) {
      OnEpisodeEnd();
    }
  }

  void CheckIfNoAppleLimit() {
    if(squaresWithoutApple == gameSizeX * gameSizeY) {
      OnEpisodeEnd();
    }
  }

  void CheckIfWin() {
    if(snakePositions.Count == gameSizeX * gameSizeY) {
      OnEpisodeEnd();
    }
  }

  void SetFitness() {
    SetReward(applesCatched - squaresWalked / (gameSizeX * gameSizeY));
  }

  void OnEpisodeEnd() {
    finished = true;
    SetFitness();
    EndEpisode();
  }

  void Start() {        
    InstantiateBackground();
    InstantiateWalls();
    InstantiateApple();
  }

  public override void OnEpisodeBegin() {
    applesCatched = 0;
    squaresWalked = 0;
    squaresWithoutApple = 0;
    snakePositions = new List<Vector2>();
    snakeDirection = Random.Range(0, 4);
    finished = false;

    for (int i = 1; i < snakeParts.Count; i++) {
      Destroy(snakeParts[i]);    
    }
    snakeParts = new List<GameObject>();

    AddRandomSnakeHead();
    AddRandomSnakePart(NextHeadPosition());
    AddRandomSnakePart(snakePositions[snakePositions.Count-2]);

    NewApplePosition();
  }

  public override void OnActionReceived(ActionBuffers actionBuffers) {
    snakeDirection = actionBuffers.DiscreteActions[0];

    Vector2 snakeTail = snakePositions[snakePositions.Count - 1];

    for (int i = snakePositions.Count - 1; i > 0; i--) {
      snakePositions[i] = snakePositions[i - 1];
    }      
    snakePositions[0] = NextHeadPosition();

    CheckIfHitApple(snakeTail);
    CheckIfNoAppleLimit();
    CheckIfHitWall();
    CheckIfHitTail();

    if(!finished) {    
      for (int i = 0; i < snakeParts.Count; i++) {
        snakeParts[i].transform.localPosition = snakePositions[i] * squareSize;
      }
      squaresWalked += 1;
      CheckIfWin();
    }
  }

  public override void CollectObservations(VectorSensor sensor) {
    List<int> encodedDirection = new List<int>{0,0,0,0};
    encodedDirection[snakeDirection] = 1;

    sensor.AddObservation(encodedDirection[0]);
    sensor.AddObservation(encodedDirection[1]);
    sensor.AddObservation(encodedDirection[2]);
    sensor.AddObservation(encodedDirection[3]);
  }
  
  public void Update() {
    // key detection for heuristic
    if(isHeuristic){
      if (Input.GetKeyDown(KeyCode.UpArrow)) {
        snakeDirection = 0;
      } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
        snakeDirection = 1;
      } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
        snakeDirection = 2;
      } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        snakeDirection = 3;
      }
    }

    Time.timeScale = timeSpeed;
  }

  public override void Heuristic(in ActionBuffers actionsOut) {
    // actually, heuristic is based on the update, since its not being called every frame
    isHeuristic = true;
    
    var discreteActionsOut = actionsOut.DiscreteActions;
    discreteActionsOut[0] = snakeDirection;
  }
}
