using UnityEngine;

public class Block
{ // 블록에 관한 정보
    public static float COLLISION_SIZE = 1.0f; // 블록의 충돌 크기
    public static float vanishTime = 3.0f; // 불 붙고 사라질 때까지의 시간
    
    public struct iPosition
    { // 그리드에서의 좌표를 나타내는 구조체
        public int x; // X 좌표
        public int y; // Y 좌표
    }

    public enum COLOR { // 블록 색상
        NONE = -1, // 색 지정 없음
        PINK = 0, BLUE, YELLOW, GREEN, // 분홍색, 파란색, 노란색, 녹색
        MAGENTA, ORANGE, GRAY, // 마젠타, 주황색, 그레이
        NUM, // 컬러가 몇 종류인지 나타냄(=7)
        FIRST = PINK, LAST = ORANGE,// 초기 컬러(분홍색), 최종 컬러(주황색)
        NORMAL_COLOR_NUM = GRAY, // 보통 컬러(회색 이외의 색)의 수
    };

    public enum DIR4
    { // 상하좌우 네 방향
        NONE = -1, // 방향지정 없음
        RIGHT, LEFT, UP, DOWN, // 우. 좌, 상, 하
        NUM,  // 방향이 몇 종류 있는지 나타냄(=4)
    };

    public static int BLOCK_NUM_X = 9; // 블록을 배치할 수 있는 X방향 최대수
    public static int BLOCK_NUM_Y = 9; // 블록을 배치할 수 있는 Y방향 최대수
    public enum STEP
    {
        // 상태 정보 없음, 대기 중, 잡혀 있음, 떨어진 순간, 슬라이드 중, 소멸 중, 재생성 중, 낙하 중, 크게 슬라이드 중, 상태 종류
        NONE = -1, IDLE = 0, GRABBED, RELEASED, SLIDE, VACANT, RESPAWN, FALL, LONG_SLIDE, NUM,
    };
}

public class BlockControl : MonoBehaviour
{
    public Block.COLOR color = (Block.COLOR)0; // 블록 색
    public BlockRoot blockRoot= null; // 블록의 신
    public Block.iPosition iPos; // 블록 좌표

    public Material opagueMaterial;
    public Material transparentMaterial;

    void Start()
    {
        this.setColor(this.color);
        this.nextStep = Block.STEP.IDLE;
    }

