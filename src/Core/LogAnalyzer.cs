using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LogAnalyzerPlugin.Core
{
    /// <summary>单个关键字的搜索命中结果</summary>
    public class SearchResult
    {
        public string Keyword { get; set; }
        public int LineNumber { get; set; }       // 1-based
        public int CharPosition { get; set; }     // 在整个文本中的偏移
        public string LineContent { get; set; }
        public List<string> ContextBefore { get; set; } = new List<string>();
        public List<string> ContextAfter { get; set; } = new List<string>();
        public DateTime? Timestamp { get; set; }
    }

    /// <summary>多个关键字在同一滑动窗口内共现的命中块</summary>
    public class CoOccurrenceBlock
    {
        public List<string> MatchedKeywords { get; set; } = new List<string>();
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public List<string> Lines { get; set; } = new List<string>();
        public DateTime? EarliestTimestamp { get; set; }
    }

    /// <summary>时间线上的关键字事件</summary>
    public class TimelineEvent
    {
        public DateTime Timestamp { get; set; }
        public string Keyword { get; set; }
        public int LineNumber { get; set; }
        public string LineContent { get; set; }
    }

    public class LogAnalyzer
    {
        private readonly int contextLines;
        private readonly int cooccurrenceWindow;

        // 常见日志时间戳格式
        private static readonly Regex[] TimestampPatterns = new[]
        {
            new Regex(@"\d{4}-\d{2}-\d{2}[T ]\d{2}:\d{2}:\d{2}(\.\d+)?", RegexOptions.Compiled),
            new Regex(@"\d{2}/\d{2}/\d{4}\s+\d{2}:\d{2}:\d{2}", RegexOptions.Compiled),
            new Regex(@"\w{3}\s+\d{1,2}\s+\d{2}:\d{2}:\d{2}", RegexOptions.Compiled),
            new Regex(@"\d{4}/\d{2}/\d{2}\s+\d{2}:\d{2}:\d{2}", RegexOptions.Compiled),
        };

        public LogAnalyzer(int contextLines = 3, int cooccurrenceWindow = 10)
        {
            this.contextLines = contextLines;
            this.cooccurrenceWindow = cooccurrenceWindow;
        }

        /// <summary>解析每行的时间戳</summary>
        private DateTime? ParseTimestamp(string line)
        {
            foreach (var pattern in TimestampPatterns)
            {
                var m = pattern.Match(line);
                if (m.Success && DateTime.TryParse(m.Value, out var dt))
                    return dt;
            }
            return null;
        }

        /// <summary>搜索所有关键字，返回每个命中的上下文结果</summary>
        public List<SearchResult> SearchKeywords(string text, List<string> keywords, bool caseSensitive = false)
        {
            var results = new List<SearchResult>();
            if (string.IsNullOrEmpty(text) || keywords == null || keywords.Count == 0)
                return results;

            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            // 预计算每行起始字符偏移
            var lineOffsets = new int[lines.Length];
            int offset = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                lineOffsets[i] = offset;
                offset += lines[i].Length + 1; // +1 for newline
            }

            for (int i = 0; i < lines.Length; i++)
            {
                foreach (var keyword in keywords)
                {
                    if (string.IsNullOrEmpty(keyword)) continue;
                    if (lines[i].IndexOf(keyword, comparison) < 0) continue;

                    var result = new SearchResult
                    {
                        Keyword = keyword,
                        LineNumber = i + 1,
                        CharPosition = lineOffsets[i],
                        LineContent = lines[i],
                        Timestamp = ParseTimestamp(lines[i])
                    };

                    // 上文
                    for (int c = Math.Max(0, i - contextLines); c < i; c++)
                        result.ContextBefore.Add($"  {c + 1,6}: {lines[c]}");

                    // 下文
                    for (int c = i + 1; c <= Math.Min(lines.Length - 1, i + contextLines); c++)
                        result.ContextAfter.Add($"  {c + 1,6}: {lines[c]}");

                    results.Add(result);
                }
            }

            results.Sort((a, b) => a.CharPosition.CompareTo(b.CharPosition));
            return results;
        }

        /// <summary>在滑动窗口内检测多关键字共现块</summary>
        public List<CoOccurrenceBlock> FindCoOccurrences(string text, List<string> keywords, bool caseSensitive = false)
        {
            var blocks = new List<CoOccurrenceBlock>();
            if (string.IsNullOrEmpty(text) || keywords == null || keywords.Count < 2)
                return blocks;

            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            for (int i = 0; i < lines.Length; i++)
            {
                int windowEnd = Math.Min(lines.Length - 1, i + cooccurrenceWindow);
                var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                for (int j = i; j <= windowEnd; j++)
                {
                    foreach (var kw in keywords)
                        if (!string.IsNullOrEmpty(kw) && lines[j].IndexOf(kw, comparison) >= 0)
                            found.Add(kw);
                }

                if (found.Count < 2)
                    continue;

                // 计算实际结束行（只到最后一个命中行）
                int actualEnd = i;
                for (int j = windowEnd; j >= i; j--)
                {
                    bool hasHit = false;
                    foreach (var kw in found)
                        if (lines[j].IndexOf(kw, comparison) >= 0) { hasHit = true; break; }
                    if (hasHit) { actualEnd = j; break; }
                }

                var block = new CoOccurrenceBlock
                {
                    MatchedKeywords = new List<string>(found),
                    StartLine = i + 1,
                    EndLine = actualEnd + 1,
                    EarliestTimestamp = ParseTimestamp(lines[i])
                };

                for (int j = i; j <= actualEnd; j++)
                    block.Lines.Add($"  {j + 1,6}: {lines[j]}");

                // 去重：与上一个块重叠时跳过
                if (blocks.Count > 0 && blocks[blocks.Count - 1].EndLine >= block.StartLine)
                    continue;

                blocks.Add(block);
                i = actualEnd; // 跳过已处理区域
            }

            return blocks;
        }

        /// <summary>生成时间线事件序列（需要日志包含可解析时间戳）</summary>
        public List<TimelineEvent> BuildTimeline(string text, List<string> keywords, bool caseSensitive = false)
        {
            var events = new List<TimelineEvent>();
            var results = SearchKeywords(text, keywords, caseSensitive);

            foreach (var r in results)
            {
                if (r.Timestamp == null) continue;
                events.Add(new TimelineEvent
                {
                    Timestamp = r.Timestamp.Value,
                    Keyword = r.Keyword,
                    LineNumber = r.LineNumber,
                    LineContent = r.LineContent
                });
            }

            events.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));
            return events;
        }
    }
}
