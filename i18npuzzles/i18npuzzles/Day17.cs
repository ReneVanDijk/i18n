
using System.Diagnostics;
using System.Text;

namespace i18npuzzles;

class Day17 : BaseDay
{
    public Day17() : base(Resource1.test_input_17, Resource1.input_17, "132")
    {
    }

    protected override string Solve(byte[] data)
    {
        var lines = GetLines(data).ToArray();
        var blocks = Blocks(lines);

        var (width, height, verticalBlocks, horizontalBlocks) = DetermineGridSize(blocks, out var topOrderBlocks);

        var minBlockHeight = blocks.Select(b => b.Lines.Count).Min();

        HashSet<Block> usedBlocks = new();

        Block[,] grid = new Block[horizontalBlocks, verticalBlocks];
        for (int i = 0; i < topOrderBlocks.Count; i++)
        {
            usedBlocks.Add(topOrderBlocks[i]);
            var y = 0;
            grid[y, i] = topOrderBlocks[i];
            var count = topOrderBlocks[i].Lines.Count - minBlockHeight;
            while (count > 0)
            {
                y++;
                grid[y, i] = topOrderBlocks[i];
                count -= minBlockHeight;
            }
        }

        (int i, int y) prev = (-1, -1);
        while (usedBlocks.Count < blocks.Count)
        {
            prev = TryFill(blocks, verticalBlocks, horizontalBlocks, minBlockHeight, usedBlocks, grid, prev);
        }

        var result = PrintGrid(grid, minBlockHeight);

        return $"{result}";
    }

    private long PrintGrid(Block[,] grid, int minBlockHeight)
    {
        var sb = new StringBuilder();

        for (int r = 0; r < grid.GetLength(0); r++)
        {
            for (int i = 0; i < minBlockHeight; i++)
            {
                var linesb = new StringBuilder();
                for (int c = 0; c < grid.GetLength(1); c++)
                {
                    var lineIdx = i;
                    var block = grid[r, c];

                    var dR = r - 1;
                    while (dR >= 0 && grid[dR, c] == block)
                    {
                        lineIdx += minBlockHeight;
                        dR--;
                    }

                    linesb.Append(block.Lines[lineIdx].Hex);
                }
                sb.AppendLine(linesb.ToString());
            }
        }
        var res = 0L;

        var lineCount = 0;
        foreach (var line in sb.ToString().Split(Environment.NewLine))
        {
            var decoded = FromHex(line);

            var runes = decoded.EnumerateRunes().ToArray();
            Console.WriteLine(decoded);

            //var idx = decoded.IndexOf('╳');
            var runeIdx = Array.IndexOf(runes, new Rune('╳'));

            if (runeIdx != -1)
            {
                if (res != 0)
                    Console.WriteLine("WEIRD");
                res = runeIdx * lineCount;
            }

            lineCount++;
        }

        return res;
    }

