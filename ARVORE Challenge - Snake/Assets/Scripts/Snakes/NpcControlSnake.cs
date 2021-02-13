using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcControlSnake : SnakeController
{
    public int awareness = 5;
    public float reactionTime = 0.2f;

    private float timer = 0;
    private GridSnake grid { get { return GridSnake.instance; } }

    public Pickable foodTarget;

    public List<int> storedMoves = new List<int>();
    private Vector2 distance;

    private List<GridSlot> avoidList = new List<GridSlot>();

    private List<Vector2> path = new List<Vector2>();

    private void Start()
    {
        snake.OnMoveGrid += OnMoveGrid;
    }

    private void OnDestroy()
    {
        snake.OnMoveGrid -= OnMoveGrid;
    }

    private void OnMoveGrid()
    {
        if (path.Count == 0) return;

        if (path[0] == snake.head.coordinates)
        {
            path.RemoveAt(0);
            SetNextDirection();
        }
    }

    private void SetNextDirection()
    {
        if (path.Count == 0) snake.nextDirection = snake.head.orientation;
        else
        {
            snake.nextDirection = path[0] - snake.head.coordinates;
        }
    }

    //update function
    protected override void DirectionManager()
    {
        //updates his behaviour acording to the reaction time
        if (Time.timeSinceLevelLoad - timer < reactionTime) return;

        Vector2 target;
        if (foodTarget == null)
        {
            int posX = Mathf.FloorToInt(Random.Range(2, grid.width-2));
            int posY = Mathf.FloorToInt(Random.Range(2, grid.height-2));

            target = new Vector2(posX, posY);
        }
        else
        {
            target = foodTarget.coordinates;
        }
        PathFinding(target);

        SetNextDirection();


        timer = Time.timeSinceLevelLoad;
            
    }

    private void PathFinding(Vector2 target)
    {
        Vision();

        path = new List<Vector2>();
        path.Add(snake.head.coordinates);

        Vector2[] pathOrientations = new Vector2[awareness+1];
        pathOrientations[0] = snake.head.orientation;

        List<Vector3> misteps = new List<Vector3>();
        List<int> turns = new List<int>();


        //I need to learn better Lambdas to create a recursive function...
        for (int i = 1; i <= awareness; i++)
        {
            distance = target - path[i-1];
            int t = 0;
            switch (turns.Count)
            {
                case 0:
                    t = Steering(Utility.BestCardinalDirection(distance), pathOrientations[i - 1]);
                    pathOrientations[i] = GetTurnOrientation(t, pathOrientations[i - 1]);
                    break;
                case 1: // if the first fails, the code will chose 1 new path
                    if (pathOrientations[i - 1] == -pathOrientations[i])
                    {
                        if (!turns.Contains(0)) t = 0;
                        else t = (turns.Contains(1) ? -1 : 1);
                        pathOrientations[i] = GetTurnOrientation(t, pathOrientations[i - 1]);
                    }
                    else t = Steering(pathOrientations[i], pathOrientations[i - 1]);
                    break;
                case 2: //if the second fails, we need to try the last
                    if (!turns.Contains(0)) t = 0;
                    else t = (turns.Contains(1)?-1:1);
                    pathOrientations[i] = GetTurnOrientation(t, pathOrientations[i - 1]);
                    break;
            }

            turns.Add(t);

            Vector2 p = path[i - 1] + pathOrientations[i];

            GridSlot slot = grid.GetGridSlot(p);
            if (slot.isInUse && avoidList.Contains(slot))
            {
                Vector3 alert = pathOrientations[i];
                if (slot.users[0] is SnakeBodyPart) //if snake, go towards the tail
                {
                    alert.z = 1;//maybe death
                    pathOrientations[i] = -slot.users[0].orientation;
                }
                else if (slot.isBorder)
                {
                    pathOrientations[i] *= -1; 
                    alert.z = 0;//certain death
                }
                else
                {
                    pathOrientations[i] =
                        new Vector2(
                        (pathOrientations[i].x == 0 ? Mathf.Sign(distance.x) : 0),
                        (pathOrientations[i].y == 0 ? Mathf.Sign(distance.y) : 0));
                    
                    alert.z = 2;//just bad
                }
                misteps.Add(alert);

                if (misteps.Count >= 3 || turns.Count >= 3)//no more options
                {
                    Vector3 bestChoice = Vector3.zero;
                    foreach (Vector3 choice in misteps)
                    {
                        if (bestChoice.z < choice.z) bestChoice = choice;

                    }
                    pathOrientations[i] = bestChoice;

                    misteps.Clear();
                    turns.Clear();
                    path.Add(p);
                }
                else i--; //voltar 1 step
            }
            else
            { //reset values
                misteps.Clear();
                turns.Clear();
                path.Add(p);
            }
        }

        path.RemoveAt(0);//remove the Head coordinates
    }

    private void Vision()
    {
        avoidList.Clear();
        for (int range = awareness; range > -awareness; range--)
        {
            int step = Mathf.Abs(awareness - range);
            for (int cross = -step; cross <= step; cross++)
            {
                Vector2 coords = snake.head.coordinates;
                coords += snake.head.orientation * range;
                if (snake.head.orientation.x != 0) coords.y += cross;
                else coords.x += cross;

                if (!grid.OutOfBounds((int)coords.x, (int)coords.y))
                {
                    GridSlot slot = grid.GetGridSlot(coords);
                    if (slot.hasComflict) avoidList.Add(slot); //avoid conflicts
                    else if (slot.isBorder) avoidList.Add(slot);
                    else if (slot.isInUse)
                    {
                        if (slot.users[0] is Pickable)
                        {
                            Pickable food = (Pickable)slot.users[0];
                            if (food == foodTarget) continue;

                            if (FoodHandler.instance.currentFruit == food)
                            {
                                if (foodTarget == null)
                                {
                                    foodTarget = food;
                                    //CalculatePath();
                                    continue;
                                }
                            }
                            else avoidList.Add(slot); //avoid others food
                        }
                        else avoidList.Add(slot); //avoid obstacles
                    }
                }
            }
        }
    }

    private int Steering(Vector2 direction, Vector2 orientation)
    {
        if (orientation == Vector2.zero) orientation = snake.head.orientation;
        //horizontal
        if (orientation.x != 0)
        {
            // 90º
            if (direction.x == 0)
            {
                return (int)-(orientation.x * direction.y);
            }
            //wrong way 180º
            else if (direction.x != orientation.x)
            {
                return (int)(Mathf.Sign(distance.y==0?1:distance.y) * direction.x);
            }
        }
        //vertical
        else
        {
            // 90º
            if (direction.y == 0)
            {
                return (int)(orientation.y * direction.x);
            }
            //wrong way 180º
            else if (direction.y != orientation.y)
            {
                return (int)-(Mathf.Sign(distance.x==0?1:distance.x) * direction.y);
            }
        }
        return 0;
    }

    private Vector2 GetTurnOrientation(int turn, Vector2 orientation)
    {
        Vector2 newOrientation = Vector2.zero;
        if (turn != 0)
        {
            if (orientation.x != 0) newOrientation.x = 0;
            else { newOrientation.x = orientation.y * turn; }
            if (orientation.y != 0) newOrientation.y = 0;
            else { newOrientation.y = -orientation.x * turn; }

            return newOrientation;
        }
        else return orientation;
    }
   

    public void SetTargetFood(Pickable food)
    {
        foodTarget = food;
        //CalculatePath();
    }
}

