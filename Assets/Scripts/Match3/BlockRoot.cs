using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public GameObject BlockPrefab = null;
    public BlockControl[,] blocks;
    public Vector2 startPos = new Vector2(6.0f, 1.0f);

    public Vector3 mousePosition;

    private List<GameObject> prefabList;
    private BlockControl canMatchBlock;
    private Vector3 canMatchDir;
    private float noMatchTime = 0.0f;
    private bool noMatchTimeFlow = true;
    private Coroutine showHintRoutine;

    public bool isPaused = false;

    public void InitialSetUp()
    {
        if (this.prefabList == null)
        {
            this.prefabList = new List<GameObject>();
        }
        if (this.prefabList.Count > 0)
        {
            foreach (GameObject prefab in this.prefabList)
            {
                Destroy(prefab);
            }
        }
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y]; // �׸����� ũ�⸦ 9��9��
        int colorIndex = 0;

        Block.COLOR color = Block.COLOR.FIRST;

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                GameObject gameObject = Instantiate(this.BlockPrefab) as GameObject;
                this.prefabList.Add(gameObject);
                BlockControl block = gameObject.GetComponent<BlockControl>(); // ����� BlockControl Ŭ������ ������
                this.blocks[x, y] = block;
                block.iPos.x = x;
                block.iPos.y = y;
                block.blockRoot = this;
                Vector3 position = CalcBlockPosition(block.iPos);
                block.transform.position = position;

                // ���� ���� Ȯ���� �������� ���� ����
                color = this.SelectBlockColor();
                block.SetColor(color);

                block.name = "block(" + block.iPos.x.ToString() + "," + block.iPos.y.ToString() + ")";
                colorIndex = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
            }
        }
    }

    // ������ �׸��� ��ǥ�� �������� ��ǥ�� ����
    public Vector3 CalcBlockPosition(Block.iPosition iPos)
    {
        // ��ġ�� ���� �� ���� ��ġ�� �ʱⰪ���� ����
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);

        // �ʱ갪 + �׸��� ��ǥ �� ��� ũ��
        position.x += (float)iPos.x * Block.COLLISION_SIZE;
        position.y += (float)iPos.y * Block.COLLISION_SIZE;

        // �� ��ġ ����
        position.x += startPos.x;
        position.y += startPos.y;

        return position;
    }

    private GameObject mainCamera = null;
    private BlockControl grabbedBlock = null;

    private ScoreCounter scoreCounter = null;
    protected bool isVanishingPrev = false;

    public TextAsset levelData = null;
    public LevelControl levelControl;

    public Dictionary<Block.COLOR, int> matchCount = new Dictionary<Block.COLOR, int>();

    // ���� �������� �ʱ�ȭ, �ε�, ���� �������� ����
    public void Create(int stage, int wave, int maxWave)
    {
        // ���� ������ �ʱ�ȭ
        this.levelControl = new LevelControl();
        SetLevelData(stage, wave, maxWave);
    }

    public void SetLevelData(int stage, int wave, int maxWave)
    {
        this.levelControl.initialize(stage, wave, maxWave);
        this.levelControl.loadLevelData(this.levelData); // ������ �б�
        this.levelControl.SelectLevel(); // ���� ����
    }

    // ���� ������ ���� Ȯ���� �������� ���� �����ؼ� ��ȯ
    public Block.COLOR SelectBlockColor()
    {
        Block.COLOR color = Block.COLOR.FIRST;
        LevelData level_data = this.levelControl.GetCurrentLevelData(); // �̹� ������ ���� �����͸� ������
        float rand = Random.Range(0.0f, 1.0f); // 0.0~1.0 ������ ����
        float sum = 0.0f; // ���� Ȯ���� �հ�
        int i = 0;

        // ����� ���� ��ü�� ó���ϴ� ����
        for (i = 0; i < level_data.probability.Length - 1; i++)
        {
            // ���� Ȯ���� 0�̸� ������ ó������ ����
            if (level_data.probability[i] == 0.0f) continue;
            sum += level_data.probability[i];
            if (rand < sum) break;
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
        if (isPaused) return;
        this.UnprojectMousePosition(out mousePosition, Input.mousePosition);
        Vector2 mousePositionXy = new Vector2(mousePosition.x, mousePosition.y);
        if (this.noMatchTimeFlow)
        {
            this.noMatchTime += Time.deltaTime;
        }
        if (this.noMatchTime >= 5.0f)
        {
            this.noMatchTime = 0.0f;
            this.noMatchTimeFlow = false;
            showHintRoutine = StartCoroutine(showHint());
        }
        if (this.grabbedBlock == null)
        {
            if (!this.IsHasFallingBlock())
            {
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
                this.SwapBlock(grabbedBlock, swapTarget, grabbedBlock.slideDir); // ����� ��ü
                this.grabbedBlock = null;
            } while (false);

            if (!Input.GetMouseButton(0))
            {
                this.grabbedBlock.EndGrab();
                this.grabbedBlock = null;
            }
        }
        if (this.IsHasFallingBlock() || this.HasSlidingBlock())
        {
        }
        else
        {
            int igniteCount = 0; // �Һ��� ����
                                 // �׸��� ���� ��� ��Ͽ� ���ؼ� ó��
            foreach (BlockControl block in this.blocks)
            {
                if (!block.IsIdle())
                { // ��� ���̸� ������ ó������ �����ϰ�
                    continue; // ���� ����� ó��
                }
                // ���� �Ǵ� ���ο� ���� �� ����� �� �� �̻� �����ߴٸ�
                if (this.checkConnection(block))
                {
                    igniteCount++; // �Һ��� ������ ����
                    this.noMatchTime = 0.0f;
                    this.noMatchTimeFlow = true;
                    if (showHintRoutine != null)
                    {
                        StopCoroutine(showHintRoutine);
                    }
                }
            }
            if (igniteCount > 0)
            {
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
                    continue;
                }
                for (int y = 0; y < Block.BLOCK_NUM_Y - 1; y++)
                {// �� ���� �ִ� ����� ���������� �˻��Ѵ�.
                    if (!this.blocks[x, y].IsVacant())
                    {
                        // ���� ����� ��ǥ�ö�� ���� �������.
                        continue;
                    }
                    for (int y1 = y + 1; y1 < Block.BLOCK_NUM_Y; y1++)
                    {
                        // ���� ��� �Ʒ��� �ִ� ����� �˻�.
                        if (this.blocks[x, y1].IsVacant()) { continue; } // �Ʒ��� �ִ� ����� ��ǥ�ö�� ���� �������.
                        this.fallBlock(this.blocks[x, y], Block.DIR4.UP, this.blocks[x, y1]); // ����� ��ü�Ѵ�.
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
        // ��� �տ� ī�޶� ���ϴ� ��(plane)�� ����
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
        // ī�޶�� ���콺�� ����ϴ� ���� ����
        Ray ray = this.mainCamera.GetComponent<Camera>().ScreenPointToRay(mousePosition);
        float depth;
        // ����(ray)�� ��(plane)�� ��Ҵٸ�
        if (plane.Raycast(ray, out depth))
        { // depth ������ ����ϰ�
            worldPosition = ray.origin + ray.direction * depth; // ���콺 ��ġ(3D)�� ���
            ret = true;
        }
        else
        { // ���� �ʾҴٸ�
            worldPosition = Vector3.zero; // ���콺 ��ġ�� 0���� ���
            ret = false;
        }
        return (ret); // ī�޶� ����ϴ� ������ ��Ͽ� ��Ҵ����� ��ȯ
    }

    public BlockControl getNextBlock(BlockControl block, Block.DIR4 dir)
    {
        BlockControl nextBlock = null;
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                if (block.iPos.x < Block.BLOCK_NUM_X - 1)
                {
                    nextBlock = this.blocks[block.iPos.x + 1, block.iPos.y];
                }
                break;
            case Block.DIR4.LEFT:
                if (block.iPos.x > 0)
                {
                    // �׸��� ���̶��
                    nextBlock = this.blocks[block.iPos.x - 1, block.iPos.y];
                }
                break;
            case Block.DIR4.UP:
                if (block.iPos.y < Block.BLOCK_NUM_Y - 1)
                {
                    // �׸��� ���̶��
                    nextBlock = this.blocks[block.iPos.x, block.iPos.y + 1];
                }
                break;
            case Block.DIR4.DOWN:
                if (block.iPos.y > 0)
                {
                    // �׸��� ���̶��
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

    public void SwapBlock(BlockControl block0, BlockControl block1, Block.DIR4 dir, bool reswapCheck = true)
    {
        // ������ ��� ���� ���
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;
        // ������ ����� Ȯ������ ���
        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;
        // ������ ����� '������� �ð�'�� ���
        float vanishTimer0 = block0.vanishTimer;
        float vanishTimer1 = block1.vanishTimer;
        // ������ ����� �̵��� ���� ����
        Vector3 offset0 = BlockRoot.getDirVector(dir);
        Vector3 offset1 = BlockRoot.getDirVector(BlockRoot.getOppositDir(dir));
        // ���� ��ü
        block0.SetColor(color1);
        block1.SetColor(color0);
        // Ȯ������ ��ü
        block0.transform.localScale = scale1;
        block1.transform.localScale = scale0;
        // '������� �ð�'�� ��ü
        block0.vanishTimer = vanishTimer1;
        block1.vanishTimer = vanishTimer0;

        // ���� ��� �̵��� ����
        block0.BeginSlide(offset0);
        block1.BeginSlide(offset1);

        block0.step = Block.STEP.SLIDE;
        block1.step = Block.STEP.SLIDE;

        bool checkBlock0 = checkConnection(block0);
        bool checkBlock1 = checkConnection(block1);

        // checking�� true�� ���� �˻�
        if (reswapCheck)
        {
            if (!checkBlock0 && !checkBlock1)
            {
                ReSwap(block0, block1, dir);
            }
            else CountMatchBlock();
        }
    }

    private async void CountMatchBlock()
    {
        await Task.Delay(500);
        int _matchCount = 0;
        foreach (KeyValuePair<Block.COLOR, int> item in matchCount)
        {
            if (_matchCount < item.Value)
            {
                _matchCount = item.Value;
            }
        }
        if (_matchCount < 4)
        {
            _matchCount = 0;
            return;
        }

        if (_matchCount >= 7)
        {
            FireEnemies(true);
            FreezeEnemies(true);
        }
        else if (_matchCount >= 5)
        {
            FireEnemies();
        }
        else if (_matchCount >= 4)
        {
            FreezeEnemies();
        }
        matchCount.Clear();
    }

    private async void FreezeEnemies(bool both = false)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<float> speedList = new List<float>();

        foreach (GameObject obj in enemies)
        {
            if (obj == null) continue;
            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.ActiveEffect(both ? "Both" : "Freeze");
            speedList.Add(enemy.speed);
            enemy.agent.speed /= 3;
        }
        await SleepWithPause(5000);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) continue;
            Enemy enemy = enemies[i].GetComponent<Enemy>();
            enemy.agent.speed = speedList[i];
            enemy.ActiveEffect(both ? "Both" : "Freeze", false);
        }
    }

    private async void FireEnemies(bool both = false)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<float> speedList = new List<float>();

        foreach (GameObject obj in enemies)
        {
            if (obj == null) continue;
            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.ActiveEffect(both ? "Both" : "Fire");
            speedList.Add(enemy.speed);
            enemy.agent.speed *= 0.6f;
        }
        for (int i = 0; i < 10; i++)
        {
            await SleepWithPause(500);
            foreach (GameObject obj in enemies)
            {
                if (obj == null) continue;
                Enemy enemy = obj.GetComponent<Enemy>();
                enemy.hp -= 0.1f;
            }
        }
        await SleepWithPause(500);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) continue;
            Enemy enemy = enemies[i].GetComponent<Enemy>();
            enemy.ActiveEffect(both ? "Both" : "Fire", false);
            enemy.agent.speed = speedList[i];
        }
    }

    private async Task SleepWithPause(int time)
    {
        int div = 100;
        for (int ms = 0; ms < time / div; ms++)
        {
            await Task.Delay(div);
            while (isPaused)
            {
                await Task.Delay(5);
            }
        }
    }

    private async void ReSwap(BlockControl block0, BlockControl block1, Block.DIR4 dir)
    {
        await Task.Delay(300);
        SwapBlock(block0, block1, dir, reswapCheck: false);
        await Task.Delay(200);
        // �߸��� �������� drag�� ������, ���� ����
        FindCanMatch();
    }

    // �μ��� ���� ����� �� ���� ��� �ȿ� ���� �� �ľ��ϴ� �޼���
    public bool checkConnection(BlockControl start, int[] otherPos = null)
    {
        bool ret = false;
        int normalBlockNum = 0;
        Block.iPosition pos;
        if (otherPos == null) pos = start.iPos;
        else
        {
            pos = new();
            pos.x = otherPos[0];
            pos.y = otherPos[1];
        }
        if (!start.IsVanishing())
        {
            // �μ��� ����� �Һ��� ������ �ƴϸ�
            normalBlockNum = 1;
        }
        int rx = pos.x;
        int lx = pos.x;
        for (int x = lx - 1; x > 0; x--)
        {
            // �׸��� ��ǥ�� ����� �д�
            // ����� ������ �˻�
            BlockControl nextBlock = this.blocks[x, pos.y];
            if (nextBlock.color != start.color) { break; }
            // ���� �ٸ���, ������ ����������
            if (nextBlock.step == Block.STEP.FALL || nextBlock.nextStep == Block.STEP.FALL) { break; } // ���� ���̸�, ������ ����������
            if (nextBlock.step == Block.STEP.SLIDE || nextBlock.nextStep == Block.STEP.SLIDE) { break; } // �����̵� ���̸�, ������ ����������
            if (!nextBlock.IsVanishing())
            {
                // �Һ��� ���°� �ƴϸ�
                normalBlockNum++;
            }
            lx = x;
        }
        for (int x = rx + 1; x < Block.BLOCK_NUM_X; x++)
        {
            // �˻�� ī���͸� ����
            // ����� �������� �˻�
            BlockControl nextBlock = this.blocks[x, pos.y];
            if (nextBlock.color != start.color) { break; }
            if (nextBlock.step == Block.STEP.FALL || nextBlock.nextStep == Block.STEP.FALL) { break; }
            if (nextBlock.step == Block.STEP.SLIDE || nextBlock.nextStep == Block.STEP.SLIDE) { break; }
            if (!nextBlock.IsVanishing()) { normalBlockNum++; }
            rx = x;
        }
        do
        {
            // ������ ����� �׸��� ��ȣ - ���� ����� �׸��� ��ȣ + �߾� ���(1)�� ���� ���� 3 �̸��̸�, ���� Ż��
            if (rx - lx + 1 < 3) { break; }
            // �Һ��� ���� ����� �ϳ��� ������, ���� Ż��
            if (normalBlockNum == 0) { break; }
            for (int x = lx; x < rx + 1; x++)
            {
                this.blocks[x, pos.y].ToVanishing();
                ret = true;
            }
        } while (false);
        normalBlockNum = 0;
        if (!start.IsVanishing())
        {
            normalBlockNum = 1;
        }
        int uy = pos.y;
        int dy = pos.y;
        for (int y = dy - 1; y > 0; y--)
        { // ����� ������ �˻�.
            BlockControl nextBlock = this.blocks[pos.x, y];
            if (nextBlock.color != start.color) { break; }
            if (nextBlock.step == Block.STEP.FALL || nextBlock.nextStep == Block.STEP.FALL) { break; }
            if (nextBlock.step == Block.STEP.SLIDE || nextBlock.nextStep == Block.STEP.SLIDE) { break; }
            if (!nextBlock.IsVanishing()) { normalBlockNum++; }
            dy = y;
        }
        for (int y = uy + 1; y < Block.BLOCK_NUM_Y; y++)
        { // ����� �Ʒ����� �˻�.
            BlockControl nextBlock = this.blocks[pos.x, y];
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
                this.blocks[pos.x, y].ToVanishing();
                ret = true;
            }
        } while (false);
        return (ret);
    }

    // Ư�� ��� ��ġ ���� ���� Ȯ��
    private bool CheckCanMatch(BlockControl block, int[] pos = null)
    {
        bool isSwap = false;
        if (pos == null)
        {
            pos = new int[] { block.iPos.x, block.iPos.y };
        }
        else isSwap = true;
        // (0: x ����, 1: y ����)
        for (int axis = 0; axis <= 1; axis++)
        {
            int count = 1; // include self
            for (int offset = -2; offset <= 2; offset++)
            {

                if (offset == 0) continue;

                // deep copy
                int[] _pos = new int[] { pos[0], pos[1] };
                _pos[axis] += offset;

                int newX = _pos[0];
                int newY = _pos[1];

                if (newX < 0 || newY < 0 || newX >= Block.BLOCK_NUM_X - 1 || newY >= Block.BLOCK_NUM_Y - 1) continue;

                // color swap
                Block.COLOR color = blocks[newX, newY].color;

                if (isSwap && newX == block.iPos.x && newY == block.iPos.y)
                {
                    Block.COLOR temp = color;
                    color = blocks[pos[0], pos[1]].color;
                }
                if (color == block.color) count++;
                else if (count >= 2) break;
            }
            if (count >= 3)
            {
                return true;
            }
        }
        return false;
    }

    // ��ü���� ��ġ ������ ��� ã��
    private bool FindCanMatch()
    {
        int maxX = Block.BLOCK_NUM_X;
        int maxY = Block.BLOCK_NUM_Y;

        for (int y = 0; y < maxY; y++)
        {
            for (int x = 0; x < maxX; x++)
            {
                BlockControl _block = blocks[x, y];
                for (int i = 0; i < (int)Block.DIR4.NUM; i++)
                {
                    Block.DIR4 dir = (Block.DIR4)i;
                    Vector3 offset = getDirVector(dir);

                    int newX = x + (int)offset.x;
                    int newY = y + (int)offset.y;


                    if (newX < 0 || newY < 0 || newX >= maxX || newY >= maxY) continue;

                    bool check = CheckCanMatch(_block, new int[] { newX, newY });
                    if (check)
                    {
                        canMatchBlock = _block;
                        canMatchDir = getDirVector(dir);
                        return true;
                    }
                }
            }
        }
        canMatchBlock = null;
        InitialSetUp();
        return false;
    }

    // �Һٴ� ���� ����� �ϳ��� ������ true�� ��ȯ�Ѵ�.
    private bool HasVanishingBlock()
    {
        bool ret = false;
        foreach (BlockControl block in this.blocks)
        {
            if (block.vanishTimer > 0.0f)
            {
                ret = true;
                break;
            }
        }
        return (ret);
    }
    // �����̵� ���� ����� �ϳ��� ������ true�� ��ȯ�Ѵ�.
    private bool HasSlidingBlock()
    {
        bool ret = false;
        foreach (BlockControl block in this.blocks)
        {
            if (block.step == Block.STEP.SLIDE)
            {
                ret = true;
                break;
            }
        }
        return (ret);
    }
    // ���� ���� ����� �ϳ��� ������ true�� ��ȯ�Ѵ�.
    private bool IsHasFallingBlock()
    {
        bool ret = false;
        foreach (BlockControl block in this.blocks)
        {
            if (block.step == Block.STEP.FALL)
            {
                ret = true;
                break;
            }
        }
        return (ret);
    }

    public void fallBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    {
        // block0�� block1�� ��, ũ��, ����� ������ �ɸ��� �ð�, ǥ��, ��ǥ��, ���¸� ���.
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
        // block0�� block1�� ���� �Ӽ��� ��ü�Ѵ�.
        block0.SetColor(color1);
        block1.SetColor(color0);
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

    // �������� �� ���Ʒ� ����� ��ü�Ѵ�.
    private bool HasSlidingBlockInColumn(int x)
    { // ������ �׸��� ��ǥ�� ��(���� ��)�� �����̵� ���� ����� �ϳ��� ������, true�� ��ȯ�Ѵ�.
        bool ret = false;
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            if (this.blocks[x, y].IsSliding())
            {
                // �����̵� ���� ����� ������,
                ret = true;
                break;
            }
        }
        return (ret);
    }

    private IEnumerator showHint()
    {
        while (true)
        {
            if (noMatchTimeFlow) break;
            FindCanMatch();
            canMatchBlock.BeginSlide(canMatchDir);
            yield return new WaitForSeconds(1);
        }
    }

    public void PointUp(Block.COLOR color, int count = 1)
    {
        scoreCounter.PointUp(color, count);
        noMatchTimeFlow = true;
    }
}
