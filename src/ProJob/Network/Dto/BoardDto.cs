namespace ProJob.Network.Dto;

public sealed class BoardDto
{
    public int Rows { get; set; }
    public int Cols { get; set; }
    public string[] Terrain { get; set; } = [];
    public List<EnemyDto> Enemies { get; set; } = [];
    public List<CellDto> ItemCells { get; set; } = [];
}