    private (int i, int y) TryFill(List<Block> blocks, int verticalBlocks, int horizontalBlocks, int minBlockHeight, HashSet<Block> usedBlocks, Block[,] grid, (int i, int y) prev)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[i, y] == null)
                {
                    if (prev == (i, y)) // We are not able to find this slot yet
                        continue;

                    var blocksToUse = blocks.Except(usedBlocks).ToArray();
                    if (y == 0)
                        blocksToUse = blocksToUse.Where(b => b.Border.HasFlag(Border.Left)).ToArray();
                    if (y == verticalBlocks - 1)
                        blocksToUse = blocksToUse.Where(b => b.Border.HasFlag(Border.Right)).ToArray();

                    if (i == 0)
                        blocksToUse = blocksToUse.Where(b => b.Border.HasFlag(Border.Top)).ToArray();
                    if (i == horizontalBlocks - 1)
                        blocksToUse = blocksToUse.Where(b => b.Border.HasFlag(Border.Bottom)).ToArray();

                    Block? blockToTheLeft = null;
                    int leftOffset = 0;
                    Block? blockToTheRight = null;
                    int rightOffset = 0;
                    if (y != 0)
                    {
                        blockToTheLeft = grid[i, y - 1];
                        if (blockToTheLeft != null)
                        {
                            for (int dI = i - 1; dI >= 0; dI--)
                            {
                                if (grid[dI, y - 1] == blockToTheLeft)
                                {
                                    leftOffset += minBlockHeight;
                                }
                            }
                        }
                    }
                    if (y != verticalBlocks - 1)
                    {
                        blockToTheRight = grid[i, y + 1];
                        if (blockToTheRight != null)
                        {
                            for (int dI = i - 1; dI >= 0; dI--)
                            {
                                if (grid[dI, y + 1] == blockToTheRight)
                                {
                                    rightOffset += minBlockHeight;
                                }
                            }
                        }
                    }

                    if (blockToTheLeft == null && blockToTheRight == null)
                        continue;

                    Debug.Assert(!(blockToTheLeft == null && blockToTheRight == null));

                    var options = new List<Block>();
                    foreach (var block in blocksToUse)
                    {
                        if ((blockToTheLeft == null || CompletesOnRighthandSide(blockToTheLeft, block, leftOffset: leftOffset)) &&
                            (blockToTheRight == null || CompletesOnRighthandSide(block, blockToTheRight, rightOffset: rightOffset)))
                        {
                            options.Add(block);
                        }

                    }

                    if (options.Count > 1)
                        return (i, y);

                    Debug.Assert(options.Count == 1);

                    var myI = i;
                    var myY = y;

                    grid[myI, myY] = options.Single();
                    usedBlocks.Add(options.Single());
                    var count = options.Single().Lines.Count - minBlockHeight;
                    while (count > 0)
                    {
                        myI++;
                        grid[myI, myY] = options.Single();
                        count -= minBlockHeight;
                    }

                    return (i, y);
                }
            }
        }

        return (-1, -1);
    }

    private int GetIncompleteHorizontal(Block[,] grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[i, y] == null)
                    return i;
            }
        }
        throw new UnreachableException();
    }

    private Block Concat(Block block, Block other)
    {
        return new(block.Border | other.Border,
            block.Lines.Index().Select(i => new BlockLine(i.Item.Hex + other.Lines[i.Index].Hex, i.Item.UTF8Decoded + other.Lines[i.Index].UTF8Decoded)).ToList());
    }

    private (int width, int height, int verticalBlocks, int horizontalBlocks) DetermineGridSize(List<Block> blocks, out List<Block> topOrderBlocks)
    {
        var topblocks = blocks.Where(b => b.Border.HasFlag(Border.Top)).ToArray();
        var topleft = topblocks.Single(b => b.Border.HasFlag(Border.Left));

        topblocks = topblocks.Except([topleft]).ToArray();

        var current = topleft;
        topOrderBlocks = [current];

        while (topblocks.Length > 0)
        {
            var fits = topblocks.Where(b => CompletesOnRighthandSide(current, b)).ToArray();
            if (fits.Count() == 1)
            {
                topOrderBlocks.Add(fits.Single());
                current = fits.Single();
                topblocks = [.. topblocks.Except([current])];
            }
        }

        var topLine = FromHex(string.Concat(topOrderBlocks.Select(b => b.Lines.First().Hex)));

        var x = topOrderBlocks.Max(tb => tb.Lines.Count);

        var height = blocks.Where(b => b.Border.HasFlag(Border.Left)).Sum(b => b.Lines.Count());

        var vertical = blocks.Where(b => b.Border.HasFlag(Border.Left)).Sum(b => b.Lines.Count()) / blocks.Min(b => b.Lines.Count);

        return (topLine.Length, height, topOrderBlocks.Count(), vertical);
    }

    private bool CompletesOnRighthandSide(Block current, Block rightHandOption, int leftOffset = 0, int rightOffset = 0)
    {
        if (current.Border.HasFlag(Border.Right))
            return false;

        for (int i = 0; i < Math.Min(current.Lines.Count - leftOffset, rightHandOption.Lines.Count - rightOffset); i++)
        {
            var line = current.Lines[i + leftOffset];
            var other = rightHandOption.Lines[i + rightOffset];

            if (line.UTF8Decoded.EndsWith('�'))
            {
                var fribbib = FromHex(line.Hex + other.Hex);
                if (fribbib[..^1].Trim('�').Contains('�'))
                    return false;
            }
        }

        return true;
    }


    private Border DetermineBorder(List<BlockLine> input)
    {
        var block = input.Select(i => i.UTF8Decoded).ToList();

        Border b = Border.None;
        if (block.All(l => l.StartsWith('╔') || l.StartsWith('╚') || l.StartsWith('|') || l.StartsWith('║')))
            b |= Border.Left;

        if (block.All(l => l.EndsWith('╗') || l.EndsWith('╝') || l.EndsWith('|') || l.EndsWith('║')))
            b |= Border.Right;

        if (block.First().All(ch => ch == '╔' || ch == '╗' || ch == '═' || ch == '-'))
            b |= Border.Top;

        if (block.Last().All(ch => ch == '╝' || ch == '╚' || ch == '═' || ch == '-'))
            b |= Border.Bottom;

        return b;
    }

    private void Print(Block block)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine(block.Border);

        foreach (var line in block.Lines)
        {
            Console.WriteLine(line.UTF8Decoded);
        }

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
    }

    private List<Block> Blocks(string[] lines)
    {
        var blocks = new List<Block>();
        var currentBlock = new List<BlockLine>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (currentBlock.Count > 0)
                {
                    blocks.Add(new(DetermineBorder([.. currentBlock]), [.. currentBlock]));
                    currentBlock.Clear();
                }
            }
            else
            {
                currentBlock.Add(new(line, FromHex(line)));
            }
        }

        if (currentBlock.Count > 0)
        {
            blocks.Add(new(DetermineBorder(currentBlock), currentBlock));
        }

        return blocks;
    }

    private string FromHex(string line) => Encoding.UTF8.GetString(Convert.FromHexString(line));

    [Flags]
    enum Border
    {
        None = 0,
        Top = 1,
        Left = 2,
        Bottom = 4,
        Right = 8
    }

    record Block(Border Border, List<BlockLine> Lines);
    record BlockLine(string Hex, string UTF8Decoded)
    {
        byte[] Bytes => Convert.FromHexString(Hex);
    }
}