    public void setColor(Block.COLOR color)
    { // 특정 color로 블록을 칠함
        this.color = color; // 이번에 지정된 색을 멤버 변수에 보관
        Color colorValue;  // Color 클래스는 색을 나타냄
        switch (this.color)
        { // 칠할 색에 따라서 갈라짐
            default:
            case Block.COLOR.PINK:
                colorValue = new Color(1.0f, 0.5f, 0.5f); break;
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
        this.GetComponent<Renderer>().material.color = colorValue; // 색상변경
    }
    public Block.STEP step = Block.STEP.NONE; // 지금 상태
    public Block.STEP nextStep = Block.STEP.NONE; // 다음 상태
    private Vector3 positionOffsetInitial = Vector3.zero; // 교체 전 위치
    public Vector3 positionOffset = Vector3.zero; // 교체 후 위치
    private struct StepFall
    {
        public float velocity; // 낙하 속도.
    }
    private StepFall fall;

    void Update()
    {
        Vector3 mousePosition;
        this.blockRoot.UnprojectMousePosition(out mousePosition, Input.mousePosition);
        Vector2 mousePositionXy = new Vector2(mousePosition.x, mousePosition.y);
        if (this.vanishTimer >= 0.0f)
        {
            // 타이머가 0 이상이면
            this.vanishTimer -= Time.deltaTime;
            if (this.vanishTimer < 0.0f)
            {
                if (this.step != Block.STEP.SLIDE)
                {
                    this.vanishTimer = -1.0f;
                    this.nextStep = Block.STEP.VACANT;
                }
                else
                {
                    this.vanishTimer = 0.0f;
                }
            }
        }
        this.stepTimer += Time.deltaTime;
        float slideTime = 0.2f;
        if (this.nextStep == Block.STEP.NONE)
        {
            switch (this.step)
            {
                case Block.STEP.SLIDE:
                    if (this.stepTimer >= slideTime)
                    {
                        // '상태 정보 없음'의 경우
                        // 슬라이드 중인 블록이 소멸되면 VACANT(사라진) 상태로 이행
                        if (this.vanishTimer == 0.0f)
                        {
                            this.nextStep = Block.STEP.VACANT;
                        }
                        else
                        {
                            this.nextStep = Block.STEP.IDLE; // vanishTimer가 0이 아니면 IDLE(대기) 상태로 이행
                        }
                    }
                    break;
                case Block.STEP.IDLE:
                    this.GetComponent<Renderer>().enabled = true;
                    break;
                case Block.STEP.FALL:
                    if (this.positionOffset.y <= 0.0f)
                    {
                        this.nextStep = Block.STEP.IDLE;
                        this.positionOffset.y = 0.0f;
                    }
                    break;
            }
        }
        while (this.nextStep != Block.STEP.NONE)
        {
            this.step = this.nextStep;
            this.nextStep = Block.STEP.NONE;
            // '다음 블록' 상태가 '정보 없음' 이외인 동안  '다음 블록' 상태가 변경된 경우
            switch (this.step)
            {
                case Block.STEP.IDLE:
                    this.positionOffset = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f; break;
                case Block.STEP.GRABBED:
                    this.transform.localScale = Vector3.one * 1.2f; break;
                case Block.STEP.RELEASED:
                    this.positionOffset = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f; break;
                case Block.STEP.VACANT:
                    this.positionOffset = Vector3.zero;
                    this.SetVisible(false);
                    break;
                case Block.STEP.RESPAWN:
                    // 색을 랜덤하게 선택하여 블록을 그 색으로 설정.
                    int colorIndex = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
                    this.setColor((Block.COLOR)colorIndex);
                    this.nextStep = Block.STEP.IDLE;
                    break;
                case Block.STEP.FALL:
                    this.SetVisible(true); // 블록을 표시.
                    this.fall.velocity = 0.0f; // 낙하 속도 리셋.
                    break;
            }
            this.stepTimer = 0.0f;
        }
        switch (this.step)
        {
            case Block.STEP.GRABBED:
                this.slideDir = this.CalcSlideDir(mousePositionXy);
                break;
            case Block.STEP.SLIDE:
                float rate = this.stepTimer / slideTime;
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.positionOffset = Vector3.Lerp(this.positionOffsetInitial, Vector3.zero, rate); break;
            case Block.STEP.FALL:
                this.fall.velocity += Physics.gravity.y * Time.deltaTime * 0.3f; // 중력의 영향을 부여
                this.positionOffset.y += this.fall.velocity * Time.deltaTime; // 세로 방향 위치를 계산
                if (this.positionOffset.y < 0.0f)
                { // 다 내려왔다면
                    this.positionOffset.y = 0.0f; // 그 자리에 머무른다 
                }
                break;
        }
        Vector3 position = BlockRoot.CalcBlockPosition(this.iPos) + this.positionOffset;
        this.transform.position = position;
        this.setColor(this.color);
        if (this.vanishTimer >= 0.0f)
        {
            float vanishTime = this.blockRoot.levelControl.GetVanishTime();
            UnityEngine.Color color0 = UnityEngine.Color.Lerp(this.GetComponent<Renderer>().material.color, UnityEngine.Color.white, 0.5f);
            UnityEngine.Color color1 = UnityEngine.Color.Lerp(this.GetComponent<Renderer>().material.color, UnityEngine.Color.black, 0.5f);
            if (this.vanishTimer < Block.vanishTime / 2.0f)
            {
                color0.a = this.vanishTimer / (Block.vanishTime / 2.0f);
                color1.a = color0.a;
                this.GetComponent<Renderer>().material = this.transparentMaterial;
            }
            float rate = 1.0f - this.vanishTimer / Block.vanishTime;
            this.GetComponent<Renderer>().material.color = UnityEngine.Color.Lerp(color0, color1, rate);
        }
    }

    public void BeginFall(BlockControl start)
    { // 낙하 시작 처리
        this.nextStep = Block.STEP.FALL;
        this.positionOffset.y = (float)(start.iPos.y - this.iPos.y)
       * Block.COLLISION_SIZE; // 지정된 블록에서 좌표를 계산
    }

    public void BeginRespawn(int startIposY)
    { // 색이 바꿔 낙하 상태로 하고 지정한 위치에 재배치
        this.positionOffset.y = (float)(startIposY - this.iPos.y)
        * Block.COLLISION_SIZE; // 지정 위치까지 y좌표를 이동
        this.nextStep = Block.STEP.FALL;
        Block.COLOR color = this.blockRoot.SelectBlockColor();
        this.setColor(color);
    }

    public bool IsVacant()
    { // 블록이 비표시(그리드상의 위치가 텅 빔)로 되어 있다면 true를 반환
        bool isVacant = false;
        if (this.step == Block.STEP.VACANT && this.nextStep == Block.STEP.NONE) {
            isVacant = true;
        }
        return (isVacant);
    }

    public bool IsSliding()
    { // 교체 중(슬라이드 중)이라면 true를 반환
        bool isSliding = (this.positionOffset.x != 0.0f);
        return (isSliding);
    }

    public void BeginGrab()
    {
        this.nextStep = Block.STEP.GRABBED;
    }

    public void EndGrab()
    {
        this.nextStep = Block.STEP.IDLE;
    }

    // 잡혔을 때 호출
    // 놓았을 때 호출
    public bool IsGrabbable()
    { // 잡을 수 있는 상태 인지 판단
        bool isGrabbable = false;
        switch (this.step)
        {
            case Block.STEP.IDLE:
                // '대기' 상태일 때만.
                isGrabbable = true;
                break;
        }
        return (isGrabbable);
    }
    // true(잡을 수 있다)를 반환
    public bool IsContainedPosition(Vector2 position)
    { // 지정된 마우스 좌표가 자신과 겹치는지 반환
        bool ret = false;
        Vector3 center = this.transform.position;
        float h = Block.COLLISION_SIZE / 2.0f;
        do
        {
            if (position.x < center.x - h || center.x + h < position.x) { break; } // X 좌표가 자신과 겹치지 않으면 루프를 빠져 나감
            if (position.y < center.y - h || center.y + h < position.y) { break; } // Y 좌표가 자신과 겹치지 않으면 루프를 빠져 나감
            ret = true;
        } while (false);
        return (ret);
    }
    public float vanishTimer = -1.0f;
    public Block.DIR4 slideDir = Block.DIR4.NONE;
    public float stepTimer = 0.0f;
    // 블록이 사라질 때까지의 시간
    // 슬라이드된 방향
    // 블록이 교체된 때의 이동시간 등
    public Block.DIR4 CalcSlideDir(Vector2 mousePosition) { // 마우스 위치를 바탕으로 슬라이드된 방향을 구함
        Block.DIR4 dir = Block.DIR4.NONE;
        Vector2 v = mousePosition - new Vector2(this.transform.position.x, this.transform.position.y); // 지정된 mouse_positio과 현재 위치의 차이
        if (v.magnitude > 0.1f)
        {
            // 벡터의 크기가 0.1보다 작으면 슬라이드 하지 않은 걸로 간주
            if (v.y > v.x)
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.UP;
                }
                else { dir = Block.DIR4.LEFT; }
            }
            else
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.RIGHT;
                }
                else { dir = Block.DIR4.DOWN; }
            }
        }
        return (dir);
    }
    public float CalcDirOffset(Vector2 position, Block.DIR4 dir)
    { // 현재 위치와 슬라이드할 곳의 거리가 어느 정도인가 반환
        float offset = 0.0f;
        Vector2 v = position - new Vector2(this.transform.position.x, this.transform.position.y); // 지정된 위치와 블록의 현재 위치의 차를 나타내는 벡터
        switch (dir)
        {
            case Block.DIR4.RIGHT: offset = v.x; break;
            case Block.DIR4.LEFT: offset = -v.x; break;
            case Block.DIR4.UP: offset = v.y; break;
            case Block.DIR4.DOWN: offset = -v.y; break;
        }
        return (offset);
    }
    public void BeginSlide(Vector3 offset)
    {
        this.positionOffsetInitial = offset;
        this.positionOffset = this.positionOffsetInitial;
        this.nextStep = Block.STEP.SLIDE;
    }
    public void ToVanishing()
    {
        // ＇사라질 때까지 걸리는 시간＇을 규정값으로 리셋
        float vanishTime = this.blockRoot.levelControl.GetVanishTime();
        this.vanishTimer = vanishTime;
    }
    public bool IsVanishing()
    {
        // vanishTimer가 0보다 크면 true
        bool isVanishing = (this.vanishTimer > 0.0f);
        return (isVanishing);
    }
    public void RewindVanishTimer()
    {
        // '사라질 때까지 걸리는 시간'을 규정값으로 리셋
        float vanishTime = this.blockRoot.levelControl.GetVanishTime();
        this.vanishTimer = vanishTime;
    }

    public bool IsVisible()
    {
        // 그리기 가능(renderer.enabled가 true) 상태라면표시
        bool isVisible = this.GetComponent<Renderer>().enabled;
        return (isVisible);
    }
    public void SetVisible(bool isVisible)
    {
        // 그리기 가능 설정에 인수를 대입
        this.GetComponent<Renderer>().enabled = isVisible;
    }
    public bool IsIdle()
    {
        bool isIdle = false;
        // 현재 블록 상태가 '대기 중'이고, 다음 블록 상태가 '없음'이면
        if (this.step == Block.STEP.IDLE && this.nextStep == Block.STEP.NONE)
        {
            isIdle = true;
        }
        return (isIdle);
    }
}