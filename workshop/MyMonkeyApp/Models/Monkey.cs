namespace MyMonkeyApp.Models;

/// <summary>
/// 원숭이 정보를 보관하는 모델 클래스입니다.
/// </summary>
public sealed class Monkey
{
    /// <summary>원숭이 이름</summary>
    public string Name { get; init; }

    /// <summary>서식지(지역)</summary>
    public string Location { get; init; }

    /// <summary>개체수(대략)</summary>
    public int Population { get; init; }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="name">원숭이 이름</param>
    /// <param name="location">원숭이 서식지</param>
    /// <param name="population">개체수</param>
    public Monkey(string name, string location, int population)
    {
        Name = name ?? throw new System.ArgumentNullException(nameof(name));
        Location = location ?? throw new System.ArgumentNullException(nameof(location));
        Population = population;
    }
}
