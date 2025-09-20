using MyMonkeyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MyMonkeyApp.Helpers;

/// <summary>
/// 원숭이 데이터에 접근하는 정적 헬퍼입니다.
/// - 가능한 경우 환경 변수로 지정된 MCP 서버에서 원숭이 데이터를 가져옵니다 (MONKEY_MCP_URL).
/// - 서버 요청이 실패하면 메모리 시드 데이터를 반환합니다.
/// - 무작위 선택 호출 수를 추적합니다.
/// </summary>
public static class MonkeyHelper
{
    private const int DEFAULT_POPULATION = 100;

    private static readonly List<Monkey> _seed = new()
    {
        new Monkey("Spider Monkey", "Central & South America", DEFAULT_POPULATION + 50),
        new Monkey("Howler Monkey", "Central & South America", DEFAULT_POPULATION + 30),
        new Monkey("Capuchin", "Central & South America", DEFAULT_POPULATION + 80),
        new Monkey("Baboon", "Africa", DEFAULT_POPULATION + 200),
        new Monkey("Golden Lion Tamarin", "Brazil", DEFAULT_POPULATION + 20),
        new Monkey("Squirrel Monkey", "Central & South America", DEFAULT_POPULATION + 120),
    };

    private static readonly HttpClient _httpClient = new();

    // MCP 서버 URL을 환경 변수에서 읽습니다. (예: https://host/api/monkeys)
    private static readonly string? _mcpEndpoint = Environment.GetEnvironmentVariable("MONKEY_MCP_URL");

    // 무작위 선택 호출 횟수 추적
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Make field readonly", Justification = "Mutated via Interlocked.Increment at runtime")]
    private static int _randomPickCount;

    /// <summary>
    /// 무작위 선택이 호출된 총 횟수입니다.
    /// </summary>
    public static int RandomPickCount => _randomPickCount;

    /// <summary>
    /// 모든 원숭이 목록을 반환합니다. 가능하면 MCP 서버에서 데이터를 가져옵니다.
    /// </summary>
    /// <param name="cancellationToken">작업 취소 토큰</param>
    public static async Task<List<Monkey>> GetMonkeys(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(_mcpEndpoint))
        {
            try
            {
                using var response = await _httpClient.GetAsync(_mcpEndpoint, cancellationToken).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var fromServer = await JsonSerializer.DeserializeAsync<List<Monkey>>(stream, options, cancellationToken).ConfigureAwait(false);
                    if (fromServer is not null && fromServer.Count > 0)
                    {
                        return fromServer;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                // 실패 시 시드 데이터로 폴백합니다.
            }
        }

        // 시뮬레이션용 소량의 비동기 지연
        await Task.Delay(5, cancellationToken).ConfigureAwait(false);
        return _seed.ToList();
    }

    /// <summary>
    /// 이름으로 원숭이를 검색합니다(대소문자 구분 안함).
    /// </summary>
    /// <param name="name">검색할 원숭이 이름</param>
    /// <param name="cancellationToken">작업 취소 토큰</param>
    public static async Task<Monkey?> GetMonkeyByName(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var monkeys = await GetMonkeys(cancellationToken).ConfigureAwait(false);
        return monkeys.FirstOrDefault(m => string.Equals(m.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 무작위 원숭이를 하나 반환하고 호출 카운트를 증가시킵니다.
    /// </summary>
    /// <param name="cancellationToken">작업 취소 토큰</param>
    public static async Task<Monkey> GetRandomMonkey(CancellationToken cancellationToken = default)
    {
        var monkeys = await GetMonkeys(cancellationToken).ConfigureAwait(false);
        if (monkeys.Count == 0) throw new InvalidOperationException("No monkeys available.");

        var index = Random.Shared.Next(monkeys.Count);
        System.Threading.Interlocked.Increment(ref _randomPickCount);
        return monkeys[index];
    }
}
