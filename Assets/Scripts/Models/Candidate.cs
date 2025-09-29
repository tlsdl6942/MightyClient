public class CandidateInfo
{
    public int playerNumber;   // 플레이어 번호
    public string suit;        // 선택한 문양 ("S", "H", "D", "C")
    public int pledgeCount;    // 공약 수량

    public CandidateInfo(int playerNumber, string suit, int pledgeCount)
    {
        this.playerNumber = playerNumber;
        this.suit = suit;
        this.pledgeCount = pledgeCount;
    }
}