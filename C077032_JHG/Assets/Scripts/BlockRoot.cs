using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public GameObject BlockPrefab = null;
    public BlockControl[,] blocks;

    public void InitialSetUp()
    {
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y]; // 그리드의 크기를 9×9로
        int colorIndex = 0;

        Block.COLOR color = Block.COLOR.FIRST;

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                GameObject gameObject = Instantiate(this.BlockPrefab) as GameObject;
                BlockControl block = gameObject.GetComponent<BlockControl>(); // 블록의 BlockControl 클래스를 가져옴
                this.blocks[x, y] = block;
                block.iPos.x = x;
                block.iPos.y = y;
                block.blockRoot = this;
                Vector3 position = BlockRoot.CalcBlockPosition(block.iPos);
                block.transform.position = position;

                // 현재 출현 확률을 바탕으로 색을 결정
                color = this.SelectBlockColor();
                block.setColor(color);

                block.name = "block(" + block.iPos.x.ToString() + "," + block.iPos.y.ToString() + ")";
                colorIndex = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
            }
        }
    }

    // 지정된 그리드 좌표로 씬에서의 좌표를 구함
    public static Vector3 CalcBlockPosition(Block.iPosition iPos)
    {
        // 배치할 왼쪽 위 구석 위치를 초기값으로 설정
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);
        // 초깃값 + 그리드 좌표 × 블록 크기
        position.x += (float)iPos.x * Block.COLLISION_SIZE;
        position.y += (float)iPos.y * Block.COLLISION_SIZE;
        return (position); // 씬에서의 좌표를 반환
    }

    private GameObject mainCamera = null;
    private BlockControl grabbedBlock = null;

    private ScoreCounter scoreCounter = null;
    protected bool isVanishingPrev = false;

    public TextAsset levelData = null;
    public LevelControl levelControl;

    public void Create()
    { // 레벨 데이터의 초기화, 로드, 패턴 설정까지 시행
        this.levelControl = new LevelControl();
        this.levelControl.initialize();
        // 레벨 데이터 초기화
        this.levelControl.loadLevelData(this.levelData); // 데이터 읽기
        this.levelControl.SelectLevel();
        // 레벨 선택
    }

    public Block.COLOR SelectBlockColor()
    { // 현재 패턴의 출현 확률을 바탕으로 색을 산출해서 반환
        Block.COLOR color = Block.COLOR.FIRST;
        // 이번 레벨의 레벨 데이터를 가져옴
        LevelData level_data = this.levelControl.GetCurrentLevelData();
        float rand = Random.Range(0.0f, 1.0f);
        // 0.0~1.0 사이의 난수
        float sum = 0.0f;
        int i = 0;
        // 출현 확률의 합계
        // 블록의 종류 전체를 처리하는 루프
        for (i = 0; i < level_data.probability.Length - 1; i++)
        {
            if (level_data.probability[i] == 0.0f)
            {
                continue;
                // 출현 확률이 0이면 루프의 처음으로 점프
            }
            sum += level_data.probability[i];
            if (rand < sum)
            {
                break;
            }
        }
        color = (Block.COLOR)i;
        return (color);
    }

    void Start()
    {
        this.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        this.scoreCounter = this.gameObject.GetComponent<ScoreCounter>();
    }

    void Update()
    {
        Vector3 mousePosition;
        this.UnprojectMousePosition(out mousePosition, Input.mousePosition);
        Vector2 mousePositionXy = new Vector2(mousePosition.x, mousePosition.y);
        if (this.grabbedBlock == null)
        {
            if (!this.isHasFallingBlock()) {
                if (Input.GetMouseButtonDown(0))
                {
                    foreach (BlockControl block in this.blocks)
                    {
                        if (!block.IsGrabbable())
                        {
                            continue;
                        }
                        if (!block.IsContainedPosition(mousePositionXy))
                        {
                            continue;
                        }
                        this.grabbedBlock = block;
                        this.grabbedBlock.BeginGrab(); break;
                    }
                }
            }
        }
        else
        {
            do
            {
                BlockControl swapTarget = this.getNextBlock(grabbedBlock, grabbedBlock.slideDir);
                if (swapTarget == null)
                {
                    break;
                }
                if (!swapTarget.IsGrabbable())
                {
                    break;
                }
                float offset = this.grabbedBlock.CalcDirOffset(mousePositionXy, this.grabbedBlock.slideDir);
                if (offset < Block.COLLISION_SIZE / 2.0f)
                {
                    break;
                }
                this.SwapBlock(grabbedBlock, grabbedBlock.slideDir, swapTarget); // 블록을 교체
                this.grabbedBlock = null;
            } while (false);

            if (!Input.GetMouseButton(0))
            {
                this.grabbedBlock.EndGrab();
                this.grabbedBlock = null;
            }
        }
        // 낙하 중 또는 슬라이드 중이면
        if (this.isHasFallingBlock() || this.HasSlidingBlock())
        {
            // 아무것도 하지 않는다
            // 낙하 중도 슬라이드 중도 아니면
        }
        else
        {
            int igniteCount = 0; // 불붙은 개수
                                  // 그리드 안의 모든 블록에 대해서 처리
            foreach (BlockControl block in this.blocks)
            {
                if (!block.IsIdle())
                { // 대기 중이면 루프의 처음으로 점프하고
                    continue; // 다음 블록을 처리
                }
                // 세로 또는 가로에 같은 색 블록이 세 개 이상 나열했다면
                if (this.checkConnection(block))
                {
                    igniteCount++; // 불붙은 개수를 증가
                }
            }
            if (igniteCount > 0)
            {
                if (!this.isVanishingPrev)
                {
                    this.scoreCounter.ClearIgniteCount();
                }
                this.scoreCounter.AddIgniteCount(igniteCount);// 점화 횟수를 증가
                this.scoreCounter.UpdateTotalScore();

                int blockCount = 0;

                foreach (BlockControl block in this.blocks)
                {
                    if (block.IsVanishing())
                    {
                        block.RewindVanishTimer();
                        blockCount++;
                    }
                }
            }
        }
        bool isVanishing = this.HasVanishingBlock();
        do
        {
            if (isVanishing) { break; }
            if (this.HasSlidingBlock()) { break; }
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (this.HasSlidingBlockInColumn(x))
                {
                    // 하나라도 연소 중인 블록이 있는가?.
                    // 연소 중인 블록이 있다면, 낙하 처리를 실행하지 않는다.
                    // 교체 중인 블록이 있다면, 낙하 처리를 실행하지 않는다.
                    // 열에 교체 중인 블록이 있다면 그 열은 처리하지 않고 다음 열로 진행.
                    continue;
                }
                for (int y = 0; y < Block.BLOCK_NUM_Y - 1; y++)
                {// 그 열에 있는 블록을 위에서부터 검사한다.
                    if (!this.blocks[x, y].IsVacant())
                    {
                        // 지정 블록이 비표시라면 다음 블록으로.
                        continue;
                    }
                    for (int y1 = y + 1; y1 < Block.BLOCK_NUM_Y; y1++)
                    {
                        // 지정 블록 아래에 있는 블록을 검사.
                        if (this.blocks[x, y1].IsVacant()) { continue; } // 아래에 있는 블록이 비표시라면 다음 블록으로.
                        this.fallBlock(this.blocks[x, y], Block.DIR4.UP, this.blocks[x, y1]); // 블록을 교체한다.
                        break;
                    }
                }
            }
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                int fall_start_y = Block.BLOCK_NUM_Y;
                for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
                {
                    if (!this.blocks[x, y].IsVacant()) { continue; }
                    this.blocks[x, y].BeginRespawn(fall_start_y);
                    fall_start_y++;
                }
            }
        } while (false);
        this.isVanishingPrev = isVanishing;
    }

    public bool UnprojectMousePosition(out Vector3 worldPosition, Vector3 mousePosition)
    {
        bool ret;
        // 블록 앞에 카메라를 향하는 판(plane)을 생성
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
        // 카메라와 마우스를 통과하는 빛을 생성
        Ray ray = this.mainCamera.GetComponent<Camera>().ScreenPointToRay(mousePosition);
        float depth;
        // 광선(ray)이 판(plane)에 닿았다면
        if (plane.Raycast(ray, out depth))
        { // depth 정보를 기록하고
            worldPosition = ray.origin + ray.direction * depth; // 마우스 위치(3D)를 기록
            ret = true;
        }
        else
        { // 닿지 않았다면
            worldPosition = Vector3.zero; // 마우스 위치를 0으로 기록
            ret = false;
        }
        return (ret); // 카메라를 통과하는 광선이 블록에 닿았는지를 반환
    }
    public BlockControl getNextBlock(BlockControl block, Block.DIR4 dir)
    {
        BlockControl nextBlock = null;
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                if (block.iPos.x < Block.BLOCK_NUM_X - 1)
                {
                    // 인수로 지정된 블록과 방향으로 블록이 슬라이드할 곳에 어느 블록이 있는지 반환
                    // 슬라이드할 곳의 블록을 여기에 저장
                    // 그리드 안이라면
                    nextBlock = this.blocks[block.iPos.x + 1, block.iPos.y];
                }
                break;
            case Block.DIR4.LEFT:
                if (block.iPos.x > 0)
                {
                    // 그리드 안이라면
                    nextBlock = this.blocks[block.iPos.x - 1, block.iPos.y];
                }
                break;
            case Block.DIR4.UP:
                if (block.iPos.y < Block.BLOCK_NUM_Y - 1)
                {
                    // 그리드 안이라면
                    nextBlock = this.blocks[block.iPos.x, block.iPos.y + 1];
                }
                break;
            case Block.DIR4.DOWN:
                if (block.iPos.y > 0)
                {
                    // 그리드 안이라면
                    nextBlock = this.blocks[block.iPos.x, block.iPos.y - 1];
                }
                break;
        }
        return (nextBlock);
    }

    public static Vector3 getDirVector(Block.DIR4 dir)
    {
        Vector3 v = Vector3.zero;
        switch (dir)
        {
            case Block.DIR4.RIGHT: v = Vector3.right; break;
            case Block.DIR4.LEFT: v = Vector3.left; break;
            case Block.DIR4.UP: v = Vector3.up; break;
            case Block.DIR4.DOWN: v = Vector3.down; break;
        }
        v *= Block.COLLISION_SIZE;
        return (v);
    }
    public static Block.DIR4 getOppositDir(Block.DIR4 dir)
    {
        Block.DIR4 opposit = dir;
        switch (dir)
        {
            case Block.DIR4.RIGHT: opposit = Block.DIR4.LEFT; break;
            case Block.DIR4.LEFT: opposit = Block.DIR4.RIGHT; break;
            case Block.DIR4.UP: opposit = Block.DIR4.DOWN; break;
            case Block.DIR4.DOWN: opposit = Block.DIR4.UP; break;
        }
        return (opposit);
    }

    public void SwapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    {
        // 각각의 블록 색을 기억
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;
        // 각각의 블록의 확대율을 기억
        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;
        // 각각의 블록의 '사라지는 시간'을 기억
        float vanishTimer0 = block0.vanishTimer;
        float vanishTimer1 = block1.vanishTimer;
        // 각각의 블록의 이동할 곳을 구함
        Vector3 offset0 = BlockRoot.getDirVector(dir);
        Vector3 offset1 = BlockRoot.getDirVector(BlockRoot.getOppositDir(dir));
        // 색을 교체
        block0.setColor(color1);
        block1.setColor(color0);
        // 확대율을 교체
        block0.transform.localScale = scale1;
        block1.transform.localScale = scale0;
        // '사라지는 시간'을 교체
        block0.vanishTimer = vanishTimer1;
        block1.vanishTimer = vanishTimer0;
        block0.BeginSlide(offset0);
        // 원래 블록 이동을 시작
        block1.BeginSlide(offset1);
    }
    // 인수로 받은 블록이 세 개의 블록 안에 들어가는 지 파악하는 메서드
    public bool checkConnection(BlockControl start)
    {
        bool ret = false;
        int normalBlockNum = 0;
        if (!start.IsVanishing())
        {
            // 인수인 블록이 불붙은 다음이 아니면
            normalBlockNum = 1;
        }
        int rx = start.iPos.x;
        int lx = start.iPos.x;
        for (int x = lx - 1; x > 0; x--)
        {
            // 그리드 좌표를 기억해 둔다
            // 블록의 왼쪽을 검사
            BlockControl nextBlock = this.blocks[x, start.iPos.y];
            if (nextBlock.color != start.color) { break; }
            // 색이 다르면, 루프를 빠져나간다
            if (nextBlock.step == Block.STEP.FALL || nextBlock.nextStep == Block.STEP.FALL) { break; } // 낙하 중이면, 루프를 빠져나간다
            if (nextBlock.step == Block.STEP.SLIDE || nextBlock.nextStep == Block.STEP.SLIDE) { break; } // 슬라이드 중이면, 루프를 빠져나간다
            if (!nextBlock.IsVanishing())
            {
                // 불붙은 상태가 아니면
                normalBlockNum++;
            }
            lx = x;
        }
        for (int x = rx + 1; x < Block.BLOCK_NUM_X; x++)
        {
            // 검사용 카운터를 증가
            // 블록의 오른쪽을 검사
            BlockControl nextBlock = this.blocks[x, start.iPos.y];
            if (nextBlock.color != start.color) { break; }
            if (nextBlock.step == Block.STEP.FALL || nextBlock.nextStep == Block.STEP.FALL) { break; }
            if (nextBlock.step == Block.STEP.SLIDE || nextBlock.nextStep == Block.STEP.SLIDE) { break; }
            if (!nextBlock.IsVanishing()) { normalBlockNum++; }
            rx = x;
        }
        do
        {
            if (rx - lx + 1 < 3) { break; }
            // 오른쪽 블록의 그리드 번호 - 왼쪽 블록의 그리드 번호 + 중앙 블록(1)을 더한 수가 3 미만이면, 루프 탈출
            if (normalBlockNum == 0) { break; }
            // 불붙지 않은 블록이 하나도 없으면, 루프 탈출
            for (int x = lx; x < rx + 1; x++)
            {
                this.blocks[x, start.iPos.y].ToVanishing();
                ret = true;
            }
        } while (false);
        normalBlockNum = 0;
        if (!start.IsVanishing())
        {
            normalBlockNum = 1;
        }
        int uy = start.iPos.y;
        int dy = start.iPos.y;
        for (int y = dy - 1; y > 0; y--)
        { // 블록의 위쪽을 검사.
            BlockControl nextBlock = this.blocks[start.iPos.x, y];
            if (nextBlock.color != start.color) { break; }
            if (nextBlock.step == Block.STEP.FALL || nextBlock.nextStep == Block.STEP.FALL) { break; }
            if (nextBlock.step == Block.STEP.SLIDE || nextBlock.nextStep == Block.STEP.SLIDE) { break; }
            if (!nextBlock.IsVanishing()) { normalBlockNum++; }
            dy = y;
        }
        for (int y = uy + 1; y < Block.BLOCK_NUM_Y; y++)
        { // 블록의 아래쪽을 검사.
            BlockControl nextBlock = this.blocks[start.iPos.x, y];
            if (nextBlock.color != start.color) { break; }
            if (nextBlock.step == Block.STEP.FALL || nextBlock.nextStep == Block.STEP.FALL) { break; }
            if (nextBlock.step == Block.STEP.SLIDE || nextBlock.nextStep == Block.STEP.SLIDE) { break; }
            if (!nextBlock.IsVanishing()) { normalBlockNum++; }
            uy = y;
        }
        do
        {
            if (uy - dy + 1 < 3) { break; }
            if (normalBlockNum == 0) { break; }
            for (int y = dy; y < uy + 1; y++)
            {
                this.blocks[start.iPos.x, y].ToVanishing();
                ret = true;
            }
        } while (false);
        return (ret);
    }

    // 불붙는 중인 블록이 하나라도 있으면 true를 반환한다.
    private bool HasVanishingBlock()
    {
        bool ret = false;
        foreach (BlockControl block in this.blocks) {
            if (block.vanishTimer > 0.0f)
            {
                ret = true;
                break;
            }
        }
        return(ret);
    }
    // 슬라이드 중인 블록이 하나라도 있으면 true를 반환한다.
    private bool HasSlidingBlock()
    {
        bool ret = false;
        foreach (BlockControl block in this.blocks)
        {
            if (block.step == Block.STEP.SLIDE) {
                ret = true;
                break;
            }
        }
        return (ret);
    }
    // 낙하 중인 블록이 하나라도 있으면 true를 반환한다.
    private bool isHasFallingBlock()
    {
        bool ret = false;
        foreach (BlockControl block in this.blocks)
        {
            if (block.step == Block.STEP.FALL) {
                ret = true;
                break;
            }
        }
        return (ret);
    }

    public void fallBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    {
        // block0과 block1의 색, 크기, 사라질 때까지 걸리는 시간, 표시, 비표시, 상태를 기록.
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;
        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;
        float vanishTimer0 = block0.vanishTimer;
        float vanishTimer1 = block1.vanishTimer;
        bool visible0 = block0.IsVisible();
        bool visible1 = block1.IsVisible();
        Block.STEP step0 = block0.step;
        Block.STEP step1 = block1.step;
        // block0과 block1의 각종 속성을 교체한다.
        block0.setColor(color1);
        block1.setColor(color0);
        block0.transform.localScale = scale1;
        block1.transform.localScale = scale0;
        block0.vanishTimer = vanishTimer1;
        block1.vanishTimer = vanishTimer0;
        block0.SetVisible(visible1);
        block1.SetVisible(visible0);
        block0.step = step1;
        block1.step = step0;
        block0.BeginFall(block1);
    }

    // 낙하했을 때 위아래 블록을 교체한다.
    private bool HasSlidingBlockInColumn(int x)
    { // 지정된 그리드 좌표의 열(세로 줄)에 슬라이드 중인 블록이 하나라도 있으면, true를 반환한다.
        bool ret = false;
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            if (this.blocks[x, y].IsSliding())
            {
                // 슬라이드 중인 블록이 있으면,
                ret = true;
                break;
            }
        }
        return (ret);
    }
}