/*
 * 
 * 
 * 
 * 
     protected override void DirectionManager()
    {
        //updates his behaviour acording to the reaction time
        if (Time.timeSinceLevelLoad - timer < reactionTime) return;

        
        Vision();

        //Just move around
        if (foodTarget == null) AvoidBehaviour();
        else //Go after the fruit
        {
            //The fruit is close
            if (DistanceFromTarget() <= awareness)
            {
                CloseBehaviour();
            }
            else // the fruit is still far
            {
                FarFoodBehaviour();
            }
        }

        if (storedMoves.Count > 0) FollowPath();
        if (foodTarget == null) AvoidBehaviour();
        else
        {
            if (DistanceFromTarget() < awareness && storedMoves.Count > 0) CloseBehaviour();
            else FarFoodBehaviour();
        }
        timer = Time.timeSinceLevelLoad;
        
    }
    private void CalculatePath()
    {
        if (foodTarget == null) return;
        CalculatePath(foodTarget.coordinates);
    }

    private void CalculatePath(Vector2 target)
    {
        distance = target - snake.head.coordinates;

        storedMoves.Clear();
        int absX = (int)Mathf.Abs(distance.x);
        int absY = (int)Mathf.Abs(distance.y);

        for (int i = 0; i < awareness; i++)
        {
            Vector2 dir = Vector2.zero;
            if (absX < absY)
            {
                if (i <= absX) dir.x = Mathf.Sign(distance.x);
                else dir.y = Mathf.Sign(distance.y);
            }
            else
            {
                if (i <= absY) dir.y = Mathf.Sign(distance.y);
                else dir.x = Mathf.Sign(distance.x);
            }
            storedMoves.Add(Steering(dir,Vector2.zero));
        }
    }

    private void FollowPath()
    {
        CloseBehaviour();
    }
    
private void CloseBehaviour()
{

    Debug.LogError("CloseBehaviour");

    Vector2 nd = GetTurnOrientation(storedMoves[0], snake.head.orientation);
    if (nd != snake.nextDirection)
    {
        snake.nextDirection = nd;
    }
}
private void FarFoodBehaviour()
{
    Debug.LogError("FarFoodBehaviour");
    Vector2 direction = Vector2.zero;
    if (Mathf.Abs(distance.x) >= Mathf.Abs(distance.y))
        direction.x = Mathf.Sign(distance.x);
    else direction.y = Mathf.Sign(distance.y);


    snake.Turn(Steering(direction, Vector2.zero));
}

private void AvoidBehaviour()
{
    Debug.LogError("AvoidBehaviour");
    float x = grid.width * 0.25f;
    float y = grid.height * 0.25f;

    int posX = Mathf.FloorToInt(Random.Range(x, x + grid.width));
    int posY = Mathf.FloorToInt(Random.Range(y, y + grid.height));

    CalculatePath(new Vector2(posX, posY));

    int count = awareness;
    do
    {
        Vector2 newOrientation =
            GetTurnOrientation(storedMoves[0], snake.head.orientation);

        Vector2 coords = snake.head.coordinates + newOrientation;

        GridSlot slot = grid.GetGridSlot(coords);
        if (avoidList.Contains(slot))
        {
            if (storedMoves[0] != 0) storedMoves[0] *= -1;
            else storedMoves[0] = 1;
        }
        else break;
        count--;

    } while (true && count >= 0);

    snake.Turn(storedMoves[0]);

}

 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 *  private int DistanceFromTarget(Vector2 target)
    {
        Vector2 dist = target - snake.head.coordinates;
        return (int)Mathf.Abs(dist.x) + (int)Mathf.Abs(dist.y);
    }

 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */
